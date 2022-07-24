using AppCoreLite.Configs;
using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;
using AppCoreLite.Utilities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace AppCoreLite.Managers.Bases
{
    /// <summary>
    /// Base export manager class for managing export operations using models.
    /// </summary>
    public abstract class ExportManagerBase : IConfig
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ReflectionUtil _reflectionUtil;

        public ExportManagerConfig Config { get; }

        protected ExportManagerBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _reflectionUtil = new ReflectionUtil();
            Config = new ExportManagerConfig();
        }

        public void Set(Languages language)
        {
            Config.Set(language);
        }

        public void Set(bool isExcelLicenseCommercial)
        {
            Config.Set(isExcelLicenseCommercial);
        }

        public virtual void ExportToExcel<TModel>(List<TModel> list) where TModel : class, new()
        {
            var data = ConvertToByteArrayForExcel(list);
            if (data != null && data.Length > 0)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Clear();
                _httpContextAccessor.HttpContext.Response.Clear();
                _httpContextAccessor.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-length", data.Length.ToString());
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=\"" + Config.FileNameWithoutExtension + ".xlsx\"");
                _httpContextAccessor.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);
                _httpContextAccessor.HttpContext.Response.Body.Flush();
            }
        }

        private byte[]? ConvertToByteArrayForExcel<TModel>(List<TModel> list) where TModel : class, new()
        {
            byte[]? data = null;
            if (list != null && list.Count > 0)
            {
                var dataTable = _reflectionUtil.ConvertToDataTable(list);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    ExcelPackage.LicenseContext = Config.IsExcelLicenseCommercial ? LicenseContext.Commercial : LicenseContext.NonCommercial;
                    ExcelPackage excelPackage = new ExcelPackage();
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add(Config.ExcelWorksheetName);
                    excelWorksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                    excelWorksheet.Cells["A:AZ"].AutoFitColumns();
                    data = excelPackage.GetAsByteArray();
                }
            }
            return data;
        }
    }
}
