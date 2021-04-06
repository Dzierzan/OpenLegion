using System.IO;
using System.Text.RegularExpressions;
using OpenRA.Mods.Example.Games.WarWind.FileSystem;
using OpenRA.Mods.Example.Games.WarWind.SpriteLoaders;

namespace OpenRA.Mods.Example.Games.WarWind
{
    public class WarWind : Game
    {
        public WarWind()
            : base("WARWIND", "War Wind")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1455545518);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterPackageLoader<ResLoader>();
            RegisterSpriteLoader<D3grLoader>();

            foreach (var file in Directory.GetFiles(installPath, "*", SearchOption.AllDirectories))
            {
                if (Regex.IsMatch(Path.GetFileName(file), "^RES.\\d{3}$", RegexOptions.IgnoreCase))
                    Mount(file);
            }
        }
    }
}
