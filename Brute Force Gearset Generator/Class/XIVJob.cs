using System;
using System.Collections.Generic;
using BruteForceGearsetGenerator.Items;
using BruteForceGearsetGenerator.Utilities;
using System.Linq;

namespace BruteForceGearsetGenerator.Class
{
    public abstract class XIVJob
    {

        protected decimal battleVoiceAvg = (15 / (decimal)120) * (decimal)0.2;
        protected decimal battleLitanyAvg = (15 / (decimal)120) * (decimal)0.1;
        protected decimal chainStratAvg = (15 / (decimal)120) * (decimal)0.1;
        protected decimal devilmentAvg = (20 / (decimal)120) * (decimal)0.2;
        protected decimal brdCritAvg = (45 / (decimal)120) * (decimal)0.02;
        protected decimal brdDhAvg = (45 / (decimal)120) * (decimal)0.03;

        public SubStatEnum statToMinimise;
        public bool hasStatToMinimise;

        public List<SubStatEnum> uselessStatsOnFood;


        public SubStatEnum speedStat;

        public string Name { get; protected set; }

        public List<decimal> optimalGCD;

        protected decimal levelMod = 1900;
        protected decimal baseMain = 390;
        protected decimal baseSub = 400;
        protected decimal JobMod;
        protected decimal magicAndMend = 1.3M;

        public XIVJob()
        {
            optimalGCD = new();
            uselessStatsOnFood = new();
        }


        public decimal CalcDamage(GearSet g)
        {
            decimal Potency = CalcPotency(g);
            int WD = g.WeaponDamage ?? 0;
            decimal MainStat = g.MainStat;
            int Det = g.TotalSubStat(SubStatEnum.Determination);
            int Crit = g.TotalSubStat(SubStatEnum.CriticalHit);
            int DH = g.TotalSubStat(SubStatEnum.DirectHitRate);
            int SS = 400;
            int TEN = g.TotalSubStat(SubStatEnum.Tenacity);


            bool hasBrd = true;
            bool hasDrg = true;
            bool hasSch = true;
            bool hasDnc = false;

            int classNum = 5;
            MainStat = (Math.Floor(MainStat * (1 + (decimal)0.01 * classNum)));
            decimal Damage = Math.Floor(Potency * (WD + Math.Floor(baseMain * JobMod / 1000)) * (100 + Math.Floor((MainStat - baseMain) * 195 / baseMain)) / 100);
            Damage = (Math.Floor(Damage * (1000 + Math.Floor(140 * (Det - baseMain) / levelMod)) / 1000));
            Damage = Math.Floor(Damage * (1000 + Math.Floor(100 * (TEN - baseSub) / levelMod)) / 1000);
            Damage = Math.Floor(Damage * (1000 + Math.Floor(130 * (SS - baseSub) / levelMod)) / 1000 / 100);
            Damage = Math.Floor(Damage * magicAndMend);
            decimal CritDamage = CalcCritDamage(Crit);
            decimal CritRate = CalcCritRate(Crit) + (hasDrg ? battleLitanyAvg : 0) + (hasSch ? chainStratAvg : 0) + (hasDnc ? devilmentAvg : 0) + (hasBrd ? brdCritAvg : 0);
            var DHRate = CalcDHRate(DH) + (hasBrd ? battleVoiceAvg + brdDhAvg : 0) + (hasDnc ? devilmentAvg : 0);
            Damage = Damage * ((1 + (DHRate / 4)) * (1 + (CritRate * CritDamage)));
            return Math.Round(Damage, 2);
        }

        protected decimal CalcCritRate(decimal Crit)
        {
            decimal crit = Math.Floor((200 * (Crit - baseSub) / levelMod + 50)) / 1000;
            return crit;
        }

        protected decimal CalcCritDamage(decimal Crit)
        {
            decimal cdamage = (Math.Floor(200 * (Crit - baseSub) / levelMod + 400)) / 1000;
            return cdamage;
        }

        protected decimal CalcDHRate(decimal DH)
        {
            decimal dhRate = Math.Floor(550 * (DH - baseSub) / levelMod) / 1000;
            return dhRate;
        }

        protected decimal CalcDetDamage(decimal Det)
        {
            decimal dd = (1000 + Math.Floor(140 * (Det - baseMain) / levelMod)) / 1000;
            return dd;
        }

        protected decimal CalcPiety(decimal Pie)
        {
            return 200 + (Math.Floor(150 * (Pie - baseMain) / levelMod));
        }

        protected static decimal SpsScalar(decimal SpS)
        {
            var S = ((1000 + Math.Floor(130 * (SpS - 400) / 1900)) / 1000);
            return S;
        }

        public bool IsOptimalGCD(decimal gcd)
        {
            if(optimalGCD.Count == 0 || optimalGCD.Any(oGCD => oGCD == gcd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract decimal CalcPotency(GearSet g);
    }


}
