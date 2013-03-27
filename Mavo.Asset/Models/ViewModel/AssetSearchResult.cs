using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class AssetSearchResult
    {
        public string MavoItemNumber { get; set; }

        public string Name { get; set; }

        public AssetKind? Kind { get; set; }

        public int? AssetId { get; set; }

        public string Category { get; set; }

        public int? CategoryId { get; set; }

        public List<AssetSearchResult> Results { get; set; }

        [Display(Name="Search Term")]
        public string SearchString { get; set; }

        public string Manufacturer { get; set; }

        public int? Quantity { get; set; }

        public IList<AssetItem> AssetItems { get; set; }
    }
}