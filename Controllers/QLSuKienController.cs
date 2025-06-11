using AutoMapper;
using CommonHelper;
using CommonHelper.Excel;
using CommonHelper.String;
using CommonHelper.Upload;
using Hinet.Model.Entities;
using Hinet.Service.Common;
using Hinet.Service.DepartmentService;
using Hinet.Service.DM_DulieuDanhmucService;
using Hinet.Service.QLNhanSuChinhThucService.Dto;
using Hinet.Service.QLSuKienService;
using Hinet.Service.QLSuKienService.Dto;
using Hinet.Web.Areas.QLSuKienArea.Models;
using Hinet.Web.Common;
using Hinet.Web.Filters;
using log4net;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;


namespace Hinet.Web.Areas.QLSuKienArea.Controllers
{
    public class QLSuKienController : BaseController
    {
        #region Khai báo các service cần sử dụng
        private readonly ILog _ilog;
        private readonly IMapper _mapper;
        private const string permissionIndex = "QLSuKien_index";
        private const string permissionCreate = "QLSuKien_create";
        private const string permissionEdit = "QLSuKien_edit";
        private const string permissionDelete = "QLSuKien_delete";
        private const string permissionImport = "QLSuKien_import";
        private const string permissionExport = "QLSuKien_export";
        private const string searchKey = "QLSuKienPageSearchModel";

        private readonly IQLSuKienService _qlSuKienService;
        private readonly IDM_DulieuDanhmucService _danhMucService;
        private readonly IDepartmentService _departmentService;
        #endregion

        public QLSuKienController(
            IQLSuKienService qlSuKienService,
            IDM_DulieuDanhmucService danhMucService,
            IDepartmentService departmentService,
            IMapper mapper,
            ILog ilog)
        {
            _qlSuKienService = qlSuKienService;
            _danhMucService = danhMucService;
            _departmentService = departmentService;
            _mapper = mapper;
            _ilog = ilog;
        }

        // GET: QLSuKienArea/QLSuKien
        // [PermissionAccess(Code = permissionIndex)] // Bỏ comment nếu muốn kiểm soát quyền truy cập
        public ActionResult Index()
        {
            // Khởi tạo searchModel mặc định (nếu cần)
            var searchModel = new QLSuKienSearchDto();
            int pageIndex = 1;
            int pageSize = 10;

            var listData = _qlSuKienService.GetDataByPage(searchModel, pageIndex, pageSize);
            // Ensure this namespace is included
            SessionManager.SetValue(searchKey, null);
            return View(listData);
        }

        // [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            var searchModel = SessionManager.GetValue(searchKey) as QLSuKienSearchDto;

            // Đảm bảo searchModel luôn được khởi tạo
            if (searchModel == null)
            {
                searchModel = new QLSuKienSearchDto();
            }

            if (!string.IsNullOrEmpty(sortQuery))
            {
                searchModel.sortQuery = sortQuery;
            }
            if (pageSize > 0)
            {
                searchModel.pageSize = pageSize;
            }

            SessionManager.SetValue(searchKey, searchModel);
            var data = _qlSuKienService.GetDataByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(QLSuKienSearchDto form)
        {
            var searchModel = SessionManager.GetValue(searchKey) as QLSuKienSearchDto;

            if (searchModel == null)
            {
                searchModel = new QLSuKienSearchDto();
                searchModel.pageSize = 10;
            }
            searchModel.TenSuKien = form.TenSuKien;
            searchModel.NgaySuKien = form.NgaySuKien;

            SessionManager.SetValue(searchKey, searchModel);

            var data = _qlSuKienService.GetDataByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }
        #region Các action phục vụ chức năng thêm mới

        // Hiển thị form thêm mới sự kiện (partial view)
        public PartialViewResult Create()
        {
            var myModel = new CreateVM();
            return PartialView("_CreatePartial", myModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(CreateVM model)
        {
            var result = new JsonResultBO(true, "Tạo sự kiện thành công");
            try
            {
                if (ModelState.IsValid)
                {
                    var entityModel = _mapper.Map<QLSuKien>(model);
                    _qlSuKienService.Create(entityModel);
                }
            }
            catch (Exception ex)
            {
                result.MessageFail(ex.Message);
                _ilog.Error("Lỗi tạo mới Sự kiện", ex);
            }
            return Json(result);
        }
        #endregion
        #region Các action phục vụ chức năng sửa

        // Hiển thị form sửa thông tin sự kiện (partial view)
        public PartialViewResult Edit(long id)
        {
            var myModel = new EditVM();
            var obj = _qlSuKienService.GetById(id);
            if (obj == null)
            {
                throw new HttpException(404, "Không tìm thấy thông tin");
            }
            myModel = _mapper.Map<EditVM>(obj);
            return PartialView("_EditPartial", myModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(EditVM model)
        {
            var result = new JsonResultBO(true);
            try
            {
                if (ModelState.IsValid)
                {
                    var obj = _qlSuKienService.GetById(model.Id);
                    if (obj == null)
                    {
                        throw new Exception("Không tìm thấy thông tin");
                    }
                    _mapper.Map(model, obj);
                    _qlSuKienService.Update(obj);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
                _ilog.Error("Lỗi cập nhật thông tin Sự kiện", ex);
            }
            return Json(result);
        }
        #endregion
        #region Action phục vụ xóa dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true, "Xóa sự kiện thành công");
            try
            {
                var suKien = _qlSuKienService.GetById(id);
                if (suKien == null)
                {
                    throw new Exception("Không tìm thấy thông tin để xóa");
                }
                _qlSuKienService.Delete(suKien);
            }
            catch (Exception ex)
            {
                result.MessageFail("Không thực hiện được");
                _ilog.Error("Lỗi khi xóa sự kiện id=" + id, ex);
            }
            return Json(result);
        }
        #endregion
        #region Action phục vụ xem thông tin chi tiết
        public ActionResult Detail(long id)
        {
            var model = new DetailVM();
            model.objInfo = _qlSuKienService.GetById(id);
            return View(model);
        }
        #endregion

        #region Action phục vụ import excel
        public ActionResult Import()
        {
            var model = new ImportVM();
            // Lấy giá trị từ AppSettings
            var importFileName = WebConfigurationManager.AppSettings["IMPORT_QLSuKien"];
            if (string.IsNullOrEmpty(importFileName))
            {
                // Nếu chưa cấu hình, báo lỗi rõ ràng hơn
                ViewBag.ImportTemplateError = "Chưa cấu hình IMPORT_QLSuKien trong Web.config hoặc giá trị rỗng.";
                model.PathTemplate = "#";
            }
            else
            {
                // Kiểm tra file có tồn tại không (tùy chọn, nên có)
                var filePath = HostingEnvironment.MapPath(Path.Combine("~/Uploads", importFileName));
                if (!System.IO.File.Exists(filePath))
                {
                    ViewBag.ImportTemplateError = "File mẫu Excel không tồn tại trên server!";
                    model.PathTemplate = "#";
                }
                else
                {
                    // Đường dẫn cho client download
                    model.PathTemplate = Path.Combine("~/Uploads", importFileName);
                }
            }
            return View(model);
        }
        #endregion
        [HttpPost]
        public ActionResult CheckImport(FormCollection collection, HttpPostedFileBase file)
        {
            JsonResultImportBO<QLSuKienImportDto> result = new JsonResultImportBO<QLSuKienImportDto>(true);
            if (file == null)
            {
                result.Status = false;
                result.Message = "Không có file đọc dữ liệu";
                return View(result);
            }

            //Lưu file upload để đọc
            var saveFileResult = UploadProvider.SaveFile(file, null, ".xls,.xlsx", null, "TempImportFile", HostingEnvironment.MapPath("/Uploads"));
            if (!saveFileResult.status)
            {
                result.Status = false;
                result.Message = saveFileResult.message;
                return View(result);
            }
            else
            {

                #region Config để import dữ liệu
                var importHelper = new ImportExcelHelper<QLSuKienImportDto>();
                importHelper.PathTemplate = saveFileResult.fullPath;
                //importHelper.StartCol = 2;
                importHelper.StartRow = collection["ROWSTART"].ToIntOrZero();
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn = ExcelImportExtention.GetConfigCol<QLSuKienImportDto>(collection);
                #endregion
                var rsl = importHelper.ImportCustomRow();
                if (rsl.Status)
                {
                    result.Status = true;
                    result.Message = rsl.Message;

                    result.ListData = rsl.ListTrue;
                    result.ListFalse = rsl.lstFalse;
                }
                else
                {
                    result.Status = false;
                    result.Message = rsl.Message;
                }

            }
            return View(result);
        }

        // Action hỗ trợ tải về các dữ liệu lỗi không import được
        [HttpPost]
        public JsonResult GetExportError(List<string> lstData)
        {
            var exPro = new ExportExcelHelper<QLSuKienImportDto>();
            exPro.PathTemplate = Path.Combine(HostingEnvironment.MapPath("~/Uploads"), "ErrorExportX");
            exPro.PathTemplate = Path.Combine(HostingEnvironment.MapPath("~/Uploads"), WebConfigurationManager.AppSettings["IMPORT_QLSuKien"]);
            exPro.StartRow = 5;
            exPro.StartCol = 2;
            exPro.FileName = "ErrorImportQLSuKien";

            var result = exPro.ExportText(lstData.Select(x => new List<string> { x }).ToList());

            if (result.Status)
            {
                result.PathStore = Path.Combine("~/Uploads/ErrorExport/", result.FileName);
            }
            return Json(result);
        }

        // Action lưu dữ liệu đúng vào CSDL sau import
        [HttpPost]
        public JsonResult SaveImportData(List<QLSuKienImportDto> Data)
        {
            var result = new JsonResultBO(true);
            var lstObjSave = new List<QLSuKien>();
            try
            {
                foreach (var item in Data)
                {
                    var obj = _mapper.Map<QLSuKien>(item);
                    _qlSuKienService.Create(obj);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Lỗi dữ liệu, không thể import";
                _ilog.Error("Lỗi Import", ex);
            }
            return Json(result);
        }
        #region Action phục vụ export dữ liệu ra file excel
        // [PermissionAccess(Code = permissionImport)]
        public FileResult ExportExcel()
        {
            var searchModel = SessionManager.GetValue(searchKey) as QLSuKienSearchDto;
            int pageIndex = 1;
            int pageSize = 10000; // Số lớn để xuất toàn bộ

            // Lấy dữ liệu phân trang, trả về PageListResultBO<QLSuKienDto>
            var pageResult = _qlSuKienService.GetDataByPage(searchModel, pageIndex, pageSize);
            var data = pageResult.ListItem;

            // Map sang DTO export
            var dataExport = data.Select(x => new QLSuKienExportDto
            {
                TenSuKien = x.TenSuKien,
                NgaySuKien = x.NgaySuKien.HasValue ? x.NgaySuKien.Value.ToString("dd/MM/yyyy") : "",
                DiaDiem = x.DiaDiem,
                MoTa = x.MoTa
            }).ToList();
            var fileExcel = ExportExcelV2Helper.Export<QLSuKienExportDto>(dataExport);
            return File(fileExcel, "application/octet-stream", "Book1.xlsx");
        }
        #endregion
    }
}