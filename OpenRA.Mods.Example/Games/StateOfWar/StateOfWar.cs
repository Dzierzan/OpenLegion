using System;
using System.IO;
using OpenRA.Mods.Example.Games.StateOfWar.FileSystem;
using OpenRA.Mods.Example.Games.StateOfWar.SpriteLoaders;

namespace OpenRA.Mods.Example.Games.StateOfWar
{
    public class StateOfWar : Game
    {
        public StateOfWar()
            : base("STATEOFWAR", "State of War")
        {
        }

        protected override string FindInstallation()
        {
            return FindSteamInstallation(748040);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterPackageLoader<DataLoader>();
            RegisterSpriteLoader<Ps6Loader>();

            foreach (var file in Directory.GetFiles(installPath, "*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".data", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
            }
        }
    }
}
