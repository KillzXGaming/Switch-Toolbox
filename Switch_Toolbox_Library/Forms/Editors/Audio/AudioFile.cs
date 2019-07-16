using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace Toolbox.Library.Forms
{
    public class AudioFile
    {
        public string Title { get; set; }

        public string Duration
        {
            get
            {
                return GetDuration();
            }
        }

        public string Status { get; set; }
        public string Artist { get; set; } = "Unknown";

        public VGAdudioFile vgAdudioFile; //Original data for saving/converting/editing
        
        public List<AudioChannel> Channels = new List<AudioChannel>();

        public int SelectedChannelIndex = 0;

        public AudioChannel GetCurrentChannel()
        {
            return Channels[SelectedChannelIndex];
        }

        private string GetDuration()
        {
            TimeSpan position = GetCurrentChannel().audioPlayer.Position;
            TimeSpan length = GetCurrentChannel().audioPlayer.Length;

            return String.Format(@"{0:mm\:ss} / {1:mm\:ss}", position, length);
        }

        public void Dispose()
        {
            foreach (var channel in Channels)
            {
                if (channel.audioPlayer != null)
                    channel.audioPlayer.Dispose();
                if (channel.audioStream != null)
                    channel.audioStream.Dispose();

                channel.Data = null;
            }
        }
    }
}
