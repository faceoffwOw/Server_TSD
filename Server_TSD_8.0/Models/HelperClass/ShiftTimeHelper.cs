namespace Server_TSD.Models.HelperClass
{
    public class ShiftTimeHelper
    {
        public static (DateTime Start, DateTime End) GetShiftTimeRange(string shiftId, DateTime referenceTime)
        {
            var date = referenceTime.Date;

            return shiftId switch
            {
                "3201" => (
                    // Начало смены
                    referenceTime.TimeOfDay < new TimeSpan(7, 30, 0)
                        ? date.AddDays(-1).AddHours(23).AddMinutes(30)
                        : date.AddHours(23).AddMinutes(30),

                    // Конец смены - всегда через 8 часов от начала
                    referenceTime.TimeOfDay < new TimeSpan(7, 30, 0)
                        ? date.AddHours(7).AddMinutes(30)  // Текущий день 7:30
                        : date.AddDays(1).AddHours(7).AddMinutes(30)  // Следующий день 7:30
                ),
                "3202" => (date.AddHours(7).AddMinutes(30), date.AddHours(15).AddMinutes(30)),
                "3203" => (date.AddHours(15).AddMinutes(30), date.AddHours(23).AddMinutes(30)),
                _ => throw new ArgumentException("Неизвестный код смены")
            };
        }
    }
}
