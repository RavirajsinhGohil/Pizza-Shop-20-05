using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Customer
{
    public int Customerid { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? UpdatedbyNavigation { get; set; }

    public virtual ICollection<Waitingticket> Waitingtickets { get; set; } = new List<Waitingticket>();
}
