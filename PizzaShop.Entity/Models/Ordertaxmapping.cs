using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Ordertaxmapping
{
    public int Ordertaxmappingid { get; set; }

    public int Orderid { get; set; }

    public int Taxid { get; set; }

    public string? Taxtype { get; set; }

    public string? Taxname { get; set; }

    public decimal? Taxvalue { get; set; }

    public decimal? Taxamount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Taxesandfee Tax { get; set; } = null!;
}
