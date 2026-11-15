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

        public bool GetValue(float frame)
        {
            if (Keys.Count == 0)
                return true;

            BooleanKeyFrame k1 = Keys[0];
            int lo = 0, hi = Keys.Count - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) / 2;
                if (Keys[mid].Frame <= frame)
                { 
                    k1 = Keys[mid]; lo = mid + 1;
                }
                else hi = mid - 1;
            }
            return k1.Visible;
        }
    }
}
