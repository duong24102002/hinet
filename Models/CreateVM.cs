using System;

namespace Hinet.Web.Areas.QLSuKienArea.Models
{
    public class CreateVM
    {
        public string TenSuKien { get; set; }
        public DateTime? NgaySuKien { get; set; }

        public string DiaDiem { get; set; }
        public string MoTa { get; set; }
    }
}