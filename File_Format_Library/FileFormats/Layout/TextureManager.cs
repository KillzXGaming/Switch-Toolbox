using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Library;
using FirstPlugin;
using System.Windows.Forms;

namespace LayoutBXLYT
{
    public class TextureManager : IDisposable
    {
        public BxlytHeader LayoutFile;

        public Dictionary<string, BNTX> BinaryContainers = new Dictionary<string, BNTX>();

        //The archive to put textures in if necessary
        public Dictionary<string, IArchiveFile> ArchiveFile = new Dictionary<string, IArchiveFile>();

        public IArchiveFile ArchiveParent
        {
            get { return LayoutFile.FileInfo.IFileInfo.ArchiveParent; }
        }

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
                        if (ArchiveParent == null) return null;

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
                                        ((BFLIM)fileFormat).Text = name;
                                        return (BFLIM)fileFormat;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PlatformType.Wii:
                    if (ArchiveParent == null) return null;
                    foreach (var file in ArchiveParent.Files)
                    {
                        if (file.FileName == name)
                        {
                            var fileFormat = file.FileFormat;
                            if (fileFormat == null)
                                fileFormat = file.OpenFile();

                            if (fileFormat is TPL)
                            {

                            }
                        }
                    }
                    break;
            }
            return texture;
        }

        public void RemoveTextures(List<STGenericTexture> textures) {
            foreach (var tex in textures)
                RemoveTexture(tex);
        }

        public void RemoveTexture(STGenericTexture texture)
        {
            switch (Platform)
            {
                case PlatformType.WiiU:
                    {
                        var archive = ArchiveParent;
                        if (archive == null) return;

                        ArchiveFileInfo fileInfoDelete = null;
                        foreach (var file in archive.Files)
                        {
                            if (file.FileName.Contains(texture.Text))
                                fileInfoDelete = file;
                        }

                        if (fileInfoDelete != null)
                            archive.DeleteFile(fileInfoDelete);
                    }
                    break;
                case PlatformType.Switch:
                    {
                        foreach (var bntx in BinaryContainers.Values)
                        {
                            if (bntx.Textures.ContainsKey(texture.Text))
                                bntx.RemoveTexture(bntx.Textures[texture.Text]);
                        }
                    }
                    break;
                default:
                    {
                        var archive = ArchiveParent;
                        if (archive == null) return;

                        ArchiveFileInfo fileInfoDelete = null;
                        foreach (var file in archive.Files)
                        {
                            if (file.FileName.Contains(texture.Text))
                                fileInfoDelete = file;
                        }

                        if (fileInfoDelete != null)
                            archive.DeleteFile(fileInfoDelete);
                    }
                    break;
            }
        }

        public List<STGenericTexture> AddTextures()
        {
            List<STGenericTexture> textures = new List<STGenericTexture>();

            switch (Platform)
            {
                case PlatformType.WiiU:
                    {
                        var archive = ArchiveParent;
                        if (archive == null) return null;

                        var matches = archive.Files.Where(p => p.FileName.Contains("bflim")).ToList();
                        string textureFolder = "timg";
                        if (matches.Count > 0)
                            textureFolder = System.IO.Path.GetDirectoryName(matches[0].FileName);

                        var bflim = BFLIM.CreateNewFromImage();

                        if (bflim == null)
                            return textures;

                        textures.Add(bflim);

                        var mem = new System.IO.MemoryStream();
                        bflim.Save(mem);
                        archive.AddFile(new ArchiveFileInfo()
                        {
                            FileData = mem.ToArray(),
                            FileName = System.IO.Path.Combine(textureFolder, bflim.Text).Replace('\\','/'),
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

                        var importedTextures = bntx.ImportTexture();

                        //Load all the additional textues
                        for (int i = 0; i < importedTextures.Count; i++)
                            textures.Add(importedTextures[i]);
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
