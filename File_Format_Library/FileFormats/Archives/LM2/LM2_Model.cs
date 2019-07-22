using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using OpenTK;

namespace FirstPlugin.LuigisMansion.DarkMoon
{
    public class LM2_Model : TreeNodeCustom
    {
        public LM2_ModelInfo ModelInfo;
        public List<LM2_Mesh> Meshes = new List<LM2_Mesh>();
        public List<uint> VertexBufferPointers = new List<uint>();
        public Matrix4 Transform { get; set; }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(this, OnPropertyChanged);
        }

        public void OnPropertyChanged() { }
    }

    public class LM2_ModelInfo
    {
        public byte[] Data;
    }

    public class RenderableMeshWrapper : STGenericObject
    {
        public LM2_Mesh Mesh { get; set; }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(Mesh, OnPropertyChanged);
        }

        public void OnPropertyChanged() { }
    }

    public class LM2_IndexList
    {
        public short[] UnknownIndices { get; set; }

        public uint Unknown { get; set; }

        public short[] UnknownIndices2 { get; set; }

        public uint[] Unknown2 { get; set; }

        public void Read(FileReader reader)
        {
            UnknownIndices = reader.ReadInt16s(4);
            Unknown = reader.ReadUInt32();
            UnknownIndices2 = reader.ReadInt16s(8);
            Unknown2 = reader.ReadUInt32s(6); //Increases by 32 each entry
        }
    }

    public class LM2_Mesh
    {
        public uint IndexStartOffset { get; private set; } //relative to buffer start
        public ushort IndexCount { get; private set; } //divide by 3 to get face count
        public ushort IndexFormat { get; private set; } //0x0 - ushort, 0x8000 - byte

        public ushort BufferStride { get; private set; }
        public ushort Unknown { get; private set; }
        public ushort Unknown2 { get; private set; }
        public ulong DataFormat { get; private set; }
        public uint Unknown4 { get; private set; }
        public uint Unknown5 { get; private set; }
        public uint Unknown6 { get; private set; }
        public ushort VertexCount { get; private set; }
        public ushort Unknown7 { get; private set; }
        public uint HashID { get; private set; }

        public void Read(FileReader reader)
        {
            IndexStartOffset = reader.ReadUInt32();
            IndexCount = reader.ReadUInt16();
            IndexFormat = reader.ReadUInt16();
            BufferStride = reader.ReadUInt16();
            Unknown = reader.ReadUInt16();
            Unknown2 = reader.ReadUInt16();
            DataFormat = reader.ReadUInt64();
            Unknown4 = reader.ReadUInt32();
            Unknown5 = reader.ReadUInt32();
            Unknown6 = reader.ReadUInt32();
            VertexCount = reader.ReadUInt16();
            Unknown7 = reader.ReadUInt16(); //0x100
            HashID = reader.ReadUInt32(); //0x100
        }

        public class FormatInfo
        {

        }
    }
}
