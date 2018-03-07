using System.Collections.Generic;

namespace Sofco.Core.Models.Admin
{
    public class UserDetailModel : UserModel
    {
        public UserDetailModel()
        {
            Roles = new List<SelectListModel>();
            Modules = new List<ModuleModelDetail>();
            Groups = new List<SelectListModel>();
        }

        public new IList<SelectListModel> Groups { get; set; }

        public IList<SelectListModel> Roles { get; set; }

        public IList<ModuleModelDetail> Modules { get; set; }
    }
}
