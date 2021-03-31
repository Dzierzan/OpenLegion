using System.Collections.Generic;
using System.IO;
using OpenRA.FileSystem;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.StateOfWar.FileFormats
{
    public class Data : IReadOnlyPackage
    {
        private class DataEntry
        {
            public int Offset;
            public int Length;
        }

        public string Name { get; }
        public IEnumerable<string> Contents => index.Keys;

        private readonly Dictionary<string, DataEntry> index = new Dictionary<string, DataEntry>();
        private readonly Stream stream;

        public Data(Stream stream, string filename)
        {
            this.stream = stream;
            Name = filename;

            index = new Dictionary<string, DataEntry>();

            var dir = Path.GetDirectoryName((stream as FileStream).Name);
            var name = Path.GetFileNameWithoutExtension(filename);
            var indexStream = new FileStream(Path.Combine(dir, name + ".info"), FileMode.Open, FileAccess.Read);

            indexStream.ReadInt32();
            var entries = indexStream.ReadInt32();

            for (var i = 0; i < entries; i++)
            {
                int temp;
                var tempName = "";

                while ((temp = indexStream.ReadByte()) != 0x00)
                {
                    tempName += (char)(temp - 0xa);
                }

                var offset = indexStream.ReadInt32();
                var length = indexStream.ReadInt32();
                index.Add(tempName, new DataEntry { Offset = offset, Length = length });
            }

            indexStream.Dispose();
        }

        public Stream GetStream(string filename)
        {
            if (!index.TryGetValue(filename, out var entry))
                return null;

            return SegmentStream.CreateWithoutOwningStream(stream, entry.Offset, entry.Length);
        }

        public bool Contains(string filename)
        {
            return index.ContainsKey(filename);
        }

        public IReadOnlyPackage OpenPackage(string filename, OpenRA.FileSystem.FileSystem context)
        {
            var childStream = GetStream(filename);

            if (childStream == null)
                return null;

            if (context.TryParsePackage(childStream, filename, out var package))
                return package;

            childStream.Dispose();

            return null;
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
