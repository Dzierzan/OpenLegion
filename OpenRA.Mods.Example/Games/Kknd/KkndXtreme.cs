using System;
using System.IO;
using OpenRA.Mods.Example.Games.Kknd.FileSystem;
using OpenRA.Mods.Example.Games.Kknd.SpriteLoaders;

namespace OpenRA.Mods.Example.Games.Kknd
{
    public class KkndXtreme : Game
    {
        public KkndXtreme()
            : base("KKND1", "Krush, Kill 'n' Destroy Xtreme")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1207659107) ?? FindSteamInstallation(1292180);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterPackageLoader<LvlLoader>();

            RegisterSpriteLoader<MobdLoader>();
            RegisterSpriteLoader<MapdLoader>();
            RegisterSpriteLoader<BlitLoader>();

            /* SoundFormats: Wav, Soun, Son */
            /* VideoFormats: Vbc */
            /* TODO: palettes */

            foreach (var file in Directory.GetFiles(installPath, "*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".lvl", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
                else if (file.EndsWith(".slv", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
            }
        }
    }
}
