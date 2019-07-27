using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Animations;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class BfresTexturePatternEditor : STForm
    {
        public PlayerState AnimationPlayerState = PlayerState.Stop;

        public enum PlayerState
        {
            Playing,
            Pause,
            Stop,
        }

        public bool IsLooping
        {
            get { return loopChkBox.Checked; }
            set { loopChkBox.Checked = value; }
        }

        public bool IsPlaying
        {
            get
            {
                return AnimationPlayerState == PlayerState.Playing;
            }
        }

        ImageList imgList = new ImageList();

        private List<MaterialAnimation> MaterialAnimations;
        public BfresTexturePatternEditor(TreeNodeCollection materialAnimations)
        {
            InitializeComponent();

            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;
            listViewCustom1.BackColor = FormThemes.BaseTheme.TextEditorBackColor;
            imgList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(32, 32),
            };

            stPanel4.BackColor = FormThemes.BaseTheme.FormBackColor;

            timer1.Interval = 100 / 60;

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            toolstripShiftUp.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            foreach (var type in Enum.GetValues(typeof(Runtime.PictureBoxBG)).Cast<Runtime.PictureBoxBG>())
                backgroundCB.Items.Add(type);

            backgroundCB.SelectedItem = Runtime.pictureBoxStyle;

            MaterialAnimations = new List<MaterialAnimation>();
            foreach (TreeNode matAnim in materialAnimations)
            {
                MaterialAnimations.Add((MaterialAnimation)matAnim);
                activeAnimCB.Items.Add(matAnim.Text);
            }
        }

        MaterialAnimation.SamplerKeyGroup activeSampler;

        MaterialAnimation _activeMaterialAnim;
        MaterialAnimation ActiveMaterialAnim
        {
            get
            {
                return _activeMaterialAnim;
            }
            set
            {
                _activeMaterialAnim = value;

                maxFrameCounterUD.Maximum = value.FrameCount;
                maxFrameCounterUD.Value = value.FrameCount;
                currentFrameCounterUD.Maximum = value.FrameCount;
                animationTrackBar.TickDivide = 1;
                animationTrackBar.Maximum = value.FrameCount;
                animationTrackBar.Minimum = 0;
                currentFrameCounterUD.Value = 0;

                frameCountUD.Maximum = value.FrameCount;
                frameCountUD.Value = value.FrameCount;
            }
        }

        private void OnPropertyChanged()
        {

        }
    
        public bool IsLoaded = false;
        public void LoadAnim(MaterialAnimation materialAnim)
        {
            if (materialAnim.Materials.Count <= 0)
                return;

            IsLoaded = false;

            ReloadAnimationView(materialAnim);

            IsLoaded = true;

            activeAnimCB.SelectItemByText(materialAnim.Text);
            animationTrackBar.Value = 0;
        }

        private void ReloadAnimationView(MaterialAnimation materialAnim)
        {
            treeView1.Nodes.Clear();

            int Index = 0;
            foreach (var mat in materialAnim.Materials)
            {
                mat.Nodes.Clear();

                var matWrapper = new TreeNode(mat.Text) { Tag = mat, };
                treeView1.Nodes.Add(matWrapper);

                foreach (var sampler in mat.Samplers)
                    matWrapper.Nodes.Add(new TreeNode(sampler.Text) { Tag = sampler, });

                if (matWrapper.Nodes.Count > 0 && Index == 0)
                    treeView1.SelectedNode = matWrapper.Nodes[0];

                Index++;
            }


            ReloadAnimationView();
        }

        private void ReloadAnimationView()
        {
            editToolStripMenuItem.Enabled = false;

            listViewCustom1.SuspendLayout();
            listViewCustom1.Items.Clear();

            LoadAniamtion(ActiveMaterialAnim, activeSampler);

            listViewCustom1.ResumeLayout();

            if (listViewCustom1.Items.Count > 0)
            {
                listViewCustom1.Items[0].Selected = true;
                listViewCustom1.Select();

                editToolStripMenuItem.Enabled = true;
            }
        }

        Dictionary<int, Bitmap> Images = new Dictionary<int, Bitmap>();
        public List<int> KeyFrames = new List<int>(); //Used for getting the frame of the list item

        public bool IsLoading = false;
        private Thread Thread;
        private void LoadAniamtion(MaterialAnimation anim, MaterialAnimation.SamplerKeyGroup activeSampler)
        {
            if (activeSampler == null || IsLoading)
                return;

            imgList.Images.Clear();
            Images.Clear();
            KeyFrames.Clear();

            listViewCustom1.SmallImageList = imgList;
            listViewCustom1.Items.Clear();
            listViewCustom1.View = View.SmallIcon;

            int ImageIndex = 0;
            for (int Frame = 0; Frame <= anim.FrameCount; Frame++)
            {
                //Constants always show so break after first frame
                if (activeSampler.Constant && Frame != 0)
                    break;

                var keyFrame = activeSampler.GetKeyFrame(Frame);
                if (keyFrame.IsKeyed || activeSampler.Constant)
                {
                    AddKeyFrame((int)keyFrame.Value, Frame, ImageIndex++);
                }
            }

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                for (int Frame = 0; Frame <= anim.FrameCount; Frame++)
                {
                    //Constants always show so break after first frame
                    if (activeSampler.Constant && Frame != 0)
                        break;

                    var keyFrame = activeSampler.GetKeyFrame(Frame);
                    if (keyFrame.IsKeyed || activeSampler.Constant)
                    {
                        AddKeyFrameImage((int)keyFrame.Value, Frame);
                    }
                }

                IsLoading = true;

                if (listViewCustom1.InvokeRequired)
                {
                    listViewCustom1.Invoke((MethodInvoker)delegate
                    {
                        if (listViewCustom1.Items.Count > 0)
                        {
                            listViewCustom1.Items[0].Selected = true;
                            listViewCustom1.Select();
                        }
                    });
                }
                else
                {
                    if (listViewCustom1.Items.Count > 0)
                    {
                        listViewCustom1.Items[0].Selected = true;
                        listViewCustom1.Select();
                    }
                }


                IsLoading = false;
            }));
            Thread.Start();
        }

        private void AddKeyFrameImage(int Index, int Frame)
        {
            KeyFrames.Add(Frame);

            var tex = activeSampler.GetActiveTexture((int)Index);
            if (tex != null)
            {
                Bitmap temp = tex.GetBitmap();

                if (!Images.ContainsKey(Frame))
                    Images.Add(Frame, temp);
                else
                {
                    Images.Remove(Frame);
                    Images.Add(Frame, temp);
                }

                if (listViewCustom1.InvokeRequired)
                {
                    listViewCustom1.Invoke((MethodInvoker)delegate {
                        // Running on the UI thread
                        imgList.Images.Add(temp);
                        var dummy = imgList.Handle;

                        listViewCustom1.Refresh();
                    });
                }
            }
        }

        private void AddKeyFrame(int Index, int Frame, int ImageIndex)
        {
            if (activeSampler == null)
                return;

            bool IsValidIndex = Index < ActiveMaterialAnim.Textures.Count && Index != -1;
            if (!IsValidIndex) //Indices can be invalid for example if an animation is switched quickly in editor
                return;

            string TextureKey = activeSampler.GetActiveTextureNameByIndex((int)Index);

            var tex = activeSampler.GetActiveTexture((int)Index);
            if (tex != null)
                listViewCustom1.Items.Add($"{Frame} / {ActiveMaterialAnim.FrameCount} \n" + tex.Text, ImageIndex);
            else
                listViewCustom1.Items.Add($"{Frame} / {ActiveMaterialAnim.FrameCount} \n" + TextureKey, ImageIndex);
        }

        private void SelectThumbnailItems()
        {

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (AnimationPlayerState == PlayerState.Playing) 
                Pause();
            else
                Play();
        }


        private void btnStop_Click(object sender, EventArgs e) {
            Stop();
        }

        private void Play()
        {
            timer1.Start();
            AnimationPlayerState = PlayerState.Playing;
            UpdateAnimationUI();
        }

        private void Pause()
        {
            timer1.Stop();
            AnimationPlayerState = PlayerState.Stop;
            UpdateAnimationUI();
        }

        private void Stop()
        {
            timer1.Stop();
            animationTrackBar.Value = 0;
            AnimationPlayerState = PlayerState.Stop;
            UpdateAnimationUI();
        }

        private void UpdateAnimationUI()
        {
            btnPlay.BackgroundImage = IsPlaying ? Toolbox.Library.Properties.Resources.PauseBtn
                : Toolbox.Library.Properties.Resources.PlayArrowR;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AnimationPlayerState == PlayerState.Playing)
            {
                if (animationTrackBar.Value == animationTrackBar.Maximum)
                {
                    if (IsLooping)
                        animationTrackBar.Value = 0;
                    else
                        Stop();
                }
                else
                {
                    animationTrackBar.Value++;
                }
            }   
        }

        private void maxFrameCounterUD_ValueChanged(object sender, EventArgs e)
        {
            if (ActiveMaterialAnim == null) return;
            if (maxFrameCounterUD.Value < 1)
            {
                maxFrameCounterUD.Maximum = 1;
                maxFrameCounterUD.Value = 1;
            }
            else
            {
                ActiveMaterialAnim.FrameCount = (int)maxFrameCounterUD.Value;
                animationTrackBar.Value = 0;
                animationTrackBar.Maximum = ActiveMaterialAnim.FrameCount;
                animationTrackBar.Minimum = 0;
            }
        }

        private void animationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            currentFrameCounterUD.Value = animationTrackBar.Value;
            SetAnimationsToFrame(animationTrackBar.Value);
        }

        private void SelectListItem(int Frame)
        {
            if (KeyFrames.Contains(Frame))
            {
                int index = KeyFrames.IndexOf(Frame);
                listViewCustom1.TrySelectItem(index);
            }
        }

        private void SetAnimationsToFrame(int Frame)
        {
            if (activeSampler != null)
            {
                var keyFrame = activeSampler.GetKeyFrame(Frame);

                var tex = activeSampler.GetActiveTexture((int)keyFrame.Value);
                if (tex != null)
                {
                    if (Images.ContainsKey(Frame))
                        pictureBoxCustom1.Image = Images[Frame];
                }
                else
                    pictureBoxCustom1.Image = null;
            }
            else
                pictureBoxCustom1.Image = null;
        }

        private void currentFrameCounterUD_ValueChanged(object sender, EventArgs e)
        {
            if (currentFrameCounterUD.Value > maxFrameCounterUD.Value)
                currentFrameCounterUD.Value = maxFrameCounterUD.Value;

            animationTrackBar.Value = (int)currentFrameCounterUD.Value;
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0 && KeyFrames.Count > 0)
            {
                editToolStripMenuItem.Enabled = true;

                int SelectedFrame = KeyFrames[listViewCustom1.SelectedIndices[0]];
                animationTrackBar.Value = SelectedFrame;
                textureFrameUD.Value = SelectedFrame;
            }
            else
                editToolStripMenuItem.Enabled = false;
        }

        private void animationTrackBar_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void btnForward1_Click(object sender, EventArgs e) {
            if (animationTrackBar.Value < animationTrackBar.Maximum)
                animationTrackBar.Value++;
        }

        private void btnBackward1_Click(object sender, EventArgs e) {
            if (animationTrackBar.Value > 0)
                animationTrackBar.Value--;
        }

        private void btnEditMaterial_Click(object sender, EventArgs e)
        {
            TexPatternMaterialEditor editor = new TexPatternMaterialEditor();
            editor.LoadAnim(ActiveMaterialAnim);

            if (editor.ShowDialog() == DialogResult.OK)
            {
             
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool IsWiiU = activeSampler is FTXP.BfresSamplerAnim;

            Texture_Selector editor = new Texture_Selector();
            editor.LoadTexture("", IsWiiU);

            if (editor.ShowDialog() == DialogResult.OK)
            {
                string NewTex = editor.GetSelectedTexture();

                if (ActiveMaterialAnim.Textures == null)
                    ActiveMaterialAnim.Textures = new List<string>();

                if (!ActiveMaterialAnim.Textures.Contains(NewTex))
                    ActiveMaterialAnim.Textures.Add(NewTex);

                activeSampler.AddKeyFrame(NewTex);
            }
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                bool IsWiiU = activeSampler is FTXP.BfresSamplerAnim;

                int SelectedFrame = KeyFrames[listViewCustom1.SelectedIndices[0]];

                string currentTex = listViewCustom1.SelectedItems[0].Text;

                Texture_Selector editor = new Texture_Selector();
                editor.LoadTexture(currentTex, IsWiiU);

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    string NewTex = editor.GetSelectedTexture();

                    if (!ActiveMaterialAnim.Textures.Contains(NewTex))
                        ActiveMaterialAnim.Textures.Add(NewTex);

                    int index = ActiveMaterialAnim.Textures.IndexOf(NewTex);
                    activeSampler.SetValue(SelectedFrame, index);
                    ActiveMaterialAnim.UpdateAnimationData();
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0 && activeSampler != null)
            {
                int Frame = animationTrackBar.Value;

                var keyFrame = activeSampler.GetKeyFrame(Frame);

                var tex = activeSampler.GetActiveTexture((int)keyFrame.Value);
                if (tex != null)
                {
                    using (var sfd = new SaveFileDialog())
                    {
                        if (tex is FTEX)
                        {
                            sfd.Filter = FileFilters.FTEX;
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                if (tex is FTEX)
                                    ((FTEX)tex).Export(sfd.FileName);
                            }
                        }
                        if (tex is TextureData)
                        {
                            sfd.Filter = FileFilters.BNTX_TEX;
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                if (tex is TextureData)
                                    ((TextureData)tex).Export(sfd.FileName);
                            }
                        }
                    }
                }
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0 && activeSampler != null)
            {
                int Frame = animationTrackBar.Value;

                var keyFrame = activeSampler.GetKeyFrame(Frame);

                var tex = activeSampler.GetActiveTexture((int)keyFrame.Value);
                if (tex != null)
                {
                    using (var ofd = new OpenFileDialog())
                    {
                        if (tex is FTEX)
                        {
                            ofd.Filter = FileFilters.FTEX;
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                if (tex is FTEX)
                                    ((FTEX)tex).Replace(ofd.FileName);
                            }
                        }
                        if (tex is TextureData)
                        {
                            ofd.Filter = FileFilters.BNTX_TEX;
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                if (tex is TextureData)
                                    ((TextureData)tex).Replace(ofd.FileName);
                            }
                        }
                    }
                }

                ReloadAnimationView();
            }
        }

        private void textureFrameUD_ValueChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0 && KeyFrames.Count > 0)
            {
                int Index = listViewCustom1.SelectedIndices[0];

                int SelectedFrame = KeyFrames[Index];
                var ListItem = listViewCustom1.SelectedItems[0];

                
            }
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (ActiveMaterialAnim == null || !IsLoaded)
                return;

            var node = treeView1.SelectedNode;
            if (node.Tag is MaterialAnimation.Material)
            {

            }
            if (node.Tag is MaterialAnimation.SamplerKeyGroup)
            {
                activeSampler = (MaterialAnimation.SamplerKeyGroup)node.Tag;

                ReloadAnimationView();
            }
        }

        private void addKeyFrameToolstrip_Click(object sender, EventArgs e)
        {
            if (activeSampler != null)
            {
                bool IsWiiU = activeSampler is FTXP.BfresSamplerAnim;

                Texture_Selector editor = new Texture_Selector();
                editor.LoadTexture("", IsWiiU);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    string NewTex = editor.GetSelectedTexture();

                    if (ActiveMaterialAnim.Textures == null)
                        ActiveMaterialAnim.Textures = new List<string>();

                    if (!ActiveMaterialAnim.Textures.Contains(NewTex))
                        ActiveMaterialAnim.Textures.Add(NewTex);

                    if (activeSampler != null)
                        activeSampler.AddKeyFrame(NewTex);
                }
            }
        }

        private void removeKeyFrameToolstrip_Click(object sender, EventArgs e)
        {
            if (activeSampler != null)
            {
                int Frame = animationTrackBar.Value;
                activeSampler.RemoveKeyFrame(Frame);
            }
        }

        private void toolstripShiftUp_Click(object sender, EventArgs e)
        {
            if (activeSampler != null)
            {
                int Frame = animationTrackBar.Value;
                activeSampler.ShiftKeyUp(Frame);
            }
        }

        private void toolstripShiftDown_Click(object sender, EventArgs e)
        {
            if (activeSampler != null)
            {
                int Frame = animationTrackBar.Value;
                activeSampler.ShiftKeyDown(Frame);
            }
        }

        private void activeAnimCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeAnimCB.SelectedIndex != -1 && IsLoaded)
            {
                ActiveMaterialAnim = MaterialAnimations[activeAnimCB.SelectedIndex];
                ReloadAnimationView(ActiveMaterialAnim);
            }
        }

        private void backgroundCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.pictureBoxStyle = (Runtime.PictureBoxBG)backgroundCB.SelectedItem;
            UpdateBackgroundImage();
        }

        private void UpdateBackgroundImage()
        {
            switch (Runtime.pictureBoxStyle)
            {
                case Runtime.PictureBoxBG.Black:
                    pictureBoxCustom1.BackgroundImage = null;
                    pictureBoxCustom1.BackColor = Color.Black;
                    break;
                case Runtime.PictureBoxBG.White:
                    pictureBoxCustom1.BackgroundImage = null;
                    pictureBoxCustom1.BackColor = Color.White;
                    break;
                case Runtime.PictureBoxBG.Checkerboard:
                    pictureBoxCustom1.BackgroundImage = Toolbox.Library.Properties.Resources.CheckerBackground;
                    break;
            }
        }
    }
}
