using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Toolbox.Library;

namespace FirstPlugin
{
    public class Sharc2XML : XmlDoc
    {
        public static void Read(SHARCFB.Header header, string FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            foreach (XmlNode node in doc.ChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name == "ShaderPrograms")
                    {
                    }
                }
            }
        }

        public static void Save(SHARCFB.Header header, string FileName)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("SHARCFB");
            AddAttribute(doc, "Name", header.Name, mainNode);
            doc.AppendChild(mainNode);
        }

        public static string WriteProgram(SHARCFBNX.ShaderVariation program)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("ShaderVariation");
            AddAttribute(doc, "Name", program.Text.Replace("\x00", ""), mainNode);
            doc.AppendChild(mainNode);

            WriteVariationSymbols(doc, program.Uniforms, "Uniform_Variables", mainNode);
            WriteVariationSymbols(doc, program.UniformBlocks, "Uniform_Blocks", mainNode);
            WriteVariationSymbols(doc, program.Samplers, "Sampler_Variables", mainNode);
            WriteVariationSymbols(doc, program.Attributes, "Attribute_Variables", mainNode);

            return DocumentToString(doc);
        }

        public static string WriteProgram(SHARC.ShaderProgram program)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("ShaderProgram");
            AddAttribute(doc, "Name", program.Text.Replace("\x00", ""), mainNode);
            doc.AppendChild(mainNode);

            WriteMarcos(doc, program.variationVertexMacroData, "vertex_macro_array", mainNode);
            WriteMarcos(doc, program.variationFragmenMacroData, "fragment_macro_array", mainNode);
            WriteMarcos(doc, program.variationGeometryMacroData, "geometry_macro_array", mainNode);

            WriteVariationSymbols(doc, program.variationSymbolData, "option_array", mainNode);
            WriteShaderSymbolData(doc, program.UniformVariables, "Uniform_Variables", mainNode);
            WriteShaderSymbolData(doc, program.UniformBlocks, "Uniform_Blocks", mainNode);
            WriteShaderSymbolData(doc, program.SamplerVariables, "Sampler_Variables", mainNode);
            WriteShaderSymbolData(doc, program.AttributeVariables, "Attribute_Variables", mainNode);


            return DocumentToString(doc);
        }

        public static string WriteProgram(SHARCFBNX.ShaderProgram program)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("ShaderProgram");
            AddAttribute(doc, "Name", program.Text.Replace("\x00", ""), mainNode);
            doc.AppendChild(mainNode);

            WriteMarcos(doc, program.variationMacroData, "macro_array", mainNode);
            WriteVariationSymbols(doc, program.variationSymbolData, "option_array", mainNode);
            WriteShaderSymbolData(doc, program.UniformVariables, "Uniform_Variables", mainNode);

            XmlNode vertexNode = doc.CreateElement("VertexVariation");
            XmlNode pixelNode = doc.CreateElement("PixelVariation");
            mainNode.AppendChild(vertexNode);
            mainNode.AppendChild(pixelNode);

            var var = program.GetDefaultVertexVariation();

            WriteVariationSymbols(doc, var.Uniforms, "Uniform_Variables", vertexNode);
            WriteVariationSymbols(doc, var.UniformBlocks, "Uniform_Blocks", vertexNode);
            WriteVariationSymbols(doc, var.Samplers, "Sampler_Variables", vertexNode);
            WriteVariationSymbols(doc, var.Attributes, "Attribute_Variables", vertexNode);

            var = program.GetDefaultPixelVariation();

            WriteVariationSymbols(doc, var.Uniforms, "Uniform_Variables", pixelNode);
            WriteVariationSymbols(doc, var.UniformBlocks, "Uniform_Blocks", pixelNode);
            WriteVariationSymbols(doc, var.Samplers, "Sampler_Variables", pixelNode);
            WriteVariationSymbols(doc, var.Attributes, "Attribute_Variables", pixelNode);

            return DocumentToString(doc);
        }

        public static string WriteProgram(SHARCFB.ShaderProgram program)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("ShaderProgram");
            AddAttribute(doc, "Name", program.Text.Replace("\x00", ""), mainNode);
            doc.AppendChild(mainNode);

            WriteVariationSymbols(doc, program.variationMacroData, "macro_array", mainNode);
            WriteVariationSymbols(doc, program.variationSymbolData, "option_array", mainNode);
            WriteShaderSymbolData(doc, program.UniformVariables, "Uniform_Variables", mainNode);
            WriteShaderSymbolData(doc, program.UniformBlocks, "Uniform_Blocks", mainNode);
            WriteShaderSymbolData(doc, program.SamplerVariables, "Sampler_Variables", mainNode);
            WriteShaderSymbolData(doc, program.AttributeVariables, "Attribute_Variables", mainNode);


            return DocumentToString(doc);
        }

        private static void WriteMarcos(XmlDocument doc, VariationMacroData var, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var macro in var.macros)
            {
                XmlNode childNode = doc.CreateElement("macro");
                AddAttribute(doc, "name", macro.Name.Replace("\x00", ""), childNode);
                AddAttribute(doc, "value", macro.Value.Replace("\x00", ""), childNode);
                //  AddAttribute(doc, "Values", macro.Values, ParamNode);
                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }

        private static void WriteVariationSymbols(XmlDocument doc, VariationSymbolData var, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var symbol in var.symbols)
            {
                XmlNode childNode = doc.CreateElement("option");
                AddAttribute(doc, "id", symbol.Name.Replace("\x00", ""), childNode);
                AddAttribute(doc, "symbol", symbol.SymbolName.Replace("\x00", ""), childNode);
                AddAttribute(doc, "values", string.Join(",", symbol.Values).Replace("\x00", ""), childNode);
                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }

        private static void WriteVariationSymbols(XmlDocument doc, List<SHARCFBNX.SymbolUniformBlock> symbols, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var symbol in symbols)
            {
                XmlNode childNode = doc.CreateElement("option");
                AddAttribute(doc, "name", symbol.Name.Replace("\x00", ""), childNode);
                AddAttribute(doc, "location", symbol.Location.ToString(), childNode);
                AddAttribute(doc, "size", symbol.Size.ToString(), childNode);

                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }

        private static void WriteVariationSymbols(XmlDocument doc, List<SHARCFBNX.SymbolUniform> symbols, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var symbol in symbols)
            {
                XmlNode childNode = doc.CreateElement("option");
                AddAttribute(doc, "name", symbol.Name.Replace("\x00", ""), childNode);
                AddAttribute(doc, "offset", symbol.Offset.ToString(), childNode);
                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }

        private static void WriteVariationSymbols(XmlDocument doc, List<SHARCFBNX.Symbol> symbols, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var symbol in symbols)
            {
                XmlNode childNode = doc.CreateElement("option");
                AddAttribute(doc, "name", symbol.Name.Replace("\x00", ""), childNode);
                AddAttribute(doc, "location", symbol.Location.ToString(), childNode);
                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }

        private static void WriteShaderSymbolData(XmlDocument doc, ShaderSymbolData symbolData, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);

            foreach (var symbol in symbolData.symbols)
            {
                XmlNode childNode = doc.CreateElement("VarSymbol");
                AddAttribute(doc, "Name", symbol.Name.Replace("\x00", ""), childNode);

                if (symbol.sharcNXValues.Count > 0)
                {
                    int i = 0;
                    foreach (var value in symbol.sharcNXValues)
                        AddAttribute(doc, $"Symbol_Name{i++}", value.Name.Replace("\x00", ""), childNode);
                }
                else
                {
                    AddAttribute(doc, "Symbol_Name", symbol.SymbolName.Replace("\x00", ""), childNode);
                }

                //   AddAttribute(doc, "Default_Value", symbol.DefaultValueString, ParamNode);
                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }
    }
}
