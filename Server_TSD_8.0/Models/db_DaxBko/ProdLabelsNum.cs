using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server_TSD.Models.db_DaxBko
{
    public class ProdLabelsNum
    {
        [Key]
        public string LabelsNumId { get; set; }//номер бирки
        public string JournalId { get; set; }//журнал предъявления или паспортизации
        public string ItemId { get; set; }//номенклатурный номер
        public string ItemNumberId { get; set; }//configId, номер изделия 
        public string BatchId { get; set; }//номер партии
        public int QtyTrayIn { get; set; }//кол-во шт. в поддоне
        public decimal QtyGood { get; set; }// нетто
        public string ItemName { get; set; }//Наименованование
        public string ItemNumName { get; set; }//фракция
        public DateTime BatchDate { get; set; }//Дата партии

    }
}
