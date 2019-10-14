using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using FirstPlugin;
using System.Windows.Forms;

namespace LayoutBXLYT
{
    public class TextureManager : IDisposable
    {
        public Dictionary<string, BNTX> BinaryContainers = new Dictionary<string, BNTX>();

        //The archive to put textures in if necessary
        public Dictionary<string, IArchiveFile> ArchiveFile = new Dictionary<string, IArchiveFile>();

        public IArchiveFile ArchiveParent;

        public PlatformType Platform = PlatformType.WiiU;

        public enum PlatformType
        {
            WiiU, //bflim
            ThreeDS, //bclim and bflim
            DS, //
            Gamecube, //bti
            Switch, //bntx
            Wii, //TPL
        }

        public STGenericTexture EditTexture(string name)
        {
            STGenericTexture texture = null;
            switch (Platform)
            {
                case PlatformType.Switch:
                    {
                        foreach (var bntx in BinaryContainers.Values)
                        {
                            Console.WriteLine("bntx " + name + " " + bntx.Textures.ContainsKey(name));
                            if (bntx.Textures.ContainsKey(name))
                            {
                                OpenFileDialog ofd = new OpenFileDialog();
                                ofd.Filter = bntx.Textures[name].ReplaceFilter;
                                if (ofd.ShowDialog() == DialogResult.OK)
                                {
                                    bntx.Textures[name].Replace(ofd.FileName);
                                    return bntx.Textures[name];
                                }
                            }
                        }
                    }
                    break;
                case PlatformType.WiiU:
                    {
                        foreach (var file in ArchiveParent.Files)
                        {
                            if (file.FileName == name)
                            {
                                var fileFormat = file.FileFormat;
                                if (fileFormat == null)
                                    fileFormat = file.OpenFile();

                                if (fileFormat is BFLIM)
                                {
                                    OpenFileDialog ofd = new OpenFileDialog();
                                    ofd.Filter = ((BFLIM)fileFormat).ReplaceFilter;
                                    ofd.FileName = name;

                                    if (ofd.ShowDialog() == DialogResult.OK)
                                    {
                                        ((BFLIM)fileFormat).Replace(ofd.FileName);
                                        return (BFLIM)fileFormat;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            return texture;
        }

        public List<STGenericTexture> AddTextures()
        {
            List<STGenericTexture> textures = new List<STGenericTexture>();

            switch (Platform)
            {
                case PlatformType.WiiU:
                    {
                        var archive = ArchiveParent;
                        var bflim = BFLIM.CreateNewFromImage();
                        if (bflim == null)
                            return textures;

                        textures.Add(bflim);

                        var mem = new System.IO.MemoryStream();
                        bflim.Save(mem);
                        archive.AddFile(new ArchiveFileInfo()
                        {
                            FileData = mem.ToArray(),
                            FileName = bflim.FileName,
                        });
                    }
                    break;
                case PlatformType.Switch:
                    {
                        BNTX bntx = null;
                        if (BinaryContainers.Count == 0)
                        {
                            //Create a new one if none exist
                            //Method for saving these will come in the save dialog
                            bntx = new BNTX();
                            bntx.IFileInfo = new IFileInfo();
                            bntx.FileName = "textures";
                            bntx.Load(new System.IO.MemoryStream(BNTX.CreateNewBNTX("textures")));
                            BinaryContainers.Add("textures", bntx);
                        }
                        else
                        {
                            //Use first container for now as archives only use one
                            bntx = BinaryContainers.Values.FirstOrDefault();
                        }

                        int startIndex = bntx.Textures.Count;
                        bntx.ImportTexture();

                        //Load all the additional textues

                        for (int i = 0; i < bntx.Textures.Count; i++)
                        {
                            if (i > startIndex - 1)
                                textures.Add(bntx.Textures.Values.ElementAt(i));
                        }
                    }
                    break;
            }

            return textures;
        }

        public void Dispose()
        {
            BinaryContainers.Clear();
            ArchiveFile.Clear();
        }
    }
}
