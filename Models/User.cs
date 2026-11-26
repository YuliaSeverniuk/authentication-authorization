using System.ComponentModel.DataAnnotations;

namespace TaskAuthenticationAuthorization.Models
{
    public static class Roles
    {
        public const string Admin = "admin";
        public const string Buyer = "buyer";

        public static readonly string[] All = { Admin, Buyer };
    }

    public static class BuyerTypes
    {
        public const string ClaimType = "buyerType";

        public const string None = "none";
        public const string Regular = "regular";
        public const string Golden = "golden";
        public const string Wholesale = "wholesale";

        public static readonly string[] All = { None, Regular, Golden, Wholesale };
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } = Roles.Buyer;

        [Required]
        [Display(Name = "Buyer type")]
        public string BuyerType { get; set; } = BuyerTypes.Regular;

        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
