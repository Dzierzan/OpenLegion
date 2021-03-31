using System;
using System.IO;
using OpenRA.FileSystem;
using OpenRA.Mods.Example.Games.StateOfWar.FileFormats;

namespace OpenRA.Mods.Example.Games.StateOfWar.FileSystem
{
    public class DataLoader : IPackageLoader
    {
        public bool TryParsePackage(Stream s, string filename, OpenRA.FileSystem.FileSystem context, out IReadOnlyPackage package)
        {
            if (!filename.EndsWith(".data", StringComparison.OrdinalIgnoreCase))
            {
                package = null;
                return false;
            }

            package = new Data(s, filename);
            return true;
        }
    }
}
