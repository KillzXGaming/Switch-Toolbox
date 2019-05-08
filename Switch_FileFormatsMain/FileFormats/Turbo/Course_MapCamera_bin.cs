using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using FirstPlugin.Forms;
using OpenTK;

namespace FirstPlugin.Turbo
{
    public class Course_MapCamera_bin : IEditor<MK8MapCameraEditor>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Default;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Course Map Camera" };
        public string[] Extension { get; set; } = new string[] { "*.bin" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(Stream stream)
        {
            return FileName == "course_mapcamera.bin";
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public MK8MapCameraEditor OpenForm()
        {
            MK8MapCameraEditor form = new MK8MapCameraEditor();
            form.Text = FileName;
            form.LoadFile(this);
            return form;
        }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            Read(new FileReader(stream));
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            Write(new FileWriter(mem));

            return mem.ToArray();
        }

        public CameraData cameraData;

        public class CameraData
        {
            public float PositionX { get; set; }
            public float PositionY { get; set; }
            public float PositionZ { get; set; }
            public float TargetX { get; set; }
            public float TargetY { get; set; }
            public float TargetZ { get; set; }

            public float Unk { get; set; }
            public float Unk2 { get; set; }
            public float Unk3 { get; set; }
            public float BoundingWidth { get; set; }
            public float BoundingHeight { get; set; }
            public byte Unk6 { get; set; }
        }


        public bool BigEndian = true;

        public void Read(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            reader.Position = 0x1C;
            float unk = reader.ReadSingle();

            //Check if this value is valid with big endianness. It should be 1
            BigEndian = unk == 1;
            if (BigEndian)
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            else
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

            reader.Position = 0;

            cameraData = new CameraData();
            cameraData.PositionX = reader.ReadSingle();
            cameraData.PositionY = reader.ReadSingle();
            cameraData.PositionZ = reader.ReadSingle();
            cameraData.TargetX = reader.ReadSingle();
            cameraData.TargetY = reader.ReadSingle();
            cameraData.TargetZ = reader.ReadSingle();
            cameraData.Unk = reader.ReadSingle();
            cameraData.Unk2 = reader.ReadSingle();
            cameraData.Unk3 = reader.ReadSingle();
            cameraData.BoundingWidth = reader.ReadSingle();
            cameraData.BoundingHeight = reader.ReadSingle();
            cameraData.Unk6 = reader.ReadByte();

            reader.Close();
            reader.Dispose();
        }
        public void Write(FileWriter writer)
        {
            if (BigEndian)
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            else
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

            writer.Write(cameraData.PositionX);
            writer.Write(cameraData.PositionY);
            writer.Write(cameraData.PositionZ);
            writer.Write(cameraData.TargetX);
            writer.Write(cameraData.TargetY);
            writer.Write(cameraData.TargetZ);
            writer.Write(cameraData.Unk);
            writer.Write(cameraData.Unk2);
            writer.Write(cameraData.Unk3);
            writer.Write(cameraData.BoundingWidth);
            writer.Write(cameraData.BoundingHeight);
            writer.Write(cameraData.Unk6);

            writer.Flush();
            writer.Close();
            writer.Dispose();
        }
    }
}
