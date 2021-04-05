using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RadWorld
{
    [HarmonyPatch(typeof(Need_Food), nameof(Need_Food.MaxLevel), MethodType.Getter)]
    public static class MaxLevel_Patch
    {
        public static void Postfix(Pawn ___pawn, ref float __result)
        {
            var hediff = ___pawn.health.hediffSet.GetFirstHediffOfDef(RW_DefOf.RW_EnlargedStomach);
            if (hediff != null)
            {
                __result *= 1.3f;
            }
            else
            {
                hediff = ___pawn.health.hediffSet.GetFirstHediffOfDef(RW_DefOf.RW_ReducedStomach);
                if (hediff != null)
                {
                    __result /= 1.3f;
                }
            }
        }
    }
}
