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
            new Harmony("Declinedkilr.RadWorld").PatchAll();
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
