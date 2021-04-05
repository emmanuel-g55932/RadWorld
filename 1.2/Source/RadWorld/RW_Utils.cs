using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
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

			foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.Humanlike ?? false))
            {
				thingDef.comps.Add(new CompProperties { compClass = typeof(CompMutation) });
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

		public static float GetRadiationImpactMultiplier(this Pawn pawn)
		{
			if (pawn.Map != null)
            {
				if (GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, 6.9f, true).Any(x => x.def == RW_DefOf.RW_RadiationCollector && x.TryGetComp<CompPowerTrader>().PowerOn))
                {
					return 0f;
                }
				if (GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, 14.9f, true).Any(x => x.def == RW_DefOf.RW_NanotechPurifier && x.TryGetComp<CompPowerTrader>().PowerOn))
				{
					return 0f;
				}
			}
			var resistance = 1f - pawn.GetStatValue(RW_DefOf.RW_RadiationResistance);
			return Mathf.Clamp01(resistance);
		}
		public static bool IsCavernBiome(this BiomeDef biomeDef)
        {
			return biomeDef == RW_DefOf.RW_CollapsedCavern || biomeDef == RW_DefOf.RW_LushCavern || biomeDef == RW_DefOf.RW_SickCavern 
				|| biomeDef == RW_DefOf.RW_InfestedCavern || biomeDef == RW_DefOf.RW_BarrenCavern || biomeDef == RW_DefOf.RW_SurfaceCavern || biomeDef == RW_DefOf.RW_Cavern;
		}
	}
}
