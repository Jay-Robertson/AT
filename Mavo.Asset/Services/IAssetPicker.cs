using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavo.Assets.Models;
using System.Data.Entity;

namespace Mavo.Assets.Services
{
    public interface IAssetPicker
    {
        IList<AssetWithQuantity> AddFromTemplate(int jobId, int templateId);
        AssetWithQuantity Add(int assetId, int? jobId = null, int? templateId = null);
        void IncreaseQuantity(int assetId, int newQuantity);
    }
    public class AssetPicker : IAssetPicker
    {
        private readonly AssetContext db;
        public AssetPicker(AssetContext db)
        {
            this.db = db;
        }
        public AssetWithQuantity Add(int assetId, int? jobId = null, int? templateId = null)
        {
            Asset asset = db.Assets.FirstOrDefault(x => x.Id == assetId);
            AssetWithQuantity newAssetWithQuantity = new AssetWithQuantity() { Asset = asset, Quantity = 1 };
            if (jobId.HasValue)
            {

                var job = db.Jobs.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == jobId);
                if (job.Assets != null && job.Assets.Any(x => x.Asset.Id == assetId))
                {
                    var assetToIncrease = job.Assets.FirstOrDefault(x => x.Asset.Id == assetId);
                    assetToIncrease.Quantity++;
                    newAssetWithQuantity.Quantity = assetToIncrease.Quantity;
                }
                else
                {
                    if (job.Assets == null)
                        job.Assets = new List<AssetWithQuantity>();

                    job.Assets.Add(newAssetWithQuantity);
                }
            }
            else if (templateId.HasValue)
            {
                newAssetWithQuantity = new TemplateAsset() { Asset = asset, Quantity = 1 }; ;

                var template = db.Templates.Include(x => x.Assets).Include("Assets.Asset").FirstOrDefault(x => x.Id == templateId);
                if (template.Assets != null && template.Assets.Any(x => x.Asset.Id == assetId))
                {
                    var assetToIncrease = template.Assets.FirstOrDefault(x => x.Asset.Id == assetId);
                    assetToIncrease.Quantity++;
                    newAssetWithQuantity.Quantity = assetToIncrease.Quantity;
                }
                else
                {
                    if (template.Assets == null)
                        template.Assets = new List<TemplateAsset>();

                    template.Assets.Add((TemplateAsset)newAssetWithQuantity);
                }
            }


            db.SaveChanges();
            return newAssetWithQuantity;
        }


        public void IncreaseQuantity(int assetId, int newQuantity)
        {
            var asset = db.JobAssets.FirstOrDefault(x => x.Id == assetId);
            asset.Quantity = newQuantity;
            db.SaveChanges();
        }

        public IList<AssetWithQuantity> AddFromTemplate(int jobId, int templateId)
        {
            IList<AssetWithQuantity> result = new List<AssetWithQuantity>();
            var template = db.Templates.Include(x => x.Assets).Include("Assets.Asset").First(x => x.Id == templateId);
            foreach (var asset in template.Assets)
            {
                result.Add(Add(asset.Asset.Id, jobId: jobId));
            }
            return result;
        }

    }
}
