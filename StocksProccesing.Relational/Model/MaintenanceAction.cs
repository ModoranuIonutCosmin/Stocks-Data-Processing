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
        public string Type { get; set; }

        [Column(TypeName = "bigint")]
        public int ReccurencyTimeSpanTicks { get; set; }
        public DateTimeOffset LastFinishedDate { get; set; }
    }
}
