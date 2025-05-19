using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Taxesandfee
{
    public int Taxid { get; set; }

    public string? Taxname { get; set; }

    public string Taxtype { get; set; } = null!;

    public decimal Taxvalue { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool Isdeleted { get; set; }

    public bool? Isenabled { get; set; }

    public bool? Isdefault { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Ordertaxmapping> Ordertaxmappings { get; set; } = new List<Ordertaxmapping>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
