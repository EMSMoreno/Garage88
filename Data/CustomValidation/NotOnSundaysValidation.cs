using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.CustomValidation
{
    public class NotOnSundaysValidation : ValidationAttribute
    {
        /// <summary>
        /// Check if the date entered is not a Sunday
        /// </summary>
        /// <param name="value">DateTime</param>
        /// <returns>True if the date is not Sunday</returns>

        public override bool IsValid(object value)
        {
            DateTime date = (DateTime)value;

            bool notSunday = date.DayOfWeek != DayOfWeek.Sunday;

            return notSunday;
        }
    }
}
