using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mavo.Assets.Models
{
    public class ItemNotPicked
    {
        public Job Job { get; set; }

        public List<NotPickedItem> NotPicked { get; set; }
    }
    public class NotPickedItem
    {
        public string AssetName { get; set; }
        public int AssetId { get; set; }
        public int Picked { get; set; }
        public int Requested { get; set; }
    }
}
