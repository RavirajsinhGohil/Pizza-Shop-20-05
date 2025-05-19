using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Order
{
    public int Orderid { get; set; }

    public int Customerid { get; set; }

    public int Tableid { get; set; }

    public string Status { get; set; } = null!;

    public string Paymentmode { get; set; } = null!;

    public decimal Totalamount { get; set; }

    public string? Admincomment { get; set; }

    public string? Customerfeedback { get; set; }

    public decimal? Avgrating { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool Isdeleted { get; set; }

    public int? Noofpersons { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Orderrating> Orderratings { get; set; } = new List<Orderrating>();

    public virtual ICollection<Orderstatus> Orderstatuses { get; set; } = new List<Orderstatus>();

    public virtual ICollection<Ordertaxmapping> Ordertaxmappings { get; set; } = new List<Ordertaxmapping>();

    public virtual ICollection<Tablegrouping> Tablegroupings { get; set; } = new List<Tablegrouping>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
