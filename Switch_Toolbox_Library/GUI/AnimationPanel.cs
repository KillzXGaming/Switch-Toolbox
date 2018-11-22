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
using WeifenLuo.WinFormsUI.Docking;

namespace Switch_Toolbox.Library
{
    //Thanks to forge! Based on
    // https://github.com/jam1garner/Smash-Forge/blob/52844da94c7bed830d841e0d7e5d49c3f2c69471/Smash%20Forge/GUI/ModelViewport.cs

    public partial class AnimationPanel : UserControl
    {
        private static AnimationPanel _instance;
        public static AnimationPanel Instance { get { return _instance == null ? _instance = new AnimationPanel() : _instance; } }

        //Animation Functions
        public int AnimationSpeed = 60;
        public float Frame = 0;
        public bool isPlaying;
        public bool isOpen = true;
        private Thread renderThread;
        public bool renderThreadIsUpdating = false;

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

                ResetModels();
                currentAnimation = value;
                totalFrame.Value = value.FrameCount;
                animationTrackBar.TickFrequency = 1;
                animationTrackBar.SetRange(0, (int)value.FrameCount);
                currentFrameUpDown.Value = 1;
                currentFrameUpDown.Value = 0;
            }
        }

        public void ResetModels()
        {
            foreach (var drawable in Runtime.abstractGlDrawables)
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
        }

        public static Stopwatch directUVTimeStopWatch = new Stopwatch();

        private void animationPlayBtn_Click(object sender, EventArgs e)
        {
            isPlaying = !isPlaying;
            animationPlayBtn.Text = isPlaying ? "Pause" : "Play";

            if (isPlaying)
                directUVTimeStopWatch.Start();
            else
                directUVTimeStopWatch.Stop();
        }

        private void totalFrame_ValueChanged(object sender, EventArgs e)
        {
            if (currentAnimation == null) return;
            if (totalFrame.Value < 1)
            {
                totalFrame.Value = 1;
            }
            else
            {
                if (currentAnimation.Tag is Animation)
                    ((Animation)currentAnimation.Tag).FrameCount = (int)totalFrame.Value;
                currentAnimation.FrameCount = (int)totalFrame.Value;
                animationTrackBar.Value = 0;
                animationTrackBar.SetRange(0, currentAnimation.FrameCount);
            }
        }
        private GL_Core.GL_ControlModern GetViewport()
        {
            Form form1 = Application.OpenForms[0];
            foreach (Control control in form1.Controls)
            {
                if (control is DockPanel)
                {
                    foreach (DockContent ctrl in ((DockPanel)control).Contents)
                    {
                        foreach (Control controls in ctrl.Controls)
                        {
                            if (controls is GL_Core.GL_ControlModern)
                            {
                                return (GL_Core.GL_ControlModern)controls;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void UpdateViewport()
        {
            if (IsDisposed)
                return;

            if (Viewport.Instance.gL_ControlModern1.InvokeRequired)
            {
                Viewport.Instance.gL_ControlModern1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    Viewport.Instance.gL_ControlModern1.Invalidate();
                });
            }
            else
            {
                Viewport.Instance.gL_ControlModern1.Invalidate();
            }
        }
        private void RenderAndAnimationLoop()
        {
            if (IsDisposed)
                return;

            // TODO: We don't really need two timers.
            Stopwatch renderStopwatch = Stopwatch.StartNew();
            Stopwatch animationStopwatch = Stopwatch.StartNew();

            // Wait for UI to load before triggering paint events.
            int waitTimeMs = 500;
            Thread.Sleep(waitTimeMs);

            UpdateViewport();

            int frameUpdateInterval = 5;
            int animationUpdateInterval = 16;

            while (isOpen)
            {
                // Always refresh the viewport when animations are playing.
                if (renderThreadIsUpdating || isPlaying)
                {
                    if (renderStopwatch.ElapsedMilliseconds > frameUpdateInterval)
                    {
                        UpdateViewport();
                        renderStopwatch.Restart();
                    }

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
        private void UpdateAnimationFrame()
        {
            if (isPlaying)
            {
                if (currentFrameUpDown.InvokeRequired)
                {
                    this.currentFrameUpDown.Invoke((MethodInvoker)delegate {
                        // Running on the UI thread
                        if (currentFrameUpDown.Value == totalFrame.Value)
                            currentFrameUpDown.Value = 0;
                        else
                            currentFrameUpDown.Value++;
                    });
                }
                else
                {
                    if (currentFrameUpDown.Value == totalFrame.Value)
                        currentFrameUpDown.Value = 0;
                    else
                        currentFrameUpDown.Value++;
                }
            }
        }
        private void nextButton_Click(object sender, EventArgs e)
        {
            // Loop the animation.
            if (currentFrameUpDown.Value == totalFrame.Value)
                currentFrameUpDown.Value = 0;
            else
                currentFrameUpDown.Value++;
        }
        private void prevButton_Click(object sender, EventArgs e)
        {
            if (currentFrameUpDown.Value != 0)
                currentFrameUpDown.Value--;
        }

        private void animationTrackBar_Scroll(object sender, EventArgs e)
        {

        }

        private void animationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (animationTrackBar.Value > (int)totalFrame.Value)
                animationTrackBar.Value = 0;
            if (animationTrackBar.Value < 0)
                animationTrackBar.Value = (int)totalFrame.Value;
            currentFrameUpDown.Value = animationTrackBar.Value;

            int currentFrame = animationTrackBar.Value;

            SetAnimationsToFrame(currentFrame);

            if (!renderThreadIsUpdating || !isPlaying)
                UpdateViewport();
        }
        private void SetAnimationsToFrame(int frameNum)
        {
            if (currentAnimation == null)
                return;

            float animFrameNum = frameNum;
            foreach (var drawable in Runtime.abstractGlDrawables)
            {
                if (drawable is STSkeleton)
                {
                    currentAnimation.SetFrame(animFrameNum);
                    currentAnimation.NextFrame((STSkeleton)drawable);
                }
            }
        }

        private void currentFrameUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (currentFrameUpDown.Value > totalFrame.Value)
                currentFrameUpDown.Value = totalFrame.Value;

            animationTrackBar.Value = (int)currentFrameUpDown.Value;
        }

        public void AnimationPanel_FormClosed()
        {
            isOpen = false;
            Dispose();
        }

        private void AnimationPanel_Load(object sender, EventArgs e)
        {
            if (Viewport.Instance.gL_ControlModern1 != null)
                Viewport.Instance.gL_ControlModern1.VSync = Runtime.enableVSync;

            renderThread = new Thread(new ThreadStart(RenderAndAnimationLoop));
            renderThread.Start();
        }

        private void AnimationPanel_Enter(object sender, EventArgs e)
        {

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
            renderThreadIsUpdating = false;
            isOpen = false;
            Dispose();
            renderThread.Abort();
        }
    }
}
