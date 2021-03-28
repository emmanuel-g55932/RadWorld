using System;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RadWorld
{
    public class BiomeWorker_Cavern : BiomeWorker
    {
		[TweakValue("0RadWorld", 0, 1f)] public static float caveGeneratorValue = 0.25f;
		public override float GetScore(Tile tile, int tileID)
		{
			if (tile.WaterCovered)
			{
				return -100f;
			}
			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
			var value = CavernPerlin.noiseElevation.GetValue(tileCenter);
			if (value > caveGeneratorValue)
			{
				return 100f;
			}
			return -100f;
		}
	}
}
