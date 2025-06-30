using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Server_TSD.Models.db_DaxBko
{
    public class SalesTable
    {
        [Key]
        public string SalesId { get; set; }
        public string NomNaryd { get; set; }
        public virtual SalesLine salesLines { get; set; }
        public int SalesType { get; set; }
    }

    public class SalesLine
    {
        //[ForeignKey("CompanyInfoKey")]
        [Key]
        public string SalesId { get; set; }
        public string NomNaryd { get; set; }
        public bool Base { get; set; }
        public string itemId { get; set; }
        public string InventDimId { get; set; }
        public decimal MarketingSalesQty { get; set; }
        public virtual SalesTable salesTables { get; set; }
    }

    public class InventDim
    {
        public string InventDimId { get; set; }
        public string ConfigId { get; set; }
        public string inventSerialId { get; set; }
        //public string inventBatchId { get; set; }
        public virtual SalesLine SalesLines { get; set; }
    }

    public class ModelSaleLine
    {
        public string NomNaryd { get; set; }
        //public string inventBatchId { get; set; }
        public string itemId { get; set; }
        public string ConfigId { get; set; }
        public string inventSerialId { get; set; }
        public decimal MarketingSalesQty { get; set; }
    }
}
