using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hinet.Web.Areas.QLNhanSuChinhThucArea.Models
{
	public class EditVM
	{
		public long Id { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string HoTen { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string ViTriUngTuyen { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public int SoDienThoai { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string TruongDaiHoc { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string CongTyCu { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public DateTime NgaySinh { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string NoiDungPhanHoi { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public int TrangThaiPhanHoi { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string GhiChuKetQua { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string TrangThaiKetQua { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string GhiChu { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập thông tin này")]
		public string TrangThai { get; set; }


	}
}