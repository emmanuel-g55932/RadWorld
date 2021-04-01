using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Noise;

namespace RadWorld
{
	[StaticConstructorOnStartup]
	public static class RW_Utils
	{
		public static string ModPackageId = "Declinedkilr.RadWorld";
		static RW_Utils()
        {
			foreach (var biomeDef in DefDatabase<BiomeDef>.AllDefs)
            {
				var options = biomeDef.GetModExtension<BiomeOptions>();
				if (options is null)
                {
					if (biomeDef.modExtensions is null)
                    {
						biomeDef.modExtensions = new List<DefModExtension>();
                    }
					biomeDef.modExtensions.Add(new BiomeOptions()
					{
						nuclearFalloutModifier = 1f
					});
				}
            }
        }

		public static float GetNuclearModifier(this BiomeDef biomeDef)
        {
			var options = biomeDef.GetModExtension<BiomeOptions>();
			if (options != null)
			{
				return options.nuclearFalloutModifier;
			}
			return 0;
		}
		public static bool IsCavernBiome(this BiomeDef biomeDef)
        {
			return biomeDef == RW_DefOf.RW_CollapsedCavern || biomeDef == RW_DefOf.RW_LushCavern || biomeDef == RW_DefOf.RW_SickCavern 
				|| biomeDef == RW_DefOf.RW_InfestedCavern || biomeDef == RW_DefOf.RW_BarrenCavern || biomeDef == RW_DefOf.RW_SurfaceCavern || biomeDef == RW_DefOf.RW_Cavern;
		}
	}
}
