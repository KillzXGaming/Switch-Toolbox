using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace Toolbox.Library.Animations
{
    public enum STInterpoaltionType
    {
        Constant,
        Step,
        Linear,
        Hermite,
        Bezier,
        Bitmap,
    }

    /// <summary>
    /// Represents how a track is played after it reaches end frame.
    /// Repeat repeats back from the start.
    /// Mirror goes from the end frame to the start
    /// </summary>
    public enum STLoopMode
    {
        Repeat,
        Mirror,
        Clamp,
    }

    /// <summary>
    /// Represents a class for animating
    /// </summary>
    public class STAnimation 
    {
        /// <summary>
        /// The name of the animation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The frame to start playing the animation.
        /// </summary>
        public float StartFrame { get; set; }

        /// <summary>
        /// The current frame of the animation.
        /// </summary>
        public float Frame { get; set; }

        /// <summary>
        /// The total amount of frames to play in the animation.
        /// </summary>
        public float FrameCount { get; set; }

        /// <summary>
        /// Whether the animation will loop or not after
        /// the playback rearches the total frame count.
        /// </summary>
        public bool Loop { get; set; }

        /// <summary>
        /// The step value when a frame advances.
        /// </summary>
        public float Step { get; set; }

        /// <summary>
        /// A list of groups that store the animation data.
        /// </summary>
        public List<STAnimGroup> AnimGroups = new List<STAnimGroup>();

        public void SetFrame(float frame)
        {
            Frame = frame;
        }

        public virtual void NextFrame()
        {
            if (Frame < StartFrame || Frame > FrameCount) return;
        }

        /// <summary>
        /// Resets the animation group values
        /// This should clear values from tracks, or reset them to base values.
        /// </summary>
        public virtual void Reset()
        {

        }
    }
}
