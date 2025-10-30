using System;
using System.Collections.Generic;

namespace SmartMeterBackend.Data.Entities;

public partial class User
{
    public long Userid { get; set; }

    public string Username { get; set; } = null!;

    public byte[] Passwordhash { get; set; } = null!;

    public string Displayname { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? testing { get; set; }

    public DateTime? Lastloginutc { get; set; } = DateTime.UtcNow;

    public string? ImageUrl { get; set; }

    public bool Isactive { get; set; }
}
