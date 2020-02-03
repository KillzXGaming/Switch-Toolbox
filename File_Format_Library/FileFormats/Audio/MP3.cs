using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using VGAudio.Formats;
using VGAudio.Containers.NintendoWare;
using CSCore;
using CSCore.Codecs;

namespace FirstPlugin
{
    public class MP3 : IEditor<AudioPlayerPanel>, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Audio;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "MPEG-1 Audio Layer-3" };
        public string[] Extension { get; set; } = new string[] { "*.mp3" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                bool IsValidSig = reader.CheckSignature(3, "ID3");
                bool IsValidExt = Utils.HasExtension(FileName, ".mp3");

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

        public AudioPlayerPanel OpenForm()
        {
            AudioPlayerPanel form = new AudioPlayerPanel();
            form.Text = FileName;
            form.Dock = DockStyle.Fill;
            return form;
        }

        public void FillEditor(UserControl control)
        {
            ((AudioPlayerPanel)control).LoadFile(waveSource, this, false, mp3Struct);
        }

        IWaveSource waveSource;
        object mp3Struct;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            waveSource = CodecFactory.Instance.GetCodec(stream, ".mp3");

            stream.Position = 0;

            mp3Struct = CSCore.Tags.ID3.ID3v1.FromStream(stream);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
