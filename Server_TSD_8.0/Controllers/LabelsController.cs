using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_TSD.Models;
using Server_TSD.Models.db_DaxBko;
using Server_TSD.Models.db_replData;
using Server_TSD.Models.HelperClass;

namespace Server_TSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        LabelContext db_Label;
        dbReplDataContext db_ReplDataContext;
        db_DaxBkoContext db_DaxBkoContext;

        public LabelsController(LabelContext _labelContext, dbReplDataContext _dbReplDataContext, db_DaxBkoContext _db_DaxBkoContext)
        {
            db_Label = _labelContext;
            db_ReplDataContext = _dbReplDataContext;
            db_DaxBkoContext = _db_DaxBkoContext;
        }

        // GET: api/labels
        [HttpGet]
        public string Get()
        {
            return "TEST";
        }

        [HttpGet("{UnitId}")]
        public async Task<ActionResult<Label>> Get(int UnitId)
        {
            Label label = await db_Label.Labels.FirstOrDefaultAsync(x => x.UnitId == UnitId);
            if (label == null)
                return NotFound();

            return new ObjectResult(label);
        }

        // GET: api/labels/GetLocation
        //получим склады
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<InventLocation>> GetLocation()
        {
            List<InventLocation> InventLocations = await db_ReplDataContext.InventLocation.Where(x => x.InventLocationId.Contains("ГП-07")).ToListAsync();

            if (InventLocations == null)
                return NotFound();

            return new ObjectResult(InventLocations);
        }

        // GET: api/labels/getWmsLocation
        //получим местоположение
        [Route("[action]/{LocationId}")]
        [HttpGet]
        public async Task<ActionResult<wmsLocation>> GetWmsLocation(string LocationId)
        {
            //List<wmsLocation> wmsLocations = await db_ReplDataContext.WmsLocation.Where(x => x.inventLocationId.Contains("ГП-07")).ToListAsync();
            List<wmsLocation> wmsLocations = await db_ReplDataContext.WmsLocation.Where(x => x.inventLocationId == LocationId).ToListAsync();
            
            if (wmsLocations == null)
                return NotFound();

            return new ObjectResult(wmsLocations);
        }

        // GET: api/labels/getSalesLine
        //получим строки salesLine
        [Route("[action]/{_NomNaryd}")]
        [HttpGet]
        public async Task<ActionResult<SalesTable>> getSalesLine(string _NomNaryd)
        {
            //ЭТО ЛАЙК НА НАРЯДЫ, НО ЛУЧШЕ ЕГО НЕ ИСПОЛЬЗОВАТЬ, ОЧЕНЬ ПЛОХАЯ ПРОИЗВОДИТЕЛЬНОСТЬ
            var result = from salesTable in db_DaxBkoContext.SalesTable 
                             //where  salesTable.NomNaryd.Contains(_NomNaryd)                             
                             where  salesTable.NomNaryd == _NomNaryd && salesTable.SalesType == 1
                         //join salesLine in db_DaxBkoContext.SalesLine on salesTable.SalesId equals salesLine.SalesId
                         join salesLine in db_DaxBkoContext.SalesLine on salesTable.NomNaryd equals salesLine.NomNaryd
                            where salesLine.Base                            
                         join inventDim in db_DaxBkoContext.InventDim on salesLine.InventDimId equals inventDim.InventDimId

                         select new ModelSaleLine
                         {
                             NomNaryd = salesTable.NomNaryd,
                             itemId = salesLine.itemId,
                             ConfigId = inventDim.ConfigId,
                             inventSerialId = inventDim.inventSerialId,
//                             inventBatchId = inventDim.inventBatchId,
                             MarketingSalesQty = salesLine.MarketingSalesQty,
                         };            

            if (result.Count() <= 0)//нет данных по такому наряду
                return NotFound();            

            return new ObjectResult(await result.ToListAsync());
        }

        // GET: api/labels/getNaryd
        //получим информацию о наряде
        [Route("[action]/{_NomNaryd}")]
        [HttpGet]
        public async Task<ActionResult<SalesTable>> getNaryd(string _NomNaryd)
        {
            var result = await db_DaxBkoContext.SalesTable
                         .Where(x => x.NomNaryd == _NomNaryd)
                         .Select(x => new SalesTable
                         {
                             SalesId = x.SalesId,
                             NomNaryd = x.NomNaryd,
                             SalesType = x.SalesType,
                         }).ToListAsync();
                         
            
            if(result.Count() <= 0)
                return NotFound();

            return new ObjectResult(result);
        }

        //получим отсканированные бирки
        //api/labels/cntScan/
        [Route("[action]/{_NomNaryd}")]
        [HttpGet]
        [HttpPost]
        public async Task<ActionResult<IJT_fromTSDtoAx>> cntScan(string _NomNaryd)
        {
            var result = await db_ReplDataContext.IJT_fromTSDtoAx
                            .Where(i => i.NomNaryd == _NomNaryd)
                            .GroupBy(g => new { g.NomNaryd})
                            //.GroupBy(g =>  new { g.NomNaryd, g.BatchNum})
                            .Select(i => new IJT_fromTSDtoAx
                            {        
                                //BatchNum = i.Key.BatchNum,
                                Count = i.Count(),
                            }).ToListAsync();            

            return Ok(result);
        }

        //получим отсканированные бирки
        //api/labels/cntScanAx/
        [Route("[action]/{_NomNaryd}")]
        [HttpGet]
        [HttpPost]
        public async Task<ActionResult<IJT_fromTSDtoAx>> cntScanAx(string _NomNaryd)
        {
            var result = await db_ReplDataContext.IJT_fromTSDtoAx
                            .Where(i => i.NomNaryd == _NomNaryd)
                            .GroupBy(g => new { g.BatchNum, g.ConfigId })
                            .Select(i => new IJT_fromTSDtoAx
                            {
                                BatchNum = i.Key.BatchNum,
                                Count = i.Count(),

                                SaleQty = i.Sum(s => s.SaleQty),
                                QtyTrayIn = i.Sum(s => s.QtyTrayIn),
                                ConfigId = i.Key.ConfigId

                            }).ToListAsync();

            return Ok(result);
            //var result = await db_ReplDataContext.IJT_fromTSDtoAx
            //                .Where(i => i.NomNaryd == _NomNaryd)                            
            //                .GroupBy(g =>  new { g.BatchNum, /*g.ConfigId*/})
            //                .Select(i => new IJT_fromTSDtoAx
            //                {
            //                    BatchNum = i.Key.BatchNum,                                
            //                    Count = i.Count(),
            //                    SaleQty = i.Sum(s => s.SaleQty),
            //                    //ConfigId = i.Key.ConfigId

            //                }).ToListAsync();

            //return Ok(result);
        }

        //получим отсканированные бирки
        //api/labels/cntScanAx/
        [Route("[action]/{_NomNaryd}")]
        [HttpGet]
        [HttpPost]
        public async Task<ActionResult<IJT_fromTSDtoAx>> cntScanAxV2(string _NomNaryd)
        {
            var result = await db_ReplDataContext.IJT_fromTSDtoAx
                            .Where(i => i.NomNaryd == _NomNaryd)
                            .GroupBy(g => new { g.BatchNum, g.ConfigId})
                            .Select(i => new IJT_fromTSDtoAx
                            {
                                BatchNum = i.Key.BatchNum,
                                Count = i.Count(),
                                
                                SaleQty = i.Sum(s => s.SaleQty),
                                QtyTrayIn = i.Sum(s => s.QtyTrayIn),
                                ConfigId = i.Key.ConfigId

                            }).ToListAsync();

            return Ok(result);
        }

        //вставим отсканированую бирку
        //api/labels/
        [HttpPost]
        public async Task<ActionResult<IJT_fromTSDtoAx>> Post([FromBody] IJT_fromTSDtoAx IJT_fromTSDtoAx)
        {            
            if (IJT_fromTSDtoAx == null)
            {
                return BadRequest();
            }

            //проверка на дубликат бирок
            IJT_fromTSDtoAx IJT_fromTSDtoAxs = db_ReplDataContext.IJT_fromTSDtoAx.FirstOrDefault(x => x.UnitId == IJT_fromTSDtoAx.UnitId);

            if (IJT_fromTSDtoAxs != null)                
                return StatusCode(StatusCodes.Status409Conflict);

            db_ReplDataContext.IJT_fromTSDtoAx.Add(IJT_fromTSDtoAx);
            await db_ReplDataContext.SaveChangesAsync();
            return Ok(IJT_fromTSDtoAx);
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------------------
        /// -------------------------------------------------------------------------------------------------------------------------------------
        /// ------------------------------------------------------------------------------------------------------------------------------------
        /// ВСЕ НОВЫЕ МЕТОДЫ ДЛЯ НОВОЙ ВЕРСИИ ПРОГРАММЫ, КОГДА УБЕРУТЬСЯ ГРИХИНЫ БИРКИ, ПЕРЕДЕЛАТЬ 7 ЦЕХ ПО НОВОМУ        
        /// </summary>
        /// <param name="_LocationId"></param>
        /// <returns></returns>
        
        // GET: api/labels/Location
        //получим склады
        [Route("[action]/{_LocationId}")]
        [HttpGet]
        public async Task<ActionResult<InventLocation>> Location(string _LocationId)
        {
            List<InventLocation> InventLocations = await db_ReplDataContext.InventLocation.Where(x => x.InventLocationId.Contains(_LocationId)).ToListAsync();

            if (InventLocations == null)
                return NotFound();

            return new ObjectResult(InventLocations);
        }

        [HttpGet("[action]/{_UnitId}")]
        public async Task<ActionResult<ProdLabelsNum>> LabelAx(string _UnitId)
        {
            ProdLabelsNum label = await db_DaxBkoContext.ProdLabelsNum.FirstOrDefaultAsync(x => x.LabelsNumId == _UnitId);           

            if (label == null)
                return NotFound();

            return new ObjectResult(label);
        }

        //вставим отсканированую бирку
        //api/labels/
        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<IJT_fromTSDtoAx>> PostV2(IJT_fromTSDtoAx IJT_fromTSDtoAx)
        {            
            if (IJT_fromTSDtoAx == null)
            {
                return BadRequest();
            }

            //проверка на дубликат бирок
            IJT_fromTSDtoAx duplicate_IJT_fromTSDtoAxs = db_ReplDataContext.IJT_fromTSDtoAx.SingleOrDefault(x => x.UnitId == IJT_fromTSDtoAx.UnitId);

            if (duplicate_IJT_fromTSDtoAxs != null)                
                return StatusCode(StatusCodes.Status409Conflict);
            
            db_ReplDataContext.IJT_fromTSDtoAx.Add(IJT_fromTSDtoAx);
            await db_ReplDataContext.SaveChangesAsync();

            return Ok(IJT_fromTSDtoAx);
        }

        [Route("[action]/{NomNaryd}")]
        [HttpGet]
        [HttpPost]
        public async Task<ActionResult<IJT_fromTSDtoAx>> Delete(string NomNaryd)
        {

            List<IJT_fromTSDtoAx> labels = await db_ReplDataContext.IJT_fromTSDtoAx.Where(x => x.NomNaryd == NomNaryd).ToListAsync();

            if (labels == null || labels.Count <= 0)
            {
                return NotFound();
            }

            foreach (var item in labels)
            {
                db_ReplDataContext.IJT_fromTSDtoAx.Remove(item);
            }

            await db_ReplDataContext.SaveChangesAsync();

            return Ok(labels.Count);           
        }

        [HttpGet("[action]")]
        public ActionResult<model_Connect_db> checkDbConnect()
        {            
            List<model_Connect_db> list_db = new List<model_Connect_db>();

            list_db.Add(new model_Connect_db { NameDb = "db_DaxBko", isConnect = true });
            list_db.Add(new model_Connect_db { NameDb = "db_ReplData", isConnect = true });

                foreach (var item in list_db)
                {
                    switch (item.NameDb)                     
                    {
                        case "db_DaxBko":
                            try
                            {
                                db_DaxBkoContext.Database.OpenConnection();
                                db_DaxBkoContext.Database.CloseConnection();
                                item.isConnect = true;
                            }
                            catch { item.isConnect = false; }
                        
                            break;
                        case "db_ReplData":
                            try
                            {
                                db_ReplDataContext.Database.OpenConnection();
                                db_ReplDataContext.Database.CloseConnection();
                                item.isConnect = true;
                            }
                            catch {
                                item.isConnect = false;
                            }
                            break;
                    }
                }                
                        
            return Ok(list_db.ToList());
        }

        // PUT api/labels/
        /*
        [HttpPut]
        public async Task<ActionResult<Label>> Put(Label label)
        {
            if (label == null)
            {
                return BadRequest();
            }
            if (!db.Labels.Any(x => x.UnitId == label.UnitId))
            {
                return NotFound();
            }

            db.Update(label);
            await db.SaveChangesAsync();
            return Ok(label);
        }
        */
    }
}