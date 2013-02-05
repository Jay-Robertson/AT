using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mavo.Assets.Models
{
    [Flags]
    public enum SendConsultant
    {
        InvoiceForApproval = 0x1,
        FinalReport = 0x2
    }
}
