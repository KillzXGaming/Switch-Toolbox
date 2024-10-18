using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;

namespace BezelEngineArchive_Lib
{
    public class FileSaver : BinaryDataWriter
    {
        private IDictionary<string, StringEntry> _savedStrings;
        private List<long> _savedBlockPositions;
        private IDictionary<object, BlockEntry> _savedBlocks;
        private List<RelocationEntry> _savedSection1Entries;
        private List<RelocationSection> _savedRelocatedSections;
        private List<ItemEntry> _savedItems;

        internal BezelEngineArchive Archive;
        private long _ofsFileSize;
        private long _ofsRelocationTable;
        private long _ofsFirstBlock;
        private long _ofsAsstArray;
        private long _ofsAsstRefArray;
        private long _ofsFileDictionary;
        private long _ofsEndOfBlock;
        private uint Section1Size;
        private uint beaSize; //Excludes data blocks
        private long _ofsAsstStart;
        
        internal FileSaver(BezelEngineArchive bea, Stream stream, bool leaveOpen = true)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            ByteOrder = ByteOrder.LittleEndian;
            Archive = bea;
        }

        internal FileSaver(BezelEngineArchive bea, string fileName)
            : this(bea, new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), true)
        {
        }

        internal void Execute()
        {
            _savedBlockPositions = new List<long>();
            _savedBlocks = new Dictionary<object, BlockEntry>();
            _savedSection1Entries = new List<RelocationEntry>();
            _savedStrings = new Dictionary<string, StringEntry>();
            _savedRelocatedSections = new List<RelocationSection>();

            ((IFileData)Archive).Save(this);

            //Ref list for version 5 and up
            long RefOffset = Position; //Offset to blocks

            if (Archive.ReferenceList != null)
            {
                SaveRelocateEntryToSection(Position, (uint)Archive.ReferenceList.Count, 1, 0, 1, "Ref String List ");
                foreach (var asstRef in Archive.ReferenceList)
                    SaveString(asstRef);
            }

            //Set enough space to add offsets later
            long OffsetArrayASST = Position; //Offset to blocks

            List<long> _ofsAsstOffsets = new List<long>();
            foreach (ASST asst in Archive.FileList.Values)
            {
                SaveRelocateEntryToSection(Position, 1, 1, 0, 1, "Asst Offset "); 
                _ofsAsstOffsets.Add(Position);
                Write(0L);
            }
            //Now padding. 40 per file
            Seek(40 * Archive.FileList.Count, SeekOrigin.Current);

            //Now save dictionary.
            long DictionaryOffset = Position; //Offset to blocks
            ((IFileData)Archive.FileDictionary).Save(this);

            long BlockOffset = Position; //Offset to blocks
            for (int i = 0; i < Archive.FileList.Count; i++)
            {
                long AsstOffset = Position;
                using (this.TemporarySeek(_ofsAsstOffsets[i], SeekOrigin.Begin))
                {
                    Write(AsstOffset);
                }
                string FileName = Archive.FileDictionary.GetKey(i);
                ((IFileData)Archive.FileList[FileName]).Save(this);
            }

            WriteStringPool();


            SetupRelocationTable();
            WriteRelocationTable();

            WriteBlocks();

            for (int i = 0; i < _savedBlockPositions.Count; i++)
            {
                Position = _savedBlockPositions[i];

                if (i == _savedBlockPositions.Count - 1)
                {
                    Write(0);
                    Write(_ofsEndOfBlock - _savedBlockPositions[i]); //Size of string table to relocation table
                }
                else
                {
                    uint blockSize = (uint)(_savedBlockPositions[i + 1] - _savedBlockPositions[i]);
                    WriteHeaderBlock(blockSize, blockSize);
                }          
            }

            Position = _ofsAsstArray;
            Write((ulong)OffsetArrayASST);

            if (_ofsAsstStart != 0)
            {
                Position = _ofsAsstStart;
                Write((ulong)BlockOffset);
            }

            Position = _ofsFirstBlock;
            Write((ushort)BlockOffset);

            Position = _ofsFileDictionary;
            Write((ulong)DictionaryOffset);

            if (_ofsAsstRefArray != 0)
            {
                Position = _ofsAsstRefArray;
                Write((ulong)RefOffset);
            }

            Position = _ofsFileSize;
            Write(beaSize);
            Flush();
        }
        internal void SaveFirstBlock()
        {
            _ofsFirstBlock = Position;
            Write((ushort)0);
        }

        internal void SaveAssetBlock()
        {
            _ofsAsstStart = Position;
            Write((ulong)0);
        }
        internal void SaveFileAsstPointer()
        {
            _ofsAsstArray = Position;
            Write(0L);
        }


        internal void SaveAssetRefPointer()
        {
            _ofsAsstRefArray = Position;
            Write(0L);
        }
        internal void SaveFileDictionaryPointer()
        {
            _ofsFileDictionary = Position;
            Write(0L);
        }
        internal void SaveRelocationTablePointer()
        {
            _ofsRelocationTable = Position;
            Write(0);
        }
        internal void SaveFileSize()
        {
            _ofsFileSize = Position;
            Write(0);
        }
        internal void WriteSize(uint Offset, uint value)
        {
            using (this.TemporarySeek(Offset, SeekOrigin.Begin))
            {
                Write(value);
            }
        }

        internal uint SaveSizePtr()
        {
            Write(0);
            return (uint)Position;
        }
        internal void SaveBlockHeader()
        {
            _savedBlockPositions.Add(Position);
            Write(0);
            Write(0L);
        }
        internal void SaveBlock(object data, uint alignment, Action callback)
        {
            if (data == null)
            {
                Write(0L);
                return;
            }
            if (_savedBlocks.TryGetValue(data, out BlockEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                _savedBlocks.Add(data, new BlockEntry((uint)Position, alignment, callback));
            }
            Write(UInt64.MaxValue);
        }
        internal void SaveString(string str, Encoding encoding = null)
        {
            if (str == null)
            {
                Write(0L);
                return;
            }
            if (_savedStrings.TryGetValue(str, out StringEntry entry))
            {
                entry.Offsets.Add((uint)Position);
            }
            else
            {
                _savedStrings.Add(str, new StringEntry((uint)Position, encoding));
            }
            Write(UInt64.MaxValue);
        }
        internal void WriteSignature(string value)
        {
            this.Write(Encoding.ASCII.GetBytes(value));
        }

        //This only should use 1 section to relocate data
        internal void SaveRelocateEntryToSection(long pos, uint OffsetCount, uint StructCount, uint PaddingCount, int SectionNumber, string Hint)
        {
            if (SectionNumber == 1)
                _savedSection1Entries.Add(new RelocationEntry((uint)pos, OffsetCount, StructCount, PaddingCount, Hint));
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void WriteHeaderBlock(uint Size, long Offset)
        {
            Write(Size);
            Write(Offset);
        }
        private void SetupRelocationTable()
        {
            this.Align(Archive.RawAlignment);
            RelocationSection FileMainSect;

            long RelocationTableOffset = Position;

            int EntryIndex = 0;
            uint EntryPos = 0;

            foreach (RelocationEntry entry in _savedSection1Entries)
            {
                Console.WriteLine("Pos = " + entry.Position + " " + entry.StructCount + " " + entry.OffsetCount + " " + entry.PadingCount + " " + entry.Hint);
            }

            _savedSection1Entries = _savedSection1Entries.OrderBy(o => o.Position).ToList();
            FileMainSect = new RelocationSection(EntryPos, EntryIndex, Section1Size, _savedSection1Entries);

            _savedRelocatedSections.Add(FileMainSect);

        }
        private void WriteRelocationTable()
        {
            uint relocationTableOffset = (uint)Position;
            WriteSignature("_RLT");
            _ofsEndOfBlock = (uint)Position;
            Write(relocationTableOffset);
            Write(_savedRelocatedSections.Count);
            Write(0); //padding

            foreach (RelocationSection section in _savedRelocatedSections)
            {
                Write(0L); //padding
                Write(section.Position);
                Write(section.Size);
                Write(section.EntryIndex);
                Write(section.Entries.Count);
            }

            foreach (RelocationSection section in _savedRelocatedSections)
            {
                foreach (RelocationEntry entry in section.Entries)
                {
                    Write(entry.Position);
                    Write((ushort)entry.StructCount);
                    Write((byte)entry.OffsetCount);
                    Write((byte)entry.PadingCount);
                }
            }

            beaSize = (uint)Position;

            using (this.TemporarySeek(_ofsRelocationTable, SeekOrigin.Begin))
            {
                Write(relocationTableOffset);
            }
        }
        private void WriteBlocks()
        {
            foreach (KeyValuePair<object, BlockEntry> entry in _savedBlocks)
            {
                // Align and satisfy offsets.
                Console.WriteLine(entry.Value.Alignment);
                Console.WriteLine(Position);
                if (entry.Value.Alignment != 0) this.Align((int)entry.Value.Alignment);
                Console.WriteLine(Position);

                using (this.TemporarySeek())
                {
                    SatisfyOffsets(entry.Value.Offsets, (uint)Position);
                }

                // Write the data.
                entry.Value.Callback.Invoke();
            }
        }
        private void WriteStringPool()
        {
            WriteSignature("_STR");
            SaveBlockHeader();
            Write(_savedStrings.Count - 1);

            foreach (KeyValuePair<string, StringEntry> entry in _savedStrings)
            {
                using (this.TemporarySeek())
                {
                    SatisfyOffsets(entry.Value.Offsets, (uint)Position);
                }
                // Write the name.
                Write(entry.Key, BinaryStringFormat.WordLengthPrefix, entry.Value.Encoding ?? Encoding);
                Align(2);
            }
            Section1Size = (uint)Position;
        }

        private void SatisfyOffsets(IEnumerable<uint> offsets, uint target)
        {
            foreach (uint offset in offsets)
            {
                Position = offset;
                Write((long)(target));
            }
        }

        private bool TryGetItemEntry(object data, ItemEntryType type, out ItemEntry entry)
        {
            foreach (ItemEntry savedItem in _savedItems)
            {
                if (savedItem.Data.Equals(data) && savedItem.Type == type)
                {
                    entry = savedItem;
                    return true;
                }
            }
            entry = null;
            return false;
        }

        private class StringEntry
        {
            internal List<uint> Offsets;
            internal Encoding Encoding;

            internal StringEntry(uint offset, Encoding encoding = null)
            {
                Offsets = new List<uint>(new uint[] { offset });
                Encoding = encoding;
            }
        }
        private class BlockEntry
        {
            internal List<uint> Offsets;
            internal uint Alignment;
            internal Action Callback;

            internal BlockEntry(uint offset, uint alignment, Action callback)
            {
                Offsets = new List<uint> { offset };
                Alignment = alignment;
                Callback = callback;
            }
        }
        private class RelocationSection
        {
            internal List<RelocationEntry> Entries;
            internal int EntryIndex;
            internal uint Size;
            internal uint Position;

            internal RelocationSection(uint position, int entryIndex, uint size, List<RelocationEntry> entries)
            {
                Position = position;
                EntryIndex = entryIndex;
                Size = size;
                Entries = entries;
            }
        }
        private enum ItemEntryType
        {
            List, Dict, FileData, Custom
        }
        private class ItemEntry
        {
            internal object Data;
            internal ItemEntryType Type;
            internal List<uint> Offsets;
            internal uint? Target;
            internal Action Callback;
            internal int Index;

            internal ItemEntry(object data, ItemEntryType type, uint? offset = null, uint? target = null,
                Action callback = null, int index = -1)
            {
                Data = data;
                Type = type;
                Offsets = new List<uint>();
                if (offset.HasValue) // Might be null for enumerable entries to resolve references to them later.
                {
                    Offsets.Add(offset.Value);
                }
                Callback = callback;
                Target = target;
                Index = index;
            }
        }
        private class RelocationEntry
        {
            internal uint Position;
            internal uint PadingCount;
            internal uint StructCount;
            internal uint OffsetCount;
            internal string Hint;

            internal RelocationEntry(uint position, uint offsetCount, uint structCount, uint padingCount, string hint)
            {
                Position = position;
                StructCount = structCount;
                OffsetCount = offsetCount;
                PadingCount = padingCount;
                Hint = hint;
            }
        }
    }
}
