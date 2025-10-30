using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;

public class ebadiService
{
    private readonly IWebHostEnvironment _env;
    private List<StuffModel> _mergedExcelData = new();

    public ebadiService(IWebHostEnvironment env)
    {
        _env = env;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        LoadAndMergeExcelFiles();
    }

    private void LoadAndMergeExcelFiles()
    {
        var directory = Path.Combine(_env.WebRootPath, "Excels");
        var files = Directory.GetFiles(directory, "*.xlsx");
        foreach (var file in files)
        {
            var data = ReadExcel(file);
            _mergedExcelData.AddRange(data);
        }
    }

    private List<StuffModel> ReadExcel(string filePath)
    {
        var result = new List<StuffModel>();
        using var package = new ExcelPackage(new FileInfo(filePath));

        var worksheet = package.Workbook.Worksheets[0];
        int rowCount = worksheet.Dimension?.Rows ?? 0;

        for (int row = 2; row <= rowCount; row++) // Assuming headers in row 1
        {
            var model = new StuffModel
            {
                StuffID = worksheet.Cells[row, 1].Text,
                Descript = worksheet.Cells[row, 2].Text,
                VatRate = worksheet.Cells[row, 3].Text,
                EditDateFa = worksheet.Cells[row, 4].Text,
                typeStuffid = worksheet.Cells[row, 5].Text
            };
            result.Add(model);
        }
        return result;
    }

    // Search and group by specified TypeStuffId and optionally VatRate
    public Dictionary<string, List<StuffModel>> SearchAndGroup(string searchTerm, string groupVatRate, string groupTypeStuffId)
    {

        var filteredData = string.IsNullOrWhiteSpace(searchTerm)
            ? _mergedExcelData
            : _mergedExcelData
                .Where(item =>
                    !string.IsNullOrEmpty(item.Descript) &&
                    item.Descript.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();


        // Filter by groupTypeStuffId if specified
        if (!string.IsNullOrWhiteSpace(groupTypeStuffId))
        {
            filteredData = filteredData.Where(item => item.typeStuffid == groupTypeStuffId).ToList();
        }

        // Group dynamically based on inputs
        if (!string.IsNullOrWhiteSpace(groupVatRate))
        {

            filteredData = filteredData.Where(item => item.VatRate == groupVatRate).ToList();

        }
        else if (!string.IsNullOrWhiteSpace(groupTypeStuffId))
        {
            // Group by TypeStuffId only
            return filteredData
                .GroupBy(item => item.typeStuffid)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );
        }
        else
        {
            // No grouping, return all filtered data under a single key
            return new Dictionary<string, List<StuffModel>>
            {
                { "All Data", filteredData }
            };
        }
        var grouped = !string.IsNullOrWhiteSpace(groupTypeStuffId)
        ? filteredData.GroupBy(item => item.typeStuffid)
        : filteredData.GroupBy(item => "All Data");

        return grouped.ToDictionary(g => g.Key, g => g.ToList());
    }

    // Get predefined TypeStuffId options for dropdown
    public List<string> GetTypeStuffIdOptions()
    {
        return new List<string>
        {
            "عمومي -کالا داخلي",
            "عمومي-کالا وارداتي",
            "اختصاصي-کالا وارداتي",
            "اختصاصي-کالا داخلي",
            "عمومي- خدمت",
            "اختصاصي- خدمت"
        };
    }
}
