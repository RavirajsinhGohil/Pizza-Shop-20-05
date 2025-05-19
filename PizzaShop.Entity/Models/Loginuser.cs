using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Loginuser
{
    public int Loginuserid { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Isfirsttimelogin { get; set; }

    public int? Userid { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual User? UpdatedbyNavigation { get; set; }
}
