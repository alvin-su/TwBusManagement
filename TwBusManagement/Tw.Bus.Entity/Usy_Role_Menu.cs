using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tw.Bus.Entity
{
    [Table("Usy_Role_Menu")]
    public partial class Usy_Role_Menu:Entity<int>
    {
        public int? RoleId { get; set; }
        public int? MenuId { get; set; }

        public virtual Usy_Menu Menu { get; set; }
        public virtual Usy_Role Role { get; set; }
    }
}
