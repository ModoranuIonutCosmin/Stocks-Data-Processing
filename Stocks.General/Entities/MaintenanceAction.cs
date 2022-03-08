using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksProccesing.Relational.Model
{
    public class MaintenanceAction
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Schedule { get; set; }

        [Column(TypeName = "bigint")]
        public long Interval { get; set; }
        public DateTimeOffset LastFinishedDate { get; set; }
    }
}
