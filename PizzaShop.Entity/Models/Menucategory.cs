using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Menucategory
{
    public int Menucategoryid { get; set; }

    public string? Categoryname { get; set; }

    public string? Description { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Modifiergroup> Modifiergroups { get; set; } = new List<Modifiergroup>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
