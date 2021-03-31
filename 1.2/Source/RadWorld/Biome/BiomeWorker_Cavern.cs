using System;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RadWorld
{
    public class BiomeWorker_Cavern : BiomeWorker
	{
		public virtual float CaveGeneratorValue => 0f;
		public float GetScore(Tile tile, int tileID, BiomeDef biomeDef)
		{
			if (tile.WaterCovered)
			{
				return -100f;
			}
			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
			var value = CavernPerlin.GetNoiseFor(biomeDef).GetValue(tileCenter);
			if (value > CaveGeneratorValue)
			{
				var result = 100f + value;
				return result;
			}
			return -100f;
		}

        public override float GetScore(Tile tile, int tileID)
        {
            throw new NotImplementedException();
        }
    }

	public class BiomeWorker_CavernRegular : BiomeWorker_Cavern
	{
		[TweakValue("0RadWorld", 0, 1f)] public static float perlinValue = 0.25f;
		public override float CaveGeneratorValue => perlinValue;
        public override float GetScore(Tile tile, int tileID)
        {
			return GetScore(tile, tileID, RW_DefOf.RW_Cavern);
        }
    }

	public class BiomeWorker_CavernCollapsed : BiomeWorker_Cavern
	{
		[TweakValue("0RadWorld", 0, 1f)] public static float perlinValue = 0.73f;
		public override float CaveGeneratorValue => perlinValue;
		public override float GetScore(Tile tile, int tileID)
		{
			return GetScore(tile, tileID, RW_DefOf.RW_CollapsedCavern);
		}
	}

	public class BiomeWorker_CavernLush : BiomeWorker_Cavern
	{
		[TweakValue("0RadWorld", 0, 1f)] public static float perlinValue = 0.54f;
		public override float CaveGeneratorValue => perlinValue;
		public override float GetScore(Tile tile, int tileID)
		{
			return GetScore(tile, tileID, RW_DefOf.RW_LushCavern);
		}
	}

	public class BiomeWorker_CavernSick : BiomeWorker_Cavern
	{
		[TweakValue("0RadWorld", 0, 1f)] public static float perlinValue = 0.46f;
		public override float CaveGeneratorValue => perlinValue;
		public override float GetScore(Tile tile, int tileID)
		{
			return GetScore(tile, tileID, RW_DefOf.RW_SickCavern);
		}
	}

	public class BiomeWorker_CavernInfested : BiomeWorker_Cavern
	{
		[TweakValue("0RadWorld", 0, 1f)] public static float perlinValue = 0.70f;
		public override float CaveGeneratorValue => perlinValue;
		public override float GetScore(Tile tile, int tileID)
		{
			return GetScore(tile, tileID, RW_DefOf.RW_InfestedCavern);
		}
	}

	public class BiomeWorker_CavernBarren : BiomeWorker_Cavern
	{
		[TweakValue("0RadWorld", 0, 1f)] public static float perlinValue = 0.60f;
		public override float CaveGeneratorValue => perlinValue;
		public override float GetScore(Tile tile, int tileID)
		{
			return GetScore(tile, tileID, RW_DefOf.RW_BarrenCavern);
		}
	}

	public class BiomeWorker_CavernSurface : BiomeWorker_Cavern
	{
		[TweakValue("0RadWorld", 0, 1f)] public static float perlinValue = 0.80f;
		public override float CaveGeneratorValue => perlinValue;
		public override float GetScore(Tile tile, int tileID)
		{
			return GetScore(tile, tileID, RW_DefOf.RW_SurfaceCavern);
		}
	}
}
