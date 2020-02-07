using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;

namespace HyruleWarriors.G1M
{
    public class G1MS : G1MChunkCommon
    {
        public STSkeleton GenericSkeleton = new STSkeleton();

        public short[] BoneIndices { get; set; }

        public G1MS ChildSkeleton { get; set; }

        public ushort[] BoneIndexList { get; set; }

        public G1MS(FileReader reader, bool isParent = true)
        {
            uint pos = (uint)reader.Position - 12;
            uint boneOffset = reader.ReadUInt32() + pos;
            ushort conditionNumber = reader.ReadUInt16();

            if (isParent && conditionNumber == 0)
            {

            }

            ushort unknown = reader.ReadUInt16();
            ushort numBones = reader.ReadUInt16();
            ushort numBoneIndices = reader.ReadUInt16();
            BoneIndices = reader.ReadInt16s((int)numBoneIndices);

            BoneIndexList = new ushort[numBones];
            for (ushort i = 0; i < BoneIndices?.Length; i++)
                if (BoneIndices[i] != -1)
                    BoneIndexList[BoneIndices[i]] = i;

            reader.SeekBegin(boneOffset);
            for (int i = 0; i < numBones; i++) {
                var scale = reader.ReadVec3();
                int parentIndex = reader.ReadInt32();
                var quat = reader.ReadQuaternion(true);
                var position = reader.ReadVec3();
                reader.ReadSingle();

             //   if ((parentIndex & 0x80000000) > 0 && parentIndex != -1)
             //       parentIndex = parentIndex & 0x7FFFFFFF;

                STBone genericBone = new STBone(GenericSkeleton);
                genericBone.Text = $"Bone {BoneIndexList[i]}";
                genericBone.Rotation = quat;
                genericBone.Scale = scale;
                genericBone.Position = position;
                genericBone.parentIndex = parentIndex;
                GenericSkeleton.bones.Add(genericBone);
            }

            GenericSkeleton.reset();
            GenericSkeleton.update();
        }
    }
}
