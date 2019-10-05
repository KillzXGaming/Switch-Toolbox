using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Animations;

namespace Toolbox.Library.Forms
{ 
    public partial class MiniAnimationPlayer : UserControl
    {
        public EventHandler OnAnimationFrameAdvance;

        public AnimPlayerState AnimationPlayerState = AnimPlayerState.Stop;

        public bool IsPlaying
        {
            get
            {
                return AnimationPlayerState == AnimPlayerState.Playing;
            }
        }

        public bool IsLooping
        {
            get { return loopChkBox.Checked; }
            set { loopChkBox.Checked = value; }
        }

        public float Frame
        {
            set
            {
                UpdateFrame(value);
            }
        }

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

        public void Reset()
        {
            currentAnimations.Clear();
        }

        private List<STAnimation> currentAnimations = new List<STAnimation>();

        public void AddAnimation(STAnimation animation, bool reset = true)
        {
            if (animation == null)
                return;

            if (reset)
                Reset();

            currentAnimations.Add(animation);

            animation.Reset();
            totalFrame.Maximum = (decimal)FrameCount;
            totalFrame.Value = (decimal)FrameCount;
            currentFrameUpDown.Maximum = (decimal)FrameCount;
            currentFrameUpDown.Value = 0;
            animationTrackBar.Maximum = (int)FrameCount;

            SetAnimationsToFrame(0);
        }

        public MiniAnimationPlayer()
        {
            InitializeComponent();

            stPanel3.BackColor = FormThemes.BaseTheme.FormBackColor;
            stPanel4.BackColor = FormThemes.BaseTheme.FormBackColor;

            SetupTimer();
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


        private void animationTimer_Tick(object sender, EventArgs e)
        {
            UpdateAnimationFrame();
        }

        private void Play()
        {
            AnimationPlayerState = AnimPlayerState.Playing;
            UpdateAnimationUI();
            animationTimer.Start();
        }

        private void Pause()
        {
            AnimationPlayerState = AnimPlayerState.Stop;
            UpdateAnimationUI();
            animationTimer.Stop();
        }

        private void Stop()
        {
            currentFrameUpDown.Value = 0;
            AnimationPlayerState = AnimPlayerState.Stop;
            UpdateAnimationUI();
            animationTimer.Stop();
        }

        private void UpdateAnimationUI()
        {
            btnPlay.BackgroundImage = IsPlaying ? Properties.Resources.stop
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

        private void AdvanceNextFrame()
        {
            if (animationTrackBar.Value == FrameCount - 1)
            {
                if (IsLooping)
                    currentFrameUpDown.Value = 0;
                else
                    Stop();
            }
            else
            {
                if (currentFrameUpDown.Value < totalFrame.Value)
                    currentFrameUpDown.Value++;
            }

            currentFrameUpDown.Refresh();
            totalFrame.Refresh();
        }

        public void UpdateFrame(float frame)
        {
            if (frame < (float)currentFrameUpDown.Maximum && frame > (float)currentFrameUpDown.Minimum)
                currentFrameUpDown.Value = (decimal)frame;
            currentFrameUpDown.Refresh();
        }

        private void currentFrameUpDown_ValueChanged(object sender, EventArgs e) {

            if (currentFrameUpDown.Value > totalFrame.Value)
                currentFrameUpDown.Value = totalFrame.Value;

            animationTrackBar.Value = (int)currentFrameUpDown.Value;

            OnFrameAdvanced();

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
                animationTrackBar.Value = 0;
            }
        }

        private void OnFrameAdvanced()
        {
            SetAnimationsToFrame(animationTrackBar.Value);
        }

        private void SetAnimationsToFrame(float frameNum)
        {
            if (currentAnimations.Count == 0)
                return;

            foreach (var anim in currentAnimations)
            {
                if (frameNum > anim.FrameCount)
                    continue;

                float animFrameNum = frameNum;

                anim.SetFrame(animFrameNum);
                anim.NextFrame();

                OnAnimationFrameAdvance?.Invoke(anim, new EventArgs());

                //Add frames to the playing animation
                anim.Frame += frameNum;

                //Reset it when it reaches the total frame count
                if (anim.Frame >= anim.FrameCount && anim.Loop)
                {
                    anim.Frame = 0;
                }
            }
        }

        private void animationTrackBar_Scroll(object sender, ScrollEventArgs e) {
            if (currentAnimations.Count == 0 || totalFrame.Value <= 0)
                return;

            currentFrameUpDown.Value = (decimal)animationTrackBar.Value;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (currentAnimations.Count == 0 || FrameCount <= 0)
                return;

            if (AnimationPlayerState == AnimPlayerState.Playing)
                Pause();
            else
                Play();
        }
    }
}
