using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Tablestatus
{
    public int Tablestatusid { get; set; }

    public string? Statusname { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
