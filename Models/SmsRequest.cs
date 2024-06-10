using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tabasco.Models
{
    public class SmsRequest
    {
        public required string Sender  { get; set; }
        public required string Message { get; set; }
        public required string Recipient { get; set; }
    }
}