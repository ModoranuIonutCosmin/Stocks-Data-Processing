using System.ComponentModel.DataAnnotations;

namespace Stocks.General.Models.Payments
{
    public class CreateCheckoutSessionResponse
    {
        [Required]
        [MaxLength(200)]
        public string SessionId { get; set; }

        public string PublicKey { get; set; }
    }
}
