using System;
using System.Collections.Generic;

namespace BruteForceGearsetGenerator.Utilities
{
    public abstract class HasMateria : HasStats
    {

        public HasMateria(int guarenteedSlots, int maxMeldSlots, int cap) : base(cap)
        {
            Initialise(guarenteedSlots, maxMeldSlots);
        }

        public HasMateria(HasMateria copy) : base(copy)
        {
            Initialise(copy.GuarenteedMeldSlots, copy.MaxMeldSlots);
            BigMelds = new(copy.BigMelds);
            SmallMelds = new(copy.SmallMelds);
            MaxBigMelds = new(copy.MaxBigMelds);
            MaxSmallMelds = new(copy.MaxSmallMelds);
            MaxStats = new(copy.MaxStats);
        }

        private void Initialise(int guarenteedSlots, int maxMeldSlots)
        {
            GuarenteedMeldSlots = guarenteedSlots;
            MaxMeldSlots = maxMeldSlots;

            BigSlots = Math.Min(maxMeldSlots, guarenteedSlots + 1);
            SmallSlots = Math.Max(maxMeldSlots - BigSlots, 0);

            BigMelds = new();
            SmallMelds = new();
            MaxBigMelds = new();
            MaxSmallMelds = new();
            MaxStats = new();
        }

        public Dictionary<SubStatEnum, int> BigMelds { get; protected set; }
        public Dictionary<SubStatEnum, int> SmallMelds { get; protected set; }
        public Dictionary<SubStatEnum, int> MaxBigMelds { get; protected set; }
        public Dictionary<SubStatEnum, int> MaxSmallMelds { get; protected set; }
        public Dictionary<SubStatEnum, int> MaxStats { get; protected set; }

        public int GuarenteedMeldSlots { get; protected set; }

        public int BigSlots { get; protected set; }

        public int SmallSlots { get; protected set; }
        public int MaxMeldSlots { get; protected set; }

        public void FillMateria(SubStatEnum s)
        {
            AddSmallMateria(s, SmallSlots);
            AddBigMateria(s, BigSlots);
        }

        public void AddSmallMateria(SubStatEnum stat)
        {
            AddStat(stat, ReferenceValues.SmallMateriaValue);
            RecordSmallMateria(stat);
        }

        public void AddSmallMateria(SubStatEnum stat, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddSmallMateria(stat);
            }
        }

        public void AddBigMateria(SubStatEnum stat)
        {
            AddStat(stat, ReferenceValues.BigMateriaValue);
            RecordBigMateria(stat);
        }

        public void AddBigMateria(SubStatEnum stat, int amount)
        {
            for(int i = 0; i<amount; i++)
            {
                AddBigMateria(stat);
            }
        }

        public void RecordBigMateria(SubStatEnum stat)
        {

            if (!BigMelds.ContainsKey(stat))
            {
                BigMelds.Add(stat, 0);
            }

            BigMelds[stat] += 1;
        }

        public void RecordSmallMateria(SubStatEnum stat)
        {

            if (!SmallMelds.ContainsKey(stat))
            {
                SmallMelds.Add(stat, 0);
            }

            SmallMelds[stat] += 1;
        }


    }
}
