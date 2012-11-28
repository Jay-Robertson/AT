using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Mavo.Asset.Models
{
    public enum AssetKind
    {
        Consumable,
        Durable,
        DurableNotTracked,
    }

    public class AssetClass
    {

    }
}
