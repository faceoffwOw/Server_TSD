using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server_TSD.Models.db_replData
{
    public class InventLocation
    {
        [Key]
        public string InventLocationId { get; set; }
        public string Name { get; set; }
    }
}
