using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using FirstPlugin.Forms;

namespace FirstPlugin.Turbo
{
    public class Course_MapCamera_bin : IEditor<MK8MapCameraEditor>, IFileFormat
    {
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
            public float MapPosX { get; set; }
            public float MapPosY { get; set; }
            public float MapPosZ { get; set; }
            public float MapPos2X { get; set; }
            public float MapPos2Y { get; set; }
            public float MapPos2Z { get; set; }

            public float Unk { get; set; }
            public float Unk2 { get; set; }
            public float Unk3 { get; set; }
            public float Unk4 { get; set; }
            public float Unk5 { get; set; }
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
            cameraData.MapPosX = reader.ReadSingle();
            cameraData.MapPosY = reader.ReadSingle();
            cameraData.MapPosZ = reader.ReadSingle();
            cameraData.MapPos2X = reader.ReadSingle();
            cameraData.MapPos2Y = reader.ReadSingle();
            cameraData.MapPos2Z = reader.ReadSingle();
            cameraData.Unk = reader.ReadSingle();
            cameraData.Unk2 = reader.ReadSingle();
            cameraData.Unk3 = reader.ReadSingle();
            cameraData.Unk4 = reader.ReadSingle();
            cameraData.Unk5 = reader.ReadSingle();
            cameraData.Unk6 = reader.ReadByte();
        }
        public void Write(FileWriter writer)
        {
            if (BigEndian)
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            else
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

            writer.Write(cameraData.MapPosX);
            writer.Write(cameraData.MapPosY);
            writer.Write(cameraData.MapPosZ);
            writer.Write(cameraData.MapPos2X);
            writer.Write(cameraData.MapPos2Y);
            writer.Write(cameraData.MapPos2Z);
            writer.Write(cameraData.Unk);
            writer.Write(cameraData.Unk2);
            writer.Write(cameraData.Unk3);
            writer.Write(cameraData.Unk4);
            writer.Write(cameraData.Unk5);
            writer.Write(cameraData.Unk6);
        }
    }
}
