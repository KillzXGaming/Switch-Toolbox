using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using BfshaLibrary;
using System.Windows.Forms;
using ResNX = Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin
{
    public class BFSHA : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Shader;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Binary Shader Archive" };
        public string[] Extension { get; set; } = new string[] { "*.bfsha" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FSHA");
            }
        }


        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        BfshaFile bfshaFile;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            bool IsWiiU = CheckWiiU(new FileReader(stream, true));
            if (IsWiiU)
            {

            }
            else
            {
                bfshaFile = new BfshaFile(stream);

                foreach (var model in bfshaFile.ShaderModels)
                {
                    var wrapper = new ShaderModelWrapper();
                    wrapper.Read(model);
                    Nodes.Add(wrapper);
                }
            }
        }

        public int GetStaticKey(ResNX.ShaderAssign ShaderAssign)
        {
            if (bfshaFile.ShaderModelDict.ContainsKey(ShaderAssign.ShadingModelName))
            {
                int ModelIndex = bfshaFile.ShaderModelDict.IndexOf(ShaderAssign.ShadingModelName);

                for (int option = 0; option < ShaderAssign.ShaderOptions.Count; option++)
                {
                    int OptionIndex = bfshaFile.ShaderModels[ModelIndex].StaticOptionDict.IndexOf(ShaderAssign.ShaderOptions[option]);
                    var OptionStatic = bfshaFile.ShaderModels[ModelIndex].StaticOptions[OptionIndex];

                    int ChoiceIndex = OptionStatic.ChoiceDict.IndexOf(ShaderAssign.ShaderOptions[option]);

                    return bfshaFile.ShaderModels[ModelIndex].GetStaticKey(OptionIndex, ChoiceIndex);
                }
            }
            else
                throw new Exception("Model not found in bfsha!");

            return -1;
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public bool CheckWiiU(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "FSHA")
                throw new Exception($"Invalid signature {Signature}! Expected FSHA.");
            uint padding = reader.ReadUInt32();

            reader.Position = 0; //Reset position
            if (padding != 0x20202020)
                return true;

            return false;
        }

        public class ShaderModelWrapper : TreeNodeCustom
        {
            ShaderModel shaderModel;
            BNSH ShaderFile;

            public override void OnClick(TreeView treeview)
            {
                TextEditor editor = (TextEditor)LibraryGUI.GetActiveContent(typeof(TextEditor));
                if (editor == null)
                {
                    editor = new TextEditor();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.FillEditor(Bfsha2Xml.WriteShaderModel(shaderModel));
                editor.IsXML = true;
            }

            //The contetns are basically the same from Wii U which has already been REed! 
            public void Read(ShaderModel model)
            {
                shaderModel = model;

                Text = shaderModel.Name;

                ShaderFile = new BNSH();
                ShaderFile.FileName = "dummy.bnsh";
                ShaderFile.Load(new System.IO.MemoryStream(shaderModel.BinaryShaderData));
                Nodes.Add(ShaderFile);
            }
        }
    }
}
