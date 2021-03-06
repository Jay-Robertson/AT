﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name="Job Site")]
        public string JobName { get; set; }

        public Address Address { get; set; }

        public string JobNumber { get; set; }

        public string Manager { get; set; }

        public string Customer { get; set; }

        [Display(Name ="Job Site Name")]
        public string JobSite { get; set; }

        public string Foreman { get; set; }

        public IList<JobAsset> Assets { get; set; }

        public int JobId { get; set; }

        public DateTime? PickStarted { get; set; }
        public DateTime? ReturnStarted { get; set; }

        public DateTime PickupTime { get; set; }

        public bool IsAddon { get; set; }

        public DateTime CompletionDate { get; set; }

        public IList<JobAsset> PickedAssets { get; set; }
        public IList<JobAsset> ReturnedAssets { get; set; }
    }
    public class JobAsset
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public int? QuantityTaken { get; set; }

        public int? QuantityNeeded { get; set; }

        public AssetKind Kind { get; set; }

        [Remote("DoesSerialExist", "Asset", AdditionalFields = "AssetId", ErrorMessage = "Serial number does not exist")]
        public string Barcode { get; set; }

        public int AssetId { get; set; }

        public bool NotEnoughQuantity { get; set; }

        public int? AssetItemId { get; set; }

        public int? QuantityAvailable { get; set; }

        public int? QuantityReturned { get; set; }

        public string AssetCategory { get; set; }

        public bool IsDamaged { get; set; }

        public string MavoItemNumber { get; set; }
    }
}