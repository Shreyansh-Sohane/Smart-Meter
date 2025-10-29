﻿using System;
using System.Collections.Generic;

namespace SmartMeterBackend.Data.Entities;

public partial class Consumer
{
    public long Consumerid { get; set; }

    public byte[] Passwordhash { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long? Aid { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int Orgunitid { get; set; }

    public int Tariffid { get; set; }      

    public string Status { get; set; } = null!;

    public DateTime Createdat { get; set; } = DateTime.UtcNow; 

    public string Createdby { get; set; } = null!;

    public DateTime? Updatedat { get; set; } = DateTime.UtcNow;

    public string? Updatedby { get; set; }

    public bool Deleted { get; set; }

    public virtual Address? AidNavigation { get; set; }

    public virtual ICollection<Arrear> Arrears { get; set; } = new List<Arrear>();

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();

    public virtual Orgunit Orgunit { get; set; } = null!;

    public virtual Tariff Tariff { get; set; } = null!;
}
