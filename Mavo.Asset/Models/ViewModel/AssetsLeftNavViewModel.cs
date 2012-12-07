using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class AssetsLeftNavViewModel
    {
        public Asset Asset { get; set; }
        public IList<Asset> Assets { get; set; }
    }
}