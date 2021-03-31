using System;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Noise;

namespace RadWorld
{
	[DefOf]
	public static class RW_DefOf
	{
		public static BiomeDef RW_CollapsedCavern;
		public static BiomeDef RW_LushCavern;
		public static BiomeDef RW_SickCavern;
		public static BiomeDef RW_InfestedCavern;
		public static BiomeDef RW_BarrenCavern;
		public static BiomeDef RW_SurfaceCavern;
		public static BiomeDef RW_Cavern;

		public static GameConditionDef RW_UndergroundCondition;
		public static MapGeneratorDef RW_CavernBase_Player;
		public static MapGeneratorDef RW_CavernBase_Faction;
		public static MapGeneratorDef RW_CavernEncounter;
	}
}
