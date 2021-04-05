using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RadWorld
{
    public class Hediff_Radiation : HediffWithComps
    {
        private static Dictionary<HediffDef, BodyPartDef> hediffsPerBodyParts = new Dictionary<HediffDef, BodyPartDef>
        {
            {RW_DefOf.RW_RadiationResistanceHediff, BodyPartDefOf.Body},
            {RW_DefOf.RW_ExtraArm, BodyPartDefOf.Torso},
            {RW_DefOf.RW_ExtraLeg, BodyPartDefOf.Torso},
            {RW_DefOf.RW_DeformedEyes, BodyPartDefOf.Eye},
            {RW_DefOf.RW_DeformedLung, DefDatabase<BodyPartDef>.GetNamed("Lung")},
            {RW_DefOf.RW_DeadNerves, BodyPartDefOf.Head},
            {RW_DefOf.RW_EnlargedStomach, BodyPartDefOf.Stomach},
            {RW_DefOf.RW_ReducedStomach, BodyPartDefOf.Stomach},
            {RW_DefOf.RW_IrradiatedBlood, BodyPartDefOf.Body},
            {RW_DefOf.RW_IrradiatedBrain, BodyPartDefOf.Brain},
            {RW_DefOf.RW_EnlargedEars, DefDatabase<BodyPartDef>.GetNamed("Ear")},
            {RW_DefOf.RW_DeformedEars, DefDatabase<BodyPartDef>.GetNamed("Ear")},
            {RW_DefOf.RW_DeformedMouth, BodyPartDefOf.Jaw},
            {RW_DefOf.RW_RadiationBurnScar, null},
            {RW_DefOf.RW_EnlargedMuscles, BodyPartDefOf.Body},
            {RW_DefOf.RW_WeakenedMuscles, BodyPartDefOf.Body},
            {RW_DefOf.RW_ScalySkin, BodyPartDefOf.Body},
        };
        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % 30 == 0)
            {
                var chance = MutationChances.Evaluate(this.Severity);
                if (Rand.Chance(chance))
                {
                    var hediffCandidates = GetFreeHediffCandidates();
                    if (hediffCandidates.Any())
                    {
                        var randomHediff = hediffCandidates.RandomElement();
                        var part = randomHediff.Value;
                        if (randomHediff.Key == RW_DefOf.RW_RadiationBurnScar)
                        {
                            part = pawn.RaceProps.body.AllParts.Where(x => x.depth == BodyPartDepth.Outside).RandomElement();
                        }
                        var hediff = HediffMaker.MakeHediff(randomHediff.Key, pawn, part);
                        Log.Message("Adding hediff: " + hediff + " to " + pawn);
                        pawn.health.AddHediff(hediff, part);
                        Find.LetterStack.ReceiveLetter("RW.NewMutation".Translate(randomHediff.Key.LabelCap, pawn.Named("PAWN")), "RW.NewMutationDesc".Translate(randomHediff.Key.LabelCap, pawn.Named("PAWN")),
                            LetterDefOf.NeutralEvent, pawn);
                    }
                }
            }
        }


        //private bool HasOtherHediff(ref List<BodyPartRecord> parts, List<HediffDef> hediffDefs)
        //{
        //    foreach (var otherHediff in hediffDefs)
        //    {
        //        Log.Message(pawn + " - otherHediff: " + otherHediff);
        //        var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(otherHediff);
        //        if (hediff != null && )
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        private Dictionary<HediffDef, BodyPartRecord> GetFreeHediffCandidates()
        {
            Dictionary<HediffDef, BodyPartRecord> hediffsWithBodyPartRecords = new Dictionary<HediffDef, BodyPartRecord>();
            Log.Message(pawn + " - start");

            foreach (var hediffData in hediffsPerBodyParts)
            {
                var hediffs = this.pawn.health.hediffSet.hediffs.Where(x => x.def == hediffData.Key);
                if (hediffData.Value != null)
                {
                    Log.Message(pawn + " - checking: " + hediffData.Key);
                    var parts = this.pawn.RaceProps.body.AllParts.Where(x => x.def == hediffData.Value).ToList();
                    var otherHediffs = hediffsPerBodyParts.Where(x => x.Value == hediffData.Value && x.Key != hediffData.Key).Select(x => x.Key).ToList();

                    foreach (var hediff in hediffs)
                    {
                        if (parts.Contains(hediff.Part))
                        {
                            parts.Remove(hediff.Part);
                        }
                    }

                    foreach (var otherHediff in otherHediffs)
                    {
                        Log.Message(pawn + " - otherHediff: " + otherHediff);
                        var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(otherHediff);
                        if (hediff != null && parts.Contains(hediff.Part))
                        {
                            parts.Remove(hediff.Part);
                        }
                    }

                    if (parts.Any())
                    {
                        hediffsWithBodyPartRecords[hediffData.Key] = parts.RandomElement();
                    }

                    //if (!HasOtherHediff(parts, otherHediffs))
                    //{
                    //    foreach (var d in parts)
                    //    {
                    //        Log.Message(pawn + " - pawn part: " + d);
                    //    }
                    //
                    //}
                }
                else
                {
                    hediffsWithBodyPartRecords[hediffData.Key] = null;
                }
            }
            Log.ResetMessageCount();
            return hediffsWithBodyPartRecords;
        }

        private static readonly SimpleCurve MutationChances = new SimpleCurve
        {
            new CurvePoint(0.6f, 0f),
            new CurvePoint(0.7f, 0.1f),
            new CurvePoint(0.8f, 0.2f),
            new CurvePoint(0.9f, 0.35f),
        }; 
    }
}
