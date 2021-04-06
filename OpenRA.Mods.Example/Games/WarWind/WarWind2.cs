using System.IO;
using System.Text.RegularExpressions;
using OpenRA.Mods.Example.Games.WarWind.FileSystem;
using OpenRA.Mods.Example.Games.WarWind.SpriteLoaders;

namespace OpenRA.Mods.Example.Games.WarWind
{
    public class WarWind2 : Game
    {
        public WarWind2()
            : base("WARWIND2", "War Wind 2: Human Onslaught")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1441098222);
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
