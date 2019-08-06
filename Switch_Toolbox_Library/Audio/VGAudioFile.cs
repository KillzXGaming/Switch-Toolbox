using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using VGAudio.Containers;
using VGAudio.Containers.Hps;
using VGAudio.Containers.Idsp;
using VGAudio.Containers.NintendoWare;
using VGAudio.Containers.Wave;
using VGAudio.Formats;
using VGAudio.Utilities;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class VGAdudioFile
    {
        public AudioData audioData;

        //Configs
        public AudioWithConfig audioWithConfig;
        public BxstmConfiguration bxstmConfiguration;
        public IdspConfiguration idspConfiguration;
        public HpsConfiguration hpsConfiguration;
        public WaveConfiguration waveConfiguration;

        //Structs
        BxstmStructure bxstmStructure;
        HpsStructure hpsStructure;
        IdspStructure idspStructure;
        WaveStructure waveStructure;

        public IFileFormat Format;

        public void LoadAudio(Stream stream, IFileFormat format)
        {
            stream.Position = 0;

            Format = format;

            foreach (string ext in Format.Extension)
            {
                string extension = ext.TrimStart('*');
                switch (extension)
                {
                    case ".bfstm":
                    case ".bcstm":
                    case ".bfwav":
                    case ".bcwav":
                        var bcfstmReader = new BCFstmReader();
                        audioWithConfig = bcfstmReader.ReadWithConfig(stream);
                        stream.Position = 0;
                        bxstmStructure = bcfstmReader.ReadMetadata(stream);
                        break;
                    case ".brstm":
                    case ".brwav":
                        var brstmReader = new BrstmReader();
                        bxstmStructure = brstmReader.ReadMetadata(stream);
                        stream.Position = 0;
                        audioWithConfig = brstmReader.ReadWithConfig(stream);
                        break;
                    case ".idsp":
                        var idspReader = new IdspReader();
                        idspStructure = idspReader.ReadMetadata(stream);
                        stream.Position = 0;
                        audioWithConfig = idspReader.ReadWithConfig(stream);
                        break;
                    case ".hps":
                        var hpsReader = new HpsReader();
                        hpsStructure = hpsReader.ReadMetadata(stream);
                        stream.Position = 0;
                        audioWithConfig = hpsReader.ReadWithConfig(stream);
                        break;
                    case ".wav":
                        var wavReader = new WaveReader();
                        waveStructure = wavReader.ReadMetadata(stream);
                        stream.Position = 0;
                        audioWithConfig = wavReader.ReadWithConfig(stream);
                        break;
                    default:
                        throw new Exception("Unsupported Extension " + ext);
                }
                audioData = audioWithConfig.Audio;
            }
        }

        public void SaveAudio(Stream stream)
        {
            using (var writer = new FileWriter(stream, true))
            {
                writer.Write(SaveAudio());
            }
        }

        public byte[] SaveAudio()
        {
            foreach (string ext in Format.Extension)
            {
                string extension = ext.TrimStart('*');
                switch (extension)
                {
                    case ".bfstm":
                        return new BCFstmWriter(NwTarget.Cafe).GetFile(audioData, bxstmConfiguration);
                    case ".bcstm":
                        return new BCFstmWriter(NwTarget.Ctr).GetFile(audioData, bxstmConfiguration);
                    case ".brstm":
                        return new BCFstmWriter(NwTarget.Revolution).GetFile(audioData, bxstmConfiguration);
                    case ".idsp":
                        return new IdspWriter().GetFile(audioData, idspConfiguration);
                    case ".hps":
                        return new HpsWriter().GetFile(audioData, hpsConfiguration);
                    case ".wav":
                        return new WaveWriter().GetFile(audioData, hpsConfiguration);
                    default:
                        throw new Exception("Unsupported Extension " + ext);
                }
            }
            return new byte[0];
        }
    }
}
