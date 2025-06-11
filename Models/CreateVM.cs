using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hinet.Web.Areas.QLNhanSuChinhThucArea.Models
{
	public class CreateVM
	{
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
        public string HoTen { get; set; }
        public int SoDienThoai { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Email { get; set; }
        public string QueQuanTinh { get; set; }
        public string QueQuanHuyen { get; set; }
        public string QueQuanXa { get; set; }
        public string QueQuanDiaChi { get; set; }
        public string DiaChiHienTaiTinh { get; set; }
        public string DiaChiHienTaiHuyen { get; set; }
        public string DiaChiHienTaiXa { get; set; }
        public string DiaChiHienTaiDiaChi { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập thông tin này")]
        public string PhongBan { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập thông tin này")]
        public string ViTri { get; set; }
        public string TrinhDoDaoTao { get; set; }
        public DateTime NgayVaoCongTy { get; set; }
        public string TrangThai { get; set; }
        public string TaiLieuDinhKem { get; set; }
    }
}