using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mavo.Assets.Models.ViewModel
{
    public class PickAJobModel
    {
        /// <summary>
        /// Initializes a new instance of the PickAJobModel class.
        /// </summary>
        public PickAJobModel()
        {
            Assets = new List<JobAsset>();
        }
        public string JobName { get; set; }

        public Address Address { get; set; }

        public string JobNumber { get; set; }

        public string Manager { get; set; }

        public string Customer { get; set; }

        public string JobSite { get; set; }

        public string Foreman { get; set; }

        public IList<JobAsset> Assets { get; set; }

        public int JobId { get; set; }

        public DateTime? PickStarted { get; set; }
        public DateTime? ReturnStarted { get; set; }

        public DateTime PickupTime { get; set; }

        public bool IsAddon { get; set; }

        public DateTime CompletionDate { get; set; }
    }
    public class JobAsset
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public int? QuantityTaken { get; set; }

        public int? QuantityNeeded { get; set; }

        public AssetKind Kind { get; set; }

        [Remote("DoesSerialExist", "Asset", ErrorMessage = "Serial number does not exist")]
        public string Barcode { get; set; }

        public int AssetId { get; set; }

        public bool NotEnoughQuantity { get; set; }

        public int? AssetItemId { get; set; }

        public int? QuantityAvailable { get; set; }


        public bool IsDamaged { get; set; }
    }
}