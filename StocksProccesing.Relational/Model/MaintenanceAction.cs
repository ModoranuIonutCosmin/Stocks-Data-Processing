using System;
using System.ComponentModel.DataAnnotations;

namespace StocksProccesing.Relational.Model
{
    public class MaintenanceAction
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Type { get; set; }
        public DateTimeOffset LastFinishedDate { get; set; }
    }
}
