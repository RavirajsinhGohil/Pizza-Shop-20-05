using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Item
{
    public int Itemid { get; set; }

    public string? Itemname { get; set; }

    public decimal? Rate { get; set; }

    public decimal? Quantity { get; set; }

    public int? Categoryid { get; set; }

    public string? Itemimage { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool Isdeleted { get; set; }

    public bool Available { get; set; }

    public bool Ismodifiable { get; set; }

    public string? Unit { get; set; }

    public decimal? Tax { get; set; }

    public string? Itemshortcode { get; set; }

    public string? Description { get; set; }

    public string? Itemimagepath { get; set; }

    public string? Itemtype { get; set; }

    public bool? Isfavorite { get; set; }

    public virtual Menucategory? Category { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Itemmodifiergroupmapping> Itemmodifiergroupmappings { get; set; } = new List<Itemmodifiergroupmapping>();

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Ordermodifierdetail> Ordermodifierdetails { get; set; } = new List<Ordermodifierdetail>();

    public virtual User? UpdatedbyNavigation { get; set; }

    public virtual ICollection<Userfavouriteitem> Userfavouriteitems { get; set; } = new List<Userfavouriteitem>();
}
