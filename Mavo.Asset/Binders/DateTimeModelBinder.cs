using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mavo.Assets.Binders
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        private Nullable<T> GetA<T>(ModelBindingContext bindingContext, string key) where T : struct
        {
            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                if (string.IsNullOrEmpty(key)) key = bindingContext.ModelName;
                else key = String.Format("{0}.{1}", bindingContext.ModelName, key);
            }
            if (string.IsNullOrEmpty(key)) return null;

            ValueProviderResult value;

            value = bindingContext.ValueProvider.GetValue(key);
            bindingContext.ModelState.SetModelValue(key, value);

            if (value == null)
            {
                return null;
            }

            Nullable<T> retVal = null;
            try
            {
                retVal = (Nullable<T>)value.ConvertTo(typeof(T));
            }
            catch (Exception) { }

            return retVal;
        }
        public override object BindModel(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");

            // Check for a simple DateTime value with no suffix
            DateTime? dateTimeAttempt = GetA<DateTime>(bindingContext, "");
            if (dateTimeAttempt != null)
            {
                return dateTimeAttempt.Value;
            }

            // Check for separate Date / Time fields
            DateTime? dateAttempt = GetA<DateTime>(bindingContext, "Date");
            DateTime? timeAttempt = GetA<DateTime>(bindingContext, "TimeOfDay");

            //If we got both parts, assemble them!
            if (dateAttempt != null && timeAttempt != null)
            {
                return new DateTime(dateAttempt.Value.Year,
                    dateAttempt.Value.Month,
                    dateAttempt.Value.Day,
                    timeAttempt.Value.Hour,
                    timeAttempt.Value.Minute,
                    timeAttempt.Value.Second);
            }

            //Only got one half? Return as much as we have!
            return dateAttempt ?? timeAttempt;
        }
    }
}