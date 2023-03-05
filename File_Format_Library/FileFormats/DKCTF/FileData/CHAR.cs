using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace DKCTF
{
    /// <summary>
    /// Represents a file format for storing character/actor data including skeleton, model and animation information.
    /// </summary>
    public class CHAR : FileForm
    {
        public string Name { get; set; }

        public CObjectId SkeletonFileID;

        public List<CCharacterModelSet> Models = new List<CCharacterModelSet>();
        public List<CAnimationInfo> Animations = new List<CAnimationInfo>();

        public bool IsSwitch;

        public CHAR() { }

        public CHAR(System.IO.Stream stream) : base(stream)
        {

        }

        public override void Read(FileReader reader)
        {
            var assetHeader = reader.ReadStruct<CAssetHeader>();
            reader.ReadStruct<SInfo>();
            //Dumb hack atm
            bool IsSwitch = false;
            using (reader.TemporarySeek(reader.Position, System.IO.SeekOrigin.Begin))
            {
                uint len = reader.ReadUInt32();
                if (len < 30)
                    IsSwitch = true;
            }

            Name = IOFileExtension.ReadFixedString(reader, IsSwitch);

            SkeletonFileID = reader.ReadStruct<CObjectId>();

            uint numModels = reader.ReadUInt32();
            for (int i = 0; i < numModels; i++)
            {
                Models.Add(new CCharacterModelSet()
                {
                    Name = IOFileExtension.ReadFixedString(reader, IsSwitch),
                    FileID = reader.ReadStruct<CObjectId>(),
                    BoundingBox = reader.ReadStruct<CAABox>(),
                });
            }

            uint numAnimations = reader.ReadUInt32();
            for (int i = 0; i < numAnimations; i++)
            {
                Animations.Add(new CAnimationInfo()
                {
                    Name = IOFileExtension.ReadFixedString(reader, IsSwitch),
                    FileID = reader.ReadStruct<CObjectId>(),
                    field_1c = reader.ReadUInt32(),
                    field_20 = reader.ReadUInt32(),
                    field_24 = reader.ReadUInt16(),
                    field_26 = reader.ReadUInt16(),
                    field_28 = reader.ReadBoolean(),
                    BoundingBox = reader.ReadStruct<CAABox>(),
                });
            }

            Console.WriteLine();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SInfo
        {
            public byte field_0;
            public byte field_1;
            public byte field_2;
            public byte field_3;
            public byte field_4;
            public byte field_5;
            public byte field_6;
            public byte field_7;
            public byte field_8;
            public byte field_9;
            public byte field_A;
            public byte field_B;
            public byte field_C;
            public byte field_D;
            public byte field_E;
            public byte field_F;

            public ushort flags1;
            public byte flags2;
        }

        public class CCharacterModelSet
        {
            public string Name;
            public CObjectId FileID;
            public CAABox BoundingBox;

            public override string ToString()
            {
                return Name;
            }
        }

        public class CAnimationInfo
        {
            public string Name;
            public CObjectId FileID;

            public uint field_1c;
            public uint field_20;
            public ushort field_24;
            public ushort field_26;
            public bool field_28;

            public CAABox BoundingBox;

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
