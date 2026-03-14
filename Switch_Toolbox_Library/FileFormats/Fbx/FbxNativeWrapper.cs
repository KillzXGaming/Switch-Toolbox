using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK;

namespace Toolbox.Library.FBX
{
    public static class FbxNativeWrapper
    {
        private const string NativeDllName = "SwitchToolbox.FbxNative.dll";

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        static FbxNativeWrapper()
        {
            EnsureNativeResolver();
        }

        private static void EnsureNativeResolver()
        {
            try
            {
                string baseDir = Runtime.ExecutableDir;
                if (string.IsNullOrEmpty(baseDir))
                    baseDir = AppDomain.CurrentDomain.BaseDirectory;

                List<string> candidateDirs = new List<string>()
                {
                    baseDir,
                    AppDomain.CurrentDomain.BaseDirectory,
                    Environment.CurrentDirectory,
                    Path.Combine(baseDir, "Lib"),
                    Path.Combine(baseDir, "dist"),
                    Path.Combine(baseDir, "FBX", "Native", "dist"),
                };

                string assemblyDir = Path.GetDirectoryName(typeof(FbxNativeWrapper).Assembly.Location);
                if (!string.IsNullOrEmpty(assemblyDir))
                {
                    candidateDirs.Add(assemblyDir);
                    candidateDirs.Add(Path.Combine(assemblyDir, "Lib"));
                    candidateDirs.Add(Path.Combine(assemblyDir, "dist"));
                    candidateDirs.Add(Path.Combine(assemblyDir, "FBX", "Native", "dist"));
                }

                AddCandidateDir(candidateDirs, baseDir, "..", "FBX", "Native", "dist");
                AddCandidateDir(candidateDirs, baseDir, "..", "..", "FBX", "Native", "dist");
                AddCandidateDir(candidateDirs, baseDir, "..", "..", "..", "FBX", "Native", "dist");
                AddParentDistCandidates(candidateDirs, baseDir, 4);
                if (!string.IsNullOrEmpty(assemblyDir))
                    AddParentDistCandidates(candidateDirs, assemblyDir, 4);

                string nativePath = null;
                foreach (string dir in candidateDirs.Where(x => !string.IsNullOrEmpty(x)).Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    string path = Path.Combine(dir, NativeDllName);
                    if (File.Exists(path))
                    {
                        nativePath = path;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(nativePath))
                    return;

                string sdkPath = FindFbxSdkDll(nativePath, baseDir, candidateDirs);
                string stageDir = StageNativeInAsciiPath(nativePath, sdkPath);
                string loadDir = !string.IsNullOrEmpty(stageDir) ? stageDir : Path.GetDirectoryName(nativePath);
                if (string.IsNullOrEmpty(loadDir) || !Directory.Exists(loadDir))
                    return;

                SetDllDirectory(loadDir);
                AppendToProcessPath(loadDir);

                if (!string.IsNullOrEmpty(sdkPath))
                    AppendToProcessPath(Path.GetDirectoryName(sdkPath));

                string stagedSdkPath = Path.Combine(loadDir, "libfbxsdk.dll");
                if (File.Exists(stagedSdkPath))
                    LoadLibrary(stagedSdkPath);
                else if (!string.IsNullOrEmpty(sdkPath) && File.Exists(sdkPath))
                    LoadLibrary(sdkPath);

                LoadLibrary(Path.Combine(loadDir, NativeDllName));
            }
            catch
            {
                // Let DllImport report any unresolved native dependency with original details.
            }
        }

        private static string StageNativeInAsciiPath(string nativePath, string sdkSourcePath)
        {
            string[] roots = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SwitchToolbox", "Native", "fbx"),
                Path.Combine(Path.GetPathRoot(nativePath) ?? "C:\\", "SwitchToolboxNative", "fbx"),
                Path.Combine(Path.GetTempPath(), "SwitchToolboxNative", "fbx"),
            };

            foreach (string root in roots.Where(x => !string.IsNullOrEmpty(x)))
            {
                try
                {
                    Directory.CreateDirectory(root);
                    if (!IsAscii(root))
                        continue;

                    string targetNative = Path.Combine(root, NativeDllName);
                    File.Copy(nativePath, targetNative, true);

                    if (!string.IsNullOrEmpty(sdkSourcePath) && File.Exists(sdkSourcePath))
                        File.Copy(sdkSourcePath, Path.Combine(root, "libfbxsdk.dll"), true);

                    return root;
                }
                catch
                {
                    //Try next root
                }
            }

            return null;
        }

        private static string FindFbxSdkDll(string nativePath, string baseDir, List<string> candidateDirs)
        {
            string arch = Environment.Is64BitProcess ? "x64" : "x86";
            List<string> candidates = new List<string>()
            {
                Path.Combine(Path.GetDirectoryName(nativePath), "libfbxsdk.dll"),
            };

            foreach (string dir in candidateDirs.Where(x => !string.IsNullOrEmpty(x)))
                candidates.Add(Path.Combine(dir, "libfbxsdk.dll"));

            AddSdkCandidate(candidates, Path.GetDirectoryName(nativePath), "..", "..", "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "release", "libfbxsdk.dll");
            AddSdkCandidate(candidates, Path.GetDirectoryName(nativePath), "..", "..", "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "debug", "libfbxsdk.dll");
            AddSdkCandidate(candidates, baseDir, "..", "..", "..", "FBX", "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "release", "libfbxsdk.dll");
            AddSdkCandidate(candidates, baseDir, "..", "..", "..", "FBX", "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "debug", "libfbxsdk.dll");
            AddSdkCandidatesFromParents(candidates, baseDir, arch, 6);
            AddSdkCandidatesFromParents(candidates, Path.GetDirectoryName(nativePath), arch, 6);

            foreach (string path in candidates.Where(x => !string.IsNullOrEmpty(x)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (File.Exists(path))
                    return path;
            }

            return FindFbxSdkByDriveScan(baseDir, arch);
        }

        private static void AddSdkCandidate(List<string> files, string basePath, params string[] relative)
        {
            try
            {
                files.Add(Path.GetFullPath(Path.Combine(new[] { basePath }.Concat(relative).ToArray())));
            }
            catch
            {
                // Ignore bad paths
            }
        }

        private static void AppendToProcessPath(string dir)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return;

            string current = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            string[] entries = current.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (entries.Any(x => string.Equals(x.Trim(), dir, StringComparison.OrdinalIgnoreCase)))
                return;

            string updated = string.IsNullOrEmpty(current) ? dir : dir + ";" + current;
            Environment.SetEnvironmentVariable("PATH", updated, EnvironmentVariableTarget.Process);
        }

        private static void AddCandidateDir(List<string> dirs, string baseDir, params string[] relative)
        {
            try
            {
                dirs.Add(Path.GetFullPath(Path.Combine(new[] { baseDir }.Concat(relative).ToArray())));
            }
            catch
            {
                //Ignore bad paths
            }
        }

        private static void AddParentDistCandidates(List<string> dirs, string startDir, int maxLevels)
        {
            if (string.IsNullOrEmpty(startDir) || maxLevels < 1)
                return;

            try
            {
                string current = Path.GetFullPath(startDir);
                for (int i = 0; i < maxLevels && !string.IsNullOrEmpty(current); i++)
                {
                    dirs.Add(Path.Combine(current, "dist"));

                    DirectoryInfo parent = Directory.GetParent(current);
                    if (parent == null)
                        break;
                    current = parent.FullName;
                }
            }
            catch
            {
                // Ignore invalid path traversal.
            }
        }

        private static void AddSdkCandidatesFromParents(List<string> files, string startDir, string arch, int maxLevels)
        {
            if (string.IsNullOrEmpty(startDir) || maxLevels < 1)
                return;

            try
            {
                string current = Path.GetFullPath(startDir);
                for (int i = 0; i < maxLevels && !string.IsNullOrEmpty(current); i++)
                {
                    AddSdkCandidate(files, current, "FBX", "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "release", "libfbxsdk.dll");
                    AddSdkCandidate(files, current, "FBX", "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "debug", "libfbxsdk.dll");
                    AddSdkCandidate(files, current, "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "release", "libfbxsdk.dll");
                    AddSdkCandidate(files, current, "FBX SDK", "2017.0.1", "lib", "vs2015", arch, "debug", "libfbxsdk.dll");
                    AddSdkCandidate(files, current, "libfbxsdk.dll");

                    DirectoryInfo parent = Directory.GetParent(current);
                    if (parent == null)
                        break;
                    current = parent.FullName;
                }
            }
            catch
            {
                // Ignore invalid path traversal.
            }
        }

        private static string FindFbxSdkByDriveScan(string baseDir, string arch)
        {
            try
            {
                string root = Path.GetPathRoot(baseDir);
                if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
                    return null;

                return FindFbxSdkInDirectory(root, arch, 0, 5);
            }
            catch
            {
                return null;
            }
        }

        private static string FindFbxSdkInDirectory(string directory, string arch, int depth, int maxDepth)
        {
            if (depth > maxDepth || string.IsNullOrEmpty(directory))
                return null;

            try
            {
                foreach (string sub in Directory.EnumerateDirectories(directory))
                {
                    string name = Path.GetFileName(sub);
                    if (string.Equals(name, "FBX SDK", StringComparison.OrdinalIgnoreCase))
                    {
                        string release = Path.Combine(sub, "2017.0.1", "lib", "vs2015", arch, "release", "libfbxsdk.dll");
                        if (File.Exists(release))
                            return release;

                        string debug = Path.Combine(sub, "2017.0.1", "lib", "vs2015", arch, "debug", "libfbxsdk.dll");
                        if (File.Exists(debug))
                            return debug;
                    }

                    if (ShouldSkipDirectory(sub))
                        continue;

                    string path = FindFbxSdkInDirectory(sub, arch, depth + 1, maxDepth);
                    if (!string.IsNullOrEmpty(path))
                        return path;
                }
            }
            catch
            {
                // Ignore inaccessible folders.
            }

            return null;
        }

        private static bool ShouldSkipDirectory(string path)
        {
            try
            {
                string name = Path.GetFileName(path);
                if (string.IsNullOrEmpty(name))
                    return true;

                if (name.StartsWith("$", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (string.Equals(name, "Windows", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(name, "Program Files", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(name, "Program Files (x86)", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(name, "ProgramData", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(name, "System Volume Information", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(name, "$Recycle.Bin", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch
            {
                return true;
            }

            return false;
        }

        private static bool IsAscii(string text)
        {
            foreach (char c in text)
            {
                if (c > 127)
                    return false;
            }
            return true;
        }

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_CreateContext();

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_DestroyContext();

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exp_Initialize(string filename);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_CreateNode(string name, string parentName, double[] matrix);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddPose(string name);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_SetClusterMatrices(string meshName, string boneName, double[] meshMtx, double[] linkMtx);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_RegisterBone(int id, string name);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_StartMesh(string name);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_SetMeshMaterial(string matName);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddVertexExtended(
            double px, double py, double pz,
            double nx, double ny, double nz,
            int b0, int b1, int b2, int b3,
            double w0, double w1, double w2, double w3
        );

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddVertexTangent(double x, double y, double z);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddVertexBitangent(double x, double y, double z);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddVertexUV(int index, double u, double v);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddVertexColor(int index, double r, double g, double b, double a);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddIndex(int index);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Exp_EndMeshWithSkinning")]
        public static extern void Exp_EndMesh();

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddMaterial(string name, double r, double g, double b);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_SetMaterialShininess(string matName, double shininess);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Exp_AddMaterialTexture(string matName, string textureName, string filePath, int textureType);

        [DllImport(NativeDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exp_Save();
    }
}
