using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace Toolbox.Library.Forms
{
    public class AudioChannel
    {
        public byte[] Data;
        public WaveFormat Format
        {
            get
            {
                return audioPlayer._waveSource.WaveFormat;
            }
        }

        public readonly MusicPlayer audioPlayer = new MusicPlayer();
        public IWaveSource audioStream
        {
            get
            {
                return audioPlayer._waveSource;
            }
        }
        public ISampleSource samplerSource
        {
            get
            {
                return audioPlayer._sampleSource;
            }
        }
        public string Name { get; set; }
    }
}
