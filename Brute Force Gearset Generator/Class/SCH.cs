using BruteForceGearsetGenerator.Items;
using BruteForceGearsetGenerator.Utilities;
using System;

namespace BruteForceGearsetGenerator.Class
{
    class SCH : XIVHealer
    {
        public SCH() : base()
        {
            Filler = 295;
            DotTick = 70;
            DotHit = 0;
            Cycle = 30;

            JobMod = 115;

            statToMinimise = SubStatEnum.Piety;
            hasStatToMinimise = true;

            optimalGCD.Add(2.5m);
            optimalGCD.Add(2.4m);
            optimalGCD.Add(2.31m);
            optimalGCD.Add(2.3m);
            uselessStatsOnFood.Add(SubStatEnum.SkillSpeed);
            uselessStatsOnFood.Add(SubStatEnum.Tenacity);
            uselessStatsOnFood.Add(SubStatEnum.Piety);

            speedStat = SubStatEnum.SpellSpeed;
            Name = "SCH";
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

            decimal B4Potency = (Filler * FillerCasts);
            decimal BioPotency = DotHit + (DotTick * ticks * scalar);
            potency = (B4Potency + BioPotency) / cycleLength;

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
