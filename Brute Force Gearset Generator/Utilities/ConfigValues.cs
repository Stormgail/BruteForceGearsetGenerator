using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteForceGearsetGenerator.Utilities
{
    public class ConfigValues
    {
        public int BigMateriaValue { get; set; }
        public int SmallMateriaValue { get; set; }
        public int MinIlvl { get; set; }
        public int MaxIlvl { get; set; }
        public int FoodIlvl { get; set; }
        public void SetReferenceValues()
        {
            ReferenceValues.BigMateriaValue = BigMateriaValue;
            ReferenceValues.SmallMateriaValue = SmallMateriaValue;
            ReferenceValues.MinIlvl = MinIlvl;
            ReferenceValues.MaxIlvl = MaxIlvl;
            ReferenceValues.FoodIlvl = FoodIlvl;
        }
    }
}
