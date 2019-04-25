using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.Animations;
using Bfres.Structs;

namespace FirstPlugin
{
    public class FileFilters
    {
        public static string BNTX_TEX = GetFilter(".bftex", ".dds",".astc", ".png", ".tga", ".jpg", ".tiff", ".tif", ".gif");
        public static string FTEX = GetFilter(".bftex", ".dds", ".dds2", ".png", ".tga", ".jpg", ".tiff", ".tif", ".gif");
        public static string FMDL = GetFilter(".bfmdl", ".dae", ".fbx", ".obj", ".csv");
        public static string FMDL_EXPORT = GetFilter(".bfmdl", ".dae");
        public static string FSKL = GetFilter(".bfskl");
        public static string FSHP = GetFilter(".bfobj");
        public static string BONE = GetFilter(".bfbon");
        public static string FMAT = GetFilter(".bfmat");

        public static string FSKA = GetFilter(".bfska", ".seanim", ".smd");
        public static string FMAA = GetFilter(".bfmaa", ".gif");
        public static string FSHU = GetFilter(".bfshu");

        public static string FCLH = GetFilter(".bfcsh");
        public static string FSTH = GetFilter(".bfsth");
        public static string FTXP = GetFilter(".bftxp", ".gif");
        public static string FMTV = GetFilter(".bfmvi");
        public static string FBNV = GetFilter(".bfbvi");
        public static string FSCN = GetFilter(".bfscn");
        public static string FSHA = GetFilter(".bfspa");

        public static string NUTEXB = GetFilter(".dds",".png", ".tga", ".jpg", ".tiff", ".tif", ".gif");

        public static string GetFilter(Type type, object CheckAnimEffect = null, bool IsExporting = false)
        {
            if (type == typeof(TextureData)) return BNTX_TEX;
            else if (type == typeof(FMDL) && IsExporting) return FMDL_EXPORT;
            else if (type == typeof(FMDL)) return FMDL;
            else if (type == typeof(FSHP)) return FSHP;
            else if (type == typeof(FMAT)) return FMAT;
            else if (type == typeof(FSKL)) return FSKL;
            else if (type == typeof(BfresBone)) return BONE;
            else if (type == typeof(FSKA)) return FSKA;
            else if (type == typeof(FMAA)) return FMAA;
            else if (type == typeof(FTXP)) return FTXP;
            else if (type == typeof(FSHA)) return FSHA;
            else if (type == typeof(FTEX)) return FTEX;
            else if (type == typeof(FSHU))
            {
                if (CheckAnimEffect is FSHU.AnimationType)
                {
                    var animType = (MaterialAnimation.AnimationType)CheckAnimEffect;

                    if (animType == MaterialAnimation.AnimationType.Color) return FCLH;
                    if (animType == MaterialAnimation.AnimationType.TextureSrt) return FSTH;
                    else return FSHA;
                }
                if (CheckAnimEffect is VisibiltyAnimType)
                {
                    var animType = (VisibiltyAnimType)CheckAnimEffect;

                    if (animType == VisibiltyAnimType.Material) return FMTV;
                    else return FBNV;
                }
                return FSHU;
            }
            else if (type == typeof(FVIS))
            {
                return FBNV;
            }
            else return "All Files (*.*)|*.*";
        }

        public static Dictionary<string, string> GetDescription(string[] extensions)
        {
            var filters = new Dictionary<string, string>();
            foreach (string ext in extensions)
            {
                switch (ext)
                {
                    case ".bfmdl": filters.Add(ext, "Bfres Model"); break;
                    case ".bfmat": filters.Add(ext, "Bfres Material"); break;
                    case ".bfobj": filters.Add(ext, "Bfres Object (shape/vertices)"); break;
                    case ".bfbn": filters.Add(ext, "Bfres Bone"); break;
                    case ".bfskl": filters.Add(ext, "Bfres Skeleton"); break;
                    case ".bfska": filters.Add(ext, "Bfres Skeletal Animation"); break;
                    case ".bfmma": filters.Add(ext, "Bfres Material Animation"); break;
                    case ".bfshu": filters.Add(ext, "Bfres Shader Param Animation"); break;
                    case ".bfcsh": filters.Add(ext, "Bfres Color Animation"); break;
                    case ".bfsth": filters.Add(ext, "Bfres Texture Srt Animation"); break;
                    case ".bftxp": filters.Add(ext, "Bfres Texture Pattern Animation"); break;
                    case ".bfbvi": filters.Add(ext, "Bfres Bone Visibilty Animation"); break;
                    case ".bfmvi": filters.Add(ext, "Bfres Material Visibilty Animation"); break;
                    case ".bfspa": filters.Add(ext, "Bfres Shape Animation"); break;
                    case ".bfscn": filters.Add(ext, "Bfres Scene Animation"); break;
                    case ".dae": filters.Add(ext, "DAE"); break;
                    case ".fbx": filters.Add(ext, "FBX"); break;
                    case ".obj": filters.Add(ext, "OBJ"); break;
                    case ".csv": filters.Add(ext, "CSV"); break;
                    case ".dds": filters.Add(ext, "Microsoft DDS"); break;
                    case ".tga": filters.Add(ext, "TGA"); break;
                    case ".png": filters.Add(ext, "Portable Network Graphics"); break;
                    case ".jpg": filters.Add(ext, "Joint Photographic Experts Group"); break;
                    case ".bmp": filters.Add(ext, "Bitmap Image"); break;
                    case ".tiff": filters.Add(ext, "Tagged Image File Format"); break;
                    case ".tif": filters.Add(ext, "Tagged Image File Format"); break;
                    case ".seanim": filters.Add(ext, "SE Animation"); break;
                    case ".smd": filters.Add(ext, "Source Model Animation"); break;
                    case ".bftex": filters.Add(ext, "Binary Texture"); break;
                    case ".astc": filters.Add(ext, "Adaptable Scalable Texture Compression"); break;
                    default:
                        filters.Add(ext, ""); break;
                }
            }
            return filters;
        }


        private static string GetFilter(params string[] extensions)
        {
            return GetCompleteFilter(GetDescription(extensions));
        }

        public static string GetCompleteFilter(Dictionary<string, string> files)
        {
            if (files.Count == 0)
                return "All Files (*.*)|*.*";

            string Filter = "All Supported Files|";
            List<string> FilterEach = new List<string>();
            foreach (var Extension in files)
            {
                Filter += $"*{Extension.Key};";
                FilterEach.Add($"{Extension.Value} (*{Extension.Key}) |*{Extension.Key}|");
            }

            Filter += "|";
            Filter += string.Join("", FilterEach.ToArray());
            Filter += "All files(*.*)|*.*";
            return Filter;
        }
    }
}
