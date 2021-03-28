using System;
using System.Text;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RadWorld
{
    [HarmonyPatch(typeof(TileFinder), nameof(TileFinder.RandomSettlementTileFor))]
    public static class RandomSettlementTileFor_Patch
    {
        public static Faction curFaction;
        public static void Prefix(Faction faction, bool mustBeAutoChoosable = false, Predicate<int> extraValidator = null)
        {
            curFaction = faction;
        }
        public static void Postfix(Faction faction, bool mustBeAutoChoosable = false, Predicate<int> extraValidator = null)
        {
            curFaction = null;
        }
    }

    [HarmonyPatch(typeof(TileFinder), nameof(TileFinder.IsValidTileForNewSettlement))]
    public static class IsValidTileForNewSettlement_Patch
    {
        public static void Postfix(ref bool __result, int tile, StringBuilder reason = null)
        {
            Tile tile2 = Find.WorldGrid[tile];
            if (tile2.biome != RW_DefOf.RW_Cavern)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(World), nameof(World.HasCaves))]
    public static class HasCaves_Patch
    {
        public static void Postfix(ref bool __result, int tile)
        {
            Tile tile2 = Find.WorldGrid[tile];
            if (tile2.biome == RW_DefOf.RW_Cavern)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(MapParent), nameof(MapParent.MapGeneratorDef), MethodType.Getter)]
    public static class MapParent_MapGeneratorDef_Patch
    {
        public static void Postfix(MapParent __instance, ref MapGeneratorDef __result)
        {
            Tile tile2 = Find.WorldGrid[__instance.Tile];
            if (tile2.biome == RW_DefOf.RW_Cavern)
            {
                if (__instance.def.mapGenerator == null)
                {
                    __result = RW_DefOf.RW_CavernEncounter;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Settlement), nameof(Settlement.MapGeneratorDef), MethodType.Getter)]
    public static class Settlement_MapGeneratorDef_Patch
    {
        public static void Postfix(Settlement __instance, ref MapGeneratorDef __result)
        {
            Tile tile2 = Find.WorldGrid[__instance.Tile];
            if (tile2.biome == RW_DefOf.RW_Cavern)
            {
                if (__instance.Faction == Faction.OfPlayer)
                {
                    __result = RW_DefOf.RW_CavernBase_Player;
                }
                else
                {
                    __result = RW_DefOf.RW_CavernBase_Faction;
                }
            }
        }
    }
}
