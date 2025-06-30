using Microsoft.EntityFrameworkCore;
using Server_TSD.Models.db_replData;

namespace Server_TSD.Models
{
    public class dbReplDataContext : DbContext
    {
        
        public DbSet<IJT_fromTSDtoAx> IJT_fromTSDtoAx { get; set; }//подключение в репликационную таблицу для отгрузки, куда будем вставлять отсканированные бирки
        public DbSet<InventLabel_fromTSDtoAx> InventLabel_fromTSDtoAx { get; set; }//подключение в репликационную таблицу для инвентаризации, куда будем вставлять отсканированные бирки
        public DbSet<InventLocation> InventLocation { get; set; }//получим склады
        public DbSet<wmsLocation> WmsLocation { get; set; }//получим Местоположения
        public DbSet<ScudCheckPoint> ScudCheckPoint { get; set; }//получим список КПП
        public DbSet<IJT_CheckPoint_fromTSDtoAX> IJT_CheckPoint_FromTSDtoAX { get; set; } //подключение в репликационную таблицу для КПП, куда будем вставлять отсканированные бирки
        public DbSet<ProdLorry_fromTSDtoAX> ProdLorry_fromTSDtoAX { get; set; } //подключение в репликационную таблицу для вагонеток, куда будет вставлять отсканированные бирки

        public dbReplDataContext(DbContextOptions<dbReplDataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventLabel_fromTSDtoAx>()
                .HasKey(c => new { c.UnitId, c.inventDate });
        }
    }
}
