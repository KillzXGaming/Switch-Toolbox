using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Animations;

namespace LayoutBXLYT
{
    /// <summary>
    /// An animation controller from a BRLYT/BCLYT/BFLYT file.
    /// </summary>
    public class LytAnimation : STAnimation
    {
        public BxlytHeader parentLayout = null;
        public BxlanHeader BxlanAnimation = null;

        public List<string> Textures = new List<string>();

        public void UpdateLayout(BxlytHeader heeader)
        {
            parentLayout = heeader;

            ReloadAnimation();
        }

        private void ReloadAnimation()
        {
            FrameCount = BxlanAnimation.AnimationTag.EndFrame;
            StartFrame = BxlanAnimation.AnimationTag.StartFrame;

            if (StartFrame == 0 && FrameCount == 0)
            {
                foreach (var tag in BxlanAnimation.AnimationInfo.Entries)
                {
                    foreach (var tagEntry in tag.Tags)
                    {
                        foreach (var subEntry in tagEntry.Entries)
                        {
                            StartFrame = Math.Min(FrameCount, subEntry.KeyFrames.Min(x => x.Frame));
                            FrameCount = Math.Max(FrameCount, subEntry.KeyFrames.Max(x => x.Frame));
                        }
                    }
                }
            }

            Textures.Clear();
            AnimGroups.Clear();
            foreach (var tex in BxlanAnimation.AnimationInfo.Textures)
                Textures.Add(tex);

            foreach (var tag in BxlanAnimation.AnimationInfo.Entries)
                AnimGroups.Add(new LytAnimGroup(tag));
        }

        public override void Reset()
        {
            if (parentLayout == null) return;

            foreach (var pane in parentLayout.PaneLookup.Values)
                pane.animController.ResetAnim();

            foreach (var mat in parentLayout.Materials)
                mat.animController.ResetAnim();
        }

        public LytAnimation(BxlanHeader header, BxlytHeader layout)
        {
            parentLayout = layout;
            BxlanAnimation = header;

            Name = header.FileName;
            FrameCount = (uint)header.AnimationTag.EndFrame;
            StartFrame = (uint)header.AnimationTag.StartFrame;

            if (StartFrame == 0 && FrameCount == 0)
            {
                foreach (var tag in header.AnimationInfo.Entries)
                {
                    foreach (var tagEntry in tag.Tags)
                    {
                        foreach (var subEntry in tagEntry.Entries)
                        {
                            StartFrame = Math.Min(FrameCount, subEntry.KeyFrames.Min(x => x.Frame));
                            FrameCount = Math.Max(FrameCount, subEntry.KeyFrames.Max(x => x.Frame));
                        }
                    }
                }
            }
            Console.WriteLine($"FrameSize {BxlanAnimation.AnimationInfo.FrameSize}");
            Console.WriteLine($"FrameCount {FrameCount}");

            Textures.Clear();
            AnimGroups.Clear();
            foreach (var tex in header.AnimationInfo.Textures)
                Textures.Add(tex);

            foreach (var tag in header.AnimationInfo.Entries)
                AnimGroups.Add(new LytAnimGroup(tag));
        }

        public STAnimationTrack FindTarget(BxlanPaiTag group, int index)
        {
            var tracks = FindTargets(group);

            Console.WriteLine($"tracks {tracks.Count}");

            if (index < tracks.Count)
                return tracks[index];
            else
                return null;
        }

        public List<STAnimationTrack> FindTargets(BxlanPaiTag group)
        {
            foreach (LytAnimGroup grp in AnimGroups) {
                foreach (SubAnimGroup sub in grp.SubAnimGroups)
                {
                    if (sub.PaiTag == group)
                        return sub.GetTracks();
                }
            }

            return new List<STAnimationTrack>();
        }

        public override void NextFrame()
        {
            if (Frame == 0)
                Reset();

            if (Frame < StartFrame || Frame > FrameCount) return;

            if (parentLayout == null) return;

            //Loop through each group. 
            for (int i = 0; i < AnimGroups.Count; i++)
            {
                var group = AnimGroups[i] as LytAnimGroup;

                switch (group.Target)
                {
                    case AnimationTarget.Material:
                        var mat = parentLayout.SearchMaterial(group.Name);
                        if (mat != null) LoadMaterialAnimation(mat, group);
                        break;
                    case AnimationTarget.Pane:
                        if (parentLayout.PaneLookup.ContainsKey(group.Name))
                            LoadPaneAnimation(parentLayout.PaneLookup[group.Name], group);
                        break;
                }
            }
        }

        private void LoadMaterialAnimation(BxlytMaterial material, LytAnimGroup group)
        {
            //Sub groups store tag entries like texture patterns, srt, etc
            foreach (var subGroup in group.SubAnimGroups)
            {
                if (subGroup is LytTexturePatternGroup)
                    LoadTexturePatternGroup(material, (LytTexturePatternGroup)subGroup);
                else if (subGroup is LytTextureSRTGroup)
                    LoadTextureSRTGroup(material, (LytTextureSRTGroup)subGroup);
                else if (subGroup is LytAlphaTestGroup)
                    LoadAlphaTestGroup(material, (LytAlphaTestGroup)subGroup);
                else if (subGroup is LytIndirectSRTGroup)
                    LoadIndirectSRTGroup(material, (LytIndirectSRTGroup)subGroup);
                else if (subGroup is LytMaterialColorGroup)
                    LoadMaterialColorGroup(material, (LytMaterialColorGroup)subGroup);
            }
        }

        private void LoadPaneAnimation(BasePane pane, LytAnimGroup group)
        {
            //Sub groups store tag entries like vertex colors, srt, etc
            foreach (var subGroup in group.SubAnimGroups)
            {
                if (subGroup is LytVertexColorGroup)
                    LoadVertexColorGroup(pane, (LytVertexColorGroup)subGroup);
                else if (subGroup is LytPaneSRTGroup)
                    LoadPaneSRTGroup(pane, (LytPaneSRTGroup)subGroup);
                else if (subGroup is LytVisibiltyGroup)
                    LoadVisibiltyGroup(pane, (LytVisibiltyGroup)subGroup);
            }
        }

        private void LoadVertexColorGroup(BasePane pane, LytVertexColorGroup group)
        {
            for (int i = 0; i < 17; i++)
            {
                if (group.GetTrack(i).HasKeys)
                {
                    var track = group.GetTrack(i);
                    var value = track.GetFrameValue(Frame, StartFrame);
                    var target = (LVCTarget)i;

                    if (!pane.animController.PaneVertexColors.ContainsKey(target))
                        pane.animController.PaneVertexColors.Add(target, value);
                    else
                        pane.animController.PaneVertexColors[target] = value;
                }
            }
        }

        private void LoadPaneSRTGroup(BasePane pane, LytPaneSRTGroup group)
        {
            for (int i = 0; i < 10; i++)
            {
                if (group.GetTrack(i).HasKeys)
                {
                    var track = group.GetTrack(i);
                    var value = track.GetFrameValue(Frame, StartFrame);
                    var target = (LPATarget)i;

                    if (!pane.animController.PaneSRT.ContainsKey(target))
                        pane.animController.PaneSRT.Add(target, value);
                    else
                        pane.animController.PaneSRT[target] = value;
                }
            }
        }

        private void LoadVisibiltyGroup(BasePane pane, LytVisibiltyGroup group)
        {
            if (group.AnimTrack.HasKeys)
            {
                var track = group.AnimTrack;
                var value = track.GetFrameValue(Frame, StartFrame);
                pane.animController.Visibile = value == 1 ? true : false;
            }
        }

        private void LoadMaterialColorGroup(BxlytMaterial mat, LytMaterialColorGroup group)
        {
            for (int i = 0; i < (group.IsRLAN ? 30 : 8); i++)
            {
                if (group.GetTrack(i).HasKeys)
                {
                    var track = group.GetTrack(i);
                    var value = track.GetFrameValue(Frame, StartFrame);
                    var target = (LMCTarget)i;

                    if (!mat.animController.MaterialColors.ContainsKey(target))
                        mat.animController.MaterialColors.Add(target, value);
                    else
                        mat.animController.MaterialColors[target] = value;
                }
            }
        }

        private void LoadTexturePatternGroup(BxlytMaterial mat, LytTexturePatternGroup group)
        {
            for (int i = 0; i < 1; i++)
            {
                if (group.GetTrack(i).HasKeys)
                {
                    var track = group.GetTrack(i);
                    var value = track.GetFrameValue(Frame, StartFrame);
                    var target = (LTPTarget)i;

                    if (Textures.Count > value && value >= 0)
                    {
                        string texName = Textures[(int)value];

                        if (!mat.animController.TexturePatterns.ContainsKey(target))
                            mat.animController.TexturePatterns.Add(target, texName);
                        else
                            mat.animController.TexturePatterns[target] = texName;
                    }
                }
            }
        }

        private void LoadTextureSRTGroup(BxlytMaterial mat, LytTextureSRTGroup group)
        {
            for (int i = 0; i < 5; i++)
            {
                if (group.GetTrack(i).HasKeys)
                {
                    var track = group.GetTrack(i);
                    var value = track.GetFrameValue(Frame, StartFrame);
                    var target = (LTSTarget)i;

                    if (!mat.animController.TextureSRTS.ContainsKey(target))
                        mat.animController.TextureSRTS.Add(target, value);
                    else
                        mat.animController.TextureSRTS[target] = value;
                }
            }
        }

        private void LoadIndirectSRTGroup(BxlytMaterial mat, LytIndirectSRTGroup group)
        {

        }

        private void LoadAlphaTestGroup(BxlytMaterial mat, LytAlphaTestGroup group)
        {

        }
    }
}
