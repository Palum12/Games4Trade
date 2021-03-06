﻿using System.Collections.Generic;

namespace Games4TradeAPI.Models
{
    public class AdQueryOptions
    {
        public string Search { get; set; }
        public string Sort { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public bool? Desc { get; set; }
        public string Type { get; set; }
        public int[] Genres { get; set; }
        public int? State { get; set; }
        public int? Region { get; set; }
        public int[] Systems { get; set; }
    }
}
