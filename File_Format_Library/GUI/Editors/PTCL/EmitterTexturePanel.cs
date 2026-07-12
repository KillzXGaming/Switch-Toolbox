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

namespace FirstPlugin.Forms
{
    public partial class EmitterTexturePanel : STUserControl
    {
        //One dropdown under each of the 3 previews: which file texture this emitter's sampler slot uses (EFTB).
        private readonly STComboBox[] slotCombos = new STComboBox[3];
        private bool slotLoading;

        public EmitterTexturePanel()
        {
            InitializeComponent();
            pictureBoxCustom1.Click += (s, e) => TextureClicked(0);
            pictureBoxCustom2.Click += (s, e) => TextureClicked(1);
            pictureBoxCustom3.Click += (s, e) => TextureClicked(2);
            pictureBoxCustom1.Cursor = pictureBoxCustom2.Cursor = pictureBoxCustom3.Cursor = Cursors.Hand;

            //One texture-slot dropdown above each preview (EFTB only). Shared row grid with the primitive column
            //in EmitterEditorNX: labels y=4, dropdowns y=26, previews y=50.
            int[] xs = { 3, 79, 155 };
            for (int i = 0; i < 3; i++)
            {
                int slot = i;
                var cb = new STComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 72,
                    Location = new Point(xs[i], 26),
                    Visible = false,
                };
                cb.SelectedIndexChanged += (s, e) => SlotChanged(slot);
                slotCombos[i] = cb;
                Controls.Add(cb);
            }

            //Make each slot's role obvious on hover (the 233x124 panel has no room for a caption row). nw::eft gives
            //every emitter a FIXED 3 samplers: two color inputs (combined by the blend op) + the engine scene buffer.
            var slotTip = new ToolTip { AutoPopDelay = 20000, InitialDelay = 300, ReshowDelay = 100 };
            string[] tips = {
                "Slot 0  -  primary colour / art texture (sampler 0).",
                "Slot 1  -  second texture, combined with slot 0 by the emitter blend op: Multiply (mask) / Add (glow) / Subtract (sampler 1).",
                "Slot 2  -  third texture input (sampler 2): a sub / detail texture combined with slots 0-1 by the emitter blend op. Used by ~370 emitters (smoke noise, layered flame, ...)."
            };
            var boxes = new[] { pictureBoxCustom1, pictureBoxCustom2, pictureBoxCustom3 };
            for (int i = 0; i < 3; i++) { slotTip.SetToolTip(slotCombos[i], tips[i]); slotTip.SetToolTip(boxes[i], tips[i]); }
        }

        //Click a texture preview to reveal + open that texture in the file explorer.
        private void TextureClicked(int index)
        {
            if (ActiveEmitter == null) return;
            var tex = ActiveEmitter.GetSamplerTexture(index); //slot-indexed (EFTB)
            if (tex != null) { global::FirstPlugin.EmitterEditorNX.RevealNode(tex); return; }
            if (index < ActiveEmitter.DrawableTex.Count)       //fallback for non-EFTB previews
                global::FirstPlugin.EmitterEditorNX.RevealNode(ActiveEmitter.DrawableTex[index] as Toolbox.Library.TreeNodeCustom);
        }

        Thread Thread;

        private PTCL.Emitter ActiveEmitter;
        public void LoadTextures(PTCL.Emitter emitter)
        {
            ActiveEmitter = emitter;

            //EFTB exposes a texture table, so drive the 3 previews slot-by-slot and offer assignment dropdowns.
            //Other PTCL variants keep the legacy DrawableTex preview with no dropdowns.
            bool eftb = emitter != null && emitter.AvailableTextures != null && emitter.AvailableTextures.Count > 0;
            for (int i = 0; i < 3; i++) slotCombos[i].Visible = eftb;

            pictureBoxCustom1.Visible = false;
            pictureBoxCustom2.Visible = false;
            pictureBoxCustom3.Visible = false;

            pictureBoxCustom1.Image = Toolbox.Library.Properties.Resources.LoadingImage;
            pictureBoxCustom2.Image = Toolbox.Library.Properties.Resources.LoadingImage;
            pictureBoxCustom3.Image = Toolbox.Library.Properties.Resources.LoadingImage;

            if (emitter == null) return;
            if (eftb) LoadSlotCombos(emitter);

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                if (eftb)
                {
                    for (int i = 0; i < 3; i++) RenderSlot(emitter, i);
                }
                else
                {
                    for (int i = 0; i < emitter.DrawableTex.Count; i++)
                    {
                        //same raw-thread constraint as RenderSlot: a decode failure must not kill the process
                        Bitmap image = null;
                        try
                        {
                            image = emitter.DrawableTex[i].GetBitmap();
                            image = emitter.DrawableTex[i].GetComponentBitmap(image, showAlphaChk.Checked);
                        }
                        catch (Exception ex) { Console.WriteLine("LoadTextures " + i + ": " + ex.Message); }
                        SafeUpdate(i, image);
                    }
                }
            }));
            Thread.Start();
        }

        //Populate each slot dropdown with "none" + every file texture, selecting the slot's current binding.
        private void LoadSlotCombos(PTCL.Emitter emitter)
        {
            slotLoading = true;
            for (int i = 0; i < 3; i++)
            {
                var cb = slotCombos[i];
                cb.Items.Clear();
                foreach (var opt in emitter.TextureOptions()) cb.Items.Add(opt);
                string cur = emitter.GetTextureSelection(i);
                if (!cb.Items.Contains(cur)) cb.Items.Add(cur); //external / just-deleted id not in the table
                cb.SelectedItem = cur;
            }
            slotLoading = false;
        }

        //Render the texture bound to a sampler slot into its preview box (hides the box when the slot is empty).
        //Runs on a raw background thread, where any uncaught exception kills the PROCESS; a texture that
        //fails to decode (e.g. a degenerate GX2 surface) must show an empty preview, not crash the toolbox.
        private void RenderSlot(PTCL.Emitter emitter, int slot)
        {
            Bitmap image = null;
            try
            {
                var tex = emitter.GetSamplerTexture(slot);
                if (tex != null)
                {
                    image = tex.GetBitmap();
                    image = tex.GetComponentBitmap(image, showAlphaChk.Checked);
                }
            }
            catch (Exception ex) { Console.WriteLine("RenderSlot " + slot + ": " + ex.Message); }
            SafeUpdate(slot, image);
        }

        //Raised after a sampler slot is repointed, so the editor can refresh the live preview.
        public event EventHandler TextureChanged;

        private void SlotChanged(int slot)
        {
            if (slotLoading || ActiveEmitter == null) return;
            ActiveEmitter.SetTextureSelection(slot, slotCombos[slot].SelectedItem as string);
            TextureChanged?.Invoke(this, EventArgs.Empty);
            //Re-render just this slot off the UI thread so a slow GX2 decode doesn't freeze the click.
            var emitter = ActiveEmitter;
            new Thread((ThreadStart)(() => RenderSlot(emitter, slot))).Start();
        }

        private void SafeUpdate(int index, Bitmap image)
        {
            if (InvokeRequired) Invoke((MethodInvoker)delegate { UpdatePicturebox(index, image); });
            else UpdatePicturebox(index, image);
        }

        private void UpdatePicturebox(int index, Bitmap image)
        {
            var box = index == 0 ? pictureBoxCustom1 : index == 1 ? pictureBoxCustom2 : pictureBoxCustom3;
            box.Visible = image != null;
            box.Image = image;
        }

        private void showAlphaChk_CheckedChanged(object sender, EventArgs e) {
            LoadTextures(ActiveEmitter);
        }
    }
}
