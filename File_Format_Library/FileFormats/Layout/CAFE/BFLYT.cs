using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using FirstPlugin.Forms;
using Syroot.Maths;
using SharpYaml.Serialization;
using FirstPlugin;
using System.ComponentModel;

namespace LayoutBXLYT.Cafe
{
    public class BFLYT : IFileFormat, IEditorForm<LayoutEditor>, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Layout (GUI)" };
        public string[] Extension { get; set; } = new string[] { "*.bflyt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FLYT");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => newFileExt;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => null;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] newFileExt = new STToolStripItem[1];

            public MenuExt()
            {
                newFileExt[0] = new STToolStripItem("Layout Editor");
                newFileExt[0].Click += LoadNewLayoutEditor;
            }

            private void LoadNewLayoutEditor(object sender, EventArgs e)
            {
                LayoutEditor editor = new LayoutEditor();
                editor.Show();
            }
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Yaml;
        public bool CanConvertBack => true;

        public string ConvertToString()
        {
            var settings = new SerializerSettings();
            settings.EmitTags = false;
            settings.EmitAlias = false;
            settings.EmitCapacityForList = false;
            settings.EmitShortTypeName = false;
            settings.ComparerForKeySorting = null;
            settings.DefaultStyle = SharpYaml.YamlStyle.Any;
            settings.LimitPrimitiveFlowSequence = 0;


         //   return FLYT.ToXml(header);

            var serializer = new Serializer(settings);
            string yaml = serializer.Serialize(header.RootPane, typeof(Header));
            return yaml;
        }

        public void ConvertFromString(string text)
        {
            header = FLYT.FromXml(text);
            header.FileInfo = this;
        }

        #endregion

        public LayoutEditor OpenForm()
        {
            LayoutEditor editor = new LayoutEditor();
            editor.Dock = DockStyle.Fill;
            editor.LoadBxlyt(header);
            return editor;
        }

        public void FillEditor(Form control) {
            ((LayoutEditor)control).LoadBxlyt(header);
        }

        public Header header;
        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream), this);
        }

        public List<GTXFile> GetShadersGTX()
        {
            List<GTXFile> shaders = new List<GTXFile>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".gsh")
                    {
                        GTXFile bnsh = (GTXFile)file.OpenFile();
                        shaders.Add(bnsh);
                    }
                }
            }
            return shaders;
        }

        public List<BNSH> GetShadersNX()
        {
            List<BNSH> shaders = new List<BNSH>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bnsh")
                    {
                        BNSH bnsh = (BNSH)file.OpenFile();
                        shaders.Add(bnsh);
                    }
                }
            }
            return shaders;
        }

        public List<BFLYT> GetLayouts()
        {
            List<BFLYT> animations = new List<BFLYT>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bflyt")
                    {
                        BFLYT bflyt = (BFLYT)file.OpenFile();
                        animations.Add(bflyt);
                    }
                }
            }
            return animations;
        }

        public List<BFLAN> GetAnimations()
        {
            List<BFLAN> animations = new List<BFLAN>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bflan" && file.FileFormat == null)
                    {
                        BFLAN bflan = (BFLAN)file.OpenFile();
                        file.FileFormat = bflan;
                        bflan.IFileInfo.ArchiveParent = IFileInfo.ArchiveParent;

                        //Disable saving unless the file gets edited
                        //Prevents broken files if version is unsupported
                        bflan.CanSave = false;
                        animations.Add(bflan);
                    }
                }
            }
            return animations;
        }

        public Dictionary<string, STGenericTexture> GetTextures()
        {
            Dictionary<string, STGenericTexture> textures = new Dictionary<string, STGenericTexture>();

            if (File.Exists(FilePath))
            {
                string folder = Path.GetDirectoryName(FilePath);
                foreach (var file in Directory.GetFiles(folder))
                {
                    try
                    {
                        if (Utils.GetExtension(file) == ".bflim")
                        {
                            BFLIM bflim = (BFLIM)STFileLoader.OpenFileFormat(file);
                            if (!textures.ContainsKey(bflim.FileName))
                                textures.Add(bflim.FileName, bflim);
                        }
                        if (Utils.GetExtension(file) == ".bntx")
                        {
                            BNTX bntx = (BNTX)STFileLoader.OpenFileFormat(file);
                            foreach (var tex in bntx.Textures)
                            {
                                if (!textures.ContainsKey(tex.Key))
                                    textures.Add(tex.Key, tex.Value);
                            }

                            string fileName = Path.GetFileName(file);
                            if (!header.TextureManager.BinaryContainers.ContainsKey(fileName))
                                header.TextureManager.BinaryContainers.Add(fileName, bntx);
                        }
                    }
                    catch (Exception ex)
                    {
                        STErrorDialog.Show($"Failed to load texture {file}. ", "Layout Editor", ex.ToString());
                    }
                }
            }

            foreach (var archive in PluginRuntime.SarcArchives)
            {
                foreach (var file in archive.Files)
                {
                    try
                    {
                        if (Utils.GetExtension(file.FileName) == ".bntx")
                        {
                            BNTX bntx = (BNTX)file.OpenFile();
                            file.FileFormat = bntx;
                            foreach (var tex in bntx.Textures)
                                if (!textures.ContainsKey(tex.Key))
                                    textures.Add(tex.Key, tex.Value);

                            if (!header.TextureManager.BinaryContainers.ContainsKey($"{archive.FileName}.bntx"))
                                header.TextureManager.BinaryContainers.Add($"{archive.FileName}.bntx", bntx);
                        }
                        if (Utils.GetExtension(file.FileName) == ".bflim")
                        {
                            BFLIM bflim = (BFLIM)file.OpenFile();
                            string name = bflim.FileName;
                            if (archive is SARC)
                            {
                                if (((SARC)archive).sarcData.HashOnly)
                                {
                                    var sarcEntry = file as SARC.SarcEntry;

                                    //Look through all textures and find a hash match
                                    name = sarcEntry.TryGetHash(header.Textures, "timg");
                                    name = Path.GetFileName(name);
                                }
                            }

                            file.FileFormat = bflim;
                            if (!textures.ContainsKey(bflim.FileName))
                                textures.Add(name, bflim);
                        }
                    }
                    catch (Exception ex)
                    {
                        STErrorDialog.Show($"Failed to load texture {file.FileName}. ", "Layout Editor", ex.ToString());
                    }
                }

                if (!header.TextureManager.ArchiveFile.ContainsKey(archive.FileName))
                    header.TextureManager.ArchiveFile.Add(archive.FileName, archive);
            }

            return textures;
        }

        public void Unload()
        {
            if (header != null)
            {
            }
        }

        public void Save(System.IO.Stream stream) {
            header.Write(new FileWriter(stream));
        }
    }
}
