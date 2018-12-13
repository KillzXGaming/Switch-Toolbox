using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.IO;
using Switch_Toolbox.Library;

namespace Switch_Toolbox
{
    //Based on
    // https://github.com/jam1garner/Smash-Forge/blob/26e0dcbd84cdf8a4ffe3fbe0b0317520a4099286/Smash%20Forge/Filetypes/Application/Config.cs
    class Config
    {
        public static void StartupFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Save();
                return;
            }

            ReadConfigFromFile(fileName);
        }

        private static void ReadConfigFromFile(string fileName)
        {
            int discordImageKey;
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            Queue<XmlNode> que = new Queue<XmlNode>();

            foreach (XmlNode node in doc.ChildNodes)
                que.Enqueue(node);

            while (que.Count > 0)
            {
                XmlNode node = que.Dequeue();

                foreach (XmlNode n in node.ChildNodes)
                    que.Enqueue(n);

                switch (node.Name)
                {
                    case "OpenStartupWindow":
                        bool.TryParse(node.InnerText, out Runtime.OpenStartupWindow);
                        break;
                    case "DisableUpdatePrompt":
                        bool.TryParse(node.InnerText, out Runtime.DisableUpdatePrompt);
                        break;
                    case "DisableViewport":
                        bool.TryParse(node.InnerText, out Runtime.DisableViewport);
                        break;
                    case "RenderModels":
                        bool.TryParse(node.InnerText, out Runtime.RenderModels);
                        break;
                    case "RenderModelSelection":
                        bool.TryParse(node.InnerText, out Runtime.RenderModelSelection);
                        break;
                    case "RenderModelWireframe":
                        bool.TryParse(node.InnerText, out Runtime.RenderModelWireframe);
                        break;
                    case "EnablePBR":
                        bool.TryParse(node.InnerText, out Runtime.EnablePBR);
                        break;
                    case "viewportShading":
                        if (node.ParentNode != null && node.ParentNode.Name.Equals("RENDERSETTINGS"))
                            Enum.TryParse(node.InnerText, out Runtime.viewportShading);
                        break;
                    case "thumbnailSize":
                        if (node.ParentNode != null && node.ParentNode.Name.Equals("OBJLISTSETTINGS"))
                            Enum.TryParse(node.InnerText, out Runtime.thumbnailSize);
                        break;
                    case "stereoscopy": bool.TryParse(node.InnerText, out Runtime.stereoscopy);
                        break;
                    case "CameraFar":
                        float.TryParse(node.InnerText, out Runtime.CameraFar);
                        break;
                    case "CameraNear":
                        float.TryParse(node.InnerText, out Runtime.CameraNear);
                        break;
                    case "PreviewScale":
                        float.TryParse(node.InnerText, out Runtime.previewScale);
                        break;
                    case "Yaz0CompressionLevel":
                        int.TryParse(node.InnerText, out Runtime.Yaz0CompressionLevel);
                        break;

                }
            }
        }
        public static void Save()
        {
            XmlDocument doc = CreateXmlFromSettings();
            doc.Save(MainForm.executableDir + "\\config.xml");
        }
        private static XmlDocument CreateXmlFromSettings()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("TOOLCONFIG");
            doc.AppendChild(mainNode);

            AppendMainFormSettings(doc, mainNode);
            AppendObjectlistSettings(doc, mainNode);
            AppendRenderSettings(doc, mainNode);
            AppendOCompressionFilelistSettings(doc, mainNode);

            return doc;
        }
        private static void AppendMainFormSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode mainSettingsNode = doc.CreateElement("MAINFORM");
            parentNode.AppendChild(mainSettingsNode);
            mainSettingsNode.AppendChild(createNode(doc, "OpenStartupWindow", Runtime.OpenStartupWindow.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "DisableViewport", Runtime.DisableViewport.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "DisableUpdatePrompt", Runtime.DisableUpdatePrompt.ToString()));
        }
        private static void AppendOCompressionFilelistSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode compSettingsNode = doc.CreateElement("COMPRESSIONSETTINGS");
            parentNode.AppendChild(compSettingsNode);
            compSettingsNode.AppendChild(createNode(doc, "Yaz0CompressionLevel", Runtime.Yaz0CompressionLevel.ToString()));
        }
        private static void AppendObjectlistSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode objlistSettingsNode = doc.CreateElement("OBJLISTSETTINGS");
            parentNode.AppendChild(objlistSettingsNode);
            objlistSettingsNode.AppendChild(createNode(doc, "thumbnailSize", Runtime.thumbnailSize.ToString()));
        }
        private static void AppendRenderSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode renderSettingsNode = doc.CreateElement("RENDERSETTINGS");
            parentNode.AppendChild(renderSettingsNode);
            renderSettingsNode.AppendChild(createNode(doc, "viewportShading", Runtime.viewportShading.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "stereoscopy", Runtime.stereoscopy.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "CameraFar", Runtime.CameraFar.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "CameraNear", Runtime.CameraNear.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "PreviewScale", Runtime.previewScale.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "EnablePBR", Runtime.EnablePBR.ToString()));
        }
        public static XmlNode createNode(XmlDocument doc, string el, string v)
        {
            XmlNode floorstyle = doc.CreateElement(el);
            floorstyle.InnerText = v;
            return floorstyle;
        }
    }
}
