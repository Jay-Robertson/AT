using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mavo.Assets.Models
{
    [Flags]
    public enum SendCustomer
    {
        CopyOfInvoice = 0x1,
        CopyFinalReport = 0x2,
        CopyOfAIADocumens = 0x4
    }
}
