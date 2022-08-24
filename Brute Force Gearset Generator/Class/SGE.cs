﻿using BruteForceGearsetGenerator.Items;
using BruteForceGearsetGenerator.Utilities;
using System;

namespace BruteForceGearsetGenerator.Class
{
    class SGE : XIVHealer
    {
        public SGE() : base()
        {
            Filler = 330;
            DotTick = 70;
            DotHit = 0;
            Cycle = 60;

            JobMod = 115;

            statToMinimise = SubStatEnum.Piety;
            hasStatToMinimise = true;

            uselessStatsOnFood.Add(SubStatEnum.SkillSpeed);
            uselessStatsOnFood.Add(SubStatEnum.Tenacity);
            uselessStatsOnFood.Add(SubStatEnum.Piety);

            speedStat = SubStatEnum.SpellSpeed;
            Name = "SGE";
        }

        protected override decimal ClipPotency(GearSet g)
        {
            decimal potency;
            int sps = g.TotalSubStat(speedStat);
            decimal scalar = SpsScalar(sps);
            decimal GCD = g.GCD();


            int FillerCasts = Decimal.ToInt32(Math.Floor(FillerPerCycle(GCD)));
            decimal cycleLength = (FillerCasts + 1) * GCD;
            decimal ticks = Math.Min(Cycle, cycleLength)/3;

            decimal fillerPotency = (Filler * FillerCasts);
            decimal DotPotency = DotHit + (DotTick * ticks * scalar);
            potency = (fillerPotency + DotPotency) / cycleLength;

            return Math.Round(potency,3);
        }


        protected override decimal DriftPotency(GearSet g)
        {
            decimal potency;
            int sps = g.TotalSubStat(speedStat);
            decimal scalar = SpsScalar(sps);
            decimal GCD = g.GCD();

            int FillerCasts = Decimal.ToInt32(Math.Ceiling(FillerPerCycle(GCD)));
            decimal cycleLength = (FillerCasts + 1) * GCD;
            decimal ticks = Math.Min(Cycle, cycleLength) / 3;

            decimal B4Potency = (Filler * FillerCasts);
            decimal BioPotency = DotHit + (DotTick * ticks * scalar);
            potency = (B4Potency + BioPotency) / cycleLength;

            return Math.Round(potency, 3);
        }



    }
}
