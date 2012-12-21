using Mavo.Assets.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAssetData
{
    class Program
    {
        static void Main(string[] args)
        {
            // load data
            Console.WriteLine("MAVO AssetItem HandiForms SQL Export Tool");
            Console.WriteLine("----");
            Console.WriteLine("Loading mdb...");
            var items = new Data.FORMS32K_MavoDataSet.AssetItemsDataTable();
            var consumables = new Data.FORMS32K_MavoDataSet.ConsumablesDataTable();
            new Data.FORMS32K_MavoDataSetTableAdapters.AssetItemsTableAdapter().Fill(items);
            new Data.FORMS32K_MavoDataSetTableAdapters.ConsumablesTableAdapter().Fill(consumables);
            Console.WriteLine("Complete, found {0} items, {1} consumables.", items.Rows.Count, consumables.Rows.Count);

            // open sconnection to database
            Database.SetInitializer<AssetContext>(null);
            var db = new AssetContext();
            
            // create missing categories
            Console.WriteLine("Processing categories...");
            var categories = db.AssetCategories.ToDictionary(x => x.Name, x => x);
            foreach (var c in items.Select(x => x.ItemCategory).Union(consumables.Select(x => x.ItemCategory)).Distinct().OrderBy(x => x).ToList())
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
            
            // create consumable Assets
            Console.WriteLine("Processing consumables...");
            var created = 0;
            foreach (var row in consumables)
            {
                if (String.IsNullOrWhiteSpace(row.ItemName)) continue;

                var asset = db.Assets.SingleOrDefault(x => x.MavoItemNumber == row.ItemNumber);
                if (null == asset)
                {
                    asset = new Asset()
                    {
                        Kind = AssetKind.Consumable,
                        MavoItemNumber = row.ItemNumber,
                        Inventory = 0
                    };
                    db.Assets.Add(asset);
                    db.AssetActivity.Add(new AssetActivity
                    {
                        Action = AssetAction.Create,
                        Asset = asset,
                        Date = row.TimeStamp,
                        User = null
                    });
                    created++;
                }
                if (!String.IsNullOrWhiteSpace(row.ItemCategory))
                {
                    asset.Category = categories[row.ItemCategory];
                }
                asset.Name = row.ItemName;
                asset.UnitOfMeasure = (row.ItemCount == "EACH" ? null : row.ItemCount);
                
                db.SaveChanges();            
            }

            Console.WriteLine("   ... created {0} new Conumable Assets (done).", created);

            // create serialized assets
            Console.WriteLine("Processing serialized asset items...");
            created = 0;
            foreach (var row in items)
            {
                // load or create the asset first
                var asset = db.Assets.SingleOrDefault(x => x.MavoItemNumber == row.ItemNumber);
                if (null != asset && asset.Kind != AssetKind.Serialized)
                {
                    Console.WriteLine("   ... switching AssetKind to Serialized for ({0},{1})", row.ItemNumber, row.ItemName);
                    asset.Kind = AssetKind.Serialized;
                }
                if (null == asset)
                {
                    asset = new Asset()
                    {
                        Kind = AssetKind.Serialized,
                        MavoItemNumber = row.ItemNumber,
                        Name = row.ItemName,
                    };
                    db.Assets.Add(asset);
                }

                // now look for an assetitem
                if (String.IsNullOrWhiteSpace(row.AssetTag))
                {
                    Console.WriteLine("   ... skipping item with null AssetTag: ({0},{1})", row.ItemNumber, row.ItemName);
                    continue;
                }
                var item = db.AssetItems.SingleOrDefault(x => x.Barcode == row.AssetTag);
                if (null == item)
                {
                    item = new AssetItem
                    {
                        Asset = asset,
                        Barcode = row.AssetTag,
                        Condition = AssetCondition.Good,
                        Manufacturer = (String.IsNullOrWhiteSpace(row.Manufacture) || row.Manufacture == "NA") ? null : row.Manufacture,
                        ModelNumber = (String.IsNullOrWhiteSpace(row.Model) || row.Model == "NA") ? null : row.Model,
                        SerialNumber = row.SerialNumber,
                        Status = InventoryStatus.In,
                    };
                    db.AssetItems.Add(item);
                    db.AssetActivity.Add(new AssetActivity
                    {
                        Action = AssetAction.Create,
                        Asset = asset,
                        Item = item,
                        Date = row.TimeStamp,
                        User = null,
                    });
                    created++;
                }
                db.SaveChanges();
            }
            Console.WriteLine("   ... done.");
            Console.ReadKey();
        }
    }
}
