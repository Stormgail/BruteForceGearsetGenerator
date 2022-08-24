using System;
using BruteForceGearsetGenerator.Utilities;
using System.Collections.Generic;
using BruteForceGearsetGenerator.XivApiItemClasses;

namespace BruteForceGearsetGenerator.Items
{
    public class ItemWithStats : HasMateria
    {

        public bool IsUnique { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
        public GearSlotEnum Slot { get; set; }

        public ItemWithStats(Result item) : base(item.MateriaSlotCount, SlotCount(item), CalculateStatCap(item))
        {
            IsUnique = item.IsUnique == 1;
            Name = item.Name_en;
            ID = item.ID;
            Slot = (GearSlotEnum)item.EquipSlotCategoryTargetID;
            if (Slot == GearSlotEnum.Weapon)
            {
                WeaponDamage = item.damageMag;
            }
            foreach (KeyValuePair<StatEnum, Stat> kvp in item.Stats)
            {
                if (kvp.Value.ID == item.BaseParam2TargetID)
                {
                    AddStat((SubStatEnum)item.BaseParam2TargetID, kvp.Value.HQ ?? kvp.Value.NQ);
                }
                else if (kvp.Value.ID == item.BaseParam3TargetID)
                {
                    AddStat((SubStatEnum)item.BaseParam3TargetID, kvp.Value.HQ ?? kvp.Value.NQ);
                }
                else if (kvp.Value.ID == item.BaseParam0TargetID)
                {
                    MainStat = kvp.Value.HQ ?? kvp.Value.NQ;
                }
            }
            FindStatMaximums();
            FindMaxBigMelds();
            FindMaxSmallMelds();

        }

        public ItemWithStats(ItemWithStats copy) : base(copy)
        {
            IsUnique = copy.IsUnique;
            Name = copy.Name;
            Slot = copy.Slot;
            ID = copy.ID;
        }

        private static int CalculateStatCap(Result i)
        {
            int cap = i.BaseParamValue2;
            foreach (KeyValuePair<StatEnum, Stat> kvp in i.Stats)
            {
                if (kvp.Value.ID == i.BaseParam2TargetID)
                {
                    cap = kvp.Value.HQ ?? kvp.Value.NQ;
                }
            }
            return cap;
        }


        private static int SlotCount(Result i)
        {
            if (i.IsAdvancedMeldingPermitted == 1)
            {
                return 5;
            }
            else
            {
                return i.MateriaSlotCount;
            }
        }

        public void FindStatMaximums()
        {
            foreach (SubStatEnum stat in Enum.GetValues(typeof(SubStatEnum)))
            {
                ItemWithStats copy = new(this);
                copy.FillMateria(stat);
                MaxStats.Add(stat, copy.SubStatValue(stat));
            }
        }

        public void FindMaxBigMelds()
        {
            foreach (SubStatEnum stat in Enum.GetValues(typeof(SubStatEnum)))
            {
                int count = 0;
                ItemWithStats copy = new(this);
                for (int i = 0; i < copy.BigSlots; i++)
                {
                    int before = copy.SubStatValue(stat);
                    copy.AddStat(stat, ReferenceValues.BigMateriaValue);
                    int diff = copy.SubStatValue(stat) - before;
                    if (diff / ReferenceValues.BigMateriaValue > 0.5)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                MaxBigMelds.Add(stat, count);
            }
        }

        public void FindMaxSmallMelds()
        {
            foreach (SubStatEnum stat in Enum.GetValues(typeof(SubStatEnum)))
            {
                int count = 0;
                ItemWithStats copy = new(this);
                for (int i = 0; i < copy.SmallSlots; i++)
                {
                    int before = copy.SubStatValue(stat);
                    copy.AddStat(stat, ReferenceValues.SmallMateriaValue);
                    int diff = copy.SubStatValue(stat) - before;
                    if (diff / ReferenceValues.SmallMateriaValue > 0.5)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                MaxSmallMelds.Add(stat, count);
            }
        }

        public int TomeCost()
        {
            List<GearSlotEnum> accessories = new List<GearSlotEnum>();
            accessories.Add(GearSlotEnum.Earring);
            if (!IsTome())
            {
                return 0;
            }
            else if (Slot == GearSlotEnum.Weapon)
            {
                return 1000;
            }
            else if (Slot == GearSlotEnum.Body || Slot == GearSlotEnum.Pants)
            {
                return 825;
            }
            else if (Slot >= GearSlotEnum.Earring && Slot <= GearSlotEnum.Ring)
            {
                return 375;
            }
            else
            {
                return 495;
            }
        }

        public bool IsTome()
        {
            if (Name.Contains("Lunar"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}




