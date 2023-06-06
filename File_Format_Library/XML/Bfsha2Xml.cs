using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Toolbox.Library;
using BfshaLibrary;

namespace FirstPlugin
{
    public class Bfsha2Xml : XmlDoc
    {
        public static void Save(BfshaFile bfshaFile, string FileName)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("SHARCFB");
            AddAttribute(doc, "Name", bfshaFile.Name, mainNode);
            doc.AppendChild(mainNode);
        }

        public static string WriteShaderModel(ShaderModel shaderModel)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("shader_model");
            AddAttribute(doc, "name", shaderModel.Name.Replace("\x00", ""), mainNode);
            doc.AppendChild(mainNode);

            WriteShaderOptions(doc, shaderModel.StaticOptions, "static_option_array", mainNode);
            WriteShaderOptions(doc, shaderModel.DynamiOptions, "dynamic_option_array", mainNode);
            WriteSamplers(doc, shaderModel.Samplers, shaderModel.Samplers, "sampler_array", mainNode);
            WriteAttributes(doc, shaderModel.Attributes, shaderModel.Attributes, "attribute_array", mainNode);
            WriteUniformBlocks(doc, shaderModel.UniformBlocks, shaderModel.UniformBlocks, "uniform_block_array", mainNode);

            return DocumentToString(doc);
        }

        private static void WriteUniformBlocks(XmlDocument doc, ResDict<UniformBlock> uniformBlocks, ResDict uniformBlockeDictionary, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var block in uniformBlocks.Values)
            {
                XmlNode childNode = doc.CreateElement("uniform_block");
                AddAttribute(doc, "name", uniformBlockeDictionary.GetKey(block.Index), childNode);
                AddAttribute(doc, "size", block.Size.ToString(), childNode);
                AddAttribute(doc, "type", block.Type.ToString(), childNode);
                AddAttribute(doc, "Index", block.Index.ToString(), childNode);

                rootNode.AppendChild(childNode);

                int ind = 0;
                foreach (var uniform in block.Uniforms.Values)
                {
                    XmlNode uniformsNode = doc.CreateElement("uniform");
                    if (ind < block.Uniforms.Count && ind >= 0)
                        AddAttribute(doc, "name", block.Uniforms.GetKey(ind), uniformsNode);
                    AddAttribute(doc, "index", uniform.Index.ToString(), uniformsNode);
                    AddAttribute(doc, "block_index", uniform.BlockIndex.ToString(), uniformsNode);
                    AddAttribute(doc, "offset", uniform.Offset.ToString(), uniformsNode);

                    if (ind < (block.Uniforms.Count - 1) && ind >= 0)
                    {
                        uint nextOffset = block.Uniforms[ind + 1].Offset;
                        uint currentOffset = block.Uniforms[ind].Offset;

                        uint Size = nextOffset - currentOffset;

                        AddAttribute(doc, "size", Size.ToString(), uniformsNode);
                    }
                    if (ind == (block.Uniforms.Count - 1))
                    {
                        AddAttribute(doc, "size", (block.Size -  uniform.Offset).ToString(), uniformsNode);
                    }

                    childNode.AppendChild(uniformsNode);

                    ind++;
                }
            }
            node.AppendChild(rootNode);
        }

        private static void WriteAttributes(XmlDocument doc, ResDict<BfshaLibrary.Attribute> attributes, ResDict attributeDictionary, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var attribute in attributes.Values)
            {
                XmlNode childNode = doc.CreateElement("attribute");
                AddAttribute(doc, "name", attributeDictionary.GetKey(attribute.Index), childNode);
                AddAttribute(doc, "location", attribute.Location.ToString(), childNode);
                AddAttribute(doc, "index", attribute.Index.ToString(), childNode);
                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }

        private static void WriteSamplers(XmlDocument doc, ResDict<Sampler> samplers, ResDict samplerDictionary, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var sampler in samplers.Values)
            {
                XmlNode childNode = doc.CreateElement("sampler");
                AddAttribute(doc, "name", samplerDictionary.GetKey(sampler.Index), childNode);
                AddAttribute(doc, "alt", sampler.AltAnnotation, childNode);
                AddAttribute(doc, "index", sampler.Index.ToString(), childNode);
                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }

        private static void WriteShaderOptions(XmlDocument doc, ResDict<ShaderOption> shaderOptions, string Name, XmlNode node)
        {
            XmlNode rootNode = doc.CreateElement(Name);
            foreach (var option in shaderOptions.Values)
            {
                XmlNode childNode = doc.CreateElement("option");
                AddAttribute(doc, "name", option.Name, childNode);
                AddAttribute(doc, "default", option.defaultChoice, childNode);

                string choices = "";
                foreach (var choice in option.ChoiceDict)
                    choices += $" {choice}";

                AddAttribute(doc, "choices", choices, childNode);

                rootNode.AppendChild(childNode);
            }
            node.AppendChild(rootNode);
        }
    }
}
