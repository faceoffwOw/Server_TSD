using System.ComponentModel.DataAnnotations;

namespace Server_TSD.Models.db_DaxBko
{
    public class WrkCtrTable
    {
        [Key]
        public string WrkCtrId { get; set; }
        public string WrkCtrGroupId { get; set; }
        public int IsGroup { get; set; }
        public string Dimension { get; set; }
    }

    public class ProdShiftTable_UN
    {
        [Key]
        public string ShiftId { get; set; }
        public int ShiftType { get; set; }
        public string Dimension { get; set; }
        public int ShiftBase { get; set; }
    }
}
