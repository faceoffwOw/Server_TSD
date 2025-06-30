using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using DateTimeConverter = Server_TSD.Models.HelperClass.Utils.DateTimeJsonConverter;

namespace Server_TSD.Models
{
    public class IJT_fromTSDtoAx
    {
        [Key]
        public string UnitId { get; set; }
        public string itemId { get; set; }
        public string BatchNum { get; set; }
        [JsonConverter(typeof(DateTimeConverter))] // Кастомный конвертер
        public DateTime BatchDate { get; set; }
        public string fromInventLocationId { get; set; }        
        public string? toInventLocationId { get; set; } // ? - Теперь может быть null
        public string fromwMSLocationId { get; set; }
        public string? towMSLocationId { get; set; } // ? - Теперь может быть null        
        public string? StandardId { get; set; } // ? - Теперь может быть null
        public string ConfigId { get; set; }
        public string JournalId { get; set; }
        public int Count { get; set; }
        [JsonConverter(typeof(DateTimeConverter))] // Кастомный конвертер
        public DateTime DateScan { get; set; }
        public string NomNaryd {get; set;}
        public float SaleQty { get; set; }
        public int QtyTrayIn { get; set; }//кол-во шт. в поддоне
    }  

}
