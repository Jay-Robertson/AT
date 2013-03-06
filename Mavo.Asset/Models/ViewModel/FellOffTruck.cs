using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Mavo.Assets.Models.ViewModel
{
    [DebuggerDisplay("\\{ Asset = {Asset}, Barcode = {Barcode}, Job = {Job}, ReturnedOn = {ReturnedOn}, ReturnedBy = {ReturnedBy} \\}")]
    public sealed class AssetsWithoutReturn : IEquatable<AssetsWithoutReturn>
    {
        private int _JobId;
        private readonly string _MavoItemNumber;
        private readonly string _Asset;
        private readonly string _Barcode;
        private int _Id = 0;
        private readonly string _Job;
        private readonly DateTime? _ReturnedOn;
        private readonly string _ReturnedBy;

        public int? QuantityLost { get; set; }
        public AssetsWithoutReturn(int id, string mavoItemNumber, string asset, AssetItem assetItem, Models.Job job, int? quantityLost)
        {
            _MavoItemNumber = mavoItemNumber;
            _JobId = job.Id;
            _Asset = asset;
            if (assetItem != null)
                _Barcode = assetItem.Barcode;
            _Job = job.Name;
            _ReturnedOn = job.ReturnedDate;
            if (job.ReturnedBy != null)
                _ReturnedBy = job.ReturnedBy.FullName;
            _Id = id;
            QuantityLost = quantityLost;
        }

        public override bool Equals(object obj)
        {
            if (obj is AssetsWithoutReturn)
                return Equals((AssetsWithoutReturn)obj);
            return false;
        }
        public bool Equals(AssetsWithoutReturn obj)
        {
            if (obj == null)
                return false;
            if (!EqualityComparer<string>.Default.Equals(_MavoItemNumber, obj._MavoItemNumber))
                return false;
            if (!EqualityComparer<string>.Default.Equals(_Asset, obj._Asset))
                return false;
            if (!EqualityComparer<int>.Default.Equals(_Id, obj._Id))
                return false;
            if (!EqualityComparer<int>.Default.Equals(_JobId, obj._JobId))
                return false;
            if (!EqualityComparer<string>.Default.Equals(_Barcode, obj._Barcode))
                return false;
            if (!EqualityComparer<string>.Default.Equals(_Job, obj._Job))
                return false;
            if (!EqualityComparer<DateTime?>.Default.Equals(_ReturnedOn, obj._ReturnedOn))
                return false;
            if (!EqualityComparer<string>.Default.Equals(_ReturnedBy, obj._ReturnedBy))
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= EqualityComparer<string>.Default.GetHashCode(_Asset);
            hash ^= EqualityComparer<string>.Default.GetHashCode(_Barcode);
            hash ^= EqualityComparer<string>.Default.GetHashCode(_Job);
            hash ^= EqualityComparer<DateTime?>.Default.GetHashCode(_ReturnedOn);
            hash ^= EqualityComparer<string>.Default.GetHashCode(_ReturnedBy);
            hash ^= EqualityComparer<int>.Default.GetHashCode(_Id);
            hash ^= EqualityComparer<int>.Default.GetHashCode(_JobId);
            return hash;
        }
        public override string ToString()
        {
            return String.Format("{{Id = {5}, Asset = {0}, Barcode = {1}, Job = {2}, ReturnedOn = {3}, ReturnedBy = {4} }}", _Asset, _Barcode, _Job, _ReturnedOn, _ReturnedBy, _Id);
        }

        public string MavoItemNumber { get { return _MavoItemNumber; } }

        public string Asset
        {
            get
            {
                return _Asset;
            }
        }
        public string Barcode
        {
            get
            {
                return _Barcode;
            }
        }
        public int Id
        {
            get
            {
                return _Id;
            }
        }

        public string Job
        {
            get
            {
                return _Job;
            }
        }
        public DateTime? ReturnedOn
        {
            get
            {
                return _ReturnedOn;
            }
        }
        public string ReturnedBy
        {
            get
            {
                return _ReturnedBy;
            }
        }
        public int jobId
        {
            get
            {
                return _JobId;
            }
        }
    }
}