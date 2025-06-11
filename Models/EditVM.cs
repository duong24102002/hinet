using System;

namespace Hinet.Web.Areas.QLSuKienArea.Models
{
    public class EditVM
    {
        public long Id { get; set; }
        public string TenSuKien { get; set; }
        public DateTime? NgaySuKien { get; set; }
        public string DiaDiem { get; set; }
        public string MoTa { get; set; }
    }
}