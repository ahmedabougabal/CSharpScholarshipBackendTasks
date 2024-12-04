using System.ComponentModel.DataAnnotations;

namespace eCommerce.Models.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }

        [Required]
        [Compare("Password")]
        public required string ConfirmPassword { get; set; }

        [Required]
        public required string UserName { get; set; }
    }
}
