using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavo.Assets.Models;

namespace Mavo.Assets.Services
{
    public interface IAssetActivityManager
    {
        void Add(AssetAction action, Asset asset, AssetItem item = null, Job job = null);
    }
    public class AssetActivityMananger : IAssetActivityManager
    {
        private readonly AssetContext db;
        private readonly ICurrentUserService GetUser;

        /// <summary>
        /// Initializes a new instance of the AssetActivityMananger class.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="getUser"></param>
        public AssetActivityMananger(AssetContext db, ICurrentUserService getUser)
        {
            this.db = db;
            GetUser = getUser;
        }

        public void Add(AssetAction action, Asset asset, AssetItem item = null, Job job = null)
        {
            db.AssetActivity.Add(new AssetActivity()
            {
                Asset = asset,
                Action = action,
                Date = DateTime.Now,
                Item = item,
                User = GetUser.GetCurrent(),
                Job = job
            });
        }

    }
}
