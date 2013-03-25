using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mavo.Assets.Controllers
{
    public class PickedAssetRow
    {
        public string MavoNumber { get; set; }
        public int AssetId { get; set; }
        public int? AssetItemId { get; set; }
        public string AssetName { get; set; }

        public int CurrentPickedQty { get; set; }
        public string Barcodes { get; set; }
        
    }
}
