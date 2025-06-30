using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using DateTimeConverter = Server_TSD.Models.HelperClass.Utils.DateTimeJsonConverter;

namespace Server_TSD.Models.db_replData
{
    public class InventLabel_fromTSDtoAx
    {
        [Key]
        public string UnitId { get; set; }        
        public DateTime inventDate { get; set; }
        public string itemId { get; set; }
        public string ItemIdName { get; set; }
        public string Frakcia { get; set; }
        public string BatchNum { get; set; }
        [JsonConverter(typeof(DateTimeConverter))] // Кастомный конвертер
        public DateTime BatchDate { get; set; }
        public string fromInventLocationId { get; set; }
        public string fromwMSLocationId { get; set; }
        public string StandardId { get; set; }
        public string ConfigId { get; set; }
        public string JournalId { get; set; }
        public int Count { get; set; }
        public float QtyGood { get; set; }
        public int QtyTrayIn { get; set; }
        [JsonConverter(typeof(DateTimeConverter))] // Кастомный конвертер
        public DateTime DateScan { get; set; }
        public float Weight { get; set; }
    }
}
