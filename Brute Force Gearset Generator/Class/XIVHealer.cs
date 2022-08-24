using System;
using System.Collections.Generic;
using BruteForceGearsetGenerator.Items;
using BruteForceGearsetGenerator.Utilities;

namespace BruteForceGearsetGenerator.Class
{
    abstract class XIVHealer : XIVJob
    {
        protected int Cycle = 30;
        protected int Filler;
        protected int DotTick;
        protected int DotHit;


        protected decimal FillerPerCycle(decimal GCD)
        {
            return (Cycle / GCD) - 1;
        }

        protected override decimal CalcPotency(GearSet g)
        {
            decimal clipPotency = ClipPotency(g);
            decimal driftPotency = DriftPotency(g);

            if (clipPotency >= driftPotency)
            {
                return clipPotency;
            }
            else
            {
                return driftPotency;
            }
        }

        protected abstract decimal ClipPotency(GearSet g);

        protected abstract decimal DriftPotency(GearSet g);
    }
}
