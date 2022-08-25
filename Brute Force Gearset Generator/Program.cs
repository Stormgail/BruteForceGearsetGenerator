using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

using Microsoft.Extensions.Configuration;

using Flurl;
using Flurl.Http;

using System.Text.Json;

using BruteForceGearsetGenerator.Utilities;
using BruteForceGearsetGenerator.XivApiItemClasses;
using BruteForceGearsetGenerator.XivApiFoodClasses;
using BruteForceGearsetGenerator.Items;
using BruteForceGearsetGenerator.Class;

namespace BruteForceGearsetGenerator
{
    class Program
    {
        static ConfigSettings configSettings;

        static List<XivApiItemClasses.Result> itemResults;
        static List<XivApiFoodClasses.Result> foodResults;


        static SortedItems parsedSet;
        static List<Food> food;
        static XIVJob Job = new SCH();
        static List<GearSet> bestSets = new();

        static readonly string ItemFilePath = Path.Combine(Directory.GetCurrentDirectory().ToString(), "gear.json");
        static readonly string FoodFilePath = Path.Combine(Directory.GetCurrentDirectory().ToString(), "food.json");

        static string outputFilePath;

        static bool processing;
        static async Task Main(string[] args)
        {
            bool startupSuccess = Startup();
            if (startupSuccess)
            {
                if (configSettings.GetNewData)
                {
                    await DownloadGearData();
                }
                ReadFiles();
                BuildItemList();
                BuildFoodList();
                //ScholarDebugSets();
                await BruteForceGearSetsAync();
                outputFilePath = Path.Combine(Directory.GetCurrentDirectory().ToString(), Job.Name + ".txt");
                OutputToFile();

            }
        }

        public static void OutputToFile()
        {
            File.WriteAllText(outputFilePath, "");
            foreach (GearSet s in bestSets.OrderByDescending(set => set.DPS))
            {
                s.ListSetToFile(outputFilePath);
            }
        }

        public static void ScholarDebugSets()
        {
            GearSet g;
            g = new(Job);
            g.StatCap = 2277;
            g.MainStat = 2560;
            g.SetStat(SubStatEnum.DirectHitRate, 436);
            g.SetStat(SubStatEnum.CriticalHit, 1989);
            g.SetStat(SubStatEnum.Determination, 1951);
            g.SetStat(g.Job.speedStat, 1412);
            g.WeaponDamage = 120;
            g.GetDPS();
            Console.WriteLine(g.MainStat);
            Console.WriteLine(g.TotalSubStat(SubStatEnum.DirectHitRate));
            Console.WriteLine(g.TotalSubStat(SubStatEnum.CriticalHit));
            Console.WriteLine(g.TotalSubStat(SubStatEnum.Determination));
            Console.WriteLine(g.TotalSubStat(g.Job.speedStat));
            Console.WriteLine(g.GCD());
            Console.WriteLine(g.DPS);

            g = new(Job);
            g.StatCap = 2277;
            g.MainStat = 2560;
            g.SetStat(SubStatEnum.DirectHitRate, 616);
            g.SetStat(SubStatEnum.CriticalHit, 2277);
            g.SetStat(SubStatEnum.Determination, 1951);
            g.SetStat(g.Job.speedStat, 944);
            g.WeaponDamage = 120;
            g.GetDPS();
            Console.WriteLine(g.MainStat);
            Console.WriteLine(g.TotalSubStat(SubStatEnum.DirectHitRate));
            Console.WriteLine(g.TotalSubStat(SubStatEnum.CriticalHit));
            Console.WriteLine(g.TotalSubStat(SubStatEnum.Determination));
            Console.WriteLine(g.TotalSubStat(g.Job.speedStat));
            Console.WriteLine(g.GCD());
            Console.WriteLine(g.DPS);


        }

        static bool Startup()
        {
            try
            {
                var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("config.json", optional: false);

                IConfiguration config = builder.Build();

                ConfigValues v = config.GetSection("Values").Get<ConfigValues>();
                v.SetReferenceValues();

                configSettings = config.GetSection("Settings").Get<ConfigSettings>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        static async Task<bool> DownloadGearData()
        {
            string columns = "&columns=ID,IsAdvancedMeldingPermitted,Name_en,MateriaSlotCount,BaseParamValue2,BaseParamValue0,BaseParamValue3,BaseParam0TargetID,BaseParam2TargetID,BaseParam3TargetID,IsUnique,EquipSlotCategoryTargetID,Stats,DamageMag";
            string baseGetBestURL = "https://xivapi.com/search?indexes=Item&filters=ClassJobCategory.<JOBNAME>=1,LevelItem%3E=<MINLVL>,LevelItem%3C=<MAXLVL>";
            baseGetBestURL = baseGetBestURL.Replace("<JOBNAME>", Job.Name);
            baseGetBestURL = baseGetBestURL.Replace("<MINLVL>", ReferenceValues.MinIlvl.ToString());
            baseGetBestURL = baseGetBestURL.Replace("<MAXLVL>", ReferenceValues.MaxIlvl.ToString());

            string baseGetRingURL = "https://xivapi.com/search?indexes=Item&string=Rinascita&filters=ClassJobCategory.<JOBNAME>=1,EquipSlotCategoryTargetID=12";
            baseGetRingURL = baseGetRingURL.Replace("<JOBNAME>", Job.Name);

            string baseGetFoodURL = "https://xivapi.com/search?indexes=Item&filters=LevelItem=<FOODILVL>,ItemKind.ID=5,ItemSearchCategory.ID=45,Bonuses.Vitality.ID=3&columns=ID,Bonuses,Name_en";
            baseGetFoodURL = baseGetFoodURL.Replace("<FOODILVL>", ReferenceValues.FoodIlvl.ToString());

            Url getBestUrl = new(baseGetBestURL + columns);
            Url getRingUrl = new(baseGetRingURL + columns);
            Url getFoodUrl = new(baseGetFoodURL);
            List<XivApiItemClasses.Result> Results = new();
            List<XivApiFoodClasses.Result> foodResults = new();
            try
            {
                Results.AddRange((await getBestUrl.GetJsonAsync<ItemSet>()).Results);
                System.Threading.Thread.Sleep(1000);
                Results.AddRange((await getRingUrl.GetJsonAsync<ItemSet>()).Results);
                System.Threading.Thread.Sleep(1000);
                foodResults.AddRange((await getFoodUrl.GetJsonAsync<FoodSet>()).Results);

                string itemJSon = JsonSerializer.Serialize(Results);
                File.WriteAllText(ItemFilePath, itemJSon);

                string foodJSon = JsonSerializer.Serialize(foodResults);
                File.WriteAllText(FoodFilePath, foodJSon);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static void ReadFiles()
        {
            string itemReadPath = ItemFilePath;
            string itemFileData = File.ReadAllText(itemReadPath);
            itemResults = new List<XivApiItemClasses.Result>(JsonSerializer.Deserialize<IList<XivApiItemClasses.Result>>(itemFileData));

            string foodReadPath = FoodFilePath;
            string foodFileData = File.ReadAllText(foodReadPath);
            foodResults = new List<XivApiFoodClasses.Result>(JsonSerializer.Deserialize<IList<XivApiFoodClasses.Result>>(foodFileData));
        }

        static void BuildItemList()
        {
            List<ItemWithStats> items = new();
            foreach (XivApiItemClasses.Result r in itemResults)
            {
                ItemWithStats i = new(r);
                items.Add(i);
            }

            parsedSet = new(items, Job, configSettings);
        }

        static void BuildFoodList()
        {
            food = new();

            foreach (XivApiFoodClasses.Result r in foodResults)
            {

                Food f = new(r);
                if (!Job.uselessStatsOnFood.Any(us => f.buffsSubStat(us)))
                {
                    food.Add(f);
                }
            }
        }


        static async Task<bool> BruteForceGearSetsAync()
        {
            int permCount = parsedSet.PermCount() * food.Count();
            Console.WriteLine("Total: " + permCount);

            List<Task> materiaTasks = new();
            ConcurrentQueue<GearSet> sets = new();
            Task deQueue = Task.Run(() => AddToBestFromQueue(sets));
            processing = true;

            foreach (ItemWithStats weaponItem in parsedSet.WeaponSlot)
            {
                foreach (ItemWithStats headItem in parsedSet.HeadSlot)
                {
                    foreach (ItemWithStats bodyItem in parsedSet.BodySlot)
                    {
                        foreach (ItemWithStats glovesItem in parsedSet.GlovesSlot)
                        {
                            foreach (ItemWithStats pantsItem in parsedSet.PantsSlot)
                            {
                                foreach (ItemWithStats bootsItem in parsedSet.BootsSlot)
                                {
                                    foreach (ItemWithStats earringItem in parsedSet.EarringSlot)
                                    {
                                        foreach (ItemWithStats neckItem in parsedSet.NeckSlot)
                                        {
                                            foreach (ItemWithStats wristItem in parsedSet.WristSlot)
                                            {
                                                foreach (List<ItemWithStats> ringPair in parsedSet.RingPair)
                                                {
                                                    foreach (Food f in food)
                                                    {
                                                        GearSet set = new(Job);
                                                        set.AddItemClone(weaponItem);
                                                        set.AddItemClone(headItem);
                                                        set.AddItemClone(bodyItem);
                                                        set.AddItemClone(glovesItem);
                                                        set.AddItemClone(pantsItem);
                                                        set.AddItemClone(bootsItem);
                                                        set.AddItemClone(earringItem);
                                                        set.AddItemClone(neckItem);
                                                        set.AddItemClone(wristItem);
                                                        set.AddItemClones(ringPair);
                                                        set.Food = f;
                                                        set.GetDPS();


                                                        if (!configSettings.LimitTomes || set.tomeCost <= configSettings.TomeLimit)
                                                        {

                                                            while (materiaTasks.Count >= configSettings.MaximumThreads)
                                                            {
                                                                Task finishedTask = await Task.WhenAny(materiaTasks);
                                                                materiaTasks.Remove(finishedTask);
                                                            }

                                                            materiaTasks.Add(Task.Run(() => BruteForceMateriaAsync(set, sets)));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            while (materiaTasks.Any())
            {
                Task finishedTask = await Task.WhenAny(materiaTasks);
                materiaTasks.Remove(finishedTask);
            }
            processing = false;
            await deQueue;
            return true;
        }


        static void BruteForceMateriaAsync(GearSet set, ConcurrentQueue<GearSet> queue)
        {

            for (int crit = set.MaxBigMelds[SubStatEnum.CriticalHit]; crit >= 0; crit--)
            {
                int postCrit = set.BigSlots - crit;
                for (int det = Math.Min(postCrit, set.MaxBigMelds[SubStatEnum.Determination]); det >= 0; det--)
                {
                    int postDet = postCrit - det;
                    for (int ss = Math.Min(postDet, set.MaxBigMelds[set.Job.speedStat]); ss >= 0; ss--)
                    {
                        int postSS = postDet - ss;
                        for (int critSmall = set.MaxSmallMelds[SubStatEnum.CriticalHit]; critSmall >= 0; critSmall--)
                        {
                            int postCritSmall = set.SmallSlots - critSmall;
                            for (int detSmall = Math.Min(postCritSmall, set.MaxSmallMelds[SubStatEnum.Determination]); detSmall >= 0; detSmall--)
                            {
                                int postDetSmall = postCritSmall - detSmall;
                                for (int ssSmall = Math.Min(postDetSmall, set.MaxSmallMelds[set.Job.speedStat]); ssSmall >= 0; ssSmall--)
                                {
                                    int postSSSmall = postDetSmall - ssSmall;
                                    int dh = Math.Min(postSS, set.MaxBigMelds[SubStatEnum.DirectHitRate]);
                                    int dhSmall = Math.Min(postSSSmall, set.MaxSmallMelds[SubStatEnum.DirectHitRate]);

                                    GearSet g = new(set);
                                    g.AddBigMateria(SubStatEnum.CriticalHit, crit);
                                    g.AddBigMateria(SubStatEnum.Determination, det);
                                    g.AddBigMateria(set.Job.speedStat, ss);
                                    g.AddBigMateria(SubStatEnum.DirectHitRate, dh);

                                    g.AddSmallMateria(SubStatEnum.CriticalHit, critSmall);
                                    g.AddSmallMateria(SubStatEnum.Determination, detSmall);
                                    g.AddSmallMateria(set.Job.speedStat, ssSmall);
                                    g.AddSmallMateria(SubStatEnum.DirectHitRate, dhSmall);
                                    if (Job.IsOptimalGCD(g.GCD()))
                                    {
                                        AddToQueue(g, queue);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddToQueue(GearSet set, ConcurrentQueue<GearSet> queue)
        {
            set.GetDPS();
            if (!queue.Any(bS => bS.HasSameStats(set)))
            {
                queue.Enqueue(set);
            }
        }

        public static bool AddToBestFromQueue(ConcurrentQueue<GearSet> queue)
        {
            while (processing || queue.Any())
            {
                bool success = queue.TryDequeue(out GearSet set);
                if (success)
                {
                    set.GetDPS();
                    if (!bestSets.Any(bS => bS.HasSameStats(set)))
                    {
                        bestSets.Add(set);
                        if (bestSets.Count > 20)
                        {
                            bestSets = (bestSets.OrderByDescending(set => set.DPS).Take(20)).ToList();
                        }
                    }
                }
            }

            return true;
        }
    }
}
