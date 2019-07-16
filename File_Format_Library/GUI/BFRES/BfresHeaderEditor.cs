using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin.Forms
{
    public partial class BfresHeaderEditor : UserControl
    {
        public BfresHeaderEditor()
        {
            InitializeComponent();
        }

        public GameVersion gameVersion;

        public enum GameVersion
        {
            BOTW,
            PaperMarioColorSplash,
            YoshiWoollyWorld,
            Splatoon,
            SuperMarioMaker,
            CaptainToad,
            MK8,
            SM3DW,
            WWHD,
            Pikmin3,
            NintendoLand,
            NSMBU,
            NoPreset,
        }

        public int[] GetVersion(int[] ResFileVersion)
        {
            switch (gameVersion)
            {
                case GameVersion.BOTW:
                    return new int[4] { 4, 5, 0, 3 };
                case GameVersion.PaperMarioColorSplash:
                case GameVersion.YoshiWoollyWorld:
                case GameVersion.SuperMarioMaker:
                case GameVersion.CaptainToad:
                case GameVersion.Splatoon:
                    return new int[4] { 3, 5, 0, 3 };
                case GameVersion.MK8:
                    return new int[4] { 3, 4, 0, 4 };
                case GameVersion.SM3DW:
                    return new int[4] { 3, 4, 0, 2 };
                case GameVersion.WWHD:
                    return new int[4] { 3, 4, 0, 1 };
                case GameVersion.Pikmin3:
                    return new int[4] { 3, 2, 0, 1 };
                case GameVersion.NintendoLand:
                case GameVersion.NSMBU:
                    return new int[4] { 3, 0, 0, 1 };
                default:
                    return ResFileVersion;
            }
        }
    }
}
