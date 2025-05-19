using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class User
{
    public int Userid { get; set; }

    public string? Username { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Country { get; set; }

    public string? States { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public byte[]? Profile { get; set; }

    public int Roleid { get; set; }

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public string? Password { get; set; }

    public string? Status { get; set; }

    public bool Isdeleted { get; set; }

    public string? Profileimagepath { get; set; }

    public virtual ICollection<Customer> CustomerCreatedbyNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Customer> CustomerUpdatedbyNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Invoice> InvoiceCreatedbyNavigations { get; set; } = new List<Invoice>();

    public virtual ICollection<Invoice> InvoiceUpdatedbyNavigations { get; set; } = new List<Invoice>();

    public virtual ICollection<Item> ItemCreatedbyNavigations { get; set; } = new List<Item>();

    public virtual ICollection<Item> ItemUpdatedbyNavigations { get; set; } = new List<Item>();

    public virtual ICollection<Loginuser> LoginuserCreatedbyNavigations { get; set; } = new List<Loginuser>();

    public virtual ICollection<Loginuser> LoginuserUpdatedbyNavigations { get; set; } = new List<Loginuser>();

    public virtual ICollection<Mainnavbar> MainnavbarCreatedbyNavigations { get; set; } = new List<Mainnavbar>();

    public virtual ICollection<Mainnavbar> MainnavbarUpdatedbyNavigations { get; set; } = new List<Mainnavbar>();

    public virtual ICollection<Menucategory> MenucategoryCreatedbyNavigations { get; set; } = new List<Menucategory>();

    public virtual ICollection<Menucategory> MenucategoryUpdatedbyNavigations { get; set; } = new List<Menucategory>();

    public virtual ICollection<Modifiergroup> ModifiergroupCreatedbyNavigations { get; set; } = new List<Modifiergroup>();

    public virtual ICollection<Modifiergroup> ModifiergroupUpdatedbyNavigations { get; set; } = new List<Modifiergroup>();

    public virtual ICollection<Order> OrderCreatedbyNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderUpdatedbyNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Orderdetail> OrderdetailCreatedbyNavigations { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Orderdetail> OrderdetailUpdatedbyNavigations { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Ordermodifierdetail> OrdermodifierdetailCreatedbyNavigations { get; set; } = new List<Ordermodifierdetail>();

    public virtual ICollection<Ordermodifierdetail> OrdermodifierdetailUpdatedbyNavigations { get; set; } = new List<Ordermodifierdetail>();

    public virtual ICollection<Orderrating> OrderratingCreatedbyNavigations { get; set; } = new List<Orderrating>();

    public virtual ICollection<Orderrating> OrderratingUpdatedbyNavigations { get; set; } = new List<Orderrating>();

    public virtual ICollection<Orderstatus> OrderstatusCreatedbyNavigations { get; set; } = new List<Orderstatus>();

    public virtual ICollection<Orderstatus> OrderstatusUpdatedbyNavigations { get; set; } = new List<Orderstatus>();

    public virtual ICollection<Permission> PermissionCreatedbyNavigations { get; set; } = new List<Permission>();

    public virtual ICollection<Permission> PermissionUpdatedbyNavigations { get; set; } = new List<Permission>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Section> SectionCreatedbyNavigations { get; set; } = new List<Section>();

    public virtual ICollection<Section> SectionUpdatedbyNavigations { get; set; } = new List<Section>();

    public virtual ICollection<Table> TableCreatedbyNavigations { get; set; } = new List<Table>();

    public virtual ICollection<Table> TableUpdatedbyNavigations { get; set; } = new List<Table>();

    public virtual ICollection<Tablegrouping> TablegroupingCreatedbyNavigations { get; set; } = new List<Tablegrouping>();

    public virtual ICollection<Tablegrouping> TablegroupingUpdatedbyNavigations { get; set; } = new List<Tablegrouping>();

    public virtual ICollection<Taxesandfee> TaxesandfeeCreatedbyNavigations { get; set; } = new List<Taxesandfee>();

    public virtual ICollection<Taxesandfee> TaxesandfeeUpdatedbyNavigations { get; set; } = new List<Taxesandfee>();

    public virtual ICollection<Userfavouriteitem> UserfavouriteitemCreatedbyNavigations { get; set; } = new List<Userfavouriteitem>();

    public virtual ICollection<Userfavouriteitem> UserfavouriteitemUpdatedbyNavigations { get; set; } = new List<Userfavouriteitem>();

    public virtual ICollection<Userfavouriteitem> UserfavouriteitemUsers { get; set; } = new List<Userfavouriteitem>();

    public virtual ICollection<Waitingticket> WaitingticketCreatedbyNavigations { get; set; } = new List<Waitingticket>();

    public virtual ICollection<Waitingticket> WaitingticketUpdatedbyNavigations { get; set; } = new List<Waitingticket>();
}
