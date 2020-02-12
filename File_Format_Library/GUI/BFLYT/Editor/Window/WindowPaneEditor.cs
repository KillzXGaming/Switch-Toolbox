using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace LayoutBXLYT
{
    public partial class WindowPaneEditor : EditorPanelBase
    {
        private IWindowPane ActivePane;
        private ContentType EditorContentType;
        private PaneEditor ParentEditor;

        public enum ContentType
        {
            WindowContent,
            Textures,
            ColorInterpolation,
            TextureCombiners,
            Blending,
            TevSwapTable,
        }

        public WindowPaneEditor()
        {
            InitializeComponent();

            windowFrameEditor1.FrameSelectChanged += OnWindowFrameChanged;
        }

        public BxlytMaterial GetActiveMaterial()
        {
            return windowFrameEditor1.GetActiveMaterial(ActivePane);
        }

        public BxlytWindowFrame GetActiveFrame()
        {
            return windowFrameEditor1.GetActiveFrame(ActivePane);
        }

        public void LoadPane(IWindowPane pane, ContentType contentType, PaneEditor paneEditor)
        {
            bool HasChanged = (pane != ActivePane);

            ActivePane = pane;
            ParentEditor = paneEditor;
            EditorContentType = contentType;
            windowFrameEditor1.LoadPane(pane, paneEditor, HasChanged);

            LoadEditors();
        }

        private void OnWindowFrameChanged(object sender, EventArgs e) {
            LoadEditors();
        }

        private void LoadEditors()
        {
            var mat = GetActiveMaterial();
            if (mat is Revolution.Material)
            {
                switch (EditorContentType)
                {
                    case ContentType.Textures:
                        var textureEditor = GetActiveEditor<PaneMatTextureMapsEditor>();
                        textureEditor.LoadMaterial(mat, ParentEditor, ParentEditor.GetTextures());
                        break;
                    case ContentType.WindowContent:
                        var contentEditor = GetActiveEditor<WindowContentEditor>();
                        contentEditor.LoadPane(ActivePane, GetActiveFrame(), ParentEditor);
                        break;
                    case ContentType.ColorInterpolation:
                        var colorEditor = GetActiveEditor<PaneMatRevColorEditor>();
                        colorEditor.LoadMaterial((Revolution.Material)mat, ParentEditor);
                        break;
                    case ContentType.TextureCombiners:
                        var texComb = GetActiveEditor<Revolution.PaneMatRevTevEditor>();
                        texComb.LoadMaterial((Revolution.Material)mat, ParentEditor);
                        break;
                    case ContentType.Blending:
                        var matBlend = GetActiveEditor<PaneMatRevBlending>();
                        matBlend.LoadMaterial((Revolution.Material)mat, ParentEditor);
                        break;
                    case ContentType.TevSwapTable:
                        var tevSwapEditor = GetActiveEditor<PaneMatRevTevSwapTableEditor>();
                        tevSwapEditor.LoadMaterial((Revolution.Material)mat, ParentEditor);
                        break;
                }
            }
            else if (mat is CTR.Material)
            {
                switch (EditorContentType)
                {
                    case ContentType.Textures:
                        var textureEditor = GetActiveEditor<PaneMatTextureMapsEditor>();
                        textureEditor.LoadMaterial(mat, ParentEditor, ParentEditor.GetTextures());
                        break;
                    case ContentType.WindowContent:
                        var contentEditor = GetActiveEditor<WindowContentEditor>();
                        contentEditor.LoadPane(ActivePane, GetActiveFrame(), ParentEditor);
                        break;
                    case ContentType.ColorInterpolation:
                        var colorEditor = GetActiveEditor<CTR.PaneMatCTRColorEditor>();
                        colorEditor.LoadMaterial((CTR.Material)mat, ParentEditor);
                        break;
                    case ContentType.TextureCombiners:
                        var texComb = GetActiveEditor<CTR.PaneMatCTRTevEditor>();
                        texComb.LoadMaterial((CTR.Material)mat, ParentEditor);
                        break;
                    case ContentType.Blending:
                        var matBlend = GetActiveEditor<PaneMatBlending>();
                        matBlend.LoadMaterial(mat, ParentEditor);
                        break;
                }
            }
            else
                    {
                switch (EditorContentType)
                {
                    case ContentType.Textures:
                        var textureEditor = GetActiveEditor<PaneMatTextureMapsEditor>();
                        textureEditor.LoadMaterial(mat, ParentEditor, ParentEditor.GetTextures());
                        break;
                    case ContentType.WindowContent:
                        var contentEditor = GetActiveEditor<WindowContentEditor>();
                        contentEditor.LoadPane(ActivePane, GetActiveFrame(), ParentEditor);
                        break;
                    case ContentType.ColorInterpolation:
                        var colorEditor = GetActiveEditor<PaneMatColorEditor>();
                        colorEditor.LoadMaterial(mat, ParentEditor);
                        break;
                    case ContentType.TextureCombiners:
                        var texComb = GetActiveEditor<PaneMatTextureCombiner>();
                        texComb.LoadMaterial(mat, ParentEditor);
                        break;
                    case ContentType.Blending:
                        var matBlend = GetActiveEditor<PaneMatBlending>();
                        matBlend.LoadMaterial(mat, ParentEditor);
                        break;
                }
            }
        }

        private Control ActiveEditor
        {
            get
            {
                if (stPanel1.Controls.Count == 0) return null;
                return stPanel1.Controls[0];
            }
        }

        private T GetActiveEditor<T>() where T : Control, new()
        {
            T instance = new T();

            if (ActiveEditor?.GetType() == instance.GetType())
                return ActiveEditor as T;
            else
            {
                DisposeEdtiors();
                stPanel1.Controls.Clear();
                instance.Dock = DockStyle.Fill;
                stPanel1.Controls.Add(instance);
            }

            return instance;
        }

        public override void OnControlClosing() {
            DisposeEdtiors();
        }

        private void DisposeEdtiors()
        {
            if (ActiveEditor == null) return;
            if (ActiveEditor is STUserControl)
                ((STUserControl)ActiveEditor).OnControlClosing();
        }
    }
}
