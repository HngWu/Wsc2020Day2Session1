using System;
using System.Collections.Generic;

namespace Wsc2020Day2Session1.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public int UserTypeId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

    public virtual UserType UserType { get; set; } = null!;
}
