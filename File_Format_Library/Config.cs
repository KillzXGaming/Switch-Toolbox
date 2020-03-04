using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.IO;
using Toolbox.Library;

namespace FirstPlugin
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

            if (File.Exists(fileName))
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
                    case "ExternalFMATPath":
                        if (node.ParentNode != null && node.ParentNode.Name.Equals("PATHSETTINGS"))
                            if (File.Exists(node.InnerText))
                                PluginRuntime.ExternalFMATPath = node.InnerText;
                        break;
                    case "MarioOdysseyGamePath":
                        if (node.ParentNode != null && node.ParentNode.Name.Equals("PATHSETTINGS"))
                            if (File.Exists(node.InnerText))
                                PluginRuntime.OdysseyGamePath = node.InnerText;
                        break;
                    case "SwapRenderInfos":
                        bool.TryParse(node.InnerText, out PluginRuntime.MaterialReplace.SwapRenderInfos);
                        break;
                    case "SwapShaderOptions":
                        bool.TryParse(node.InnerText, out PluginRuntime.MaterialReplace.SwapShaderOptions);
                        break;
                    case "SwapShaderParams":
                        bool.TryParse(node.InnerText, out PluginRuntime.MaterialReplace.SwapShaderParams);
                        break;
                    case "SwapTextures":
                        bool.TryParse(node.InnerText, out PluginRuntime.MaterialReplace.SwapTextures);
                        break;
                    case "SwapUserData":
                        bool.TryParse(node.InnerText, out PluginRuntime.MaterialReplace.SwapUserData);
                        break;
                }
            }
        }
        public static void Save()
        {
            XmlDocument doc = CreateXmlFromSettings();
            doc.Save(Runtime.ExecutableDir + "/Lib/Plugins/config.xml");
        }
        private static XmlDocument CreateXmlFromSettings()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("FORGECONFIG");
            doc.AppendChild(mainNode);

            AppendDOCKSettings(doc, mainNode);
            AppendPathSettings(doc, mainNode);
            AppendMaterialReplaceSettings(doc, mainNode);

            return doc;
        }
        private static void AppendPathSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode pathSettingsNode = doc.CreateElement("PATHSETTINGS");
            parentNode.AppendChild(pathSettingsNode);
            pathSettingsNode.AppendChild(createNode(doc, "ExternalFMATPath", PluginRuntime.ExternalFMATPath));
            pathSettingsNode.AppendChild(createNode(doc, "MarioOdysseyGamePath", PluginRuntime.OdysseyGamePath));
        }
        private static void AppendMaterialReplaceSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode matReplaceSettingsNode = doc.CreateElement("MAT_REPLACE");
            parentNode.AppendChild(matReplaceSettingsNode);
            matReplaceSettingsNode.AppendChild(createNode(doc, "SwapRenderInfos", PluginRuntime.MaterialReplace.SwapRenderInfos.ToString()));
            matReplaceSettingsNode.AppendChild(createNode(doc, "SwapShaderOptions", PluginRuntime.MaterialReplace.SwapShaderOptions.ToString()));
            matReplaceSettingsNode.AppendChild(createNode(doc, "SwapShaderParams", PluginRuntime.MaterialReplace.SwapShaderParams.ToString()));
            matReplaceSettingsNode.AppendChild(createNode(doc, "SwapTextures", PluginRuntime.MaterialReplace.SwapTextures.ToString()));
            matReplaceSettingsNode.AppendChild(createNode(doc, "SwapUserData", PluginRuntime.MaterialReplace.SwapUserData.ToString()));
        }
        private static void AppendDOCKSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode renderSettingsNode = doc.CreateElement("DOCKSETTINGS");
            parentNode.AppendChild(renderSettingsNode);
        }
        public static XmlNode createNode(XmlDocument doc, string el, string v)
        {
            XmlNode floorstyle = doc.CreateElement(el);
            floorstyle.InnerText = v;
            return floorstyle;
        }
    }
}
