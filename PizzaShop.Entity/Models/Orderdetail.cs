using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Orderdetail
{
    public int Orderdetailid { get; set; }

    public int? Orderid { get; set; }

    public int? Itemid { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool? Isdeleted { get; set; }

    public int? Availablequantity { get; set; }

    public string? Iteminstruction { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ICollection<Ordermodifierdetail> Ordermodifierdetails { get; set; } = new List<Ordermodifierdetail>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
