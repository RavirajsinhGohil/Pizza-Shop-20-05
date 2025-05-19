using System;
using System.Collections;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Userfavouriteitem
{
    public int Userfavouriteitem1 { get; set; }

    public int? Userid { get; set; }

    public int? Itemid { get; set; }

    public BitArray? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Item? Item { get; set; }

    public virtual User? UpdatedbyNavigation { get; set; }

    public virtual User? User { get; set; }
}
