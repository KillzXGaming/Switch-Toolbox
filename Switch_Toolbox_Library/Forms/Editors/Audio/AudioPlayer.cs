using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using VGAudio.Containers.Wave;
using VGAudio.Formats;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformsVisualization.Visualization;
using CSCore.DSP;
using CSCore.Streams.Effects;
using CSCore;
using CSCore.SoundOut;
using CSCore.CoreAudioAPI;
using CSCore.Tags.ID3;

namespace Toolbox.Library.Forms
{
    public partial class AudioPlayer : STForm
    {
        private LineSpectrum lineSpectrum;
        private PitchShifter _pitchShifter;
        private bool stopSliderUpdate;
        private MMDevice activeDevice;

        public AudioFile selectedFile;

        public VisualSetting visualSetting = VisualSetting.WaveSpectum;

        public List<IFileFormat> AudioFileFormats = new List<IFileFormat>();

        public enum VisualSetting
        {
            Artwork,
            WaveSpectum,
        }

        private AudioChannel GetActiveAudio()
        {
            if (selectedFile == null)
                return null;

            return selectedFile.GetCurrentChannel();
        }

        public AudioPlayer()
        {
            InitializeComponent();
            SetTheme();
            SetDevices();
            audioListView.FillLastColumnSpace(true);
        }

        #region File Open

        public void LoadFile(string AudioFileName)
        {

        }

        public void LoadFile(byte[] AudioData)
        {

        }

        public void LoadFile(IWaveSource source, IFileFormat fileFormat, bool ClearPlaylist = false, object AudioStruct = null)
        {
            if (ClearPlaylist)
                audioListView.Items.Clear();

            AudioFile file = new AudioFile();
            file.Title = fileFormat.FileName;

            if (AudioStruct is ID3v1)
            {
                var mp3 = (ID3v1)AudioStruct;

                file.Title = mp3.Title;
                file.Artist = mp3.Artist;
            }

            AudioFileFormats.Add(fileFormat);


            audioListView.AddObject(file);

            AudioChannel audioChannel = new AudioChannel();
            audioChannel.Name = $"Channel [0]";
            file.Channels.Add(audioChannel);
            audioChannel.audioPlayer.Open(source, activeDevice);

            audioChannel.audioPlayer.PlaybackStopped += (s, args) =>
            {
                //WasapiOut uses SynchronizationContext.Post to raise the event
                //There might be already a new WasapiOut-instance in the background when the async Post method brings the PlaybackStopped-Event to us.
                if (audioChannel.audioPlayer.PlaybackState != PlaybackState.Stopped)
                {

                }
            };
            audioListView.UpdateObject(file);

        }

        public void LoadFile(AudioData audioData, IFileFormat fileFormat, bool ClearPlaylist = false)
        {
            if (ClearPlaylist)
                audioListView.Items.Clear();

            AudioFileFormats.Add(fileFormat);

            //Load Channel Info
            AudioFile file = new AudioFile();
            file.Title = fileFormat.FileName;
            if (fileFormat is VGAdudioFile)
                file.vgAdudioFile = (VGAdudioFile)fileFormat;

            //Loop through each channel and set it's own 
            var format = audioData.GetAllFormats().ToArray()[0];
            for (int c = 0; c < format.ChannelCount; c++)
            {
                using (var memWav = new MemoryStream())
                {
                    AudioChannel audioChannel = new AudioChannel();
                    audioChannel.Name = $"Channel [{c}]";
                    file.Channels.Add(audioChannel);

                    //Load data and write to stream
                    var audio = format.GetChannels(c).ToPcm16();
                    var writer = new WaveWriter();
                    writer.WriteToStream(audio, memWav);
                    audioChannel.Data = memWav.ToArray();

                    memWav.Position = 0;

                    //Load the player
                    audioChannel.audioPlayer.Open(new MemoryStream(audioChannel.Data), "test.wav", activeDevice);

                    /*     OpenFileDialog openFileDialog = new OpenFileDialog();
                         if (openFileDialog.ShowDialog() == DialogResult.OK)
                         {
                             audioChannel.audioPlayer.Open(openFileDialog.FileName, activeDevice);
                         }*/


                    audioChannel.audioPlayer.PlaybackStopped += (s, args) =>
                    {
                        //WasapiOut uses SynchronizationContext.Post to raise the event
                        //There might be already a new WasapiOut-instance in the background when the async Post method brings the PlaybackStopped-Event to us.
                        if (audioChannel.audioPlayer.PlaybackState != PlaybackState.Stopped)
                        {

                        }
                    };
                }
            }

            audioListView.AddObject(file);

            if (audioListView.Items.Count != 0)
                audioListView.SelectedIndex = 0;
        }

        #endregion


        #region GUI

        private void UpdateAudioData()
        {
            foreach (var audio in audioListView.Objects)
                audioListView.RefreshObject(audio);
        }

        private void FillArtPanel()
        {
            switch (visualSetting)
            {
                case VisualSetting.Artwork:
                    break;
                case VisualSetting.WaveSpectum:
                    GenerateLineSpectrum();
                    break;
            }
        }

        private void FillForm()
        {
            if (selectedFile == null)
            {
                editToolStripMenuItem.Enabled = false;
                return;
            }
            editToolStripMenuItem.Enabled = true;

            //Load channel data
            channelCB.Items.Clear();
            channelCB.Items.Add("Channel [All]");
            for (int i = 0; i < selectedFile.Channels.Count; i++)
            {
                channelCB.Items.Add(selectedFile.Channels[i].Name);
            }
            channelCB.SelectedIndex = selectedFile.SelectedChannelIndex;

            //Setup a sample source for spectum data
            lineSpectrum = (selectedFile.GetCurrentChannel().audioPlayer._lineSpectrum);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var channel = GetActiveAudio();
            if (channel == null)
                return;

            TimeSpan position = channel.audioPlayer.Position;
            TimeSpan length = channel.audioPlayer.Length;
            if (position > length)
            {
                length = position;
            }
            if (position >= length)
            {
                if (chkLoopPlayer.Checked)
                {
                    channel.audioPlayer.Position = TimeSpan.Zero;
                    colorSlider1.Value = 0;
                    channel.audioPlayer.Play();
                }
            }


            stLabel1.Text = String.Format(@"{0:mm\:ss}", position);
            stLabel2.Text = String.Format(@"{0:mm\:ss}", length);

            FillArtPanel();

            if (!stopSliderUpdate &&
                length != TimeSpan.Zero && position != TimeSpan.Zero)
            {
                double perc = position.TotalMilliseconds / length.TotalMilliseconds * colorSlider1.Maximum;
                colorSlider1.Value = (int)perc;
            }
            audioListView.RefreshObject(selectedFile);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            var channel = GetActiveAudio();
            if (channel == null)
                return;


            UpdateAudioData();

            //Pause if being played, or vice versa
            if (channel.audioPlayer.PlaybackState == PlaybackState.Playing)
            {
                channel.audioPlayer.Pause();
                btnPlay.BackgroundImage = Properties.Resources.PlayArrowR;
            }
            else
            {
                channel.audioPlayer.Play();
                btnPlay.BackgroundImage = Properties.Resources.PauseBtn;
            }

            //Set the status in the list and update it
            selectedFile.Status = channel.audioPlayer.PlaybackState.ToString();
        }

        private void audioListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (audioListView.SelectedObject != null)
            {
                if ((AudioFile)audioListView.SelectedObject == selectedFile)
                    return;

                selectedFile = (AudioFile)audioListView.SelectedObject;
                stLabel2.Text = selectedFile.Duration;

                FillForm();
            }
        }

        #endregion

        #region SpectumBar


        private void GenerateLineSpectrum()
        {
            if (lineSpectrum == null)
                return;

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            Image image = pictureBox1.Image;
            var newImage = lineSpectrum.CreateSpectrumLine(pictureBox1.Size, Color.Green, Color.Red, Color.Black, true);
            if (newImage != null)
            {
                pictureBox1.Image = newImage;
                if (image != null)
                    image.Dispose();
            }
        }

        #endregion

        #region Themes
        private void SetTheme()
        {
            foreach (BrightIdeasSoftware.OLVColumn column in audioListView.Columns)
            {
                var headerstyle = new BrightIdeasSoftware.HeaderFormatStyle();
                headerstyle.SetBackColor(FormThemes.BaseTheme.FormBackColor);
                headerstyle.SetForeColor(FormThemes.BaseTheme.FormForeColor);
                column.HeaderFormatStyle = headerstyle;
            }

            stPanel1.BackColor = FormThemes.BaseTheme.ObjectEditorBackColor;

            btnPlay.BackColor = stPanel1.BackColor;
            btnBackward1.BackColor = stPanel1.BackColor;
            btnForward1.BackColor = stPanel1.BackColor;
            audioListView.BackColor = FormThemes.BaseTheme.TextEditorBackColor;
            audioListView.FullRowSelect = true;
            audioBarPanel.BackColor = stPanel1.BackColor;
        }
        #endregion

        #region Audio Devices

        //Get audio devices to play audio out
        private void SetDevices()
        {
            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (
                    var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        audioDevice.Items.Add(device);

                        if (device.DeviceState == DeviceState.Active)
                            audioDevice.SelectedIndex = audioDevice.Items.IndexOf(device);
                    }
                }
            }

            if (audioDevice.Items.Count <= 0)
                throw new Exception("No audio devices found!");

            activeDevice = (MMDevice)audioDevice.SelectedItem;
        }

        #endregion

        private void ResetPlayers()
        {
            if (selectedFile == null || selectedFile.Channels == null)
                return;

            foreach (var channel in selectedFile.Channels)
            {
                channel.audioPlayer.Stop();
                channel.audioPlayer.Position = TimeSpan.Zero;
            }
        }

        private void colorSlider1_ValueChanged(object sender, EventArgs e)
        {
            var channel = GetActiveAudio();
            if (channel == null)
                return;

            if (stopSliderUpdate)
            {
                double perc = colorSlider1.Value / (double)colorSlider1.Maximum;
                TimeSpan position = TimeSpan.FromMilliseconds(channel.audioPlayer.Length.TotalMilliseconds * perc);
                channel.audioPlayer.Position = position;
            }
        }

        private void colorSlider1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                stopSliderUpdate = true;
        }

        private void colorSlider1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                stopSliderUpdate = false;
        }

        private void trackbarVolume_ValueChanged(object sender, EventArgs e)
        {
            var channel = GetActiveAudio();
            if (channel == null)
                return;

            channel.audioPlayer.Volume = trackbarVolume.Value;
        }


        private void AudioPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("Closing audio form");

            var channel = GetActiveAudio();
            if (channel != null)
            {
                channel.audioPlayer.Stop();
                channel.audioPlayer.Position = TimeSpan.Zero;
                colorSlider1.Value = 0;
            }

            channelCB.Items.Clear();

            foreach (var obj in audioListView.Objects)
            {
                foreach (var chan in ((AudioFile)obj).Channels)
                {
                    chan.audioStream.Dispose();
                    chan.audioPlayer.Dispose();
                    chan.samplerSource.Dispose();
                }

                ((AudioFile)obj).Dispose();
            }
            audioListView.ClearObjects();
        }

        private void loopingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedFile != null && selectedFile.vgAdudioFile != null   )
            {
                LoopEditor editor = new LoopEditor();
                editor.chkCanLoop.Checked = selectedFile.vgAdudioFile.audioWithConfig.AudioFormat.Looping;
                editor.startLoopUD.Value = (decimal)selectedFile.vgAdudioFile.audioWithConfig.AudioFormat.LoopStart;
                editor.endLoopUD.Value = (decimal)selectedFile.vgAdudioFile.audioWithConfig.AudioFormat.LoopEnd;

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    selectedFile.vgAdudioFile.audioData.SetLoop(
                         editor.chkCanLoop.Checked,
                    (int)editor.startLoopUD.Value,
                    (int)editor.endLoopUD.Value);
                }
            }

     
        }

        private void audioDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (audioDevice.SelectedIndex != -1)
                activeDevice = (MMDevice)audioDevice.SelectedItem;
        }

        private void channelCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedFile == null)
                return;

            ResetPlayers();
            if (channelCB.SelectedIndex >= 1)
            {
                var prevChannel = selectedFile.GetCurrentChannel();

                switch (prevChannel.audioPlayer.PlaybackState)
                {
                    case PlaybackState.Playing:
                        selectedFile.GetCurrentChannel().audioPlayer.Play();
                        break;
                    case PlaybackState.Stopped:
                        selectedFile.GetCurrentChannel().audioPlayer.Stop();
                        break;
                    case PlaybackState.Paused:
                        selectedFile.GetCurrentChannel().audioPlayer.Pause();
                        break;
                }

                selectedFile.SelectedChannelIndex = channelCB.SelectedIndex - 1;
                selectedFile.GetCurrentChannel().audioPlayer.Position = prevChannel.audioPlayer.Position;
                trackbarVolume.Value = selectedFile.GetCurrentChannel().audioPlayer.Volume;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            var channel = GetActiveAudio();
            if (channel == null)
                return;

            if (channel.audioPlayer.PlaybackState != PlaybackState.Stopped)
            {
                channel.audioPlayer.Stop();
                channel.audioPlayer.Position = TimeSpan.Zero;
                colorSlider1.Value = 0;
            }
        }

        private void trackbarVolume_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var channel = GetActiveAudio();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WAV |*.wav;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                channel.audioStream.WriteToFile(ofd.FileName);
            }
        }
    }
}
