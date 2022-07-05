using System;
using System.Collections.Generic;

namespace VitalenCase.Models
{
    public partial class Logging
    {
        public int Id { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? RequestTime { get; set; }
        public long? ResponseMillis { get; set; }
        public int? UserId { get; set; }
        public string Method { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string Path { get; set; }
    }
}
