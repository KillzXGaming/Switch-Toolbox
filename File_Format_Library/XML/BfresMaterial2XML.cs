using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Syroot.NintenTools.NSW.Bfres;
using System.IO;
using Bfres.Structs;

namespace FirstPlugin
{
    public class FMAT2XML
    {
        public static void Read(FMAT mat, string FileName, bool OnlyMatParams = false)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            foreach (XmlNode node in doc.ChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name == "MaterialParameters")
                    {
                        ReadShaderParams(doc, n, mat);
                    }
                }
            }
        }


        public static void ReadTextureRefs(XmlDocument doc, XmlNode parentNode, FMAT mat)
        {

        }

        private static void WriteTextureRefs(FMAT mat, XmlDocument doc, XmlNode parentNode)
        {
            XmlNode matParamsNode = doc.CreateElement("TextureRefs");
            parentNode.AppendChild(matParamsNode);
        }


        #region    Read ShaderParams 


        public static void ReadShaderParams(XmlDocument doc, XmlNode parentNode, FMAT mat)
        {
            mat.matparam.Clear();
            int Index = 0;
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                string Value = node.Attributes[0].Value;
                BfresShaderParam param = new BfresShaderParam();
                param.Name = node.Name;
                param.DependedIndex = (ushort)Index;
                param.DependIndex = (ushort)Index;

                ushort DependedIndex = 0;
                ushort DependIndex = 0;

                Index++;
                foreach (XmlAttribute att in node.Attributes)
                {
                    Console.WriteLine(att);

                    if (att.Name == "DependedIndex")
                    {
                        string DependedIndexStr = att.Value;
                        if (ushort.TryParse(DependedIndexStr, out DependedIndex))
                        {
                            param.DependedIndex = DependedIndex;
                        }
                    }
                    if (att.Name == "DependIndex")
                    {
                        string DependIndexStr = att.Value;
                        if (ushort.TryParse(DependIndexStr, out DependIndex))
                        {
                            param.DependIndex = DependIndex;
                        }
                    }
                    if (att.Name == "Format")
                    {
                        string Format = att.Value;

                        ShaderParamType type;
                        if (Enum.TryParse(Format, out type))
                        {
                            param.Type = type;
                            switch (type)
                            {
                                case ShaderParamType.Bool:
                                case ShaderParamType.Bool2:
                                case ShaderParamType.Bool3:
                                case ShaderParamType.Bool4:
                                    param.ValueBool = Array.ConvertAll(Value.Split(','), bool.Parse);
                                    break;
                                case ShaderParamType.Float:
                                case ShaderParamType.Float2:
                                case ShaderParamType.Float3:
                                case ShaderParamType.Float4:
                                case ShaderParamType.Float2x2:
                                case ShaderParamType.Float2x3:
                                case ShaderParamType.Float2x4:
                                case ShaderParamType.Float3x2:
                                case ShaderParamType.Float3x3:
                                case ShaderParamType.Float3x4:
                                case ShaderParamType.Float4x2:
                                case ShaderParamType.Float4x3:
                                case ShaderParamType.Float4x4:
                                    param.ValueFloat = Array.ConvertAll(Value.Split(','), float.Parse);
                                    break;
                                case ShaderParamType.Int:
                                case ShaderParamType.Int2:
                                case ShaderParamType.Int3:
                                case ShaderParamType.Int4:
                                    param.ValueInt = Array.ConvertAll(Value.Split(','), int.Parse);
                                    break;
                                case ShaderParamType.Reserved2:
                                case ShaderParamType.Reserved3:
                                case ShaderParamType.Reserved4:
                                    param.ValueReserved = Array.ConvertAll(Value.Split(','), byte.Parse);
                                    break;
                                case ShaderParamType.Srt2D:
                                    param.ValueSrt2D = SetSrt2D(node);
                                    break;
                                case ShaderParamType.Srt3D:
                                    param.ValueSrt3D = SetSrt3D(node);
                                    break;
                                case ShaderParamType.TexSrt:
                                    param.ValueTexSrt = SetTexSRT(node);
                                    break;
                                case ShaderParamType.TexSrtEx:
                                    param.ValueTexSrtEx = SetTexSRTEx(node);
                                    break;
                                case ShaderParamType.UInt:
                                case ShaderParamType.UInt2:
                                case ShaderParamType.UInt3:
                                case ShaderParamType.UInt4:
                                    param.ValueUint = Array.ConvertAll(Value.Split(','), uint.Parse);
                                    break;
                            }
                        }
                    }
                }

                mat.matparam.Add(param.Name, param);
            }
        }
        static float X, Y, Z;
        public static Srt2D SetSrt2D(XmlNode node)
        {
            Srt2D srt2D = new Srt2D();
            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == "Scaling")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    srt2D.Scaling = new Syroot.Maths.Vector2F(X, Y);
                }
                if (att.Name == "Rotation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    srt2D.Rotation = X;
                }
                if (att.Name == "Translation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    srt2D.Translation = new Syroot.Maths.Vector2F(X, Y);
                }
            }


            return srt2D;
        }
        public static Srt3D SetSrt3D(XmlNode node)
        {
            Srt3D srt3D = new Srt3D();

            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == "Scaling")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    float.TryParse(values[2], out Z);
                    srt3D.Scaling = new Syroot.Maths.Vector3F(X, Y, Z);
                }
                if (att.Name == "Rotation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    float.TryParse(values[2], out Z);
                    srt3D.Rotation = new Syroot.Maths.Vector3F(X, Y, Z);
                }
                if (att.Name == "Translation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    float.TryParse(values[2], out Z);
                    srt3D.Translation = new Syroot.Maths.Vector3F(X, Y, Z);
                }
            }

            return srt3D;
        }
        public static TexSrt SetTexSRT(XmlNode node)
        {
            TexSrt texSrt = new TexSrt();

            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == "Mode")
                {
                    TexSrtMode Mode;
                    Enum.TryParse(att.Value, out Mode);
                    texSrt.Mode = Mode;
                }
                if (att.Name == "Scaling")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    texSrt.Scaling = new Syroot.Maths.Vector2F(X, Y);
                }
                if (att.Name == "Rotation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    texSrt.Rotation = X;
                }
                if (att.Name == "Translation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    texSrt.Translation = new Syroot.Maths.Vector2F(X, Y);
                }
            }

            return texSrt;
        }
        public static string[] GetSrtValues(string str)
        {
            string[] charsToRemove = new string[] { "X", "Y", "Z", "W", "{", "}", "=" };
            foreach (var c in charsToRemove)
            {
                str = str.Replace(c, string.Empty);
            }
            return str.Split(',');
        }
        public static TexSrtEx SetTexSRTEx(XmlNode node)
        {
            TexSrtEx texSrtEx = new TexSrtEx();
            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == "Mode")
                {
                    TexSrtMode Mode;
                    Enum.TryParse(att.Value, out Mode);
                    texSrtEx.Mode = Mode;
                }
                if (att.Name == "Scaling")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    texSrtEx.Scaling = new Syroot.Maths.Vector2F(X, Y);
                }
                if (att.Name == "Rotation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    texSrtEx.Rotation = X;
                }
                if (att.Name == "Translation")
                {
                    string[] values = GetSrtValues(att.Value);
                    float.TryParse(values[0], out X);
                    float.TryParse(values[1], out Y);
                    texSrtEx.Translation = new Syroot.Maths.Vector2F(X, Y);
                }
                if (att.Name == "MatrixPointer")
                {
                    uint ptr;
                    uint.TryParse(att.Value, out ptr);
                    texSrtEx.MatrixPointer = ptr;
                }
            }
            return texSrtEx;
        }

        #endregion

        public static void Save(FMAT mat, string FileName, bool OnlyMaterialParams = false)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("FMAT");
            AddAttribute(doc, "Name", mat.Text, mainNode);
            doc.AppendChild(mainNode);
         
            if (OnlyMaterialParams)
            {
                AppendMaterialParams(mat, doc, mainNode);
            }
            else
            {
                AppendMaterialParams(mat, doc, mainNode);
            }
            doc.Save(FileName);
        }
        private static void AppendMaterialParams(FMAT mat, XmlDocument doc, XmlNode parentNode)
        {
            XmlNode matParamsNode = doc.CreateElement("MaterialParameters");
            parentNode.AppendChild(matParamsNode);

            bool IsSrt = false;
            foreach (var param in mat.matparam)
            {
                string Value = "";
                switch (param.Value.Type)
                {
                    case ShaderParamType.Bool:
                    case ShaderParamType.Bool2:
                    case ShaderParamType.Bool3:
                    case ShaderParamType.Bool4:
                        Value = string.Join(",", param.Value.ValueBool);
                        break;
                    case ShaderParamType.Float:
                    case ShaderParamType.Float2:
                    case ShaderParamType.Float3:
                    case ShaderParamType.Float4:
                    case ShaderParamType.Float2x2:
                    case ShaderParamType.Float2x3:
                    case ShaderParamType.Float2x4:
                    case ShaderParamType.Float3x2:
                    case ShaderParamType.Float3x3:
                    case ShaderParamType.Float3x4:
                    case ShaderParamType.Float4x2:
                    case ShaderParamType.Float4x3:
                    case ShaderParamType.Float4x4:
                        Value = string.Join(",", param.Value.ValueFloat);
                        break;
                    case ShaderParamType.Int:
                    case ShaderParamType.Int2:
                    case ShaderParamType.Int3:
                    case ShaderParamType.Int4:
                        Value = string.Join(",", param.Value.ValueInt);
                        break;
                    case ShaderParamType.Reserved2:
                    case ShaderParamType.Reserved3:
                    case ShaderParamType.Reserved4:
                        Value = string.Join(",", param.Value.ValueReserved);
                        break;
                    case ShaderParamType.Srt2D:
                        WriteSrt2DParamNode(doc, param.Value, matParamsNode);
                        IsSrt = true;
                        break;
                    case ShaderParamType.Srt3D:
                        WriteSrt3DParamNode(doc, param.Value, matParamsNode);
                        IsSrt = true;
                        break;
                    case ShaderParamType.TexSrt:
                        WriteTexSrtParamNode(doc, param.Value, matParamsNode);
                        IsSrt = true;
                        break;
                    case ShaderParamType.TexSrtEx:
                        WriteTexSrtExParamNode(doc, param.Value, matParamsNode);
                        IsSrt = true;
                        break;
                    case ShaderParamType.UInt:
                    case ShaderParamType.UInt2:
                    case ShaderParamType.UInt3:
                    case ShaderParamType.UInt4:
                        Value = string.Join(",", param.Value.ValueUint);
                        break;
                }

                if (!IsSrt)
                {
                    XmlNode ParamNode = doc.CreateElement(param.Key);
                    AddAttribute(doc, "Value", Value, ParamNode);
                    AddAttribute(doc, "Format", param.Value.Type.ToString(), ParamNode);
                    AddAttribute(doc, "DependedIndex",  param.Value.DependedIndex.ToString(), ParamNode);
                    AddAttribute(doc, "DependIndex", param.Value.DependIndex.ToString(), ParamNode);
                    matParamsNode.AppendChild(ParamNode);
                }
            }
        }
        private static void WriteSrt2DParamNode(XmlDocument doc,BfresShaderParam param, XmlNode node)
        {
            XmlNode ParamNode = doc.CreateElement(param.Name);
            AddAttribute(doc, "Scaling", param.ValueSrt2D.Scaling.ToString(), ParamNode);
            AddAttribute(doc, "Rotation", param.ValueSrt2D.Rotation.ToString(), ParamNode);
            AddAttribute(doc, "Translation", param.ValueSrt2D.Translation.ToString(), ParamNode);
            AddAttribute(doc, "Format", ShaderParamType.Srt2D.ToString(), ParamNode);
            AddAttribute(doc, "DependedIndex", param.DependedIndex.ToString(), ParamNode);
            AddAttribute(doc, "DependIndex", param.DependIndex.ToString(), ParamNode);
            node.AppendChild(ParamNode);
        }
        private static void WriteSrt3DParamNode(XmlDocument doc, BfresShaderParam param, XmlNode node)
        {
            XmlNode ParamNode = doc.CreateElement(param.Name);
            AddAttribute(doc, "Scaling", param.ValueSrt3D.Scaling.ToString(), ParamNode);
            AddAttribute(doc, "Rotation", param.ValueSrt3D.Rotation.ToString(), ParamNode);
            AddAttribute(doc, "Translation", param.ValueSrt3D.Translation.ToString(), ParamNode);
            AddAttribute(doc, "Format", ShaderParamType.Srt3D.ToString(), ParamNode);
            AddAttribute(doc, "DependedIndex", param.DependedIndex.ToString(), ParamNode);
            AddAttribute(doc, "DependIndex", param.DependIndex.ToString(), ParamNode);
            node.AppendChild(ParamNode);
        }
        private static void WriteTexSrtParamNode(XmlDocument doc, BfresShaderParam param, XmlNode node)
        {
            XmlNode ParamNode = doc.CreateElement(param.Name);
            AddAttribute(doc, "Mode", param.ValueTexSrt.Mode.ToString(), ParamNode);
            AddAttribute(doc, "Scaling", param.ValueTexSrt.Scaling.ToString(), ParamNode);
            AddAttribute(doc, "Rotation", param.ValueTexSrt.Rotation.ToString(), ParamNode);
            AddAttribute(doc, "Translation", param.ValueTexSrt.Translation.ToString(), ParamNode);
            AddAttribute(doc, "Format", ShaderParamType.TexSrt.ToString(), ParamNode);
            AddAttribute(doc, "DependedIndex", param.DependedIndex.ToString(), ParamNode);
            AddAttribute(doc, "DependIndex", param.DependIndex.ToString(), ParamNode);
            node.AppendChild(ParamNode);
        }
        private static void WriteTexSrtExParamNode(XmlDocument doc, BfresShaderParam param, XmlNode node)
        {
            XmlNode ParamNode = doc.CreateElement(param.Name);
            AddAttribute(doc, "Mode", param.ValueTexSrtEx.Mode.ToString(), ParamNode);
            AddAttribute(doc, "Scaling", param.ValueTexSrtEx.Scaling.ToString(), ParamNode);
            AddAttribute(doc, "Rotation", param.ValueTexSrtEx.Rotation.ToString(), ParamNode);
            AddAttribute(doc, "Translation", param.ValueTexSrtEx.Translation.ToString(), ParamNode);
            AddAttribute(doc, "MatrixPointer", param.ValueTexSrtEx.MatrixPointer.ToString(), ParamNode);
            AddAttribute(doc, "Format", ShaderParamType.TexSrtEx.ToString(), ParamNode);
            AddAttribute(doc, "DependedIndex", param.DependedIndex.ToString(), ParamNode);
            AddAttribute(doc, "DependIndex", param.DependIndex.ToString(), ParamNode);
            node.AppendChild(ParamNode);
        }


        private static void AddAttribute(XmlDocument doc, string name, string value, XmlNode node)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            node.Attributes.Append(att);
        }
    }
}
