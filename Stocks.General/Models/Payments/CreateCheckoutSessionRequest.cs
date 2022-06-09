using System.ComponentModel.DataAnnotations;

namespace Stocks.General.Models.Payments
{
    public class CreateCheckoutSessionRequest
    {
        [Required]
        [MaxLength(200)]
        public string PriceId { get; set; }

        [Required]
        [MaxLength(4096)]
        public string FrontEndSuccesUrl { get; set; }

        [Required]
        [MaxLength(4096)]
        public string FrontEndFailureUrl { get; set; }
    }
}
