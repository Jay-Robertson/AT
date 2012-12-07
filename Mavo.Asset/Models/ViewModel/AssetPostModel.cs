using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class AssetPostModel
    {
        public int? Id { get; set; }
        public string Barcode { get; set; }         // mavo barcode value
        public int? CategoryId { get; set; }
        public string Name { get; set; }

        // warehousing data
        public AssetKind Kind { get; set; }
        public int? Inventory { get; set; }          // only valid for Consumable and NotSerialized assets

        // manufactuer/model/vendor data
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string UPC { get; set; }
    }

    public class AssetScanPostModel
    {
        public int? Id { get; set; }
        public int AssetId { get; set; }
        [Required]
        public string Barcode { get; set; }                 // mavo barcode value
        public AssetCondition Condition { get; set; }

        // purchasing/warranty data
        public string SerialNumber { get; set; }            // manufacturer's serial number
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? WarrantyExpiration { get; set; }

    }
}