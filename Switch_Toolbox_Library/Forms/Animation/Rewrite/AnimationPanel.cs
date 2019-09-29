using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Toolbox.Library.Forms;
using Toolbox.Library.Animations;

namespace Toolbox.Library
{
    public partial class STAnimationPanel : STUserControl
    {
        private OpenTK.GLControl _viewport;
        public virtual OpenTK.GLControl Viewport
        {
            get
            {
                if (_viewport == null)
                    _viewport = GetActiveViewport();

                return _viewport;
            }
        }

        private OpenTK.GLControl GetActiveViewport()
        {
            var viewport = LibraryGUI.GetActiveViewport();
            if (viewport == null)
                return null;

            if (viewport.GL_ControlModern != null)
                return viewport.GL_ControlModern;
            else
                return viewport.GL_ControlLegacy;
        }

        public void SetViewport(OpenTK.GLControl control)
        {
            _viewport = control;
        }

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

        private static STAnimationPanel _instance;
        public static STAnimationPanel Instance { get { return _instance == null ? _instance = new STAnimationPanel() : _instance; } }

        //Animation Functions
        public int AnimationSpeed = 60;
        public float Frame = 0;

        // Frame rate control
        public bool isOpen = true;

        public float FrameCount
        {
            get
            {
                float frameCount = 1;
                for (int i = 0; i < currentAnimations.Count; i++)
                    frameCount = Math.Max(frameCount, currentAnimations[i].FrameCount);

                return frameCount;
            }
        }

        private List<STAnimation> currentAnimations = new List<STAnimation>();

        public void AddAnimation(STAnimation animation, bool reset = true)
        {
            if (animation == null)
                return;

            if (reset)
                Reset();

            Console.WriteLine($"Add anim " + animation.Name);

            currentAnimations.Add(animation);

            ResetModels();
            animation.Reset();
            totalFrame.Maximum = (decimal)FrameCount;
            totalFrame.Value = (decimal)FrameCount;
            currentFrameUpDown.Maximum = (decimal)FrameCount;
            animationTrackBar.FrameCount = (float)FrameCount;
            currentFrameUpDown.Value = 0;

            SetAnimationsToFrame(0);
            UpdateViewport();
        }

        public void Reset()
        {
            currentAnimations.Clear();
            Frame = 0;
        }

        public void ResetModels()
        {
            if (Viewport == null)
                return;

            foreach (var anim in currentAnimations) {
                if (anim is STSkeletonAnimation)
                {
                    ((STSkeletonAnimation)anim).GetActiveSkeleton()?.reset();
                }
            }
        }

        public STAnimationPanel()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            animationTrackBar.BackColor = FormThemes.BaseTheme.FormBackColor;
            animationTrackBar.ForeColor = FormThemes.BaseTheme.FormForeColor;
            animationTrackBar.FrameChanged += new EventHandler(animationTrackBar_ValueChanged);

          /*  animationTrackBar.ThumbInnerColor = FormThemes.BaseTheme.TimelineThumbColor;
            animationTrackBar.ThumbOuterColor = FormThemes.BaseTheme.TimelineThumbColor;

            this.animationTrackBar.BarInnerColor = FormThemes.BaseTheme.FormBackColor;
            this.animationTrackBar.BarPenColorBottom = FormThemes.BaseTheme.FormBackColor;
            this.animationTrackBar.BarPenColorTop = FormThemes.BaseTheme.FormBackColor;
            this.animationTrackBar.ElapsedInnerColor = FormThemes.BaseTheme.FormBackColor;
            this.animationTrackBar.ElapsedPenColorBottom = FormThemes.BaseTheme.FormBackColor;
            this.animationTrackBar.ElapsedPenColorTop = FormThemes.BaseTheme.FormBackColor;
            */
            panel1.BackColor = FormThemes.BaseTheme.FormBackColor;
            animationPlayBtn.BackColor = FormThemes.BaseTheme.FormBackColor;
            button2.BackColor = FormThemes.BaseTheme.FormBackColor;
            animationPlayBtn.ForeColor = FormThemes.BaseTheme.FormForeColor;
            button2.ForeColor = FormThemes.BaseTheme.FormForeColor;

            totalFrame.ForeColor = FormThemes.BaseTheme.FormForeColor;
            totalFrame.BackColor = FormThemes.BaseTheme.FormBackColor;
            currentFrameUpDown.ForeColor = FormThemes.BaseTheme.FormForeColor;
            currentFrameUpDown.BackColor = FormThemes.BaseTheme.FormBackColor;

            SetupTimer();

            this.LostFocus += new System.EventHandler(AnimationPanel_LostFocus);
        }

        private void Play()
        {
            AnimationPlayerState = PlayerState.Playing;
            UpdateAnimationUI();
            animationTrackBar.Play();
            animationTimer.Start();
        }

        private void Pause()
        {
            AnimationPlayerState = PlayerState.Stop;
            UpdateAnimationUI();
            animationTrackBar.Stop();
            animationTimer.Stop();
        }

        private void Stop()
        {
            currentFrameUpDown.Value = 0;
            AnimationPlayerState = PlayerState.Stop;
            UpdateAnimationUI();
            animationTimer.Stop();
        }

        private void UpdateAnimationUI()
        {
            animationPlayBtn.BackgroundImage = IsPlaying ? Properties.Resources.stop
                : Properties.Resources.arrowR;
        }

        private void UpdateAnimationFrame()
        {
            if (IsPlaying)
            {
                if (currentFrameUpDown.InvokeRequired)
                {
                    currentFrameUpDown.BeginInvoke((Action)(() =>
                    {
                        AdvanceNextFrame();
                    }));
                }
                else
                {
                    AdvanceNextFrame();
                }
            }
        }

        private Timer animationTimer;
        private void SetupTimer()
        {
            animationTimer = new Timer
            {
                Interval = 100 / 60
            };
            animationTimer.Tick += new EventHandler(animationTimer_Tick);
        }

        private void AdvanceNextFrame()
        {
            if (animationTrackBar.CurrentFrame == animationTrackBar.FrameCount - 1)
            {
                if (IsLooping)
                    currentFrameUpDown.Value = 0;
                else
                    Stop();
            }
            else if (!animationTrackBar.Locked)
            {
                if (currentFrameUpDown.Value < totalFrame.Value)
                    currentFrameUpDown.Value++;
            }

            currentFrameUpDown.Refresh();
            totalFrame.Refresh();
        }

        private void animationPlayBtn_Click(object sender, EventArgs e)
        {
            if (currentAnimations.Count == 0 || FrameCount <= 0)
                return;

            if (AnimationPlayerState == PlayerState.Playing)
                Pause();
            else
                Play();
        }

        private void totalFrame_ValueChanged(object sender, EventArgs e)
        {
            if (currentAnimations.Count == 0) return;

            if (totalFrame.Value < 1)
            {
                totalFrame.Value = 1;
            }
            else
            {
                animationTrackBar.CurrentFrame = 0;
                animationTrackBar.FrameCount = FrameCount;
            }
        }
        private void UpdateViewport()
        {
            if (IsDisposed || Viewport == null || Viewport.IsDisposed || Viewport.Disposing)
                return;

            if (Viewport.InvokeRequired) {
                Viewport.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    Viewport.Invalidate();
                });
            }
            else
                Viewport.Invalidate();
        }

        private void nextButton_Click(object sender, EventArgs e) {
            if (animationTrackBar.CurrentFrame < animationTrackBar.FrameCount)
                animationTrackBar.CurrentFrame++;
        }
        private void prevButton_Click(object sender, EventArgs e) {
              if (animationTrackBar.CurrentFrame > 0)
                animationTrackBar.CurrentFrame--;
        }

        private void animationTrackBar_Scroll(object sender, EventArgs e)
        {

        }

        private void animationTrackBar_ValueChanged(object sender, EventArgs e) {
            if (currentAnimations.Count == 0 || totalFrame.Value <= 0)
                return;

            currentFrameUpDown.Value = (decimal)animationTrackBar.CurrentFrame;
        }

        private void OnFrameAdvanced()
        {
            UpdateViewport();
            SetAnimationsToFrame(animationTrackBar.CurrentFrame);

            if (!IsPlaying)
                UpdateViewport();
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            UpdateAnimationFrame();
        }

        private void SetAnimationsToFrame(float frameNum)
        {
            if (Viewport == null || currentAnimations.Count == 0)
                return;

            foreach (var anim in currentAnimations)
            {
                if (frameNum > anim.FrameCount)
                    continue;

                float animFrameNum = frameNum;

                anim.SetFrame(animFrameNum);
                anim.NextFrame();

                //Add frames to the playing animation
                anim.Frame += frameNum;

                //Reset it when it reaches the total frame count
                if (anim.Frame >= anim.FrameCount && anim.Loop)
                {
                    anim.Frame = 0;
                }
            }
        }

        private void currentFrameUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (currentFrameUpDown.Value > totalFrame.Value)
                currentFrameUpDown.Value = totalFrame.Value;

            //Check locked state current frame will change during playing
            if (!animationTrackBar.Locked)
            {
                animationTrackBar.CurrentFrame = (int)currentFrameUpDown.Value;
            }
            OnFrameAdvanced();
        }

        public override void OnControlClosing()
        {
            isOpen = false;
            Dispose();
            ClosePanel();
        }

        private void AnimationPanel_Load(object sender, EventArgs e)
        {
            if (Viewport != null)
                Viewport.VSync = Runtime.enableVSync;
        }

        private void AnimationPanel_Enter(object sender, EventArgs e)
        {
        }

        private void AnimationPanel_LostFocus(object sender, EventArgs e)
        {
        }

        private void AnimationPanel_Click(object sender, EventArgs e)
        {
        }

        private void AnimationPanel_Leave(object sender, EventArgs e)
        {
        }
        public void ClosePanel()
        {
            ResetModels();
            currentAnimations.Clear();
            isOpen = false;
            Dispose();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
