using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class Article
{

    public Guid Id { get; private set; }

    [Required]
    public Guid UserId { get; set; }
    public string URL { get; set; }

    public string Title { get; set; } = String.Empty;

    public bool IsRead { get; set; }

    public bool IsFavourite { get; set; }

    public String Content { get; set; } = String.Empty;

    public DateTime SavedAt { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();


    public Article()
    {
        Id = Guid.NewGuid();
        UserId = Guid.Empty;
        URL = string.Empty;
        Title = string.Empty;
        IsRead = false;
        IsFavourite = false;
        SavedAt = DateTime.UtcNow;
        Content = string.Empty;
        Categories = new HashSet<Category>();
    }

    public Article(
        Guid userId,
        string url,
        string title = "",
        string content = "")
        : this()
    {
        UserId = userId;
        URL = url;
        Title = title;
        Content = content;
    }
}

