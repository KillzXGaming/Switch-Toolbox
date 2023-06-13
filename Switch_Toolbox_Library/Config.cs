using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.IO;

namespace Toolbox.Library
{
    //Based on
    // https://github.com/jam1garner/Smash-Forge/blob/26e0dcbd84cdf8a4ffe3fbe0b0317520a4099286/Smash%20Forge/Filetypes/Application/Config.cs
    public class Config
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

        public static void GamePathsFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                SavePaths();
                return;
            }

            ReadConfigFromFile(fileName);
        }

        private static void ReadConfigFromFile(string fileName) {
            ReadConfigXMLFromFile(fileName);
        }

        private static XmlDocument ReadConfigXMLFromFile(string fileName)
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
                    case "UseDebugDomainExceptionHandler":
                        bool.TryParse(node.InnerText, out Runtime.UseDebugDomainExceptionHandler);
                        break;
                    case "OpenStartupWindow":
                        bool.TryParse(node.InnerText, out Runtime.OpenStartupWindow);
                        break;
                    case "EnableVersionCheck":
                        bool.TryParse(node.InnerText, out Runtime.EnableVersionCheck);
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
						Runtime.CameraNear = (float)Math.Min(Math.Max(0.1, Runtime.CameraNear), 1.0f);
                        break;
                    case "PreviewScale":
                        float.TryParse(node.InnerText, out Runtime.previewScale);
                        break;
                    case "Yaz0CompressionLevel":
                        int.TryParse(node.InnerText, out Runtime.Yaz0CompressionLevel);
                        break;
                    case "FormTheme":
                        Toolbox.Library.Forms.FormThemes.Preset preset;
                        Enum.TryParse(node.InnerText, out preset);
                        Toolbox.Library.Forms.FormThemes.ActivePreset = preset;
                        break;
                    case "MaximizeMdiWindow": bool.TryParse(node.InnerText, out Runtime.MaximizeMdiWindow);
                        break;
                    case "PreferredTexFormat":
                        TEX_FORMAT texFormat;
                        Enum.TryParse(node.InnerText, out texFormat);
                        Runtime.PreferredTexFormat = texFormat;
                        break;
                    case "backgroundGradientTop":
                        TryParseHexColor(node, ref Runtime.backgroundGradientTop);
                        break;
                    case "backgroundGradientBottom":
                        TryParseHexColor(node, ref Runtime.backgroundGradientBottom);
                        break;
                    case "renderVertColor":
                        bool.TryParse(node.InnerText, out Runtime.renderVertColor);
                        break;
                    case "renderReflection":
                        bool.TryParse(node.InnerText, out Runtime.renderReflection);
                        break;
                    case "renderSpecular":
                        bool.TryParse(node.InnerText, out Runtime.renderSpecular);
                        break;
                    case "useNormalMap":
                        bool.TryParse(node.InnerText, out Runtime.useNormalMap);
                        break;
                    case "CellSize":
                        float.TryParse(node.InnerText, out Runtime.gridSettings.CellSize);
                        break;
                    case "CellAmount":
                        uint.TryParse(node.InnerText, out Runtime.gridSettings.CellAmount);
                        break;
                    case "GridColor":
                        TryParseHexColor(node, ref Runtime.gridSettings.color);
                        break;
                    case "ListPanelWidth":
                        int.TryParse(node.InnerText, out Runtime.ObjectEditor.ListPanelWidth);
                        break;
                    case "EditorDiplayIndex":
                        int.TryParse(node.InnerText, out Runtime.ObjectEditor.EditorDiplayIndex);
                        break;
                    case "ViewportWidth":
                        int.TryParse(node.InnerText, out Runtime.ViewportEditor.Width);
                        break;
                    case "renderBones":
                        bool.TryParse(node.InnerText, out Runtime.renderBones);
                        break;
                    case "normalsLineLength":
                        float.TryParse(node.InnerText, out Runtime.normalsLineLength);
                        break;
                    case "bonePointSize":
                        float.TryParse(node.InnerText, out Runtime.bonePointSize);
                        break;
                    case "MaxCameraSpeed":
                        float.TryParse(node.InnerText, out Runtime.MaxCameraSpeed);
                        break;
                    case "AddFilesToActiveObjectEditor":
                        bool.TryParse(node.InnerText, out Runtime.AddFilesToActiveObjectEditor);
                        break;
                    case "SmoGamePath":
                        Runtime.SmoGamePath = node.InnerText;
                        break;
                    case "Mk8GamePath":
                        Runtime.Mk8GamePath = node.InnerText;
                        break;
                    case "Mk8dGamePath":
                        Runtime.Mk8dGamePath = node.InnerText;
                        break;
                    case "TpGamePath":
                        Runtime.TpGamePath = node.InnerText;
                        break;
                    case "PkSwShGamePath":
                        Runtime.PkSwShGamePath = node.InnerText;
                        break;
                    case "BotwGamePath":
                        Runtime.BotwGamePath = node.InnerText;
                        break;
                    case "TotkGamePath":
                        Runtime.TotkGamePath = node.InnerText;
                        break;
                    case "SpecularCubeMapPath":
                        Runtime.PBR.SpecularCubeMapPath = node.InnerText;
                        break;
                    case "DiffuseCubeMapPath":
                        Runtime.PBR.DiffuseCubeMapPath = node.InnerText;
                        break;
                    case "renderBoundingBoxes":
                        bool.TryParse(node.InnerText, out Runtime.renderBoundingBoxes);
                        break;
                    case "UseOpenGL":
                        bool.TryParse(node.InnerText, out Runtime.UseOpenGL);
                        break;
                    case "DisplayViewport":
                        bool.TryParse(node.InnerText, out Runtime.DisplayViewport);
                        break;
                    case "DisplayVertical":
                        bool.TryParse(node.InnerText, out Runtime.ImageEditor.DisplayVertical);
                        break;
                    case "ShowPropertiesPanel":
                        bool.TryParse(node.InnerText, out Runtime.ImageEditor.ShowPropertiesPanel);
                        break;
                    case "DisplayAlpha":
                        bool.TryParse(node.InnerText, out Runtime.ImageEditor.DisplayAlpha);
                        break;
                    case "UseComponetSelector":
                        bool.TryParse(node.InnerText, out Runtime.ImageEditor.UseComponetSelector);
                        break;
                    case "EnableImageZoom":
                        bool.TryParse(node.InnerText, out Runtime.ImageEditor.EnableImageZoom);
                        break;
                    case "EnablePixelGrid":
                        bool.TryParse(node.InnerText, out Runtime.ImageEditor.EnablePixelGrid);
                        break;
                    case "OpenModelsOnOpen":
                        bool.TryParse(node.InnerText, out Runtime.ObjectEditor.OpenModelsOnOpen);
                        break;
                    case "UseSkybox":
                        bool.TryParse(node.InnerText, out Runtime.PBR.UseSkybox);
                        break;
                    case "UseDiffuseSkyTexture":
                        bool.TryParse(node.InnerText, out Runtime.PBR.UseDiffuseSkyTexture);
                        break;
                    case "ViewportCameraMode":
                        Runtime.CameraMode cameraMode;
                        Enum.TryParse(node.InnerText, out cameraMode);
                        Runtime.ViewportCameraMode = cameraMode;
                        break;
                    case "BotwTable":
                        bool.TryParse(node.InnerText, out Runtime.ResourceTables.BotwTable);
                        break;
                    case "TpTable":
                        bool.TryParse(node.InnerText, out Runtime.ResourceTables.TpTable);
                        break;
                    case "DEVELOPER_DEBUG_MODE":
                        bool.TryParse(node.InnerText, out Runtime.DEVELOPER_DEBUG_MODE);
                        break;
                    case "FrameCamera":
                        bool.TryParse(node.InnerText, out Runtime.FrameCamera);
                        break;
                    case "UseDirectXTexDecoder":
                      //  bool.TryParse(node.InnerText, out Runtime.UseDirectXTexDecoder);
                        break;
                    case "AlwaysCompressOnSave":
                        bool.TryParse(node.InnerText, out Runtime.AlwaysCompressOnSave);
                        break;
                    case "CustomPicureBoxBGColor":
                        TryParseHexColor(node, ref Runtime.CustomPicureBoxBGColor);
                        break;
                    case "UseSingleInstance":
                        bool.TryParse(node.InnerText, out Runtime.UseSingleInstance);
                        break;
                    case "LayoutBackgroundColor":
                        TryParseHexColor(node, ref Runtime.LayoutEditor.BackgroundColor);
                        break;
                    case "LayoutShadingMode":
                        Runtime.LayoutEditor.DebugShading shadingMode;
                        Enum.TryParse(node.InnerText, out shadingMode);
                        Runtime.LayoutEditor.Shading = shadingMode;
                        break;
                    case "IsGamePreview":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.IsGamePreview);
                        break;
                    case "DisplayBoundryPane":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.DisplayBoundryPane);
                        break;
                    case "DisplayNullPane":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.DisplayNullPane);
                        break;
                    case "DisplayPicturePane":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.DisplayPicturePane);
                        break;
                    case "DisplayWindowPane":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.DisplayWindowPane);
                        break;
                    case "DisplayTextPane":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.DisplayTextPane);
                        break;
                    case "LayoutDisplayGrid":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.DisplayGrid);
                        break;
                    case "TitleKeys":
                        Runtime.SwitchKeys.TitleKeys = node.InnerText;
                        break;
                    case "ProdKeys":
                        Runtime.SwitchKeys.ProdKeys = node.InnerText;
                        break;
                    case "PreviewGammaFix":
                        bool.TryParse(node.InnerText, out Runtime.ImageEditor.PreviewGammaFix);
                        break;
                    case "PartsAsNullPanes":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.PartsAsNullPanes);
                        break;
                    case "TransformPaneChidlren":
                        bool.TryParse(node.InnerText, out Runtime.LayoutEditor.TransformChidlren);
                        break;
                    case "DumpShadersDEBUG":
                        bool.TryParse(node.InnerText, out Runtime.DumpShadersDEBUG);
                        break;
                    case "BymlTextFormat":
                        Runtime.ByamlTextFormat textFormat;
                        Enum.TryParse(node.InnerText, out textFormat);
                        Runtime.ByamlEditor.TextFormat = textFormat;
                        break;
                    case "cameraMovement":
                        Runtime.CameraMovement cameraMovement;
                        Enum.TryParse(node.InnerText, out cameraMovement);
                        Runtime.cameraMovement = cameraMovement;
                        break;
                    case "KCLGamePreset":
                        Runtime.CollisionSettings.KCLGamePreset = node.InnerText;
                        break;
                    case "KCLPlatform":
                        Runtime.CollisionSettings.KCLPlatform = node.InnerText;
                        break;
                    case "KCLUsePresetEditor":
                        bool.TryParse(node.InnerText, out Runtime.CollisionSettings.KCLUsePresetEditor);
                        break;
                    case "ShowCloseDialog":
                        bool.TryParse(node.InnerText, out Runtime.ShowCloseDialog);
                        break;
                    case "displayGrid":
                        bool.TryParse(node.InnerText, out Runtime.displayGrid);
                        break;
                    case "displayAxisLines":
                        bool.TryParse(node.InnerText, out Runtime.displayAxisLines);
                        break;
                }
            }

            return doc;
        }

        private static bool TryParsePoint(XmlNode node, ref Point point)
        {
            try
            {
                string[] values = node.InnerText.Split(',');


                int x = -1;
                int y = -1;

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Replace('=', ' ');
                    values[i] = values[i].Replace('}', ' ');
                    values[i] = values[i].Replace('{', ' ');

                    if (values[i].Contains("X"))
                    {
                        values[i] = values[i].Replace('X', ' ');
                        int.TryParse(values[i], out x);
                    }
                    if (values[i].Contains("Y"))
                    {
                        values[i] = values[i].Replace('Y', ' ');
                        int.TryParse(values[i], out y);
                    }
                }


                if (x == -1 || y == -1)
                    return false;

                point.X = x;
                point.Y = y;

                return true;
            }
            catch (Exception)
            {
                // Invalid hex format.
                return false;
            }
        }

        private static bool TryParseHexColor(XmlNode node, ref Color color)
        {
            try
            {
                color = ColorTranslator.FromHtml(node.InnerText);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Save()
        {
            XmlDocument doc = CreateXmlFromSettings();
            doc.Save(Runtime.ExecutableDir + "\\config.xml");

            SavePaths();
        }

        private static void SavePaths()
        {
            XmlDocument doc = CreatePathXmlFromSettings();
            doc.Save(Runtime.ExecutableDir + "\\config_paths.xml");
        }

        private static XmlDocument CreatePathXmlFromSettings()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("TOOLCONFIG");
            doc.AppendChild(mainNode);

            AppendPathSettings(doc, mainNode);
            AppendSwitchKeyPathSettings(doc, mainNode);

            return doc;
        }

        private static XmlDocument CreateXmlFromSettings()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode mainNode = doc.CreateElement("TOOLCONFIG");
            doc.AppendChild(mainNode);

            AppendMainFormSettings(doc, mainNode);
            AppendImageEditorSettings(doc, mainNode);
            AppendObjectlistSettings(doc, mainNode);
            AppendRenderSettings(doc, mainNode);
            AppendOCompressionFilelistSettings(doc, mainNode);
            AppendEditorSettings(doc, mainNode);
            AppendBackgroundSettings(doc, mainNode);
            AppendGridSettings(doc, mainNode);
            AppenPBRSettings(doc, mainNode);
            AppendResourceTableSettings(doc, mainNode);
            AppendDeveloperSettings(doc, mainNode);
            AppendLayoutEditorSettings(doc, mainNode);
            AppendByamlEditorSettings(doc, mainNode);
            AppendCollisionSettings(doc, mainNode);

            return doc;
        }


        private static void AppendCollisionSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode layoutSettingsNode = doc.CreateElement("KCL_EDITOR");
            parentNode.AppendChild(layoutSettingsNode);

            layoutSettingsNode.AppendChild(createNode(doc, "KCLGamePreset", Runtime.CollisionSettings.KCLGamePreset.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "KCLPlatform", Runtime.CollisionSettings.KCLPlatform.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "KCLUsePresetEditor", Runtime.CollisionSettings.KCLUsePresetEditor.ToString()));
        }

        private static void AppendLayoutEditorSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode layoutSettingsNode = doc.CreateElement("LAYOUT_EDITOR");
            parentNode.AppendChild(layoutSettingsNode);

            layoutSettingsNode.AppendChild(createNode(doc, "LayoutShadingMode", Runtime.LayoutEditor.Shading.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "LayoutBackgroundColor", ColorTranslator.ToHtml(Runtime.LayoutEditor.BackgroundColor)));
            layoutSettingsNode.AppendChild(createNode(doc, "IsGamePreview", Runtime.LayoutEditor.IsGamePreview.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "LayoutDisplayGrid", Runtime.LayoutEditor.DisplayGrid.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "DisplayNullPane", Runtime.LayoutEditor.DisplayNullPane.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "DisplayBoundryPane", Runtime.LayoutEditor.DisplayBoundryPane.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "DisplayPicturePane", Runtime.LayoutEditor.DisplayPicturePane.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "DisplayWindowPane", Runtime.LayoutEditor.DisplayWindowPane.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "DisplayTextPane", Runtime.LayoutEditor.DisplayTextPane.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "PartsAsNullPanes", Runtime.LayoutEditor.PartsAsNullPanes.ToString()));
            layoutSettingsNode.AppendChild(createNode(doc, "TransformPaneChidlren", Runtime.LayoutEditor.TransformChidlren.ToString()));
        }

        private static void AppendDeveloperSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode mainSettingsNode = doc.CreateElement("DEVELOPER_SETTINGS_DONT_TOUCH");
            parentNode.AppendChild(mainSettingsNode);
            mainSettingsNode.AppendChild(createNode(doc, "DEVELOPER_DEBUG_MODE", Runtime.DEVELOPER_DEBUG_MODE.ToString()));
        }

        private static void AppendMainFormSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode mainSettingsNode = doc.CreateElement("MAINFORM");
            parentNode.AppendChild(mainSettingsNode);

            mainSettingsNode.AppendChild(createNode(doc, "UseSingleInstance", Runtime.UseSingleInstance.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "UseDirectXTexDecoder", Runtime.UseDirectXTexDecoder.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "AlwaysCompressOnSave", Runtime.AlwaysCompressOnSave.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "DisplayViewport", Runtime.DisplayViewport.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "UseOpenGL", Runtime.UseOpenGL.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "DumpShadersDEBUG", Runtime.DumpShadersDEBUG.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "UseDebugDomainExceptionHandler", Runtime.UseDebugDomainExceptionHandler.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "OpenStartupWindow", Runtime.OpenStartupWindow.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "EnableVersionCheck", Runtime.EnableVersionCheck.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "FormTheme", Toolbox.Library.Forms.FormThemes.ActivePreset.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "MaximizeMdiWindow", Runtime.MaximizeMdiWindow.ToString()));
            mainSettingsNode.AppendChild(createNode(doc, "ShowCloseDialog", Runtime.ShowCloseDialog.ToString()));
        }
        private static void AppendImageEditorSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode PathsNode = doc.CreateElement("ImageEditor");
            parentNode.AppendChild(PathsNode);
            PathsNode.AppendChild(createNode(doc, "DisplayVertical", Runtime.ImageEditor.DisplayVertical.ToString()));
            PathsNode.AppendChild(createNode(doc, "ShowPropertiesPanel", Runtime.ImageEditor.ShowPropertiesPanel.ToString()));
            PathsNode.AppendChild(createNode(doc, "DisplayAlpha", Runtime.ImageEditor.DisplayAlpha.ToString()));
            PathsNode.AppendChild(createNode(doc, "UseComponetSelector", Runtime.ImageEditor.UseComponetSelector.ToString()));
            PathsNode.AppendChild(createNode(doc, "EnablePixelGrid", Runtime.ImageEditor.EnablePixelGrid.ToString()));
            PathsNode.AppendChild(createNode(doc, "EnableImageZoom", Runtime.ImageEditor.EnableImageZoom.ToString()));
            parentNode.AppendChild(createNode(doc, "CustomPicureBoxBGColor", ColorTranslator.ToHtml(Runtime.CustomPicureBoxBGColor)));
            PathsNode.AppendChild(createNode(doc, "PreviewGammaFix", Runtime.ImageEditor.PreviewGammaFix.ToString()));
        }

        private static void AppendPathSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode PathsNode = doc.CreateElement("PATHS");
            parentNode.AppendChild(PathsNode);
            PathsNode.AppendChild(createNode(doc, "SmoGamePath", Runtime.SmoGamePath.ToString()));
            PathsNode.AppendChild(createNode(doc, "Mk8GamePath", Runtime.Mk8GamePath.ToString()));
            PathsNode.AppendChild(createNode(doc, "Mk8dGamePath", Runtime.Mk8dGamePath.ToString()));
            PathsNode.AppendChild(createNode(doc, "TpGamePath", Runtime.TpGamePath.ToString()));
            PathsNode.AppendChild(createNode(doc, "BotwGamePath", Runtime.BotwGamePath.ToString()));
            PathsNode.AppendChild(createNode(doc, "TotkGamePath", Runtime.TotkGamePath.ToString()));
            PathsNode.AppendChild(createNode(doc, "SpecularCubeMapPath", Runtime.PBR.SpecularCubeMapPath.ToString()));
            PathsNode.AppendChild(createNode(doc, "DiffuseCubeMapPath", Runtime.PBR.DiffuseCubeMapPath.ToString()));
            PathsNode.AppendChild(createNode(doc, "PkSwShGamePath", Runtime.PkSwShGamePath.ToString()));
        }

        private static void AppendSwitchKeyPathSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode PathsNode = doc.CreateElement("SWITCH_KEY_PATHS");
            parentNode.AppendChild(PathsNode);
            PathsNode.AppendChild(createNode(doc, "TitleKeys", Runtime.SwitchKeys.TitleKeys.ToString()));
            PathsNode.AppendChild(createNode(doc, "ProdKeys", Runtime.SwitchKeys.ProdKeys.ToString()));
        }

        private static void AppendByamlEditorSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode PathsNode = doc.CreateElement("ByamlEditor");
            parentNode.AppendChild(PathsNode);
            PathsNode.AppendChild(createNode(doc, "BymlTextFormat", Runtime.ByamlEditor.TextFormat.ToString()));
        }
        
        private static void AppenPBRSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode SettingsNode = doc.CreateElement("PBR");
            parentNode.AppendChild(SettingsNode);
            SettingsNode.AppendChild(createNode(doc, "UseSkybox", Runtime.PBR.UseSkybox.ToString()));
            SettingsNode.AppendChild(createNode(doc, "EnablePBR", Runtime.EnablePBR.ToString()));
            SettingsNode.AppendChild(createNode(doc, "UseDiffuseSkyTexture", Runtime.PBR.UseDiffuseSkyTexture.ToString()));     
        }
        private static void AppendEditorSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode SettingsNode = doc.CreateElement("EDITORSETTINGS");
            parentNode.AppendChild(SettingsNode);
            SettingsNode.AppendChild(createNode(doc, "PreferredTexFormat", Runtime.PreferredTexFormat.ToString()));
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
            objlistSettingsNode.AppendChild(createNode(doc, "EditorDiplayIndex", Runtime.ObjectEditor.EditorDiplayIndex.ToString()));
            objlistSettingsNode.AppendChild(createNode(doc, "thumbnailSize", Runtime.thumbnailSize.ToString()));
            objlistSettingsNode.AppendChild(createNode(doc, "ListPanelWidth", Runtime.ObjectEditor.ListPanelWidth.ToString()));
            objlistSettingsNode.AppendChild(createNode(doc, "AddFilesToActiveObjectEditor", Runtime.AddFilesToActiveObjectEditor.ToString()));
            objlistSettingsNode.AppendChild(createNode(doc, "OpenModelsOnOpen", Runtime.ObjectEditor.OpenModelsOnOpen.ToString()));
        }
        private static void AppendRenderSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode renderSettingsNode = doc.CreateElement("RENDERSETTINGS");
            parentNode.AppendChild(renderSettingsNode);
            renderSettingsNode.AppendChild(createNode(doc, "ViewportCameraMode", Runtime.ViewportCameraMode.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "viewportShading", Runtime.viewportShading.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "stereoscopy", Runtime.stereoscopy.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "CameraFar", Runtime.CameraFar.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "CameraNear", Runtime.CameraNear.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "PreviewScale", Runtime.previewScale.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "renderVertColor", Runtime.renderVertColor.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "renderReflection", Runtime.renderReflection.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "renderSpecular", Runtime.renderSpecular.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "renderBones", Runtime.renderBones.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "renderBoundingBoxes", Runtime.renderBoundingBoxes.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "useNormalMap", Runtime.useNormalMap.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "ViewportWidth", Runtime.ViewportEditor.Width.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "normalsLineLength", Runtime.normalsLineLength.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "bonePointSize", Runtime.bonePointSize.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "MaxCameraSpeed", Runtime.MaxCameraSpeed.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "FrameCamera", Runtime.FrameCamera.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "cameraMovement", Runtime.cameraMovement.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "displayAxisLines", Runtime.displayAxisLines.ToString()));
            renderSettingsNode.AppendChild(createNode(doc, "displayGrid", Runtime.displayGrid.ToString()));

        }

        private static void AppendResourceTableSettings(XmlDocument doc, XmlNode parentNode)
        {
            XmlNode resourceTableNode = doc.CreateElement("ResourceTables");
            parentNode.AppendChild(resourceTableNode);
            resourceTableNode.AppendChild(createNode(doc, "BotwTable", Runtime.ResourceTables.BotwTable.ToString()));
            resourceTableNode.AppendChild(createNode(doc, "TpTable", Runtime.ResourceTables.TpTable.ToString()));
        }

        private static void AppendGridSettings(XmlDocument doc, XmlNode parentNode)
        {
            parentNode.AppendChild(createNode(doc, "GridColor", ColorTranslator.ToHtml(Runtime.gridSettings.color)));
            parentNode.AppendChild(createNode(doc, "CellAmount", Runtime.gridSettings.CellAmount.ToString()));
            parentNode.AppendChild(createNode(doc, "CellSize", Runtime.gridSettings.CellSize.ToString()));
        }

        private static void AppendBackgroundSettings(XmlDocument doc, XmlNode parentNode)
        {
            parentNode.AppendChild(createNode(doc, "renderBackGround", Runtime.renderBackGround.ToString()));
            parentNode.AppendChild(createNode(doc, "backgroundGradientTop", ColorTranslator.ToHtml(Runtime.backgroundGradientTop)));
            parentNode.AppendChild(createNode(doc, "backgroundGradientBottom", ColorTranslator.ToHtml(Runtime.backgroundGradientBottom)));
        }

        public static XmlNode createNode(XmlDocument doc, string el, string v)
        {
            XmlNode floorstyle = doc.CreateElement(el);
            floorstyle.InnerText = v;
            return floorstyle;
        }
    }
}
