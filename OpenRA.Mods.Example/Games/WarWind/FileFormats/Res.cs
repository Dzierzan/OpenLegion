using System.Collections.Generic;
using System.IO;
using OpenRA.FileSystem;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
    public class Res : IReadOnlyPackage
    {
        private class ResEntry
        {
            public int Offset;
            public int Length;
        }

        public string Name { get; }
        public IEnumerable<string> Contents => index.Keys;

        private readonly Dictionary<string, ResEntry> index = new Dictionary<string, ResEntry>();
        private readonly Stream stream;

        public Res(Stream stream, string filename)
        {
            this.stream = stream;
            Name = filename;

            index = new Dictionary<string, ResEntry>();

            var entries = stream.ReadInt32();

            for (var i = 0; i < entries; i++)
            {
                stream.Position = 4 + i * 4;
                var offset = stream.ReadInt32();
                var length = (i + 1 == entries ? (int)stream.Length : stream.ReadInt32()) - offset;
                stream.Position = offset;
                var test = stream.ReadUInt32();

                // .000 File contains strings.. except second last file, which is something else.
                if (test == 0x52473344)
                    index.Add($"{i}.D3GR", new ResEntry { Offset = offset, Length = length });
                else if (test == 0x46464952)
                    index.Add($"{i}.WAV", new ResEntry { Offset = offset, Length = length });
            }
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
