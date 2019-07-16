using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace Toolbox.Library.Animations
{
    public class BooleanKeyGroup : TreeNode
    {
        //Determines which bone to toggle in a list
        public uint AnimDataOffset;

        public bool Constant = false;

        public float Scale { get; set; }

        public float Offset { get; set; }

        public float StartFrame { get; set; }

        public float EndFrame { get; set; }

        public float Delta { get; set; }

        public bool HasAnimation()
        {
            return Keys.Count > 0;
        }

        public List<BooleanKeyFrame> Keys = new List<BooleanKeyFrame>();
        public float FrameCount
        {
            get
            {
                float fc = 0;
                foreach (BooleanKeyFrame k in Keys)
                    if (k.Frame > fc) fc = k.Frame;
                return fc;
            }
        }

        public bool SetBoolean(float Frame, bool Value)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                BooleanKeyFrame key = Keys[i];
                if (key.Frame == Frame)
                {
                    key.Visible = Value;
                    return true;
                }
            }
            return false;
        }

        public BooleanKeyFrame GetKeyFrame(float frame)
        {
            BooleanKeyFrame key = null;
            int i;
            for (i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Frame == frame)
                {
                    key = Keys[i];
                    break;
                }
                if (Keys[i].Frame > frame)
                {
                    break;
                }
            }

            if (key == null)
            {
                key = new BooleanKeyFrame();
                key.Frame = frame;
                Keys.Insert(i, key);
            }

            return key;
        }

        int LastFound = 0;
        float LastFrame;
        public bool GetValue(float frame)
        {
            BooleanKeyFrame k1 = (BooleanKeyFrame)Keys[0], k2 = (BooleanKeyFrame)Keys[0];
            int i = 0;
            if (frame < LastFrame)
                LastFound = 0;
            for (i = LastFound; i < Keys.Count; i++)
            {
                LastFound = i % (Keys.Count);
                BooleanKeyFrame k = Keys[LastFound];
                if (k.Frame < frame)
                {
                    k1 = k;
                }
                else
                {
                    k2 = k;
                    break;
                }
            }
            LastFound -= 1;
            if (LastFound < 0)
                LastFound = 0;
            if (LastFound >= Keys.Count - 2)
                LastFound = 0;
            LastFrame = frame;

            if (k1.InterType == InterpolationType.STEPBOOL)
                return k1.Visible;

            return k1.Visible;
        }
    }
}
