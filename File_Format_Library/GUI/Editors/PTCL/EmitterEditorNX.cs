using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using FirstPlugin.Forms;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.Interfaces;

namespace FirstPlugin
{
    public partial class EmitterEditorNX : STUserControl
    {
        public EmitterEditorNX()
        {
            InitializeComponent();
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            tabPageData.BackColor = FormThemes.BaseTheme.TabPageActive;

            //All three types are offered for every track, including alpha. Alpha=Constant (fixed opacity, no
            //fade) is never shipped in BotW (0 of 670 alpha tracks) but is technically valid: the type is purely
            //key-count driven (no separate animation-function field in EFTB, verified by field correlation), and
            //count=0 = "use the constant value" provably works for colour, symmetrically for alpha's ConstantColor.A.
            //It's just rarely desirable visually, so use it deliberately. (Verify in-game if relied upon.)
            foreach (PTCL.Emitter.ColorType format in (PTCL.Emitter.ColorType[])Enum.GetValues(typeof(PTCL.Emitter.ColorType)))
            {
                color0TypeCB.Items.Add(format);
                color1TypeCB.Items.Add(format);
                alpha0TypeCB.Items.Add(format);
                alpha1TypeCB.Items.Add(format);
            }

            //Editable colour-track type. DropDownList = pick-from-list only; the change handler restructures the
            //track's data (key count + 8-key array).
            color0TypeCB.DropDownStyle = ComboBoxStyle.DropDownList;
            color1TypeCB.DropDownStyle = ComboBoxStyle.DropDownList;
            alpha0TypeCB.DropDownStyle = ComboBoxStyle.DropDownList;
            alpha1TypeCB.DropDownStyle = ComboBoxStyle.DropDownList;
            color0TypeCB.SelectedIndexChanged += (s, e) => ColorTypeChanged(0, color0TypeCB);
            color1TypeCB.SelectedIndexChanged += (s, e) => ColorTypeChanged(1, color1TypeCB);
            alpha0TypeCB.SelectedIndexChanged += (s, e) => ColorTypeChanged(2, alpha0TypeCB);
            alpha1TypeCB.SelectedIndexChanged += (s, e) => ColorTypeChanged(3, alpha1TypeCB);

            //Time box: press Enter to set the selected keyframe's time precisely (complements dragging it).
            timeTB.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { CommitTimeEdit(); e.SuppressKeyPress = true; } };

            //The EFTB-only surface (Parameters grid, live preview, mesh-slot column) is built lazily in BuildEftbUi()
            //on the first EFTB emitter, so the 3DS/Switch PTCL editor keeps its stock layout.
        }

        private bool eftbUiBuilt;
        // EFTB-only editor surface, built once the first time an EFTB emitter is loaded (EmitterData != null).
        private void BuildEftbUi()
        {
            if (eftbUiBuilt) return;
            eftbUiBuilt = true;

            //Parameters tab: edit documented emitter fields + probe undocumented offsets (saved in place).
            var paramTab = new TabPage("Parameters");
            parameterGrid = new PropertyGrid() { Dock = DockStyle.Fill };
            parameterGrid.PropertyValueChanged += (s, e) =>
            {
                //Re-pointing a shader index doesn't change the GLSL preview, and rebuilding the grid from inside its
                //own change handler corrupts the open dropdown. Skip the rebuild for the shader fields.
                var n = (e.ChangedItem != null && e.ChangedItem.PropertyDescriptor != null) ? e.ChangedItem.PropertyDescriptor.Name : null;
                if (n == "ShaderVertexIndex" || n == "ShaderFragmentIndex") return;
                RefreshPreview();   // edit a field -> live preview updates
            };
            paramTab.Controls.Add(parameterGrid);
            stTabControl1.TabPages.Add(paramTab);

            //Live preview SPLIT: the GL viewport sits BESIDE the tabs so you edit + watch at the same time (no tab-
            //switching, no extra window). Only stTabControl1 is on the root, so re-parent it into the split's left pane.
            //GL is lazy + fully guarded -> a GL failure leaves a blank right pane; the editor itself never breaks.
            try
            {
                this.Controls.Remove(stTabControl1);
                //Panel1MinSize keeps the left pane at least as wide as the data tab (~576px) so the colour picker +
                //tracks are never clipped; the preview (fixed Panel2) yields width instead.
                var split = new SplitContainer() { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, FixedPanel = FixedPanel.Panel2, SplitterWidth = 5, Panel1MinSize = 580 };
                split.Panel1.Controls.Add(stTabControl1);
                previewHost = new STPanel() { Dock = DockStyle.Fill };
                //GPU mode: render with the emitter's own decompiled game shaders (EftGpuPreview). On by default, since
                //it is the emitter as the game draws it; unchecking falls back to the software approximation, as does an
                //emitter whose shader pair will not decompile or a file that carries no shader bundle.
                gpuPreviewCheck = new STCheckBox() { Text = "Game shaders (GPU)", AutoSize = true, Dock = DockStyle.Bottom, Checked = true };
                gpuPreviewCheck.CheckedChanged += (s, e) => RefreshPreview();
                //init/render failures inside the GL paint would otherwise show as a silent blank pane
                gpuStatusLabel = new STLabel() { AutoSize = true, Dock = DockStyle.Bottom, ForeColor = System.Drawing.Color.Orange, Visible = false };
                split.Panel2.Controls.Add(previewHost);
                split.Panel2.Controls.Add(gpuPreviewCheck);
                split.Panel2.Controls.Add(gpuStatusLabel);
                previewHost.BringToFront();   //Fill pane lays out above the Bottom-docked toggle
                this.Controls.Add(split);
                this.HandleCreated += (s, e) => { try { split.SplitterDistance = Math.Max(580, this.Width - 300); RefreshPreview(); } catch { } };
                if (this.IsHandleCreated) { try { split.SplitterDistance = Math.Max(580, this.Width - 300); } catch { } }
            }
            catch { previewFailed = true; }
            emitterTexturePanel1.TextureChanged += (s, e) => RefreshPreview();   // repoint a texture slot -> live preview updates

            //Mesh selector + preview: its own column, left-aligned under the colour picker / Time label above it
            //(deliberate gap from the 3 texture slots). Same 3-row grid as the Textures panel: label row (y+4),
            //dropdown row (y+26), preview row (y+50) -> [Primitive Mesh] / [combo] / [thumb].
            int colX = colorSelector1.Left, topY = emitterTexturePanel1.Top;
            primitiveLabel = new STLabel() { Text = "Primitive Mesh", AutoSize = true,
                Location = new System.Drawing.Point(colX, topY + 4) };
            primitiveCB = new STComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160,
                Location = new System.Drawing.Point(colX, topY + 26) };
            primitiveCB.SelectedIndexChanged += PrimitiveSelected;
            primitiveThumb = new System.Windows.Forms.PictureBox() { BorderStyle = BorderStyle.FixedSingle,
                Size = new System.Drawing.Size(160, 120), Cursor = Cursors.Hand,
                Location = new System.Drawing.Point(colX, topY + 50) };
            primitiveThumb.Click += PrimitiveThumbClick; //click the preview to view the mesh
            stPanel2.Controls.Add(primitiveLabel);
            stPanel2.Controls.Add(primitiveCB);
            stPanel2.Controls.Add(primitiveThumb);
        }

        private PropertyGrid parameterGrid;
        // Live in-editor preview (see ctor). GL is created lazily + guarded so it never crashes the editor.
        private STPanel previewHost;
        private STCheckBox gpuPreviewCheck;
        private STLabel gpuStatusLabel;
        private Viewport previewViewport; private DrawableContainer previewContainer; private bool previewFailed = false;
        private EftEmitterRender previewRender;   // the render currently in the viewport, pulled out before the next one is added
        private EftGpuPreviewDrawable previewGpu; // the GPU-mode render, ditto
        private bool previewFramed = false;       // true once the first emitter has framed the camera; later emitters keep the view
        // Rebuild the preview render from the current emitter (resolved textures/mesh) and refresh the viewport.
        private void RefreshPreview()
        {
            if (previewFailed || ActiveEmitter == null || previewHost == null) return;
            try
            {
                if (!(Runtime.UseOpenGL && !Runtime.UseLegacyGL)) return;
                ActiveEmitter.FlushColorsToData();   // push edited colour/alpha tracks into EmitterData so the preview shows them
                if (gpuStatusLabel != null) gpuStatusLabel.Visible = false;
                // The GPU path renders the emitter's own shaders, which only the Wii U effect files carry; the
                // toggle stays hidden for formats that have none (3DS and Switch .pctl).
                bool hasShaders = ActiveEmitter.VtxGroupBytes != null && ActiveEmitter.FragGroupBytes != null;
                if (gpuPreviewCheck != null) gpuPreviewCheck.Visible = hasShaders;
                AbstractGlDrawable drawable = null;
                // Frame the camera only for the FIRST emitter shown this session; afterwards leave the camera where
                // the user put it, so switching between emitters (whose sizes vary a lot) does not snap the view back.
                if (hasShaders && gpuPreviewCheck != null && gpuPreviewCheck.Checked)
                {
                    // without its shipped inputs the GPU path would draw the shaders with the engine-bound banks
                    // and textures missing, which looks faithful and is not; stay on the software preview instead
                    string missing = EftGpuPreview.MissingAsset();
                    var gpu = missing == null ? BuildGpuPreview() : null;   // null also when the pair will not decompile
                    if (gpu != null)
                    {
                        gpu.AutoFrame = !previewFramed;
                        previewFramed = true;
                        drawable = gpu;
                    }
                    else if (gpuStatusLabel != null)
                    {
                        gpuStatusLabel.Text = missing != null
                            ? "GPU preview: " + missing + " is missing from the plugin folder, showing software preview"
                            : "GPU preview: no shader bundle in this emitter, showing software preview";
                        gpuStatusLabel.Visible = true;
                    }
                }
                EftEmitterRender render = null;
                if (drawable == null)
                {
                    var inp = EftEmitterRender.BuildInput(ActiveEmitter, "emitter");
                    if (inp == null || inp.Data == null) return;
                    render = new EftEmitterRender(new List<EftEmitterRender.EmitterInput> { inp });
                    render.AutoFrame = !previewFramed;
                    previewFramed = true;
                    drawable = render;
                }
                if (previewViewport == null)
                {
                    previewContainer = new DrawableContainer() { Name = "emitter" };
                    previewViewport = new Viewport(new List<DrawableContainer> { previewContainer }) { Dock = DockStyle.Fill };
                    previewHost.Controls.Add(previewViewport);
                }
                // Viewport.ReloadDrawables only ADDS to scene.staticObjects (it never removes), so clearing the container
                // alone leaves the previous emitter's render in the scene -> every selection stacks another emitter in
                // the preview. Explicitly pull the old render from the viewport's scene before swapping in the new one.
                if (previewRender != null) { previewViewport.RemoveDrawable(previewRender); previewRender.QueueDispose(); previewRender = null; }
                if (previewGpu != null) { previewViewport.RemoveDrawable(previewGpu); previewGpu.QueueDispose(); previewGpu = null; }
                previewContainer.Drawables.Clear();
                previewContainer.Drawables.Add(drawable);
                previewRender = drawable as EftEmitterRender;
                previewGpu = drawable as EftGpuPreviewDrawable;
                previewViewport.ReloadDrawables(previewContainer);
            }
            catch { previewFailed = true; }   // any GL/setup failure -> stop trying, leave a blank tab (editor unaffected)
        }

        // Editor teardown (swapping to another editor, or closing the file). The framework does not dispose a
        // swapped-out editor, and with GraphicsContext.ShareContexts on (OpenTKSharedResources) a preview's
        // program, textures and UBOs are SHARED objects that outlive their viewport's context, so retire the
        // preview showing at close: QueueDispose frees its GL objects on the next preview Draw (where a context
        // is current) and stops its repaint loop; disposing the viewport frees its GL control/context.
        public override void OnControlClosing()
        {
            try
            {
                if (previewGpu != null) { previewGpu.QueueDispose(); previewGpu = null; }
                if (previewRender != null) { previewRender.QueueDispose(); previewRender = null; }
                if (previewViewport != null) { previewViewport.Dispose(); previewViewport = null; }
            }
            catch { }
            base.OnControlClosing();
        }

        // GPU-preview inputs from the loaded file: the emitter's decompilable shader pair (cached at load
        // by MapShaderInfo) plus the payload/art assembled by EftGpuPreview.BuildInputs.
        private EftGpuPreviewDrawable BuildGpuPreview()
        {
            byte[] payload; List<EftGpuPreview.TextureInput> art;
            float[] meshVerts, meshNormals; int[] meshIndices;
            if (!EftGpuPreview.BuildInputs(ActiveEmitter, out payload, out art, out meshVerts, out meshNormals, out meshIndices))
                return null;
            var gpu = new EftGpuPreviewDrawable(ActiveEmitter.VtxGroupBytes, ActiveEmitter.FragGroupBytes, payload, art,
                                                meshVerts, meshNormals, meshIndices);
            gpu.ErrorReported = msg =>
            {
                if (gpuStatusLabel == null) return;
                gpuStatusLabel.Text = "GPU preview: " + msg;
                gpuStatusLabel.Visible = true;
            };
            return gpu;
        }
        private Thread Thread;
        PTCL.Emitter ActiveEmitter;

        private STComboBox primitiveCB;
        private STLabel primitiveLabel;
        private System.Windows.Forms.PictureBox primitiveThumb;
        private bool primLoading;
        private bool colorTypeLoading; //true while LoadEmitter sets the type combos, so the change handler is inert

        private IColorPanelCommon ActivePanel;

        private void LoadColors(STColor[] colors, PTCL.Emitter.ColorType colorType, int type)
        {
            STPanel panel = new STPanel();
            if (type == 0)
                panel = stPanel3;
            if (type == 1)
                panel = stPanel4;
            if (type == 2)
                panel = stPanel5;
            if (type == 3)
                panel = stPanel6;

            panel.Controls.Clear();

            if (colorType == PTCL.Emitter.ColorType.Animated8Key)
            {
                Color8KeySlider colorSlider = new Color8KeySlider();
                colorSlider.Dock = DockStyle.Fill;
                colorSlider.ColorSelected += ColorPanelSelected;
                colorSlider.KeysEdited += ColorSliderKeysEdited;
                colorSlider.TimeChanged += (s, e) => UpdateTimeDisplay(((Color8KeySlider)s).GetTime());
                panel.Controls.Add(colorSlider);
                colorSlider.IsAlpha = (type == 2 || type == 3);

                //type 0/1/2/3 == the track row; the slider edits the emitter's array + key count directly.
                colorSlider.LoadColors(colors, (int)ActiveEmitter.GetColorKeyCount(type), ActiveEmitter, type);
            }
            else if (colorType == PTCL.Emitter.ColorType.Random)
            {
                ColorRandomPanel colorRandomPnl = new ColorRandomPanel();
                colorRandomPnl.ColorSelected += ColorPanelSelected;
                panel.Controls.Add(colorRandomPnl);
                colorRandomPnl.IsAlpha = (type == 2 || type == 3);

                colorRandomPnl.LoadColors(colors);
            }
            else
            {
                ColorConstantPanel colorConstantPnl = new ColorConstantPanel();
                colorConstantPnl.ColorSelected += ColorPanelSelected;
                panel.Controls.Add(colorConstantPnl);
                colorConstantPnl.IsAlpha = (type == 2 || type == 3);

                if (type == 0)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantColor0);
                if (type == 1)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantColor1);
                if (type == 2)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantAlpha0);
                if (type == 3)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantAlpha1);
            }
        }

        private void ColorPanelSelected(object sender, EventArgs e)
        {
            var panel = sender as IColorPanelCommon;
            if (panel != null)
            {
                hexTB.Text = "";

                ActivePanel = panel;
                if (ActivePanel.IsAlpha)
                {
                    int alpha = panel.GetColor().R;

                    colorSelector1.DisplayColor = false;
                    colorSelector1.DisplayAlpha = true;
                    colorSelector1.Alpha = alpha;
                    UpdateColorSelector(Color.FromArgb(alpha, alpha, alpha));
                }
                else
                {
                    colorSelector1.DisplayColor = true;
                    colorSelector1.DisplayAlpha = false;
                    UpdateColorSelector(panel.GetColor());
                }


                if (panel is Color8KeySlider)
                    UpdateTimeDisplay(((Color8KeySlider)panel).GetTime());
            }
        }

        private void UpdateTimeDisplay(float time)
        {
            timeTB.Text = time.ToString();
        }

        public void LoadEmitter(PTCL.Emitter Emitter)
        {
            IsColorsLoaded = false;

            ActiveEmitter = Emitter;
            if (Emitter.EmitterData != null) BuildEftbUi();   // EFTB-only surface, built once on the first EFTB emitter
            LoadPrimitiveSelector(Emitter);

            //The Parameters tab is EFTB-only (its offsets are EFTB emitter-struct offsets), so it only exists once an
            //EFTB emitter has been loaded; bind the grid to this emitter when present.
            if (parameterGrid != null)
                parameterGrid.SelectedObject = Emitter.EmitterData != null
                    ? new PTCL.Emitter.EmitterParameters(Emitter) : null;

            colorTypeLoading = true;
            color0TypeCB.SelectedItem = Emitter.Color0Type;
            color1TypeCB.SelectedItem = Emitter.Color1Type;
            alpha0TypeCB.SelectedItem = Emitter.Alpha0Type;
            alpha1TypeCB.SelectedItem = Emitter.Alpha1Type;
            colorTypeLoading = false;

            LoadColors(Emitter.Color0Array, Emitter.Color0Type, 0);
            LoadColors(Emitter.Color1Array, Emitter.Color1Type, 1);
            LoadColors(Emitter.Color0AlphaArray, Emitter.Alpha0Type, 2);
            LoadColors(Emitter.Color1AlphaArray, Emitter.Alpha1Type, 3);

            stLabel1.Text = $"Color 0 ({Emitter.Color0KeyCount} Keys)";
            stLabel2.Text = $"Color 1 ({Emitter.Color1KeyCount} Keys)";
            stLabel3.Text = $"Alpha 0 ({Emitter.Alpha0KeyCount} Keys)";
            stLabel4.Text = $"Alpha 1 ({Emitter.Alpha1KeyCount} Keys)";

            UpdateColorSelector(Color.Black);

            IsColorsLoaded = true;

            //Always reload, even with 0 textures, so the panel clears the previous emitter's textures.
            emitterTexturePanel1.LoadTextures(Emitter);

            RefreshPreview();   // update the live preview for the newly-selected emitter
        }

        //Mesh dropdown + preview: shown only when the file has primitives (EFTB). Writes the link back to the emitter.
        private void LoadPrimitiveSelector(PTCL.Emitter emitter)
        {
            if (primitiveLabel == null) return;   //mesh column is part of the EFTB-only surface
            bool has = emitter.AvailablePrimitives != null && emitter.AvailablePrimitives.Count > 0;
            primitiveLabel.Visible = primitiveCB.Visible = primitiveThumb.Visible = has;
            if (!has)
            {
                //Free the previous emitter's thumbnail; nothing below runs to dispose it on this path.
                if (primitiveThumb.Image != null) { primitiveThumb.Image.Dispose(); primitiveThumb.Image = null; }
                return;
            }

            primLoading = true;
            primitiveCB.Items.Clear();
            foreach (var opt in emitter.PrimitiveOptions()) primitiveCB.Items.Add(opt);
            string cur = emitter.GetPrimitiveSelection();
            if (!primitiveCB.Items.Contains(cur)) primitiveCB.Items.Add(cur); //e.g. an external mesh not in this file
            primitiveCB.SelectedItem = cur;
            primLoading = false;
            UpdatePrimitivePreview();
        }

        private void PrimitiveSelected(object sender, EventArgs e)
        {
            if (primLoading || ActiveEmitter == null) return;
            ActiveEmitter.SetPrimitiveSelection(primitiveCB.SelectedItem as string);
            UpdatePrimitivePreview();
            RefreshPreview();
        }

        //The Primitive object behind the current dropdown value ("Primitive N"), or null for none/external.
        private PTCL.Primitive SelectedPrimitive()
        {
            string sel = primitiveCB.SelectedItem as string;
            if (ActiveEmitter == null || sel == null) return null;
            if (sel == PTCL.Emitter.PrimExternalLabel)   // external mesh: resolve from a sibling loaded .sesetlist (preview + navigate)
                return PTCL.FindGlobalPrimitive(ActiveEmitter.ExternalPrimHash);
            if (!sel.StartsWith("Primitive ")) return null;
            int idx;
            if (!int.TryParse(sel.Substring(10).Trim(), out idx)) return null;
            foreach (var p in ActiveEmitter.AvailablePrimitives)
                if (p.Index == idx) return p;
            return null;
        }

        private void UpdatePrimitivePreview()
        {
            var prim = SelectedPrimitive();
            primitiveThumb.Cursor = prim != null ? Cursors.Hand : Cursors.Default;
            if (primitiveThumb.Image != null) { primitiveThumb.Image.Dispose(); primitiveThumb.Image = null; }
            if (prim != null)
                primitiveThumb.Image = RenderMeshThumb(prim, primitiveThumb.Width, primitiveThumb.Height);
        }

        private void PrimitiveThumbClick(object sender, EventArgs e)
        {
            RevealNode(SelectedPrimitive());
        }

        //Reveal + select a node in the file explorer; the tree's selection handler then opens its viewer.
        public static void RevealNode(TreeNodeCustom node)
        {
            if (node == null) return;
            var tv = node.TreeView;
            if (tv == null) { node.OnClick(null); return; }
            if (tv.SelectedNode == node) node.OnClick(tv); //already selected: open the viewer directly
            else tv.SelectedNode = node;                    //changing selection fires OnClick via the tree handler
            node.EnsureVisible();
        }

        //Cheap orthographic wireframe of the mesh (no GL) so the shape is recognisable in-tab.
        private static Bitmap RenderMeshThumb(PTCL.Primitive prim, int w, int h)
        {
            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.FromArgb(45, 45, 48));
                var obj = prim.Objects != null ? prim.Objects.FirstOrDefault() : null;
                if (obj == null || obj.vertices.Count == 0) return bmp;

                //rotate (yaw around Y, then pitch around X) for a 3/4 view, then drop Z (orthographic)
                double cy = Math.Cos(0.6), sy = Math.Sin(0.6), cp = Math.Cos(0.45), sp = Math.Sin(0.45);
                int n = obj.vertices.Count;
                var px = new float[n]; var py = new float[n];
                float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
                for (int i = 0; i < n; i++)
                {
                    var p = obj.vertices[i].pos;
                    double z1 = -p.X * sy + p.Z * cy;
                    px[i] = (float)(p.X * cy + p.Z * sy);
                    py[i] = (float)(p.Y * cp - z1 * sp);
                    if (px[i] < minx) minx = px[i]; if (px[i] > maxx) maxx = px[i];
                    if (py[i] < miny) miny = py[i]; if (py[i] > maxy) maxy = py[i];
                }
                float bw = Math.Max(maxx - minx, 1e-4f), bh = Math.Max(maxy - miny, 1e-4f), pad = 8f;
                float scale = Math.Min((w - 2 * pad) / bw, (h - 2 * pad) / bh);
                var sx = new float[n]; var syc = new float[n];
                for (int i = 0; i < n; i++)
                {
                    sx[i] = (px[i] - minx) * scale + (w - bw * scale) / 2f;
                    syc[i] = h - ((py[i] - miny) * scale + (h - bh * scale) / 2f); //flip Y for screen
                }
                var faces = obj.faces;
                using (var pen = new Pen(Color.FromArgb(170, 120, 200, 255), 1f))
                    for (int i = 0; i + 3 <= faces.Count; i += 3)
                    {
                        int a = faces[i], b = faces[i + 1], c = faces[i + 2];
                        if (a >= 0 && b >= 0 && c >= 0 && a < n && b < n && c < n)
                        {
                            g.DrawLine(pen, sx[a], syc[a], sx[b], syc[b]);
                            g.DrawLine(pen, sx[b], syc[b], sx[c], syc[c]);
                            g.DrawLine(pen, sx[c], syc[c], sx[a], syc[a]);
                        }
                    }
            }
            return bmp;
        }

        bool IsColorsLoaded = false;

        public ColorAlphaBox GetColor(int colorType, int index)
        {
            foreach (Control control in stPanel2.Controls)
            {
                if (control.Name == $"color{colorType}Index{index}")
                    return (ColorAlphaBox)control;
            }
            return null;
        }

        public void RefreshColorBoxes()
        {
            foreach (Control control in stPanel2.Controls)
            {
                if (control is ColorAlphaBox)
                    control.Refresh();
            }
        }

        //User picked a new colour-track type: restructure the track's data in place, then refresh just that row.
        private void ColorTypeChanged(int row, STComboBox cb)
        {
            if (colorTypeLoading || ActiveEmitter == null) return;
            if (!(cb.SelectedItem is PTCL.Emitter.ColorType)) return;
            if (ActiveEmitter.SetColorTrackType(row, (PTCL.Emitter.ColorType)cb.SelectedItem))
                RefreshColorRow(row);
            RefreshPreview();   // colour-track type change -> live preview updates
        }
        //Reload one colour row's type combo + panel + "(N Keys)" label after its type/keys changed (rows: 0/1
        //colour, 2/3 alpha). The combo is set under colorTypeLoading so it doesn't re-fire a type conversion.
        private void RefreshColorRow(int row)
        {
            if (ActiveEmitter == null) return;
            colorTypeLoading = true;
            switch (row)
            {
                case 0: color0TypeCB.SelectedItem = ActiveEmitter.Color0Type; LoadColors(ActiveEmitter.Color0Array, ActiveEmitter.Color0Type, 0); break;
                case 1: color1TypeCB.SelectedItem = ActiveEmitter.Color1Type; LoadColors(ActiveEmitter.Color1Array, ActiveEmitter.Color1Type, 1); break;
                case 2: alpha0TypeCB.SelectedItem = ActiveEmitter.Alpha0Type; LoadColors(ActiveEmitter.Color0AlphaArray, ActiveEmitter.Alpha0Type, 2); break;
                case 3: alpha1TypeCB.SelectedItem = ActiveEmitter.Alpha1Type; LoadColors(ActiveEmitter.Color1AlphaArray, ActiveEmitter.Alpha1Type, 3); break;
            }
            colorTypeLoading = false;
            UpdateKeyCountLabel(row);
        }

        //Update just the "(N Keys)" label for a row (after add/remove of a keyframe).
        private void UpdateKeyCountLabel(int row)
        {
            if (ActiveEmitter == null) return;
            switch (row)
            {
                case 0: stLabel1.Text = $"Color 0 ({ActiveEmitter.Color0KeyCount} Keys)"; break;
                case 1: stLabel2.Text = $"Color 1 ({ActiveEmitter.Color1KeyCount} Keys)"; break;
                case 2: stLabel3.Text = $"Alpha 0 ({ActiveEmitter.Alpha0KeyCount} Keys)"; break;
                case 3: stLabel4.Text = $"Alpha 1 ({ActiveEmitter.Alpha1KeyCount} Keys)"; break;
            }
        }

        //A keyframe was added/removed on a slider: update its key-count label; if the track dropped to 0 keys it
        //is now Constant, so rebuild that row to swap in the constant panel + combo value.
        private void ColorSliderKeysEdited(object sender, EventArgs e)
        {
            var sl = sender as Color8KeySlider;
            if (sl == null || ActiveEmitter == null) return;
            int row = sl.Row;
            UpdateKeyCountLabel(row);
            if (ActiveEmitter.GetColorKeyCount(row) == 0)
                RefreshColorRow(row);
            RefreshPreview();   // add/remove a colour key -> live preview updates
        }

        //Apply the Time box value to the selected keyframe of the active 8-key slider (press Enter).
        private void CommitTimeEdit()
        {
            var sl = ActivePanel as Color8KeySlider;
            float t;
            if (sl != null && float.TryParse(timeTB.Text, out t))
                sl.SetSelectedKeyTime(t);
        }

        private void ExportImage0(object sender, EventArgs e)
        {
            ActiveEmitter.DrawableTex[0].ExportImage();
        }
        private void ReplaceImage0(object sender, EventArgs e)
        {
            if (ActiveEmitter is PTCL_WiiU.EmitterU)
            {
                var emitter = (PTCL_WiiU.EmitterU)ActiveEmitter;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                             "Microsoft DDS |*.dds|" +
                             "Portable Network Graphics |*.png|" +
                             "Joint Photographic Experts Group |*.jpg|" +
                             "Bitmap Image |*.bmp|" +
                             "Tagged Image File Format |*.tiff|" +
                             "All files(*.*)|*.*";

                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ((PTCL_WiiU.TextureInfo)emitter.DrawableTex[0]).Replace(ofd.FileName);
                }
            }
        }

        private void SetEmitterColor(Color color, int index, bool IsColor0)
        {
            if (IsColor0)
                ActiveEmitter.Color0Array[index].Color = color;
            else
                ActiveEmitter.Color1Array[index].Color = color;
        }

        public Color SetColor(Color input, Color output)
        {
            return Color.FromArgb(input.A, output.R, output.G, output.B);
        }

        private void hexTB_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                ((TextBox)sender).MaxLength = 8;

                if (((TextBox)sender).Text.Length != 8)
                    return;

                var color = Utils.HexToColor(((TextBox)sender).Text);
                UpdateColorSelector(color);
            }
        }

        private bool _UpdateSelector = true;
        private void colorSelector1_ColorChanged(object sender, EventArgs e)
        {
            if (!IsColorsLoaded)
                return;

            _UpdateSelector = false;

            hexTB.Text = Utils.ColorToHex(colorSelector1.Color);
            pictureBox4.BackColor = colorSelector1.Color;

            if (ActivePanel != null)
                ActivePanel.SetColor(colorSelector1.Color);

            _UpdateSelector = true;
            RefreshPreview();   // colour edit -> live preview updates
        }

        private void UpdateColorSelector(Color color) {
            if (_UpdateSelector)
                colorSelector1.Color = color;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}
