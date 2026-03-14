using System.Runtime.InteropServices;

namespace Toolbox.Library.FBX
{
    public static class FbxAnimationNativeWrapper
    {
        const string Is64Bit = "SwitchToolbox.FbxNative.dll";

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_Anim_Reset();

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exp_Anim_Begin(string name, double frameRate, double startFrame, double endFrame, int looping);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exp_Anim_SetBoneDefaults(
            string boneName,
            double tx, double ty, double tz,
            double rx, double ry, double rz,
            double sx, double sy, double sz);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exp_Anim_AddKey(
            string boneName,
            int channel,
            double frame,
            double value,
            int interpolation,
            double tangentIn,
            double tangentOut);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_Anim_End();
    }
}
