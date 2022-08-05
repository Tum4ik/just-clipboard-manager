using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tum4ik.JustClipboardManager.Data.Models;

[Table("clips")]
internal class Clip
{
  public int Id { get; set; }

  public byte[] Data { get; set; } = null!;
  public DateTime ClippedAt { get; set; }

  public virtual ICollection<ClipType> ClipTypes { get; set; } = null!;
}
