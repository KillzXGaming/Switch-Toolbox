using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using FirstPlugin;
using LayoutBXLYT.Cafe;

namespace LayoutBXLYT
{
    /// <summary>
    /// A mapper which applies hard coded panes to the proper place
    /// </summary>
    public class LayoutCustomPaneMapper
    {
        private bool Loaded = false;
        //MK8 character select
        public void LoadMK8DCharaSelect(Dictionary<string, STGenericTexture> textures, BxlytHeader header)
        {
            var archive = header.FileInfo.IFileInfo.ArchiveParent;

            if (archive == null || !header.PaneLookup.ContainsKey("L_Chara_00"))
                return;

            if (!Loaded)
            {
                var parentArchive = ((IFileFormat)archive).IFileInfo.ArchiveParent;
                if (parentArchive == null) return;

                foreach (var files in parentArchive.Files)
                {
                    if (files.FileName.Contains("mn_L_CharaIcon_00"))
                    {
                        var charIconSzs = (IArchiveFile)files.OpenFile();
                        foreach (var file in charIconSzs.Files)
                        {
                            if (Utils.GetExtension(file.FileName) == ".bntx")
                            {
                               var bntx = (BNTX)file.OpenFile();
                                foreach (var tex in bntx.Textures)
                                    if (!textures.ContainsKey(tex.Key))
                                    {
                                        Console.WriteLine("Adding icon " + tex.Key);
                                        textures.Add(tex.Key, tex.Value);
                                    }

                                Loaded = true;
                            }
                        }
                    }
                }
            }

            //Map to all icons
            for (int i = 0; i < 42; i++)
            {
                Console.WriteLine($"L_Chara_{i.ToString("00")}");
                if (!header.PaneLookup.ContainsKey($"L_Chara_{i.ToString("00")}"))
                    continue;

                var partPane = (Cafe.PRT1)header.PaneLookup[$"L_Chara_{i.ToString("00")}"];
                var charPane = partPane.GetExternalPane();
                if (charPane == null) return;
                var iconPane = charPane.SearchPane("P_Chara_00");
                if (iconPane == null) return;

                var mat = ((IPicturePane)iconPane).Material;

                string textureName = "Mario";
                switch (i)
                {
                    case 0: textureName = "Mario"; break;
                    case 1: textureName = "Luigi"; break;
                    case 2: textureName = "Peach"; break;
                    case 3: textureName = "Daisy"; break;
                    case 4: textureName = "Rosetta"; break;
                    case 5: textureName = "TanukiMario"; break;
                    case 6: textureName = "CatPeach"; break;
                    case 7: textureName = "Yoshi00"; break;
                    case 8: textureName = "Kinopio"; break; //Toad
                    case 9: textureName = "Nokonoko"; break; //Koopa
                    case 10: textureName = "Heyho00"; break;
                    case 11: textureName = "Jugemu"; break;
                    case 12: textureName = "Kinopico"; break; //Toadette
                    case 13: textureName = "KingTeresa"; break;
                    case 14: textureName = "BbMario"; break;
                    case 15: textureName = "BbLuigi"; break;
                    case 16: textureName = "BbPeach"; break;
                    case 17: textureName = "BbDaisy"; break;
                    case 18: textureName = "BbRosetta"; break;
                    case 19: textureName = "MetalMario"; break;
                    case 20: textureName = "PGoldPeach"; break;
                    case 21: textureName = "Wario"; break;
                    case 22: textureName = "Waluigi"; break;
                    case 23: textureName = "DK"; break;
                    case 24: textureName = "Koopa"; break;  //Bowser
                    case 25: textureName = "Karon"; break;
                    case 26: textureName = "KoopaJr"; break;
                    case 27: textureName = "HoneKoopa"; break;
                    case 28: textureName = "Lemmy"; break;
                    case 29: textureName = "Larry"; break;
                    case 30: textureName = "Wendy"; break;
                    case 31: textureName = "Ludwig"; break;
                    case 32: textureName = "Iggy"; break;
                    case 33: textureName = "Roy"; break;
                    case 34: textureName = "Morton"; break;
                    case 35: textureName = "SplatoonGirl00"; break;
                    case 36: textureName = "SplatoonBoy00"; break;
                    case 37: textureName = "Link"; break;
                    case 38: textureName = "AnimalBoyA"; break;
                    case 39: textureName = "AnimalGirlA"; break;
                    case 40: textureName = "Shizue"; break;
                    case 41: textureName = "MiiAmiibo"; break;
                }

                textureName = $"tc_Chara_{textureName}^l";

                if (!mat.animController.TexturePatterns.ContainsKey(LTPTarget.Image1))
                    mat.animController.TexturePatterns.Add(LTPTarget.Image1, textureName);
                else
                    mat.animController.TexturePatterns[LTPTarget.Image1] = textureName;
            }
        }
    }
}
