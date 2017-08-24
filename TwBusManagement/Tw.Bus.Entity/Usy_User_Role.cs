using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tw.Bus.Entity
{
    [Table("Usy_User_Role")]
    public partial class Usy_User_Role:Entity<int>
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }

        public virtual Usy_User User { get; set; }


        public virtual Usy_Role Role { get; set; }
    }
}
