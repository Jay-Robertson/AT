using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Mavo.Assets.Models;

namespace Mavo.Assets.Services
{
    public interface ICurrentUserService
    {
        User GetCurrent();
    }
    public class CurrentUserService : ICurrentUserService
    {
        private readonly AssetContext db;
        public CurrentUserService(AssetContext db)
        {
            this.db = db;
        }
        public User GetCurrent()
        {
            return this.db.Users.FirstOrDefault(x => x.Email == HttpContext.Current.User.Identity.Name);
        }
    }

}
