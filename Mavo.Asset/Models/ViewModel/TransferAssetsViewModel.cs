using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class TransferAssetsViewModel
    {
        public int JobToTransferTo { get; set; }
        public int JobToTransferFrom { get; set; }
        public List<PickedAsset> Assets { get; set; }
        public List<TranferredAsset> TransferredAssets { get; set; }
        public List<Job> Jobs { get; set; }

        public string JobToTransferFromName { get; set; }
    }
    public class TranferredAsset
    {
        public int PickedAssetId { get; set; }
        public int? Quantity { get; set; }
        public bool IsSelected { get; set; }
    }
}