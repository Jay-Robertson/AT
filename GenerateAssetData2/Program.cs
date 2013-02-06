using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Configuration;

using Mavo.Assets.Models;

namespace GenerateAssetData2
{
    public class ImportAsset
    {
        [CsvField(Index = 0)] public string LoadSheet { get; set; }
        [CsvField(Index = 1)] public string Category { get; set; }
        [CsvField(Index = 2)] public string Number { get; set; }
        [CsvField(Index = 3)] public string Name { get; set; }
        [CsvField(Index = 4)] public string Unit { get; set; }
        [CsvField(Index = 5)] public string Kind { get; set; }

        public Asset Import(AssetContext db, IDictionary<string, AssetCategory> categories)
        {
            // already exists, by item number
            var n = Number.Trim().ToLowerInvariant();
            var asset = db.Assets.FirstOrDefault(x => x.MavoItemNumber == n);
            if (null == asset)
            {
                asset = new Asset
                {
                    MavoItemNumber = n
                };
                db.Assets.Add(asset);
            }
            asset.Name = Name;
            asset.UnitOfMeasure = Unit;
            asset.Category = categories[Category];

            return asset;
        }
    }

    public class ImportItem
    {
        [CsvField(Index =  0)] public string UnitID { get; set; }
        [CsvField(Index =  1)] public string TimeStamp { get; set; }
        [CsvField(Index =  2)] public string EquipmentBarcoding { get; set; }
        [CsvField(Index =  3)] public string ItemNumber { get; set; }
        [CsvField(Index =  4)] public string ItemName { get; set; }
        [CsvField(Index =  5)] public string ItemCategory { get; set; }
        [CsvField(Index =  6)] public string ItemCount { get; set; }
        [CsvField(Index =  7)] public string AssetTag { get; set; }
        [CsvField(Index =  8)] public string SerialNumber { get; set; }
        [CsvField(Index =  9)] public string AssetType { get; set; }
        [CsvField(Index = 10)] public string Manufacturer { get; set; }
        [CsvField(Index = 11)] public string Model { get; set; }
        [CsvField(Index = 12)] public string EquipCondition { get; set; }
        [CsvField(Index = 13)] public string Cost { get; set; }
        [CsvField(Index = 14)] public string DatePurchased { get; set; }
        [CsvField(Index = 15)] public string OwnOrLeased { get; set; }

        public ImportAsset ToAsset()
        {
            return new ImportAsset()
            {
                Category = ItemCategory,
                Number = ItemNumber,
                Name = ItemName,
                Unit = ItemCount,
                Kind = AssetType
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MAVO AssetItem CSV Import Tool");
            Console.WriteLine("----");

            Console.WriteLine("Loading assets.csv...");
            var assets = new List<ImportAsset>();
            using (var file = File.OpenText("Data\\assets.csv"))
            {
                using (var csv = new CsvReader(file))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    assets.AddRange(csv.GetRecords<ImportAsset>());
                }
            }

            Console.WriteLine("Loading items.csv...");
            var items = new List<ImportItem>();
            using (var file = File.OpenText("Data\\items.csv"))
            {
                using (var csv = new CsvReader(file))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    items.AddRange(csv.GetRecords<ImportItem>());
                }
            }
 
            // open connection to database
            Database.SetInitializer<AssetContext>(null);
            var db = new AssetContext();
           
            // create missing categories
            Console.WriteLine("Processing categories...");
            var categories = db.AssetCategories.ToDictionary(x => x.Name, x => x);
            foreach (var c in assets.Select(x => x.Category).Union(items.Select(x => x.ItemCategory)))
            {
                if (!String.IsNullOrWhiteSpace(c) && !categories.ContainsKey(c))
                {
                    var a = new AssetCategory() {
                        Name = c,
                        Description = String.Empty,
                        SortOrder = categories.Count+1,
                    };
                    db.AssetCategories.Add(a);
                    categories[a.Name] = a;
                    Console.WriteLine("    created new category '{0}'", c);
                }
            }
            db.SaveChanges();

            // load assets
            Console.WriteLine("Creating assets...");
            var z = 0;
            foreach (var a in assets)
            {
                if (z % 100 == 0)
                {
                    Console.WriteLine("  {0}/{1}", ++z, assets.Count);
                }
                a.Import(db, categories);
                db.SaveChanges();
            }
            
            // load asset items
            Console.WriteLine("Creating items...");
            z = 0;
            foreach (var i in items)
            {
                if (z % 100 == 0)
                {
                    Console.WriteLine("  {0}/{1}", ++z, assets.Count);
                }
                var a = i.ToAsset().Import(db, categories);
                var item = db.AssetItems.SingleOrDefault(x => x.Barcode == i.AssetTag);
                if (null == item)
                {
                    item = new AssetItem
                    {
                        Barcode = i.AssetTag
                    };
                    db.AssetItems.Add(item);
                }
                item.Manufacturer = (i.Manufacturer == "NA" ? null : i.Manufacturer);
                item.ModelNumber = (i.Model == "NA" ? null : i.Model);
                item.SerialNumber = i.SerialNumber;
                db.SaveChanges();
            }
        }
    }
}
