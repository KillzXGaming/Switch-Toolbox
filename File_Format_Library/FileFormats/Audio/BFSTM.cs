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
    public class BFSTM : VGAdudioFile, IEditor<AudioPlayerPanel>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Audio;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Stream" };
        public string[] Extension { get; set; } = new string[] { "*.bfstm" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FSTM");
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
            if (audioData == null)
                throw new Exception("Audio data is null!");

            AudioPlayerPanel form = new AudioPlayerPanel();
            form.Text = FileName;
            form.Dock = DockStyle.Fill;

            try
            {
                form.LoadFile(audioData, this);
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to open audio player!", "Audio Player" , ex.ToString());
            }


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
