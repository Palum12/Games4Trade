using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Models
{
    public class OperationResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public object Payload { get; set; }
    }
}
