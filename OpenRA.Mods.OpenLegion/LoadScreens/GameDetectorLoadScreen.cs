using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.LoadScreens;

namespace OpenRA.Mods.OpenLegion.LoadScreens
{
    public class GameDetectorLoadScreen : BlankLoadScreen
    {
        private List<Games.Game> games = new List<Games.Game>();

        public override void Init(ModData modData, Dictionary<string, string> info)
        {
            foreach (var type in GetType().Assembly.GetTypes()
                .Where(type => type.IsAssignableTo(typeof(Games.Game)) && !type.IsAbstract))
            {
                var game = (Games.Game)Activator.CreateInstance(type);

                if (game == null)
                    continue;

                games.Add(game);
                game.TryAdd(modData);
            }

            base.Init(modData, info);
        }

        public override void StartGame(Arguments args)
        {
            foreach (var game in games)
                game.CreateRules();

            base.StartGame(args);
        }
    }
}
