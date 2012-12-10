using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    public class TemplatesLeftNavViewModel
    {
        public IEnumerable<Template> Templates { get; set; }

        public Template Template { get; set; }
    }
    public class AssetsLeftNavViewModel
    {
        public Asset Asset { get; set; }
        public IList<Asset> Assets { get; set; }
    }
}