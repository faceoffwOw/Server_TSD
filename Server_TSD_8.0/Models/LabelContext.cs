using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_TSD.Models
{
    public class LabelContext : DbContext
    {
        //подключение к таблице, куда будем вставлять отсканированные бирки
        public DbSet<Label> Labels { get; set; }


        public LabelContext(DbContextOptions<LabelContext> options) : base(options)
        { }
    }
}
