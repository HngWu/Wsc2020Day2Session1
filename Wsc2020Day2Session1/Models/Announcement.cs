using System;
using System.Collections.Generic;

namespace Wsc2020Day2Session1.Models;

public partial class Announcement
{
    public int Id { get; set; }

    public DateTime Announcementdate { get; set; }

    public string AnnouncementTitle { get; set; } = null!;

    public string AnnouncementDescription { get; set; } = null!;
}
