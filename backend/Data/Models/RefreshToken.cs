
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } 
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    [ForeignKey("UserId")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}