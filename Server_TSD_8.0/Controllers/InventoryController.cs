using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_TSD.Models;
using Server_TSD.Models.db_replData;

namespace Server_TSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        dbReplDataContext db_ReplDataContext;

        public InventoryController(dbReplDataContext _dbReplDataContext)
        {            
            db_ReplDataContext = _dbReplDataContext;            
        }

        // GET: api/Inventory
        [HttpGet]
        public string Get()
        {
            return "InventoryController";
        }

        // GET: api/labels/GetLocation
        //получим бирку
        [Route("[action]/{curInventDate}")]
        [HttpGet]
        public async Task<ActionResult<InventLabel_fromTSDtoAx>> getLabels(DateTime curInventDate)
        {
            curInventDate = startDateMonth(curInventDate);

            List<InventLabel_fromTSDtoAx> inventLabel = await db_ReplDataContext.InventLabel_fromTSDtoAx.Where(x => x.inventDate.Date == curInventDate).ToListAsync();
                                                                                        //.Where(y => y.UnitId.Contains("test")).ToListAsync();

            if (inventLabel == null)
                return NotFound();

            return new ObjectResult(inventLabel);
        }

        // GET: api/labels/GetLocation
        //получим бирку
        [Route("[action]/{curInventDate}/{inventLocationId}")]
        [HttpGet]
        public async Task<ActionResult<InventLabel_fromTSDtoAx>> getLabels_Test(DateTime curInventDate, string inventLocationId)
        {
            curInventDate = startDateMonth(curInventDate);

            List<InventLabel_fromTSDtoAx> inventLabel = await db_ReplDataContext.InventLabel_fromTSDtoAx.Where(x => x.inventDate.Date == curInventDate)
                                                                                                        .Where(w => w.fromInventLocationId.Contains(inventLocationId)).ToListAsync();
            //.Where(y => y.UnitId.Contains("test")).ToListAsync();

            if(inventLabel == null)
                return NotFound();

            return new ObjectResult(inventLabel);
        }

        // GET: api/labels/getLabelsGroupBy
        //получим бирку
        [Route("[action]/{curInventDate}")]
        [HttpGet]
        public async Task<ActionResult<InventLabel_fromTSDtoAx>> getLabelsGroupBy(DateTime curInventDate)
        {
            curInventDate = startDateMonth(curInventDate);

            List<InventLabel_fromTSDtoAx> inventLabel = await db_ReplDataContext.InventLabel_fromTSDtoAx.Where(x => x.inventDate.Date == curInventDate)                                                                                                        
                                                                                                        .GroupBy(d => new { d.itemId, d.ItemIdName, d.Frakcia, d.BatchNum, d.BatchDate, d.ConfigId, d.QtyTrayIn }).Select(
                                                                                                        s => new InventLabel_fromTSDtoAx
                                                                                                        {
                                                                                                            UnitId = "0",
                                                                                                            itemId = s.Key.itemId,
                                                                                                            ItemIdName = s.Key.ItemIdName,
                                                                                                            Frakcia = s.Key.Frakcia,
                                                                                                            BatchNum = s.Key.BatchNum,
                                                                                                            BatchDate = s.Key.BatchDate,
                                                                                                            ConfigId = s.Key.ConfigId,
                                                                                                            Count = s.Sum(x => x.Count),
                                                                                                            QtyTrayIn = s.Sum(x => x.QtyTrayIn),
                                                                                                            QtyGood = (float)Math.Round( s.Sum( x => x.QtyGood), 3),

                                                                                                        }).ToListAsync();
            //.Where(y => y.UnitId.Contains("test")).ToListAsync();

            if (inventLabel == null)
                return NotFound();

            return new ObjectResult(inventLabel);
        }

        //вставим отсканированую бирку
        //api/Inventory/
        [HttpPost]
        public async Task<ActionResult<InventLabel_fromTSDtoAx>> Post(InventLabel_fromTSDtoAx inventLabel)
        {
            if (inventLabel == null)
            {
                return BadRequest();
            }

            //проверка на дубликат бирок
            InventLabel_fromTSDtoAx checkInventLabel = db_ReplDataContext.InventLabel_fromTSDtoAx.Where(x => x.UnitId == inventLabel.UnitId)
                                                                                                 .Where(y => y.inventDate == inventLabel.inventDate).SingleOrDefault();

            if (checkInventLabel != null)
                return StatusCode(StatusCodes.Status409Conflict);

            db_ReplDataContext.InventLabel_fromTSDtoAx.Add(inventLabel);

            await db_ReplDataContext.SaveChangesAsync();

            return Ok(inventLabel);
        }
        //Обновить бирку
        // PUT api/Inventory/     
        [Route("[action]/{_curInventDate}")]
        [HttpPost]
        public async Task<ActionResult<InventLabel_fromTSDtoAx>> Update(InventLabel_fromTSDtoAx inventLabel, DateTime _curInventDate)
        {
            _curInventDate = startDateMonth(_curInventDate);

            if (inventLabel == null)
            {
                return BadRequest();
            }

            if (!db_ReplDataContext.InventLabel_fromTSDtoAx.Any(x => x.UnitId == inventLabel.UnitId && x.inventDate == _curInventDate))
            {
                return NotFound();
            }

            db_ReplDataContext.Update(inventLabel);

            await db_ReplDataContext.SaveChangesAsync();

            return Ok(inventLabel);
        }

        [HttpGet("[action]/{_InventDate}/{_UnitId}")]
        public async Task<ActionResult<InventLabel_fromTSDtoAx>> getLabel(DateTime _InventDate, string _UnitId)
        {
            _InventDate = startDateMonth(_InventDate);

            InventLabel_fromTSDtoAx label = await db_ReplDataContext.InventLabel_fromTSDtoAx.Where(y => y.inventDate == _InventDate)
                                                                                            .Where(x => x.UnitId == _UnitId).SingleOrDefaultAsync();

            if (label == null)
                return NotFound();

            return new ObjectResult(label);
        }

        private DateTime startDateMonth(DateTime _dateTime)
        {
            DateTime dateTime = new DateTime(_dateTime.Year, _dateTime.Month, 1);

            return dateTime;
        }
    }
}