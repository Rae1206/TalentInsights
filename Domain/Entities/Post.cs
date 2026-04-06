using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Post
{
    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
