using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.LoadScreens;

namespace OpenRA.Mods.Example.LoadScreens
{
    public class GameDetectorLoadScreen : BlankLoadScreen
    {
        public override void Init(ModData modData, Dictionary<string, string> info)
        {
            foreach (var type in GetType().Assembly.GetTypes()
                .Where(type => type.IsAssignableTo(typeof(Games.Game)) && !type.IsAbstract))
            {
                ((Games.Game)Activator.CreateInstance(type))?.TryAdd(modData);
            }

            base.Init(modData, info);
        }
    }
}
