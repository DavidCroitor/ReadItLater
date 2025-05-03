using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User;

    public class RequestRefreshToken
    {
        [Required]
        public string AccessToken { get; set; } // The expired one

        [Required]
        public string RefreshToken { get; set; } // The one to validate
            
    }