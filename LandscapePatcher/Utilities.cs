using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapePatcher
{
    internal static class Utilities
    {
        public static ISkyrimModGetter? GetModByFileName(this ILoadOrder<IModListing<ISkyrimModGetter>>? LoadOrder, string Name)
        {
            if (LoadOrder == null) return null;
            var Mods = LoadOrder.Keys.Where(x => ((string)x.FileName).ToLower() == Name.ToLower()).ToList();
            return (Mods.Count > 0) ? LoadOrder[Mods.First()].Mod : null;
        }
    }
}
