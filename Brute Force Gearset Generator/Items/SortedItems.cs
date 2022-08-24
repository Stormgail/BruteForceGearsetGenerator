using System.Collections.Generic;
using System.Linq;
using System;
using BruteForceGearsetGenerator.Utilities;
using BruteForceGearsetGenerator.Class;

namespace BruteForceGearsetGenerator.Items
{
    class SortedItems
    {
        public List<ItemWithStats> HeadSlot { get; set; }
        public List<ItemWithStats> BodySlot { get; set; }
        public List<ItemWithStats> GlovesSlot { get; set; }
        public List<ItemWithStats> PantsSlot { get; set; }
        public List<ItemWithStats> BootsSlot { get; set; }
        public List<ItemWithStats> EarringSlot { get; set; }
        public List<ItemWithStats> NeckSlot { get; set; }
        public List<ItemWithStats> WristSlot { get; set; }
        public List<List<ItemWithStats>> RingPair { get; set; }
        public List<ItemWithStats> WeaponSlot { get; set; }

        public XIVJob Job;

        private ConfigSettings Config;

        public SortedItems(IEnumerable<ItemWithStats> unsorted, XIVJob job, ConfigSettings config)
        {
            Job = job;
            Config = config;
            HeadSlot = filterBySlot(unsorted, GearSlotEnum.Head);
            BodySlot = filterBySlot(unsorted, GearSlotEnum.Body);
            GlovesSlot = filterBySlot(unsorted, GearSlotEnum.Gloves);
            PantsSlot = filterBySlot(unsorted, GearSlotEnum.Pants);
            BootsSlot = filterBySlot(unsorted, GearSlotEnum.Boots);
            EarringSlot = filterBySlot(unsorted, GearSlotEnum.Earring);
            NeckSlot = filterBySlot(unsorted, GearSlotEnum.Neck);
            WristSlot = filterBySlot(unsorted, GearSlotEnum.Wrist);
            RingPair = ringSets(unsorted);
            WeaponSlot = filterBySlot(unsorted, GearSlotEnum.Weapon);
        }

        List<ItemWithStats> filterBySlot(IEnumerable<ItemWithStats> unsorted, GearSlotEnum slot)
        {
            List<ItemWithStats> sorted = unsorted.Where(i => i.Slot == slot).ToList();
            if (slot != GearSlotEnum.Weapon && Job.hasStatToMinimise)
            {
                int minStat = sorted.Min(i => i.SubStats[Job.statToMinimise]);
                sorted.RemoveAll(i => i.SubStats[Job.statToMinimise] > minStat);
            }

            if (Config.LimitTomes)
            {
                sorted.RemoveAll(i => i.TomeCost() >= Config.TomeLimit);
            }

            return sorted;
        }

        List<List<ItemWithStats>> ringSets(IEnumerable<ItemWithStats> set)
        {
            List<List<ItemWithStats>> pairs = new();
            List<IEnumerable<ItemWithStats>> prePairs = new();
            GearSlotEnum slot = GearSlotEnum.Ring;
            List<ItemWithStats> sorted = set.Where(i => i.Slot == slot).ToList();

            if (Job.hasStatToMinimise)
            {
                int maxStat = sorted.Max(i => i.SubStats[Job.statToMinimise]);
                if(sorted.Count(r => r.SubStats[Job.statToMinimise]< maxStat) >= 2)
                {
                    sorted.RemoveAll(i => i.SubStats[Job.statToMinimise] == maxStat);
                }
            }

            prePairs.AddRange(UsefulFunctions.GetPermutationsWithoutRept(sorted, 2));
            prePairs.RemoveAll(p => p.Any(r => r.IsUnique) && p.Count(r => p.First().Name == r.Name) >= 2);

            foreach (IEnumerable<ItemWithStats> pair in prePairs)
            {
                int idOne = pair.ElementAt(0).ID;
                int idTwo = pair.ElementAt(1).ID;

                if(!pairs.Any(p=> p.Count(pOne => pOne.ID == idOne)>0 && p.Count(pTwo => pTwo.ID == idTwo) > 0))
                {
                    pairs.Add(pair.ToList());
                }
            }

            return pairs;
        }

        public int PermCount()
        {
            return HeadSlot.Count * BodySlot.Count * GlovesSlot.Count * PantsSlot.Count * BootsSlot.Count * EarringSlot.Count * NeckSlot.Count * GlovesSlot.Count * WristSlot.Count * RingPair.Count * WeaponSlot.Count;
        }
    }


}




