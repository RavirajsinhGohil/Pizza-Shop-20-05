using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class State
{
    public int Stateid { get; set; }

    public string? Statename { get; set; }

    public int? Countryid { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country? Country { get; set; }
}
