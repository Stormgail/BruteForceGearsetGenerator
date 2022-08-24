using System;
using System.Collections.Generic;
using BruteForceGearsetGenerator.Utilities;
using BruteForceGearsetGenerator.Class;
using System.IO;

namespace BruteForceGearsetGenerator.Items
{
    public class GearSet : HasMateria
    {
        public List<ItemWithStats> Items { get; protected set; }
        public decimal DPS { get; set; }
        public XIVJob Job { get; private set; }
        private Dictionary<SubStatEnum, int> baseStats;
        public Food Food;

        public int tomeCost;


        public GearSet(XIVJob c) : base(0, 0, 0)
        {
            InitialiseValues(c);
        }

        private void InitialiseValues(XIVJob c)
        {
            SetBaseStats(c);
            tomeCost = 0;
        }

        public GearSet(GearSet g) : base(0, 0, 0)
        {
            InitialiseValues(g.Job);
            Food = g.Food;
            WeaponDamage = g.WeaponDamage;
            foreach (ItemWithStats i in g.Items)
            {
                AddItemClone(i);
            }
        }

        protected void SetBaseStats(XIVJob c)
        {
            Job = c;
            Items = new();
            baseStats = new();
            this.MainStat = 451;
            SetBaseStat(SubStatEnum.Piety, 390);
            SetBaseStat(SubStatEnum.DirectHitRate, 400);
            SetBaseStat(SubStatEnum.CriticalHit, 400);
            SetBaseStat(SubStatEnum.Determination, 390);
            SetBaseStat(SubStatEnum.SpellSpeed, 400);
            SetBaseStat(SubStatEnum.SkillSpeed, 400);
            SetBaseStat(SubStatEnum.Tenacity, 400);
        }

        public void SetBaseStat(SubStatEnum type, int value)
        {

            if (!baseStats.ContainsKey(type))
            {
                baseStats.Add(type, 0);
            }


            baseStats[type] = value;
        }

        public override void SetStat(SubStatEnum type, int value)
        {
            int modifiedValue = value;

            if (baseStats.ContainsKey(type))
            {
                modifiedValue = Math.Max(0, value - baseStats[type]);
            }
            base.SetStat(type, modifiedValue);
        }

        public void GetDPS()
        {
            DPS = Job.CalcDamage(this);
        }

        public int GetWD()
        {
            return this.WeaponDamage ?? 0;
        }

        public void AddItemClone(ItemWithStats i)
        {
            Items.Add(new(i));
            StatCap += i.StatCap;
            AddStats(i.SubStats);
            MainStat += i.MainStat;
            BigSlots += i.BigSlots;
            SmallSlots += i.SmallSlots;
            MaxMeldSlots += i.MaxMeldSlots;

            this.tomeCost += i.TomeCost();
            HasStats.AddToSubstatDictionary(MaxStats, i.MaxStats);
            HasStats.AddToSubstatDictionary(MaxBigMelds, i.MaxBigMelds);
            HasStats.AddToSubstatDictionary(MaxSmallMelds, i.MaxSmallMelds);

            if (i.Slot == GearSlotEnum.Weapon)
            {
                WeaponDamage = i.WeaponDamage;
            }
        }

        public void AddItemClones(IEnumerable<ItemWithStats> iList)
        {
            foreach(ItemWithStats i in iList)
            {
                AddItemClone(i);
            }
        }

        public int TotalSubStat(SubStatEnum stat)
        {
            int total = SubStatValue(stat) + BaseSubStatValue(stat);
            if (Food != null)
            {
                total += Food.ReturnFoodBonus(stat, total);
            }
            return total;
        }

        public int TotalSubStatFromGear(SubStatEnum stat)
        {
            int total = SubStatValue(stat);
            return total;
        }

        public int BaseSubStatValue(SubStatEnum stat)
        {
            return baseStats[stat];
        }

        public decimal GCD()
        {
            decimal speed = TotalSubStat(Job.speedStat);
            decimal GCD = Math.Round((100) * ((2500 * (1000 - Math.Round((130 * (speed - 400) / 1900), 0, MidpointRounding.ToZero)) / 1000) / 1000), 0, MidpointRounding.ToZero) / 100;
            return GCD;
        }

        public void ListSetToFile(string Path)
        {
            List<string> lines = new();

            lines.Add("Gear:");
            foreach (ItemWithStats i in Items)
            {
                lines.Add(i.Name);
            }
            lines.Add("Materia:");

            foreach (KeyValuePair<SubStatEnum,int> kvp in BigMelds)
            {
                lines.Add(kvp.Key + "+" + ReferenceValues.BigMateriaValue + " x " + kvp.Value);
            }

            foreach (KeyValuePair<SubStatEnum, int> kvp in SmallMelds)
            {
                lines.Add(kvp.Key + "+" + ReferenceValues.SmallMateriaValue + " x " + kvp.Value);
            }

            lines.Add("Food:");

            if (Food != null)
            {
                lines.Add(Food.Name);
            }
            lines.Add("Stats:");
            lines.Add("DPS: " + DPS.ToString());
            lines.Add("GCD: " + GCD().ToString());
            lines.Add("Main Stat: " + MainStat.ToString());
            lines.Add(SubStatEnum.DirectHitRate.ToString() + " : " + TotalSubStat(SubStatEnum.DirectHitRate));
            lines.Add(SubStatEnum.CriticalHit.ToString() + " : " + TotalSubStat(SubStatEnum.CriticalHit));
            lines.Add(SubStatEnum.Determination.ToString() + " : " + TotalSubStat(SubStatEnum.Determination));
            if(TotalSubStat(SubStatEnum.Piety) > BaseSubStatValue(SubStatEnum.Piety))
            {
                lines.Add(SubStatEnum.Piety.ToString() + " : " + TotalSubStat(SubStatEnum.Piety));
            }
            if (TotalSubStat(SubStatEnum.Tenacity) > BaseSubStatValue(SubStatEnum.Tenacity))
            {
                lines.Add(SubStatEnum.Tenacity.ToString() + " : " + TotalSubStat(SubStatEnum.Tenacity));
            }
            lines.Add(Job.speedStat.ToString() + " : " + TotalSubStat(Job.speedStat));

            lines.Add("-------------------------------------------------");

            File.AppendAllLines(Path,lines);
        }





    }
}
