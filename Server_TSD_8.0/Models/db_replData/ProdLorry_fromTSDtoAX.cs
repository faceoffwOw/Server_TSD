using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server_TSD.Models.db_replData
{
    public class ProdLorry_fromTSDtoAX
    {
        [Key]
        public string StoveReplId { get; set; }
        public string LorryNum { get; set; }
        public string EmplId { get; set; }
        public string ShiftId { get; set; }
        public string WrkCtrId { get; set; }
        public DateTime DateScan { get; set; }
        public string Department { get; set; }
        public bool isSortCompleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScanId { get; set; }
        public bool isPreScan { get; set; }
    }
}
