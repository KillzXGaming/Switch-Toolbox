using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VGAudio;
using NAudio.Wave;

namespace FirstPlugin
{
    public partial class BFAVEditor : UserControl
    {
        public BFAVEditor()
        {
            InitializeComponent();

        }
 
        public void LoadFile(BARS.AudioEntry entry)
        {
            propertyGrid1.SelectedObject = entry;

            MemoryStream audio = new MemoryStream(entry.BfwavToWav());

            if (!Directory.Exists("Temp"))
                Directory.CreateDirectory("Temp");

            File.WriteAllBytes($"Temp/{entry.Text}.wav", audio.ToArray());

        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
        }


        private void onPlaybackFinished(object sender, StoppedEventArgs e)
        {
       
        }

        bool IsPlaying = false;
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
            }
            else
            {
                IsPlaying = false;
            }
        }
    }
}
