using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }

    public bool IsEmailVerified { get; set; } = false;

    // Navigation property for the user's articles
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    // Navigation property for the user's categories
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    // Navigation property for the user's refresh tokens
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // Parameterless constructor for EF Core
    public User()
    {
        Articles = new HashSet<Article>();
        Categories = new HashSet<Category>();
        RefreshTokens = new HashSet<RefreshToken>();
    }

    public User(string username, string email) : this()
    {
        Id = Guid.NewGuid();
        Email = email;
        UserName = username;
        CreatedAt = DateTime.UtcNow;
    }
}