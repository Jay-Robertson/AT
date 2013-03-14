using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Mavo.Assets.Models;

namespace Mavo.Assets.Controllers
{
    public class CategoryController : ApiController
    {
        private AssetContext _db;

        public CategoryController() : this(new AssetContext()) { }
        public CategoryController(AssetContext db)
        {
            _db = db;
        }

        public AssetCategory PostCategory(AssetCategory proto)
        {
            var category = new AssetCategory { Name = proto.Name, Description = proto.Description };
            _db.AssetCategories.Add(category);
            _db.SaveChanges();
            return category;
        }

        public AssetCategory[] GetCategories()
        {
            return _db.AssetCategories.ToArray();
        }
    }
}
