using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Section
{
    public int Sectionid { get; set; }

    public string Sectionname { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual User? UpdatedbyNavigation { get; set; }

    public virtual ICollection<Waitingticket> Waitingtickets { get; set; } = new List<Waitingticket>();
}
