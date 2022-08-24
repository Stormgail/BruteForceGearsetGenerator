using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteForceGearsetGenerator.Utilities
{
    public abstract class HasStats
    {

        public int StatCap { get; set; }
        public int MainStat { get; set; }


        public int? WeaponDamage { get; set; }

        public Dictionary<SubStatEnum, int> SubStats { get; protected set; }

        public HasStats(int cap)
        {
            StatCap = cap;
            SubStats = new Dictionary<SubStatEnum, int>();
            foreach (SubStatEnum stat in Enum.GetValues(typeof(SubStatEnum)))
            {
                AddStat(stat, 0);
            }
        }

        public HasStats(HasStats copy)
        {
            StatCap = copy.StatCap;
            WeaponDamage = copy.WeaponDamage;           
            MainStat = copy.MainStat;

            SubStats = new(copy.SubStats);
        }

        public bool HasSameStats(HasStats s)
        {
            return !SubStats.Any(sS => s.SubStatValue(sS.Key) != sS.Value);
        }


        public int SubStatValue(SubStatEnum stat)
        {
            return SubStats[stat];
        }

        public virtual void SetStat(SubStatEnum type, int value)
        {
            if (!SubStats.ContainsKey(type))
            {
                SubStats.Add(type, 0);
            }

            SubStats[type] = value;
        }

        public void AddStat(SubStatEnum type, int value)
        {

            if (!SubStats.ContainsKey(type))
            {
                SubStats.Add(type, 0);
            }
            int currentValue = SubStats.GetValueOrDefault(type);
            int addedValue = currentValue + value;

            SubStats[type] = Math.Min(addedValue, StatCap);
        }

        public void AddStats(Dictionary<SubStatEnum, int> stats)
        {
            foreach (KeyValuePair<SubStatEnum, int> pair in stats)
            {
                AddStat(pair.Key, pair.Value);
            }
        }

        protected static void AddToSubstatDictionary(Dictionary<SubStatEnum, int> toAdd, Dictionary<SubStatEnum, int> from)
        {
            foreach (KeyValuePair<SubStatEnum, int> kvp in from)
            {
                if (toAdd.ContainsKey(kvp.Key))
                {
                    toAdd[kvp.Key] += kvp.Value;
                }
                else
                {
                    toAdd.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
