using System.ComponentModel.DataAnnotations;

namespace Server_TSD.Models.db_replData
{
    public class ScudCheckPoint
    {
        [Key]
        public int CodeApId { get; set; }
        public string NameApId { get; set; }
        public bool Hidden { get; set; }
    }
}
