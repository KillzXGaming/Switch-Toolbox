using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Toolbox.Library.Forms;
using Toolbox.Library.Animations;

namespace Toolbox.Library
{
    //Thanks to forge! Based on
    // https://github.com/jam1garner/Smash-Forge/blob/52844da94c7bed830d841e0d7e5d49c3f2c69471/Smash%20Forge/GUI/ModelViewport.cs

    public partial class AnimationPanel : STUserControl
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

        private static AnimationPanel _instance;
        public static AnimationPanel Instance { get { return _instance == null ? _instance = new AnimationPanel() : _instance; } }

        //Animation Functions
        public int AnimationSpeed = 30;
        public float Frame = 0;

        // Frame rate control
        private Thread renderThread;
        private bool renderThreadIsUpdating = false;
        public bool isOpen = true;

        private STAnimation stCurrentAnimation;
        public STAnimation CurrentSTAnimation
        {
            get
            {
                return stCurrentAnimation;
            }
            set
            {
                if (value == null)
                    return;

                int frameCount = 1;

                if (value.FrameCount != 0)
                    frameCount = (int)value.FrameCount;

                ResetModels();
                stCurrentAnimation = value;
                totalFrame.Maximum = frameCount;
                totalFrame.Value = frameCount;
                currentFrameUpDown.Maximum = frameCount;
                animationTrackBar.FrameCount = frameCount;
                currentFrameUpDown.Value = 0;

                SetAnimationsToFrame(0);
                UpdateViewport();
            }
        }

        private Animation currentAnimation;
        public Animation CurrentAnimation
        {
            get
            {
                return currentAnimation;
            }
            set
            {
                if (value == null)
                    return;

                int frameCount = 1;

                if (value.FrameCount != 0)
                    frameCount = value.FrameCount;

                ResetModels();
                currentAnimation = value;
                totalFrame.Maximum = frameCount;
                totalFrame.Value = frameCount;
                currentFrameUpDown.Maximum = frameCount;
                animationTrackBar.FrameCount = frameCount;
                currentFrameUpDown.Value = 0;

                SetAnimationsToFrame(0);
                UpdateViewport();
            }
        }

        public void ResetModels()
        {
            var viewport = LibraryGUI.GetActiveViewport();
            if (viewport == null)
                return;
            if (viewport.scene == null)
                return;

            foreach (var drawable in viewport.scene.objects)
            {
                if (drawable is STSkeleton)
                {
                    ((STSkeleton)drawable).reset();
                }
            }
        }

        public AnimationPanel()
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

            this.LostFocus += new System.EventHandler(AnimationPanel_LostFocus);
        }

        private void Play()
        {
            AnimationPlayerState = PlayerState.Playing;
            UpdateAnimationUI();
            animationTrackBar.Play();
        }

        private void Pause()
        {
            AnimationPlayerState = PlayerState.Stop;
            UpdateAnimationUI();
            animationTrackBar.Stop();
        }

        private void Stop()
        {
            currentFrameUpDown.Value = 0;
            AnimationPlayerState = PlayerState.Stop;
            UpdateAnimationUI();
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
            if (currentAnimation?.FrameCount <= 0 &&
                stCurrentAnimation?.FrameCount <= 0)
                return;

            if (AnimationPlayerState == PlayerState.Playing)
                Pause();
            else
                Play();
        }

        private void totalFrame_ValueChanged(object sender, EventArgs e)
        {
            if (currentAnimation == null && stCurrentAnimation == null) return;
            if (totalFrame.Value < 1)
            {
                totalFrame.Value = 1;
            }
            else
            {
                if (stCurrentAnimation != null)
                {
                    stCurrentAnimation.FrameCount = (int)totalFrame.Value;
                    animationTrackBar.CurrentFrame = 0;
                    animationTrackBar.FrameCount = stCurrentAnimation.FrameCount;
                }
                else
                {
                    if (currentAnimation.Tag is Animation)
                        ((Animation)currentAnimation.Tag).FrameCount = (int)totalFrame.Value;
                    currentAnimation.FrameCount = (int)totalFrame.Value;
                    animationTrackBar.CurrentFrame = 0;
                    animationTrackBar.FrameCount = currentAnimation.FrameCount;
                }
            }
        }
        private void UpdateViewport()
        {
            if (IsDisposed)
                return;

            Viewport viewport = LibraryGUI.GetActiveViewport();

            if (viewport == null)
                return;

            if (viewport.GL_Control == null || viewport.GL_Control.IsDisposed || viewport.GL_Control.Disposing)
                return;

            if (viewport.GL_Control.InvokeRequired)
            {
                viewport.GL_Control.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    viewport.GL_Control.Invalidate();
                });
            }
            else
            {
                viewport.GL_Control.Invalidate();
            }
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
            if (currentAnimation == null && stCurrentAnimation == null || totalFrame.Value <= 0)
                return;

            currentFrameUpDown.Value = (decimal)animationTrackBar.CurrentFrame;
        }

        private void OnFrameAdvanced()
        {
            UpdateViewport();
            SetAnimationsToFrame(animationTrackBar.CurrentFrame);

            if (!renderThreadIsUpdating || !IsPlaying)
                UpdateViewport();
        }

        private void SetAnimationsToFrame(float frameNum)
        {
            if (currentAnimation == null && stCurrentAnimation == null)
                return;

            var viewport = LibraryGUI.GetActiveViewport();
            if (viewport == null || viewport.scene == null)
                return;

            if (stCurrentAnimation != null)
            {
                Console.WriteLine("SetAnimationsToFrame");

                if (frameNum > stCurrentAnimation.FrameCount)
                    return;

                float animFrameNum = frameNum;

                stCurrentAnimation.SetFrame(animFrameNum);
                stCurrentAnimation.NextFrame();

                //Add frames to the playing animation
                stCurrentAnimation.Frame += frameNum;

                //Reset it when it reaches the total frame count
                if (stCurrentAnimation.Frame >= stCurrentAnimation.FrameCount && stCurrentAnimation.Loop)
                {
                    stCurrentAnimation.Frame = 0;
                }
            }
            else
            {

                var anim = currentAnimation.Tag;

                float animFrameNum = frameNum;

                if (anim is MaterialAnimation)
                {
                    ((MaterialAnimation)anim).SetFrame(animFrameNum);
                    ((MaterialAnimation)anim).NextFrame(viewport);
                }
                else if (anim is VisibilityAnimation)
                {
                    ((VisibilityAnimation)anim).SetFrame(animFrameNum);
                    ((VisibilityAnimation)anim).NextFrame(viewport);
                }
                else if (anim is CameraAnimation)
                {
                    ((CameraAnimation)anim).SetFrame(animFrameNum);
                    ((CameraAnimation)anim).NextFrame(viewport);
                }
                else if (anim is LightAnimation)
                {
                    ((LightAnimation)anim).SetFrame(animFrameNum);
                    ((LightAnimation)anim).NextFrame(viewport);
                }
                else if (anim is FogAnimation)
                {
                    ((FogAnimation)anim).SetFrame(animFrameNum);
                    ((FogAnimation)anim).NextFrame(viewport);
                }
                else //Play a skeletal animation if it's not the other types
                {
                    foreach (var drawable in viewport.scene.objects)
                    {
                        if (drawable is STSkeleton)
                        {
                            currentAnimation.SetFrame(animFrameNum);
                            currentAnimation.NextFrame((STSkeleton)drawable);
                        }
                    }
                }


                //Add frames to the playing animation
                currentAnimation.Frame += frameNum;

                //Reset it when it reaches the total frame count
                if (currentAnimation.Frame >= currentAnimation.FrameCount)
                {
                    currentAnimation.Frame = 0;
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

        public void AnimationPanel_FormClosed()
        {
            isOpen = false;
            Dispose();
        }

        private void AnimationPanel_Load(object sender, EventArgs e)
        {
            Viewport viewport = LibraryGUI.GetActiveViewport();
            if (viewport != null)
            {
                if (viewport.GL_Control != null)
                    viewport.GL_Control.VSync = Runtime.enableVSync;
            }

            renderThread = new Thread(new ThreadStart(RenderAndAnimationLoop));
            renderThread.Start();
        }

        private void RenderAndAnimationLoop()
        {
            if (IsDisposed)
                return;
            // TODO: We don't really need two timers.
            Stopwatch animationStopwatch = Stopwatch.StartNew();


            // Wait for UI to load before triggering paint events.
            //  int waitTimeMs = 500;
            //  Thread.Sleep(waitTimeMs);

           // UpdateViewport();

            int frameUpdateInterval = 5;
            int animationUpdateInterval = 1000 / AnimationSpeed;

            while (isOpen)
            {
                // Always refresh the viewport when animations are playing.
                if (renderThreadIsUpdating || IsPlaying)
                {
                    if (animationStopwatch.ElapsedMilliseconds > animationUpdateInterval)
                    {
                        UpdateAnimationFrame();
                        animationStopwatch.Restart();
                    }
                }
                else
                {
                    // Avoid wasting the CPU if we don't need to render anything.
                    Thread.Sleep(1);
                }
            }
        }

        private void AnimationPanel_Enter(object sender, EventArgs e)
        {
        }

        private void AnimationPanel_LostFocus(object sender, EventArgs e)
        {
            renderThreadIsUpdating = false;
        }

        private void AnimationPanel_Click(object sender, EventArgs e)
        {
            renderThreadIsUpdating = true;
        }

        private void AnimationPanel_Leave(object sender, EventArgs e)
        {
            renderThreadIsUpdating = false;
        }
        public void ClosePanel()
        {
            ResetModels();

            renderThread.Abort();
            renderThreadIsUpdating = false;
            stCurrentAnimation = null;
            currentAnimation = null;
            isOpen = false;
            Dispose();

            Console.WriteLine("Disposeing ANIM PANEL!!");

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void toggleFrameRateBtn_Click(object sender, EventArgs e)
        {
            if (AnimationPlayerState == PlayerState.Playing)
                ToggleFrameRate();
        }

        private void ToggleFrameRate()
        {
            if (AnimationSpeed == 60)
            {
                AnimationSpeed = 30; 
                toggleFrameRateBtn.Text = "30 FPS"; 
            }
            else
            {
                AnimationSpeed = 60; 
                toggleFrameRateBtn.Text = "60 FPS"; 
            }

            UpdateAnimationUpdateInterval();
        }

        private void UpdateAnimationUpdateInterval()
        {
            int animationUpdateInterval = 1000 / AnimationSpeed;

            if (renderThreadIsUpdating || IsPlaying)
            {
                renderThread.Abort(); 
                renderThread = new Thread(new ThreadStart(RenderAndAnimationLoop));
                renderThread.Start();
            }
        }

        private void GoToPreviousFrame(object sender, EventArgs e)
        {
            if (currentFrameUpDown.Value > 0)
            {
                Pause();
                currentFrameUpDown.Value--; 
                OnFrameAdvanced(); 
            }
        }
    }
}
