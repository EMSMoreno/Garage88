using System.ComponentModel.DataAnnotations;

namespace Garage88.Data.CustomValidation
{
    public class DayAfterCurrentTimeValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            return dateTime > DateTime.Now;
        }
    }
}
