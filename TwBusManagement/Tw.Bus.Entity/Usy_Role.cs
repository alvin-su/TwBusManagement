using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tw.Bus.Entity
{
    [Table("Usy_Role")]
    public partial class Usy_Role:Entity<int>
    {
        public Usy_Role()
        {
            UsyRoleActionPermission = new HashSet<Usy_RoleActionPermission>();
            UsyRoleMenu = new HashSet<Usy_Role_Menu>();
        }

        public string Name { get; set; }
        public string R_Desc { get; set; }
        public int? R_Status { get; set; }
        public string AddUser { get; set; }
        public DateTime? AddTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual ICollection<Usy_RoleActionPermission> UsyRoleActionPermission { get; set; }
        public virtual ICollection<Usy_Role_Menu> UsyRoleMenu { get; set; }
    }
}
