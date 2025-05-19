using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Itemmodifiergroupmapping
{
    public int Itemmodifiergroupmappingid { get; set; }

    public int? Itemid { get; set; }

    public int? Modifiergroupid { get; set; }

    public bool? Isitemmodifiable { get; set; }

    public int? Minquantity { get; set; }

    public int? Maxquantity { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Modifiergroup? Modifiergroup { get; set; }
}
