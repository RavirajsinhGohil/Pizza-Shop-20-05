using System;
using System.Collections;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Orderrating
{
    public int Orderratingid { get; set; }

    public int? Orderid { get; set; }

    public int? Ratingid { get; set; }

    public int? Ratingorder { get; set; }

    public string? Description { get; set; }

    public BitArray? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? UpdatedbyNavigation { get; set; }
}
