using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RadWorld
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("Declinedkilr.RadWorld");
            harmony.PatchAll();
            var prefix = AccessTools.Method(typeof(HarmonyPatches), "Prefix");
            var postfix = AccessTools.Method(typeof(HarmonyPatches), "Postfix");

            foreach (var type in typeof(PawnGroupKindWorker).AllSubclassesNonAbstract())
            {
                var methodToPatch = AccessTools.Method(type, "GeneratePawns", new Type[] { typeof(PawnGroupMakerParms), typeof(PawnGroupMaker), typeof(List<Pawn>), typeof(bool) });
                if (methodToPatch != null)
                {
                    harmony.Patch(methodToPatch, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                }
                else
                {
                    Log.Error("Can't patch " + type);
                }
            }


            //var tryIssueJobPackage_Track = AccessTools.Method(typeof(HarmonyPatches), "TryIssueJobPackage_Track");
            //foreach (var type in typeof(ThinkNode).AllSubclassesNonAbstract())
            //{
            //    try
            //    {
            //        var methodToPatch = AccessTools.Method(type, "TryIssueJobPackage");
            //        if (methodToPatch != null)
            //        {
            //            harmony.Patch(methodToPatch, null, new HarmonyMethod(tryIssueJobPackage_Track));
            //        }
            //        else
            //        {
            //            Log.Error("Can't patch " + type);
            //        }
            //    }
            //    catch { };
            //}
        }

        private static void TryIssueJobPackage_Track(ThinkNode_JobGiver __instance, ThinkResult __result, Pawn pawn, JobIssueParams jobParams)
        {
        	if (pawn.RaceProps.Humanlike && __result.Job != null)
        	{
        		Log.Message(pawn + " gets " + __result.Job + " from " + __instance);
        	}
        }
        private static void Prefix()
        {
            Patch_GenerateStartingApparelFor.generateRadProtectiveGear = true;
        }

        private static void Postfix()
        {
            Patch_GenerateStartingApparelFor.generateRadProtectiveGear = false;
        }
    }

    [HarmonyPatch(typeof(StartingPawnUtility), "NewGeneratedStartingPawn")]
    internal static class Patch_NewGeneratedStartingPawn
    {
        public static bool generationIsActive;
        private static void Prefix()
        {
            generationIsActive = true;
        }
        private static void Postfix()
        {
            generationIsActive = false;
        }
    }

    [HarmonyPatch(typeof(PawnApparelGenerator), "GenerateStartingApparelFor")]
    internal static class Patch_GenerateStartingApparelFor
    {
        public static bool generateRadProtectiveGear;
        private static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            if (request.Tile != -1)
            {
                Tile tile = Find.WorldGrid[request.Tile];
                Log.Message(pawn + " - " + pawn.Map + " - " + request.Tile + " - tile: " + tile + " biome: " + tile?.biome);
                if (pawn.Faction != null && (tile.biome != null && tile.biome.GetNuclearModifier() > 0 || pawn.Faction.def == RW_DefOf.RW_VaultRough) && pawn.Faction.def != RW_DefOf.RW_MutantRough && pawn.apparel != null && (generateRadProtectiveGear || Patch_NewGeneratedStartingPawn.generationIsActive && Rand.Chance(0.3f)))
                {
                    var allApparelPairs = ThingStuffPair.AllWith((ThingDef td) => td.IsApparel && (int)pawn.Faction.def.techLevel >= (int)td.techLevel && td.GetStatValueAbstract(RW_DefOf.RW_RadiationResistanceOffset) > 0);
                    Log.Message(pawn + " - pawn.Faction: " + pawn.Faction.def.techLevel);

                    var hats = allApparelPairs.Where(x => IsHeadgear(x.thing) && CanUseStuff(pawn, x) && x.thing.apparel.CorrectGenderForWearing(pawn.gender));
                    foreach (var ap in hats)
                    {
                        Log.Message("Hat: " + ap.thing + " - " + ap.thing.techLevel + " - " + IsHeadgear(ap.thing));
                    }
                    if (hats.TryRandomElementByWeight(pa => pa.Commonality, out var hatPair))
                    {
                        var hat = ThingMaker.MakeThing(hatPair.thing, hatPair.stuff) as Apparel;
                        pawn.apparel.Wear(hat, false);
                        var comp = hat.TryGetComp<CompGasMaskReloadable>();
                        if (comp != null)
                        {
                            var randomFilterCount = Rand.RangeInclusive(1, comp.Props.maxCharges);
                            for (var i = 0; i < randomFilterCount; i++)
                            {
                                var fuel = ThingMaker.MakeThing(comp.Props.ammoDef);
                                comp.ReloadFrom(fuel);
                                Log.Message("Reloading: " + comp + " - " + comp.RemainingCharges);
                            }
                        }
                        Log.Message(pawn + " 0 is wearing " + hat);
                    }

                    var apparels = allApparelPairs.Where(x => !hats.Any(y => x.thing == y.thing) && CanUseStuff(pawn, x) && x.thing.apparel.CorrectGenderForWearing(pawn.gender));
                    foreach (var ap in apparels)
                    {
                        Log.Message("Apparel: " + ap.thing + " - " + ap.thing.techLevel + " - " + IsHeadgear(ap.thing));
                    }
                    if (apparels.TryRandomElementByWeight(pa => pa.Commonality, out var apparelPair))
                    {
                        var apparel = ThingMaker.MakeThing(apparelPair.thing, apparelPair.stuff) as Apparel;
                        pawn.apparel.Wear(apparel, false);
                        Log.Message(pawn + " 1 is wearing " + apparel);
                    }
                }
            }

        }
        public static bool IsHeadgear(ThingDef td)
        {
            if (!td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead))
            {
                return td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead);
            }
            return true;
        }
        private static bool CanUseStuff(Pawn pawn, ThingStuffPair pair)
        {
            if (pair.stuff != null && pawn.Faction != null && !pawn.Faction.def.CanUseStuffForApparel(pair.stuff))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Dialog_FormCaravan), "TryFormAndSendCaravan")]
    internal static class Patch_TryFormAndSendCaravan
    {
        public static bool considerOutdoor;
		private static void Prefix()
        {
            considerOutdoor = true;
        }
        private static void Postfix()
        {
            considerOutdoor = false;
        }
    }

    [HarmonyPatch(typeof(Room), "PsychologicallyOutdoors", MethodType.Getter)]
    internal static class Patch_PsychologicallyOutdoors
    {
        private static void Postfix(ref bool __result)
        {
            if (Patch_TryFormAndSendCaravan.considerOutdoor)
            {
                __result = true;
            }
        }
    }
}
