using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_TSD.Models;
using Server_TSD.Models.db_DaxBko;
using Server_TSD.Models.db_replData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_TSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        dbReplDataContext db_ReplDataContext;

        public TransferController(dbReplDataContext _dbReplDataContext)
        {
            db_ReplDataContext = _dbReplDataContext;
        }

        // GET: api/Transfer
        [HttpGet]
        public string Get()
        {
            return "TransferController";
        }

        // GET: api/transfer/getCheckpointList
        //получим список КПП
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<ScudCheckPoint>> getCheckpointList()
        {
            List<string> scudCheckPoints = new List<string>();

            var result = await db_ReplDataContext.ScudCheckPoint.OrderBy(x => x.NameApId)
                         .Where(x => x.NameApId.Contains("шлагбаум"))
                         .Select(x => new ScudCheckPoint
                         {
                             CodeApId = x.CodeApId,
                             NameApId = x.NameApId,
                             Hidden = x.Hidden,
                         }).ToListAsync();


            if (result.Count() <= 0)
                return NotFound();

            //Чтобы убрать дубли переберем список
            /*foreach (var item in result)
            {
                int startIdx = item.NameApId.IndexOf("КПП №", 0);
                string checkName = item.NameApId.Substring(startIdx, startIdx + 7);

                var match = scudCheckPoints.FirstOrDefault(stringToCheck => stringToCheck.Contains(checkName));

                if (match == null)
                    scudCheckPoints.Add(item.NameApId.Trim());
            }*/

            //В зарплатной аксапте отказались заводить КПП без оборудования
            //Выбирают не то КПП...
            scudCheckPoints.Add("КПП №7 шлагбаум (ЦС)");
            scudCheckPoints.Add("КПП №20 металлосклад");
            //scudCheckPoints.Add("КПП №36 (БЗДС)");
            //scudCheckPoints.Add("КПП №37 (БЗДС)");

            return new ObjectResult(scudCheckPoints);
        }

        //вставим отсканированую бирку
        //api/transfer/
        [HttpPost]
        public async Task<ActionResult<IJT_CheckPoint_fromTSDtoAX>> Post([FromBody] IJT_CheckPoint_fromTSDtoAX IJT_CheckPoint_fromTSDtoAX)
        {
            if (IJT_CheckPoint_fromTSDtoAX == null)
            {
                return BadRequest();
            }

            //проверка на дубликат бирок
            IJT_CheckPoint_fromTSDtoAX IJT_CheckPoint_FromTSDtoAX_exist = db_ReplDataContext.IJT_CheckPoint_FromTSDtoAX.FirstOrDefault(x => x.JournalId == IJT_CheckPoint_fromTSDtoAX.JournalId);

            //Если нашли бирку, проверим разрешен ли повторный выезд
            if (IJT_CheckPoint_FromTSDtoAX_exist != null)
            {
                if (IJT_CheckPoint_FromTSDtoAX_exist.isAllowDeparture)
                {
                    //Обновим данные
                    IJT_CheckPoint_FromTSDtoAX_exist.fromInventLocationId = IJT_CheckPoint_fromTSDtoAX.fromInventLocationId;
                    IJT_CheckPoint_FromTSDtoAX_exist.toInventLocationId = IJT_CheckPoint_fromTSDtoAX.toInventLocationId;
                    IJT_CheckPoint_FromTSDtoAX_exist.CheckPointName = IJT_CheckPoint_fromTSDtoAX.CheckPointName;
                    IJT_CheckPoint_FromTSDtoAX_exist.DateScan = IJT_CheckPoint_fromTSDtoAX.DateScan;
                    IJT_CheckPoint_FromTSDtoAX_exist.isAllowDeparture = false;
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict);
                }
            }
            else
            {
                //Добавим в запись в таблицу
                db_ReplDataContext.IJT_CheckPoint_FromTSDtoAX.Add(IJT_CheckPoint_fromTSDtoAX);
            }

            await db_ReplDataContext.SaveChangesAsync();

            return Ok(IJT_CheckPoint_fromTSDtoAX);
        }
    }
}
