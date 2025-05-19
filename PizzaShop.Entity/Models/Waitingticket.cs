using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Waitingticket
{
    public int Waitingticketid { get; set; }

    public int? Customerid { get; set; }

    public int? Sectionid { get; set; }

    public int? Noofpersons { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? Tableassigntime { get; set; }

    public bool? Isactive { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Section? Section { get; set; }

    public virtual User? UpdatedbyNavigation { get; set; }
}
