using System;
using System.IO;
using OpenRA.Mods.Example.Games.Earth2140.FileSystem;
using OpenRA.Mods.Example.Games.Earth2140.SpriteLoaders;

namespace OpenRA.Mods.Example.Games.Earth2140
{
    public class Earth2140 : Game
    {
        public Earth2140()
            : base("EARTH2140", "Earth 2140")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1207658738);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterPackageLoader<WdLoader>();
            RegisterSpriteLoader<MixLoader>();

            foreach (var file in Directory.GetFiles(installPath))
            {
                if (file.EndsWith(".wd", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
            }
        }
    }
}
