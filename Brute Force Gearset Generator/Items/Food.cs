using System;
using System.Collections.Generic;
using BruteForceGearsetGenerator.Utilities;
using BruteForceGearsetGenerator.XivApiFoodClasses;

namespace BruteForceGearsetGenerator.Items
{
    public class Food
    {
        Dictionary<SubStatEnum,int> statCaps;

        public String Name;

        public Food()
        {
            statCaps = new();
        }

        public Food(Result r)
        {
            statCaps = new();
            foreach (KeyValuePair<StatEnum,Bonus> kvp in r.Bonuses)
            {
                if (Enum.IsDefined(typeof(SubStatEnum), (int)kvp.Key))
                {
                    statCaps.Add((SubStatEnum)kvp.Key, kvp.Value.MaxHQ);
                }
            }
            Name = r.Name_en;
        }

        public void AddFoodEffect(SubStatEnum s, int cap)
        {
            statCaps.Add(s,cap);
        }

        public bool buffsSubStat(SubStatEnum s)
        {
            return statCaps.ContainsKey(s);
        }

        public int ReturnFoodBonus(SubStatEnum s, int baseValue)
        {
            int bonus = 0;
            if (statCaps.ContainsKey(s))
            {
                bonus = (int)Math.Floor(Math.Min(statCaps[s], baseValue * 0.1));
            }
            return bonus;
        }
    }
}
