using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tum4ik.JustClipboardManager.Data.Models;

[Table("clip_types")]
[Index(nameof(Name), IsUnique = true)]
internal class ClipType
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public virtual ICollection<Clip> Clips { get; set; } = null!;
}
