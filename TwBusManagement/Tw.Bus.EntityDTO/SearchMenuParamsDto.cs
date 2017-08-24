using System;
using System.Collections.Generic;
using System.Text;

namespace Tw.Bus.EntityDTO
{
    public class SearchMenuParamsDto
    {
        public int parentId { get; set; }

        public List<int> lstRoleId { get; set; } = null;

        public bool IsShowHide { get; set; } = true;
    }
}
