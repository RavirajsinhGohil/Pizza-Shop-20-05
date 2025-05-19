using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Itemcategorymapping
{
    public int Itemcategorymappingid { get; set; }

    public int? Itemid { get; set; }

    public int? Menucategoryid { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Menucategory? Menucategory { get; set; }
}
