using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    // Parameterless constructor for deserialization
    public Category()
    {
        Articles = new HashSet<Article>();
    }

    public Category(
        Guid userId, 
        string name): this()
    {
        UserId = userId;
        Id = Guid.NewGuid();
        Name = name;
    }

}