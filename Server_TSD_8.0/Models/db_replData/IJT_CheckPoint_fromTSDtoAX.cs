using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using DateTimeConverter = Server_TSD.Models.HelperClass.Utils.DateTimeJsonConverter;

namespace Server_TSD.Models.db_replData
{
    public class IJT_CheckPoint_fromTSDtoAX
    {
        [Key]
        public string JournalId { get; set; }
        public string fromInventLocationId { get; set; }
        public string toInventLocationId { get; set; }
        public string CheckPointName { get; set; }
        [JsonConverter(typeof(DateTimeConverter))] // Кастомный конвертер
        public DateTime DateScan { get; set; }
        public bool isAllowDeparture { get; set; }
    }
}
