using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Games4Trade.Services
{
    public static class OtherServices
    {
 
        /// <summary>
        ///  Method used for returning detailed model error from validating.
        /// </summary>
        /// <param name="modelState"> State of validated model.</param>
        /// <returns> Collection of error messages.</returns>
        public static IEnumerable<string> ReturnAllModelErrors(ModelStateDictionary modelState)
        {
            var allErrors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return allErrors;
        }
    }
}
