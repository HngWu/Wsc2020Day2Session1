using System;
using System.Collections.Generic;

namespace Wsc2020Day2Session1.Models;

public partial class UserType
{
    public int Id { get; set; }

    public string UserTypeName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
