using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Permission
{
    public int Permissionid { get; set; }

    public int Roleid { get; set; }

    public string? Permissionname { get; set; }

    public int? Menuid { get; set; }

    public bool Canview { get; set; }

    public bool Canaddedit { get; set; }

    public bool Candelete { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User? UpdatedbyNavigation { get; set; }
}
