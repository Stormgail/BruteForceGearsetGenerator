using System.Collections.Generic;
using BruteForceGearsetGenerator.Utilities;

namespace BruteForceGearsetGenerator.XivApiFoodClasses
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

    public class Bonus
    {
        public int ID { get; set; }
        public int Max { get; set; }
        public int MaxHQ { get; set; }
        public bool Relative { get; set; }
        public int Value { get; set; }
        public int ValueHQ { get; set; }
    }


    public class Result
    {
        public Dictionary<StatEnum,Bonus> Bonuses { get; set; }
        public int ID { get; set; }
        public string Name_en { get; set; }
    }

    public class FoodSet
    {
        public Pagination Pagination { get; set; }
        public IList<Result> Results { get; set; }
        public int SpeedMs { get; set; }
    }

}
