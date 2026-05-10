using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Toolbox.Library.Forms;

namespace Toolbox.Library
{
    public abstract class TreeNodeCustom : TreeNode
    {
        public virtual void OnClick(TreeView treeview) { }
        public virtual void OnMouseLeftClick(TreeView treeview) { }
        public virtual void OnMouseRightClick(TreeView treeview) { }
        public virtual void OnDoubleMouseClick(TreeView treeview) { }

        public virtual void OnExpand() { }
        
        public TreeNodeCustom()
        {
        }
        public TreeNodeCustom RootNode
        {
            get
            {
                TreeNodeCustom currentNode = this;
                while (currentNode.Parent != null)
                {
                    currentNode = currentNode.Parent as TreeNodeCustom;
                }

                return currentNode;
            }
        }
    }

public class TreeNodeFile : TreeNodeCustom
    {
        public bool CanDelete
        {
            set
            {
                if (value == true)
                {
                    if (ContextMenuStrip == null)
                        ContextMenuStrip = new STContextMenuStrip();

                    ContextMenuStrip.Items.Add(new STToolStipMenuItem("Delete", null, Delete, Keys.Control | Keys.Delete));
                }
            }
        }

        public virtual UserControl GetEditor() { return new STUserControl() { Dock = DockStyle.Fill }; }
        public virtual void FillEditor(UserControl control) { }

        public override void OnClick(TreeView treeview)
        {
            var Editor = GetEditor();
            Editor.Dock = DockStyle.Fill;

            var ActiveEditor = LibraryGUI.GetActiveContent(Editor.GetType());
            if (ActiveEditor != null)
                Editor = ActiveEditor;
            else
                LibraryGUI.LoadEditor(Editor);

            FillEditor(Editor);
        }

        public TreeNodeFile()
        {

        }

        public TreeNodeFile(string text)
        {
            Text = text;
        }

        private void Delete(object sender, EventArgs args)
        {
            var editor = LibraryGUI.GetObjectEditor();
            if (editor != null)
            {
                editor.ResetControls();
            }
        }

        public virtual void OnAfterAdded() //After added to treeview
        {

        }
    }

    public class TreeViewCustom : TreeView
    {
        private readonly Dictionary<int, TreeNode> _treeNodes = new Dictionary<int, TreeNode>();

        public List<ITextureContainer> TextureIcons = new List<ITextureContainer>();
        public List<ISingleTextureIconLoader> SingleTextureIcons = new List<ISingleTextureIconLoader>();

        public TreeViewCustom()
        {
            ReloadImages();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public static ImageList imgList = new ImageList();

        public void AddImage(Image image)
        {
            imgList.Images.Add(image);
        }

        private Thread Thread;
        public void ReloadTextureIcons(bool CanAbortThread)
        {
            if (Thread != null && Thread.IsAlive && CanAbortThread)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                for (int i = 0; i < TextureIcons.Count; i++)
                {
                    if (!TextureIcons[i].DisplayIcons)
                        continue;

                    foreach (TreeNode node in TextureIcons[i].TextureList)
                    {
                        if (node is STGenericTexture)
                        {
                            try
                            {
                                var image = ((STGenericTexture)node).GetBitmap();
                                if (image != null)
                                    AddImageOnThread(image, node);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                for (int i = 0; i < SingleTextureIcons.Count; i++)
                {
                    if (SingleTextureIcons[i] == null || SingleTextureIcons[i].IconTexture == null)
                        continue;

                    try
                    {
                        var image = SingleTextureIcons[i].IconTexture.GetBitmap();
                        if (image != null)
                            AddImageOnThread(image, SingleTextureIcons[i].IconTexture);
                    }
                    catch
                    {

                    }
                }
            }));
            Thread.Start();
        }

        public void ReloadTextureIcons(List<ISingleTextureIconLoader> textureIcons, bool CanAbortThread)
        {
            if (Thread != null && Thread.IsAlive && CanAbortThread)
                Thread.Abort();

            this.BeginUpdate();

            Thread = new Thread((ThreadStart)(() =>
            {
                List<TreeNode> treeNodes = new List<TreeNode>();
                List<Image> imageIcons = new List<Image>();

                foreach (TreeNode node in textureIcons)
                {
                    if (node is STGenericTexture)
                    {
                        try
                        {
                            var image = ((STGenericTexture)node).GetBitmap();
                            if (image != null)
                            {
                                image = ((STGenericTexture)node).GetComponentBitmap(image);

                                imageIcons.Add(image);
                                treeNodes.Add(node);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }


                for (int i = 0; i < treeNodes.Count; i++)
                {
                    AddImageOnThread(i, treeNodes[i]);
                }

                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.ImageList.Images.AddRange(imageIcons.ToArray());
                    });
                }

                for (int i = 0; i < treeNodes.Count; i++)
                    imageIcons[i].Dispose();

                imageIcons.Clear();
                treeNodes.Clear();
                textureIcons.Clear();

                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.EndUpdate();
                    });
                }
            }));
            Thread.Start();
        }

        public void ReloadTextureIcons(ISingleTextureIconLoader textureIcon, Image image)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ReloadTextureIcons(textureIcon.IconTexture, image);
                });
            }
            else
            {
                ReloadTextureIcons(textureIcon.IconTexture, image);
            }
        }

        private void ReloadTextureIcons(TreeNode node, Image image)
        {
            image = BitmapExtension.Resize(image, this.ImageList.ImageSize);

            if (node.ImageIndex != -1 && node.ImageIndex < this.ImageList.Images.Count)
                this.ImageList.Images[node.ImageIndex] = image;
            else
            {
                node.ImageIndex = this.ImageList.Images.Count;
                node.SelectedImageIndex = node.ImageIndex;

                this.ImageList.Images.Add(image);
            }
            this.Refresh();
        }

        public void ReloadTextureIcons(ISingleTextureIconLoader textureIcon, bool CanAbortThread)
        {
            if (Thread != null && Thread.IsAlive && CanAbortThread)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                try
                {
                    var image = textureIcon.IconTexture.GetBitmap();
                    image = textureIcon.IconTexture.GetComponentBitmap(image);
                    AddImageOnThread(image, textureIcon.IconTexture);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }));
            Thread.Start();
        }

        public void ReloadTextureIcons(ITextureContainer textureIconList)
        {
            if (!textureIconList.DisplayIcons)
                return;

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            this.BeginUpdate();

            Thread = new Thread((ThreadStart)(() =>
            {
                List<TreeNode> treeNodes = new List<TreeNode>();
                List<Image> imageIcons = new List<Image>();

                foreach (TreeNode node in textureIconList.TextureList)
                {
                    if (node is STGenericTexture)
                    {
                        try
                        {
                            var image = ((STGenericTexture)node).GetBitmap();
                            if (image != null)
                            {
                                image = ((STGenericTexture)node).GetComponentBitmap(image);
                                treeNodes.Add(node);
                                imageIcons.Add(image);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }


                for (int i = 0; i < treeNodes.Count; i++)
                {
                    AddImageOnThread(i, treeNodes[i]);
                }

                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.ImageList.Images.AddRange(imageIcons.ToArray());
                    });
                }

                for (int i = 0; i < treeNodes.Count; i++)
                    imageIcons[i].Dispose();

                imageIcons.Clear();
                treeNodes.Clear();

               if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.EndUpdate();
                    });
                }
            }));
            Thread.Start();
        }

        public void AddImageOnThread(Image image, TreeNode node)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread

                    node.ImageIndex = this.ImageList.Images.Count;
                    node.SelectedImageIndex = node.ImageIndex;

                    imgList.Images.Add(image);

                    image.Dispose();
                });
            }
        }

        public void AddImageOnThread(int index, TreeNode node)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread

                    node.ImageIndex = this.ImageList.Images.Count + index;
                    node.SelectedImageIndex = node.ImageIndex;
                });
            }
        }

        public void ReloadImages(int Width = 22, int Height = 22)
        {
            imgList = new ImageList();
            imgList.ColorDepth = ColorDepth.Depth32Bit;
            imgList.ImageSize = new Size(Width, Height);
            imgList.Images.Add("folder", Properties.Resources.Folder);
            imgList.Images.Add("resource", Properties.Resources.Folder);
            imgList.Images.Add("Texture", Properties.Resources.Texture);
            imgList.Images.Add("fileBlank", Properties.Resources.FileBlank);
            imgList.Images.Add("bfres", Properties.Resources.Bfres);
            imgList.Images.Add("byaml", Properties.Resources.Byaml);
            imgList.Images.Add("aamp", Properties.Resources.Aamp);
            imgList.Images.Add("bntx", Properties.Resources.Bntx);
            imgList.Images.Add("bfsha", Properties.Resources.Bfsha);
            imgList.Images.Add("bnsh", Properties.Resources.Bnsh);
            imgList.Images.Add("mesh", Properties.Resources.mesh);
            imgList.Images.Add("skeletonAnimation", Properties.Resources.skeletonAnimation);
            imgList.Images.Add("bone", Properties.Resources.Bone);
            imgList.Images.Add("bfwav", Properties.Resources.Music1);
            imgList.Images.Add("bfstp", Properties.Resources.Music2);
            imgList.Images.Add("material", Properties.Resources.materialSphere);
            imgList.Images.Add("model", Properties.Resources.model);
            imgList.Images.Add("folder", Properties.Resources.skeleton);
            imgList.Images.Add("TextureMaterialMap", Properties.Resources.TextureMaterialMap);

            imgList.Images.Add("MetaInfo", Properties.Resources.MetaInfo);

            imgList.Images.Add("MaterialTranslucent", Properties.Resources.materialSphereTranslucent);
            imgList.Images.Add("MaterialTransparent", Properties.Resources.materialSphereTransparent);

            //Data types
            imgList.Images.Add("bool", Properties.Resources.IconBool);
            imgList.Images.Add("buffer", Properties.Resources.IconBuffer);
            imgList.Images.Add("curve", Properties.Resources.IconCurve);
            imgList.Images.Add("float", Properties.Resources.IconFloat);
            imgList.Images.Add("int", Properties.Resources.IconInt);
            imgList.Images.Add("list", Properties.Resources.IconList);
            imgList.Images.Add("object", Properties.Resources.IconObject);
            imgList.Images.Add("vec2", Properties.Resources.IconVec2);
            imgList.Images.Add("vec3", Properties.Resources.IconVec3);
            imgList.Images.Add("vec4", Properties.Resources.IconVec4);

            Bitmap bmp = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);

            imgList.Images.Add("empty", bmp);

            
            this.ImageList = imgList;
        }

        /// <summary>
        /// Load the TreeView with items.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="items">Collection of items</param>
        /// <param name="getId">Function to parse Id value from item object</param>
        /// <param name="getParentId">Function to parse parentId value from item object</param>
        /// <param name="getDisplayName">Function to parse display name
        /// value from item object. This is used as node text.</param>
        public void LoadItems<T>(IEnumerable<T> items, Func<T, int> getId,
               Func<T, int?> getParentId, Func<T, string> getDisplayName)
        {
            // Clear view and internal dictionary
            Nodes.Clear();
            _treeNodes.Clear();

            // Load internal dictionary with nodes
            foreach (var item in items)
            {
                var id = getId(item);
                var displayName = getDisplayName(item);
                var node = new TreeNode
                {
                    Name = id.ToString(),
                    Text = displayName,
                    Tag = item
                };
                _treeNodes.Add(getId(item), node);
            }

            // Create hierarchy and load into view
            foreach (var id in _treeNodes.Keys)
            {
                var node = GetNode(id);
                var obj = (T)node.Tag;
                var parentId = getParentId(obj);
                if (parentId.HasValue)
                {
                    var parentNode = GetNode(parentId.Value);
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    Nodes.Add(node);
                }
            }
        }
        /// <summary>
        /// Retrieve TreeNode by Id.
        /// Useful when you want to select a specific node.
        /// </summary>
        /// <param name="id">Item id</param>
        public TreeNode GetNode(int id)
        {
            return _treeNodes[id];
        }
    }
    public static class TreeViewExtensions
    {
        public static IEnumerable<TreeNode> Collect(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;

                foreach (var child in Collect(node.Nodes))
                    yield return child;
            }
        }


        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam,
                                                 ref TVITEM lParam);

        /// <summary>
        /// Hides the checkbox for the specified node on a TreeView control.
        /// </summary>
        public static void HideCheckBox(this TreeNode node)
        {
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            SendMessage(node.TreeView.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
        }
    }
}
