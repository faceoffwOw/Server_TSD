using Microsoft.AspNetCore.Mvc;
using Server_TSD.Models;
using Server_TSD.Models.db_DaxBko;
using Server_TSD.Models.db_replData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server_TSD.DTO;
using Server_TSD.Models.HelperClass;

namespace Server_TSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdLorryController : ControllerBase
    {
        dbReplDataContext db_ReplDataContext;
        db_DaxBkoContext db_DaxBkoContext;

        public ProdLorryController(dbReplDataContext _dbReplDataContext, db_DaxBkoContext _db_DaxBkoContext)
        {
            db_ReplDataContext = _dbReplDataContext;
            db_DaxBkoContext = _db_DaxBkoContext;
        }

        // GET: api/ProdLorry
        [HttpGet]
        public string Get()
        {
            return "ProdLorryController";
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<WrkCtrTable>> getWrkCtrList()
        {
            var result = await db_DaxBkoContext.WrkCtrTable
                        .OrderBy(x => x.WrkCtrId)
                        .Where(x => x.WrkCtrGroupId == "Сорт" && x.Dimension == "ОП-32" && x.IsGroup == 0)
                        .Select(x => x.WrkCtrId)
                        .ToArrayAsync();


            if (result.Count() <= 0)
                return NotFound();

            return Ok(result);
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<ProdShiftTable_UN>> getShiftList()
        {
            var result = await db_DaxBkoContext.ProdShiftTable_UN
                        .OrderBy(x => x.ShiftId)
                        .Where(x => x.Dimension == "ОП-32" && x.ShiftBase == 1)
                        .Select(x => x.ShiftId)
                        .ToListAsync();


            if (result.Count() <= 0)
                return NotFound();

            return Ok(result);
        }
        
        [HttpPost("scan")]
        public async Task<ActionResult<ProdLorry_fromTSDtoAX>> PostLorryScan([FromBody] ProdLorryScanRequest request)
        {
            //знак вопроса (?.) — это оператор условного null-проверки (null-conditional operator).
            //Он позволяет безопасно обращаться к членам объекта, который может быть null, без риска получить NullReferenceException
            // Валидация входных данных
            if (request?.LorryData == null)
            {
                return BadRequest("Данные вагонетки не предоставлены");
            }

            ProdLorry_fromTSDtoAX savedRecord;

            // Получаем границы смены для текущего ShiftId
            var (shiftStart, shiftEnd) = ShiftTimeHelper.GetShiftTimeRange(request.LorryData.ShiftId, DateTime.Now);


            // Проверка существования записи
            var existingRecord = await db_ReplDataContext.ProdLorry_fromTSDtoAX.FirstOrDefaultAsync(x => x.StoveReplId == request.LorryData.StoveReplId && 
                                                                                                    x.ShiftId == request.LorryData.ShiftId &&
                                                                                                    x.DateScan >= shiftStart &&
                                                                                                    x.DateScan <= shiftEnd);


            if (existingRecord != null)
            {
                if (!existingRecord.isPreScan)
                {
                    return Conflict($"Вагонетка {request.LorryData.LorryNum} уже была отсканирована ранее");
                }

                existingRecord.LorryNum = request.LorryData.LorryNum;
                existingRecord.EmplId = request.LorryData.EmplId;
                existingRecord.WrkCtrId = request.LorryData.WrkCtrId;
                existingRecord.Department = request.LorryData.Department;
                existingRecord.DateScan = request.LorryData.DateScan;
                existingRecord.isPreScan = request.LorryData.isPreScan;
                existingRecord.isSortCompleted = false;

                savedRecord = request.LorryData;
                savedRecord.ScanId = existingRecord.ScanId;
            }
            else
            {
                // Добавляем новую запись
                request.LorryData.isSortCompleted = false; // Маркируем как рассортировка не завершена
                db_ReplDataContext.ProdLorry_fromTSDtoAX.Add(request.LorryData);
                savedRecord = request.LorryData;
            }
            

            try
            {
                await db_ReplDataContext.SaveChangesAsync();
                return Ok(savedRecord);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Ошибка при сохранении данных: {ex.InnerException?.Message}");
            }
        }

        [HttpGet("shift-time")]
        public IActionResult TestShiftTimeRange(string shiftId, DateTime? checkTime = null)
        {
            try
            {
                var referenceTime = checkTime ?? DateTime.Now;
                var (shiftStart, shiftEnd) = ShiftTimeHelper.GetShiftTimeRange(shiftId, referenceTime);

                var result = new
                {
                    ShiftId = shiftId,
                    ReferenceTime = referenceTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ShiftStart = shiftStart.ToString("yyyy-MM-dd HH:mm:ss"),
                    ShiftEnd = shiftEnd.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsInsideShift = referenceTime >= shiftStart && referenceTime <= shiftEnd,
                    ShiftDuration = (shiftEnd - shiftStart).TotalHours + " hours"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }
    }
}
