using System.IO;
using System.Text.RegularExpressions;
using OpenRA.FileSystem;
using OpenRA.Mods.Example.Games.WarWind.FileFormats;

namespace OpenRA.Mods.Example.Games.WarWind.FileSystem
{
    public class ResLoader : IPackageLoader
    {
        public bool TryParsePackage(Stream s, string filename, OpenRA.FileSystem.FileSystem context, out IReadOnlyPackage package)
        {
            if (!Regex.IsMatch(Path.GetFileName(filename), "^RES.\\d{3}$", RegexOptions.IgnoreCase))
            {
                package = null;
                return false;
            }

            package = new Res(s, filename);
            return true;
        }
    }
}
