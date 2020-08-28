using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Api.Models
{
    public class Response
    {
        public string Payload { get; set; }
        public string Type { get; set; }
        public ResponseType ResponseType { get; set; }
    }
}
