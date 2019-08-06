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

namespace FirstPlugin
{
    public class BCWAV : VGAdudioFile, IEditor<AudioPlayerPanel>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Audio;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Ctr Wave" };
        public string[] Extension { get; set; } = new string[] { "*.bcwav" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "CWAV");
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
            form.LoadFile(audioData, this);
            return form;
        }

        public void FillEditor(UserControl control)
        {
            ((AudioPlayerPanel)control).LoadFile(audioData, this);
        }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            LoadAudio(stream, this);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            SaveAudio(stream);
        }
    }
}
