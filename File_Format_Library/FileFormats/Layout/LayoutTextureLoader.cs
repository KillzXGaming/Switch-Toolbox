using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstPlugin;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public class LayoutTextureLoader
    {
        public static List<BNTX> SwitchTextures = new List<BNTX>();
        public static Dictionary<string, STGenericTexture> Textures = new Dictionary<string, STGenericTexture>();
        public static List<string> SearchFolderPaths = new List<string>();

    /*    public static Dictionary<string, STGenericTexture> GetTextures(List<string> textureList, IArchiveFile archive = null)
        {
            var textures = new Dictionary<string, STGenericTexture>();
            if (archive != null)
            {
                foreach (var file in archive.Files)
                {

                }
            }
        }*/
    }
}
