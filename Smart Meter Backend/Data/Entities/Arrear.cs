using System;
using System.Collections.Generic;

namespace SmartMeterBackend.Data.Entities;

public partial class Arrear
{
    public long Aid { get; set; }

    public long Consumerid { get; set; }

    public string Atype { get; set; } = null!;

    public string Paidstatus { get; set; } = null!;

    public long Billid { get; set; }

    public virtual Billing Bill { get; set; } = null!;

    public virtual Consumer Consumer { get; set; } = null!;
}
