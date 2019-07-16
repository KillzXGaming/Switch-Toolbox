using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class GX2TexRegisters
    {
        //From https://github.com/aboood40091/GTX-Extractor/blob/master/texRegisters.py
        /*
         Copyright © 2018 AboodXD
         Licensed under GNU GPLv3*/
        public static uint _register0(uint width, uint pitch, uint tileType, uint tileMode, uint dim)
        {
            return (
              (width & 0x1FFF) << 19
              | (pitch & 0x7FF) << 8
              | (tileType & 1) << 7
              | (tileMode & 0xF) << 3
              | (dim & 7));
        }


        public static int _register1(int format_, int depth, int height)
        {
            return (
               (format_ & 0x3F) << 26
             | (depth & 0x1FFF) << 13
             | (height & 0x1FFF));
        }

        public static int _register2(int baseLevel, int dstSelW, int dstSelZ, int dstSelY, int dstSelX, int requestSize, int endian, int forceDegamma, int surfMode, int numFormat, int formatComp)
        {
            return (
                (baseLevel & 7) << 28
                | (dstSelW & 7) << 25
                | (dstSelZ & 7) << 22
                | (dstSelY & 7) << 19
                | (dstSelX & 7) << 16
                | (requestSize & 3) << 14
                | (endian & 3) << 12
                | (forceDegamma & 1) << 11
                | (surfMode & 1) << 10
                | (numFormat & 3) << 8
                | (formatComp & 3) << 6
                | (formatComp & 3) << 4
                | (formatComp & 3) << 2
                | (formatComp & 3));
        }

        public static int _register3(int yuvConv, int lastArray, int baseArray, int lastLevel)
        {
            return (
          (yuvConv & 3) << 30
        | (lastArray & 0x1FFF) << 17
        | (baseArray & 0x1FFF) << 4
        | (lastLevel & 0xF));
        }

        public static int _register4(int type_, int advisClampLOD, int advisFaultLOD, int interlaced, int perfModulation, int maxAnisoRatio, int MPEGClamp)
        {
            return (
          (type_ & 3) << 30
          | (advisClampLOD & 0x3F) << 13
          | (advisFaultLOD & 0xF) << 9
          | (interlaced & 1) << 8
          | (perfModulation & 7) << 5
          | (maxAnisoRatio & 7) << 2
          | (MPEGClamp & 3));
        }

        public static uint[] CreateTexRegs(uint width, uint height, uint numMips, uint format_, uint tileMode, uint pitch, byte[] compSel)
        {
            if (compSel == null || compSel.Length != 4)
            {
                compSel = new byte[4] { 0, 1, 2, 3 };

                if (format_ == 8)
                    compSel = new byte[4] { 0, 1, 2, 5 };
            }

            pitch = Math.Max(pitch, 8);
            var register0 = _register0(width - 1, (pitch / 8) - 1, 0, tileMode, 1);

            // register1
            var register1 = _register1((int)format_, 0, (int)height - 1);

            // register2
            var formatComp = 0;
            var numFormat = 0;
            var forceDegamma = 0;

            if ((format_ & 0x200) != 0)
                formatComp = 1;

            if ((format_ & 0x800) != 0)
                numFormat = 2;
            else if ((format_ & 0x100) != 0)
                numFormat = 1;

            if ((format_ & 0x400) != 0)
                forceDegamma = 1;

            var register2 = _register2(0, compSel[3], compSel[2], compSel[1], compSel[0], 2, 0, forceDegamma, 0, numFormat, formatComp);

            // register3
            var register3 = _register3(0, 0, 0, (int)numMips - 1);

            // register4
            var register4 = _register4(2, 0, 0, 0, 7, 4, 0);  // (2, 0, 0, 0, 0, 4, 0) in NSMBU

       /*     byte[] reg0 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(register0));
            byte[] reg1 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(register1));
            byte[] reg2 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(register2));
            byte[] reg3 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(register3));
            byte[] reg4 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(register4));*/

            return new uint[5] { register0, (uint)register1, (uint)register2, (uint)register3, (uint)register4 };
        }

    }
}
