using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Switch_Toolbox.Library.Animations;
using Switch_Toolbox.Library.Forms;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class BfresTexturePatternEditor : UserControl
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

        public BfresTexturePatternEditor()
        {
            InitializeComponent();

            btnEditSamplers.Enabled = false;
            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;
            listViewCustom1.BackColor = FormThemes.BaseTheme.TextEditorBackColor;
            imgList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(32, 32),
            };

            stPanel4.BackColor = FormThemes.BaseTheme.FormBackColor;

            timer1.Interval = 100 / 60;
        }

        MaterialAnimation.Material material;
        FMAA.BfresSamplerAnim activeSampler;

        FTXP.BfresSamplerAnim activeSampleU;

        MaterialAnimation activeMaterialAnim;
        MaterialAnimation ActiveMaterialAnim
        {
            get
            {
                return activeMaterialAnim;
            }
            set
            {
                activeMaterialAnim = value;

                maxFrameCounterUD.Maximum = value.FrameCount;
                maxFrameCounterUD.Value = value.FrameCount;
                currentFrameCounterUD.Maximum = value.FrameCount;
                animationTrackBar.TickDivide = 1;
                animationTrackBar.Maximum = value.FrameCount;
                animationTrackBar.Minimum = 0;
                currentFrameCounterUD.Value = 0;
            }
        }

        private void OnPropertyChanged()
        {

        }

        public void LoadAnim(FTXP materialAnim)
        {
            if (materialAnim.Materials.Count <= 0)
                return;

            IsLoaded = false;

            ActiveMaterialAnim = materialAnim;


            materialCB.Items.Clear();
            samplerCB.Items.Clear();

            foreach (var mat in materialAnim.Materials)
                materialCB.Items.Add(mat.Text);

            materialCB.SelectedIndex = 0;

            material = materialAnim.Materials[materialCB.SelectedIndex];

            if (material.Samplers.Count <= 0)
                return;

            foreach (var sampler in material.Samplers)
                samplerCB.Items.Add(sampler.Text);

            samplerCB.SelectedIndex = 0;

            activeSampleU = (FTXP.BfresSamplerAnim)material.Samplers[samplerCB.SelectedIndex];

            listViewCustom1.SuspendLayout();
            listViewCustom1.Items.Clear();

            LoadAniamtion(materialAnim, activeSampleU);

            listViewCustom1.ResumeLayout();

            IsLoaded = true;
            animationTrackBar.Value = 0;
        }



        public bool IsLoaded = false;
        public void LoadAnim(FMAA materialAnim)
        {
            if (materialAnim.Materials.Count <= 0)
                return;

            IsLoaded = false;

            ActiveMaterialAnim = materialAnim;


            materialCB.Items.Clear();
            samplerCB.Items.Clear();

            foreach (var mat in materialAnim.Materials)
                materialCB.Items.Add(mat.Text);

            materialCB.SelectedIndex = 0;

            material = materialAnim.Materials[materialCB.SelectedIndex];

            if (material.Samplers.Count <= 0)
                return;

            foreach (var sampler in material.Samplers)
                samplerCB.Items.Add(sampler.Text);

            samplerCB.SelectedIndex = 0;

            activeSampler = (FMAA.BfresSamplerAnim)material.Samplers[samplerCB.SelectedIndex];

            listViewCustom1.SuspendLayout();
            listViewCustom1.Items.Clear();

            LoadAniamtion(materialAnim, activeSampler);

            listViewCustom1.ResumeLayout();

            IsLoaded = true;
            animationTrackBar.Value = 0;
        }

        Dictionary<int, Bitmap> Images = new Dictionary<int, Bitmap>();
        public List<int> KeyFrames = new List<int>(); //Used for getting the frame of the list item

        public bool IsLoading = false;
        private void LoadAniamtion(MaterialAnimation anim, MaterialAnimation.Material.Sampler activeSampler)
        {
            if (activeSampler == null || IsLoading)
                return;

            int imageIndex = 0;

            imgList.Images.Clear();
            Images.Clear();
            KeyFrames.Clear();

            listViewCustom1.SmallImageList = imgList;
            listViewCustom1.Items.Clear();
            listViewCustom1.View = View.SmallIcon;

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                IsLoading = true;

                for (int Frame = 0; Frame < anim.FrameCount; Frame++)
                {
                    //Constants always show so break after first frame
                    if (activeSampler.group.Constant && Frame != 0) 
                        break;

                    var keyFrame = activeSampler.group.GetKeyFrame(Frame);

                    if (keyFrame.IsKeyed || activeSampler.group.Constant)
                    {
                        var tex = activeSampler.GetActiveTexture(Frame);

                        if (tex != null)
                        {
                            Bitmap temp = tex.GetBitmap();
                            Images.Add(Frame, temp);
                            KeyFrames.Add(Frame);

                            if (listViewCustom1.InvokeRequired)
                            {
                                listViewCustom1.Invoke((MethodInvoker)delegate {
                                    // Running on the UI thread
                                    listViewCustom1.Items.Add($"{Frame} / {anim.FrameCount} \n" + tex.Text, imageIndex++);
                                    imgList.Images.Add(temp);
                                    var dummy = imgList.Handle;
                                });
                            }
                            else
                                listViewCustom1.Items.Add($"{Frame} / {anim.FrameCount} \n" + tex.Text, imageIndex++);
                        }
                        else
                        {
                            if (listViewCustom1.InvokeRequired)
                            {
                                listViewCustom1.Invoke((MethodInvoker)delegate {
                                    listViewCustom1.Items.Add($"{Frame} / {anim.FrameCount} \n" + activeSampler.GetActiveTextureName(Frame), imageIndex++);
                                });
                            }
                            else
                                listViewCustom1.Items.Add($"{Frame} / {anim.FrameCount} \n" + activeSampler.GetActiveTextureName(Frame), imageIndex++);
                        }
                    }
                }

                IsLoading = false;
            }));
            Thread.Start();
        }

        private void SelectThumbnailItems()
        {

        }

        private void materialCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (material == null || !IsLoaded)
                return;

            if (materialCB.SelectedIndex >= 0)
            {
                btnEditSamplers.Enabled = true;

                material = ActiveMaterialAnim.Materials[materialCB.SelectedIndex];

                if (activeSampleU != null)
                    LoadAniamtion(ActiveMaterialAnim, activeSampleU);
                else
                    LoadAniamtion(ActiveMaterialAnim, activeSampler);
            }
            else
            {
                btnEditSamplers.Enabled = false;
            }
        }

        private void samplerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (material == null || !IsLoaded)
                return;

            if (samplerCB.SelectedIndex >= 0)
            {
                if (activeSampleU != null)
                {
                    activeSampleU = (FTXP.BfresSamplerAnim)material.Samplers[samplerCB.SelectedIndex];
                    LoadAniamtion(ActiveMaterialAnim, activeSampleU);
                }
                else
                {
                    activeSampler = (FMAA.BfresSamplerAnim)material.Samplers[samplerCB.SelectedIndex];
                    LoadAniamtion(ActiveMaterialAnim, activeSampler);
                }
            }
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
            btnPlay.BackgroundImage = IsPlaying ? Switch_Toolbox.Library.Properties.Resources.PauseBtn
                : Switch_Toolbox.Library.Properties.Resources.PlayArrowR;
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
                var tex = activeSampler.GetActiveTexture(Frame);
                if (tex != null)
                {
                    if (Images.ContainsKey(Frame))
                        pictureBoxCustom1.Image = Images[Frame];
                }
            }
            if (activeSampleU != null)
            {
                var tex = activeSampleU.GetActiveTexture(Frame);
                if (tex != null)
                {
                    if (Images.ContainsKey(Frame))
                        pictureBoxCustom1.Image = Images[Frame];
                }
            }

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
                int SelectedFrame = KeyFrames[listViewCustom1.SelectedIndices[0]];
                animationTrackBar.Value = SelectedFrame;
                textureFrameUD.Value = SelectedFrame;
            }
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
            Texture_Selector editor = new Texture_Selector();
            editor.LoadTexture("", activeSampleU != null);

            if (editor.ShowDialog() == DialogResult.OK)
            {
                string NewTex = editor.GetSelectedTexture();

                if (!ActiveMaterialAnim.Textures.Contains(NewTex))
                    ActiveMaterialAnim.Textures.Add(NewTex);
            }
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                int SelectedFrame = KeyFrames[listViewCustom1.SelectedIndices[0]];

                string currentTex = listViewCustom1.SelectedItems[0].Text;

                Texture_Selector editor = new Texture_Selector();
                editor.LoadTexture(currentTex, activeSampleU != null);

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    string NewTex = editor.GetSelectedTexture();

                    if (!ActiveMaterialAnim.Textures.Contains(NewTex))
                        ActiveMaterialAnim.Textures.Add(NewTex);

                    int index = ActiveMaterialAnim.Textures.IndexOf(NewTex);
                    if (activeSampleU != null)
                        activeSampleU.group.SetValue(SelectedFrame, index);
                    else
                        activeSampler.group.SetValue(SelectedFrame, index);

                    ActiveMaterialAnim.UpdateAnimationData();
                }
            }
        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            if (materialCB.SelectedIndex < 0)
                return;

            TexPatternInfoEditor editor = new TexPatternInfoEditor();
            editor.LoadAnim(ActiveMaterialAnim.Materials[materialCB.SelectedIndex]);
            if (editor.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void textureFrameUD_ValueChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0 && KeyFrames.Count > 0)
            {
                int SelectedFrame = KeyFrames[listViewCustom1.SelectedIndices[0]];

            }
        }
    }
}
