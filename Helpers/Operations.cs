using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Helpers
{
    public class Operations
    {
        public static List<string> ConvertModelStateToErrorsList(ModelStateDictionary modelstate)
        {
            List<string> errorList = new List<string>();
            var errors = modelstate.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

            foreach (var error in errors.Values)
            {
                errorList.Add(error[0]);
            }
            return errorList;
        }
    }
}
