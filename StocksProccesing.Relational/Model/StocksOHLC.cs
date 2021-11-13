using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model
{
    public class StocksOHLC
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 4)")]
        public decimal High { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 4)")]
        public decimal Low { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 4)")]
        public decimal OpenValue { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 4)")]
        public decimal CloseValue { get; set; }

        public DateTimeOffset Date { get; set; }

        [Required]
        [Column(TypeName = "bigint")]
        public long Period { get; set; }

        [MaxLength(10)]
        public string CompanyTicker { get; set; }

        public Company Company { get; set; }
    }
}
