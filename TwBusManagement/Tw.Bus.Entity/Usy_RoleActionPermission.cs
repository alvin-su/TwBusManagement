using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tw.Bus.Entity
{
    [Table("Usy_RoleActionPermission")]
    public partial class Usy_RoleActionPermission:Entity<int>
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int? RoleId { get; set; }

        public virtual Usy_Role Role { get; set; }
    }
}
