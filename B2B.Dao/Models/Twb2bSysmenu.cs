using System;
using System.Collections.Generic;

namespace B2B.Dao.Models
{
    public partial class Twb2bSysmenu
    {
        public byte Id { get; set; }
        public byte? Seq { get; set; }
        public string? Name { get; set; }
        public string? Prev { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
    }
}
