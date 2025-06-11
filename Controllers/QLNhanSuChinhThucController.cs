using AutoMapper;
using CommonHelper.String;
using CommonHelper.Upload;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Hinet.Model.IdentityEntities;
using Hinet.Model.Entities;
using Hinet.Service.Common;
using Hinet.Service.Constant;
using Hinet.Web.Areas.QLNhanSuChinhThucArea.Models;
using Hinet.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Hinet.Web.Filters;
using Hinet.Service.QLNhanSuChinhThucService;
using Hinet.Service.QLNhanSuChinhThucService.Dto;
using CommonHelper.Excel;
using CommonHelper.ObjectExtention;
using Hinet.Web.Common;
using System.IO;
using System.Web.Configuration;
using CommonHelper;
using Hinet.Service.DM_DulieuDanhmucService;



namespace Hinet.Web.Areas.QLNhanSuChinhThucArea.Controllers
{
    public class QLNhanSuChinhThucController : BaseController
    {
        private readonly ILog _Ilog;
        private readonly IMapper _mapper;
        public const string permissionIndex = "QLNhanSuChinhThuc_index";
        public const string permissionCreate = "QLNhanSuChinhThuc_create";
        public const string permissionEdit = "QLNhanSuChinhThuc_edit";
        public const string permissionDelete = "QLNhanSuChinhThuc_delete";
        public const string permissionImport = "QLNhanSuChinhThuc_import";
        public const string permissionExport = "QLNhanSuChinhThuc_export";
        public const string searchKey = "QLNhanSuChinhThucPageSearchModel";
        private readonly IQLNhanSuChinhThucService _QLNhanSuChinhThucService;
        private readonly IDM_DulieuDanhmucService _dM_DulieuDanhmucService;


        public QLNhanSuChinhThucController(IQLNhanSuChinhThucService QLNhanSuChinhThucService, ILog Ilog,

        IDM_DulieuDanhmucService dM_DulieuDanhmucService,
            IMapper mapper
            )
        {
            _QLNhanSuChinhThucService = QLNhanSuChinhThucService;
            _Ilog = Ilog;
            _mapper = mapper;
            _dM_DulieuDanhmucService = dM_DulieuDanhmucService;

        }
        // GET: QLNhanSuChinhThucArea/QLNhanSuChinhThuc
        //[PermissionAccess(Code = permissionIndex)]
        public ActionResult Index()
        {
            var listData = _QLNhanSuChinhThucService.GetDaTaByPage(null);
            SessionManager.SetValue(searchKey, null);
            return View(listData);
        }

        [HttpPost]
        public JsonResult getData(int indexPage, string sortQuery, int pageSize)
        {
            var searchModel = SessionManager.GetValue(searchKey) as QLNhanSuChinhThucSearchDto;
            if (!string.IsNullOrEmpty(sortQuery))
            {
                if (searchModel == null)
                {
                    searchModel = new QLNhanSuChinhThucSearchDto();
                }
                searchModel.sortQuery = sortQuery;
                if (pageSize > 0)
                {
                    searchModel.pageSize = pageSize;
                }
                SessionManager.SetValue(searchKey, searchModel);
            }
            var data = _QLNhanSuChinhThucService.GetDaTaByPage(searchModel, indexPage, pageSize);
            return Json(data);
        }
        //[PermissionAccess(Code = permissionCreate)]
        public PartialViewResult Create()
        {
            var myModel = new CreateVM();

            return PartialView("_CreatePartial", myModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Create(CreateVM model)
        {
            var result = new JsonResultBO(true, "Thêm thông tin nhân viên thành công");
            try
            {
                if (ModelState.IsValid)
                {
                    var EntityModel = _mapper.Map<QLNhanSuChinhThuc>(model);

                    _QLNhanSuChinhThucService.Create(EntityModel);
                }

            }
            catch (Exception ex)
            {
                result.MessageFail(ex.Message);
                _Ilog.Error("Lỗi tạo mới Quản lý thông tin ứng viên", ex);
            }
            return Json(result);
        }

        //[PermissionAccess(Code = permissionEdit)]
        public PartialViewResult Edit(long id)
        {
            var myModel = new EditVM();

            var obj = _QLNhanSuChinhThucService.GetById(id);
            if (obj == null)
            {
                throw new HttpException(404, "Không tìm thấy thông tin");
            }

            myModel = _mapper.Map(obj, myModel);
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

                    var obj = _QLNhanSuChinhThucService.GetById(model.Id);
                    if (obj == null)
                    {
                        throw new Exception("Không tìm thấy thông tin");
                    }

                    obj = _mapper.Map(model, obj);

                    _QLNhanSuChinhThucService.Update(obj);

                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Không cập nhật được";
                _Ilog.Error("Lỗi cập nhật thông tin Quản lý thông tin ứng viên", ex);
            }
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult searchData(QLNhanSuChinhThucSearchDto form)
        {
            var searchModel = SessionManager.GetValue(searchKey) as QLNhanSuChinhThucSearchDto;

            if (searchModel == null)
            {
                searchModel = new QLNhanSuChinhThucSearchDto();
                searchModel.pageSize = 20;
            }
            /*
            searchModel.HoTenFilter = form.HoTenFilter;
            searchModel.ViTriUngTuyenFilter = form.ViTriUngTuyenFilter;
            searchModel.EmailFilter = form.EmailFilter;
            searchModel.SoDienThoaiFilter = form.SoDienThoaiFilter;
            searchModel.TruongDaiHocFilter = form.TruongDaiHocFilter;
            searchModel.CongTyCuFilter = form.CongTyCuFilter;
            searchModel.TrangThaiGoiDienFilter = form.TrangThaiGoiDienFilter;
            searchModel.TrangThaiGuiEmailFilter = form.TrangThaiGuiEmailFilter;
            */

            SessionManager.SetValue((searchKey), searchModel);

            var data = _QLNhanSuChinhThucService.GetDaTaByPage(searchModel, 1, searchModel.pageSize);
            return Json(data);
        }

        //[PermissionAccess(Code = permissionDelete)]
        [HttpPost]
        public JsonResult Delete(long id)
        {
            var result = new JsonResultBO(true, "Xóa Quản lý thông tin ứng viên thành công");
            try
            {
                var user = _QLNhanSuChinhThucService.GetById(id);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy thông tin để xóa");
                }
                _QLNhanSuChinhThucService.Delete(user);
            }
            catch (Exception ex)
            {
                result.MessageFail("Không thực hiện được");
                _Ilog.Error("Lỗi khi xóa tài khoản id=" + id, ex);
            }
            return Json(result);
        }


        public ActionResult Detail(long id)
        {
            var model = new DetailVM();
            model.objInfo = _QLNhanSuChinhThucService.GetDtoById(id);
            return View(model);
        }
        //[PermissionAccess(Code = permissionExport)]
        //[PermissionAccess(Code = permissionImport)]
        public FileResult ExportExcel()
        {
            var searchModel = SessionManager.GetValue(searchKey) as QLNhanSuChinhThucSearchDto;
            var data = _QLNhanSuChinhThucService.GetDaTaByPage(searchModel).ListItem;
            var dataExport = _mapper.Map<List<QLNhanSuChinhThucExportDto>>(data);
            var fileExcel = ExportExcelV2Helper.Export<QLNhanSuChinhThucExportDto>(dataExport);
            return File(fileExcel, "application/octet-stream", "QLNhanSuChinhThuc.xlsx");
        }

        //[PermissionAccess(Code = permissionImport)]
        //[PermissionAccess(Code = permissionImport)]
        public ActionResult Import()
        {
            var model = new ImportVM();
            model.PathTemplate = Path.Combine(@"/Uploads", WebConfigurationManager.AppSettings["IMPORT_QLNhanSuChinhThuc"]);

            return View(model);
        }

        [HttpPost]
        public ActionResult CheckImport(FormCollection collection, HttpPostedFileBase fileImport)
        {
            JsonResultImportBO<QLNhanSuChinhThucImportDto> result = new JsonResultImportBO<QLNhanSuChinhThucImportDto>(true);
            //Kiểm tra file có tồn tại k?
            if (fileImport == null)
            {
                result.Status = false;
                result.Message = "Không có file đọc dữ liệu";
                return View(result);
            }

            //Lưu file upload để đọc
            var saveFileResult = UploadProvider.SaveFile(fileImport, null, ".xls,.xlsx", null, "TempImportFile", HostingEnvironment.MapPath("/Uploads"));
            if (!saveFileResult.status)
            {
                result.Status = false;
                result.Message = saveFileResult.message;
                return View(result);
            }
            else
            {

                #region Config để import dữ liệu
                var importHelper = new ImportExcelHelper<QLNhanSuChinhThucImportDto>();
                importHelper.PathTemplate = saveFileResult.fullPath;
                //importHelper.StartCol = 2;
                importHelper.StartRow = collection["ROWSTART"].ToIntOrZero();
                importHelper.ConfigColumn = new List<ConfigModule>();
                importHelper.ConfigColumn = ExcelImportExtention.GetConfigCol<QLNhanSuChinhThucImportDto>(collection);
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


        [HttpPost]
        public JsonResult GetExportError(List<List<string>> lstData)
        {
            ExportExcelHelper<QLNhanSuChinhThucImportDto> exPro = new ExportExcelHelper<QLNhanSuChinhThucImportDto>();
            exPro.PathStore = Path.Combine(HostingEnvironment.MapPath("/Uploads"), "ErrorExport");
            exPro.PathTemplate = Path.Combine(HostingEnvironment.MapPath("/Uploads"), WebConfigurationManager.AppSettings["IMPORT_QLNhanSuChinhThuc"]);
            exPro.StartRow = 5;
            exPro.StartCol = 2;
            exPro.FileName = "ErrorImportQLNhanSuChinhThuc";
            var result = exPro.ExportText(lstData);
            if (result.Status)
            {
                result.PathStore = Path.Combine(@"/Uploads/ErrorExport", result.FileName);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveImportData(List<QLNhanSuChinhThucImportDto> Data)
        {
            var result = new JsonResultBO(true);

            var lstObjSave = new List<QLNhanSuChinhThuc>();
            try
            {
                foreach (var item in Data)
                {
                    var obj = _mapper.Map<QLNhanSuChinhThuc>(item);
                    _QLNhanSuChinhThucService.Create(obj);
                }

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Lỗi dữ liệu, không thể import";
                _Ilog.Error("Lỗi Import", ex);
            }

            return Json(result);
        }

    }
}