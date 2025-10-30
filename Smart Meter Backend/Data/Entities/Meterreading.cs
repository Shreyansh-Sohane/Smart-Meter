﻿using System;
using System.Collections.Generic;

namespace SmartMeterBackend.Data.Entities;

public partial class Meterreading
{
    public int Meterreadingid { get; set; }

    public string Meterid { get; set; } = null!;

    public DateTime Meterreadingdate { get; set; }

    public decimal? Energyconsumed { get; set; }

    public decimal Voltage { get; set; }

    public decimal Current { get; set; }

    public virtual Meter Meter { get; set; } = null!;
}
