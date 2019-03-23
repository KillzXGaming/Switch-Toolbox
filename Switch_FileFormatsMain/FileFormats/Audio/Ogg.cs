using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using VGAudio.Formats;
using VGAudio.Containers.NintendoWare;
using CSCore;
using CSCore.Codecs;

namespace FirstPlugin
{
    public class OGG : IEditor<AudioPlayer>, IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Ogg-vorbis" };
        public string[] Extension { get; set; } = new string[] { "*.ogg" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                bool IsValidSig = reader.CheckSignature(4, "OggS");
                bool IsValidExt = Utils.HasExtension(FileName, ".ogg");

                if (IsValidExt || IsValidSig)
                    return true;
                else
                    return false;
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public AudioPlayer OpenForm()
        {
            AudioPlayer form = new AudioPlayer();
            form.Text = FileName;
            form.Dock = DockStyle.Fill;
            form.LoadFile(waveSource, this);

            return form;
        }

        IWaveSource waveSource;

        public void Load(System.IO.Stream stream)
        {
            stream.Position = 0;

            CanSave = true;
            waveSource = CodecFactory.Instance.GetCodec(stream, ".ogg");

        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }
    }
}
