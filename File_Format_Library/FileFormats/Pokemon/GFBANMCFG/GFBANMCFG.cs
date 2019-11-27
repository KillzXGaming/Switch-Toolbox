using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using OpenTK;
using System.Reflection;
using Toolbox.Library.Forms;
using Newtonsoft.Json;

namespace FirstPlugin
{
    public class GFBANMCFG : IEditor<TextEditor>, IFileFormat, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GFBANMCFG" };
        public string[] Extension { get; set; } = new string[] { "*.gfbanmcfg" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".gfbanmcfg";
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public AnimConfig Config;

        public void Load(System.IO.Stream stream)
        {
            Config = new AnimConfig();

            var flatBuffer = FlatBuffers.Gfbanmcfg.AnimationConfig.GetRootAsAnimationConfig(
                new FlatBuffers.ByteBuffer(stream.ToBytes()));

            if (flatBuffer.Animations != null) {
                for (int i = 0; i < flatBuffer.Animations.Value.AnimationsLength; i++)
                {
                    var anim = flatBuffer.Animations.Value.Animations(i).Value;
                    Console.WriteLine(anim.Name);

                    Config.Animations.Add(new Animation()
                    {
                        Name = anim.Name,
                        FileName = anim.File,
                    });
                }
            }
        }

        public class AnimConfig
        {
            public List<Animation> Animations = new List<Animation>();
        }

        public class Animation
        {
            public string Name { get; set; }
            public string FileName { get; set; }
        }

        public TextEditor OpenForm()
        {
            return new TextEditor();
        }

        public void FillEditor(UserControl control)
        {
            ((TextEditor)control).FileFormat = this;
            ((TextEditor)control).FillEditor(ConvertToString());
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Json;
        public bool CanConvertBack => false;

        public string ConvertToString() {
            return JsonConvert.SerializeObject(Config, Formatting.Indented, new JsonSerializerSettings());
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
