using System;
using System.ComponentModel.DataAnnotations;

namespace Server_TSD.Models
{
    public class Label
    {        
        [Key]
        public Int64 UnitId { get; set; }
        public DateTime datePrint { get; set; }
        public string frakciya { get; set; }
        public string BatchNum { get; set; }
        public DateTime BatchData { get; set; }
        public string itemId { get; set; }
        public string StandardId { get; set; }
        public string ConfigId { get; set; }
        public string LineRecId { get; set; }
    }
}
