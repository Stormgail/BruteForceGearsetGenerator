using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteForceGearsetGenerator.Utilities
{
    public class ConfigSettings
    {
        public bool GetNewData { get; set; }
        public int MaximumThreads { get; set; }

        public bool LimitTomes { get; set; }
        public int TomeLimit { get; set; }
    }
}
