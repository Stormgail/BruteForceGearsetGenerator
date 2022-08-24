
using System.Collections.Generic;
using BruteForceGearsetGenerator.Utilities;

namespace BruteForceGearsetGenerator.XivApiItemClasses
{
    public class Pagination
    {
        public int Page { get; set; }
        public object PageNext { get; set; }
        public object PagePrev { get; set; }
        public int PageTotal { get; set; }
        public int Results { get; set; }
        public int ResultsPerPage { get; set; }
        public int ResultsTotal { get; set; }
    }

    public class Stat
    {
        public int ID { get; set; }

        public int NQ { get; set; }

        public int? HQ { get; set; }
    }

    public class Result
    {
        public int BaseParamValue0 { get; set; }
        public int BaseParamValue2 { get; set; }
        public int BaseParamValue3 { get; set; }
        public int BaseParam0TargetID { get; set; }
        public int BaseParam2TargetID { get; set; }
        public int BaseParam3TargetID { get; set; }
        public int ID { get; set; }
        public int IsAdvancedMeldingPermitted { get; set; }
        public int MateriaSlotCount { get; set; }
        public int IsUnique { get; set; }
        public int EquipSlotCategoryTargetID { get; set; }
        public string Name_en { get; set; }

        public int? damageMag { get; set; }

        public Dictionary<StatEnum, Stat> Stats { get; set; }
    }

    public class ItemSet
    {
        public Pagination Pagination { get; set; }
        public IList<Result> Results { get; set; }
        public int SpeedMs { get; set; }
    }
}
