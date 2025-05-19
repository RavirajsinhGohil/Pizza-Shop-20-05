using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Modifiergroup
{
    public int Modifiergroupid { get; set; }

    public string? Modifiername { get; set; }

    public string? Description { get; set; }

    public int? Menucategoryid { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Itemmodifiergroupmapping> Itemmodifiergroupmappings { get; set; } = new List<Itemmodifiergroupmapping>();

    public virtual Menucategory? Menucategory { get; set; }

    public virtual User? UpdatedbyNavigation { get; set; }
}
