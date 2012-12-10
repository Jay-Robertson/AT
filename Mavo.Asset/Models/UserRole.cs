using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mavo.Assets.Models
{
    [Flags]
    public enum UserRole
    {
        ProjectManager = 1,
        WarehouseManager = 2,
        WarehouseStaff = 4,
        Foreman = 8,

        // all roles
        Administrator = 15
    }
}
