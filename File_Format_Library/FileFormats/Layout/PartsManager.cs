using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace LayoutBXLYT
{
    /// <summary>
    /// A class that manages parts for layout files
    /// </summary>
    public class PartsManager : IDisposable
    {
        public Dictionary<string, BxlytHeader> PartLayouts = new Dictionary<string, BxlytHeader>();
        public Dictionary<string, BxlanHeader> PartAnimations = new Dictionary<string, BxlanHeader>();
        public Dictionary<string, IArchiveFile> PartArchives = new Dictionary<string, IArchiveFile>();

        public void AddLayout(BxlytHeader header)
        {
            if (!PartLayouts.ContainsKey(header.FileName))
                PartLayouts.Add(header.FileName, header);
        }

        public IFileFormat TryGetLayout(string fileName)
        {
            if (PartLayouts.ContainsKey(fileName))
                return PartLayouts[fileName].FileInfo;
            return null;
        }

        public void AddAnimation(BxlanHeader header)
        {
            if (!PartAnimations.ContainsKey(header.FileName))
                PartAnimations.Add(header.FileName, header);
        }

        public void AddArchive(IArchiveFile archive)
        {
            if (!PartArchives.ContainsKey(((IFileFormat)archive).FileName))
                PartArchives.Add(((IFileFormat)archive).FileName, archive);
        }

        public void Dispose()
        {
            foreach (var file in PartLayouts.Values)
                file.Dispose();

            foreach (var file in PartAnimations.Values)
                file.Dispose();

            foreach (var file in PartArchives.Values)
                ((IFileFormat)file).Unload();

            PartLayouts.Clear();
            PartAnimations.Clear();
            PartArchives.Clear();
        }
    }
}
