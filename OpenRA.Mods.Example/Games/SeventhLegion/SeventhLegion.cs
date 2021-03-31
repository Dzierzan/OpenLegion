using System;
using System.IO;
using OpenRA.Mods.Example.Games.SeventhLegion.SpriteLoaders;

namespace OpenRA.Mods.Example.Games.SeventhLegion
{
    public class SeventhLegion : Game
    {
        public SeventhLegion()
            : base("7LEGION", "7th Legion")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1207660773) ?? FindSteamInstallation(327910);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterSpriteLoader<BimLoader>();

            foreach (var directory in Directory.GetDirectories(installPath))
            {
                if (Path.GetFileName(directory).Equals("gfx", StringComparison.OrdinalIgnoreCase))
                    Mount(directory);
            }
        }
    }
}
