using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.CustomValidation
{
    public class ScheduleValidation : ValidationAttribute
    {
        /// <summary>
        /// Check if the appointment is made during business hours
        /// </summary>
        /// <param name="value">Datetime</param>
        /// <returns>True if the appointment is during business hours</returns>

        public override bool IsValid(object value)
        {

            var shopOpenningHours = 9;

            var shopClosingHours = 18;

            var dateTime = Convert.ToDateTime(value);

            if (dateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                shopClosingHours = 13;
            }

            if (dateTime.Hour < shopOpenningHours || dateTime.Hour > shopClosingHours)
            {
                return false;
            }

            return true;
        }
    }
}
