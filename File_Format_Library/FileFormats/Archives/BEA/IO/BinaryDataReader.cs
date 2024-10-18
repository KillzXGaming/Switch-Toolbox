using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;

namespace BezelEngineArchive_Lib
{
    //Thanks to Syroot for the IO and methods
    public class FileLoader : BinaryDataReader
    {
        private IDictionary<uint, IFileData> _dataMap;

        public BezelEngineArchive Archive;

        public FileLoader(BezelEngineArchive bea, Stream stream, bool leaveOpen = true)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            ByteOrder = ByteOrder.LittleEndian;
            Archive = bea;
            _dataMap = new Dictionary<uint, IFileData>();
        }

        internal FileLoader(BezelEngineArchive bea, string fileName)
             : this(bea, new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        internal T LoadCustom<T>(Func<T> callback, long? offset = null)
        {
            offset = offset ?? ReadOffset();
            if (offset == 0) return default(T);

            using (this.TemporarySeek(offset.Value, SeekOrigin.Begin))
            {
                return callback.Invoke();
            }
        }

        internal string LoadString(Encoding encoding = null)
        {
            long offset = ReadInt64();
            if (offset == 0) return null;

            encoding = encoding ?? Encoding;
            using (this.TemporarySeek(offset, SeekOrigin.Begin))
            {
                ushort count = ReadUInt16();
                return this.ReadString(count, encoding: encoding);
            }
        }
        internal void LoadBlockHeader()
        {
            uint offset = ReadUInt32();
            ulong size = ReadUInt64();
        }

        internal void Execute()
        {
            Seek(0, SeekOrigin.Begin);
            ((IFileData)Archive).Load(this);
        }

        internal long ReadOffset()
        {
            long offset = ReadInt64();
            return offset == 0 ? 0 : offset;
        }

        internal T Load<T>()
            where T : IFileData, new()
        {
            long offset = ReadOffset();
            if (offset == 0) return default(T);

            // Seek to the instance data and load it.
            using (this.TemporarySeek(offset, SeekOrigin.Begin))
            {
                return ReadData<T>();
            }
        }

        internal IList<T> LoadList<T>(int count, long? offset = null)
            where T : IFileData, new()
        {
            List<T> list = new List<T>(count);
            offset = offset ?? ReadOffset();
            if (offset == 0 || count == 0) return list;

            // Seek to the list start and read it.
            using (this.TemporarySeek(offset.Value, SeekOrigin.Begin))
            {
                for (; count > 0; count--)
                {
                    list.Add(ReadData<T>());
                }
                return list;
            }
        }

        internal ResDict LoadDict()
        {
            long offset = ReadInt64();
            if (offset == 0) return new ResDict();

            using (this.TemporarySeek(offset, SeekOrigin.Begin))
            {
                ResDict dict = new ResDict();
                ((IFileData)dict).Load(this);
                return dict;
            }
        }

        internal void CheckSignature(string validSignature)
        {
            // Read the actual signature and compare it.
            string signature = this.ReadString(sizeof(uint), Encoding.ASCII);
            if (signature != validSignature)
            {
                throw new Exception($"Invalid signature, expected '{validSignature}' but got '{signature}'.");
            }
        }

        private T ReadData<T>()
     where T : IFileData, new()
        {
            uint offset = (uint)Position;

            // Same data can be referenced multiple times. Load it in any case to move in the stream, needed for lists.
            T instance = new T();
            instance.Load(this);

            // If possible, return an instance already representing the data.
            if (_dataMap.TryGetValue(offset, out IFileData existingInstance))
            {
                return (T)existingInstance;
            }
            else
            {
                _dataMap.Add(offset, instance);
                return instance;
            }
        }
    }
}
