using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;

namespace Toolbox.Library.FBX
{
    public static class FbxNativeWrapper
    {
        const string Is64Bit = "SwitchToolbox.FbxNative.dll";

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_CreateContext();

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_DestroyContext();

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exp_Initialize(string filename);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_CreateNode(string name, string parentName, double[] matrix);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddPose(string name);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_SetClusterMatrices(string meshName, string boneName, double[] meshMtx, double[] linkMtx);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_RegisterBone(int id, string name);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_StartMesh(string name);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_SetMeshMaterial(string matName);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddVertex(
            double px, double py, double pz,
            double nx, double ny, double nz,
            double u0x, double u0y,
            double cr, double cg, double cb, double ca,
            int b0, int b1, int b2, int b3,
            double w0, double w1, double w2, double w3
        );

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddIndex(int index);

        // We switched to EndMeshWithSkinning in C++ as the primary way
        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Exp_EndMeshWithSkinning")]
        public static extern void Exp_EndMesh();

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddMaterial(string name, double r, double g, double b);

        [DllImport(Is64Bit, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exp_Save();
    }
}
