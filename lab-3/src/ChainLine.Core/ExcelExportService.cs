using ClosedXML.Excel;

namespace ChainLine;

/// <summary>
/// Exports source parameters and computed values to an Excel workbook.
/// </summary>
public static class ExcelExportService
{
    public static void Export(string filePath, FunctionComputationResult result)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
        }

        ArgumentNullException.ThrowIfNull(result);

        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using XLWorkbook workbook = new();
        IXLWorksheet parametersSheet = workbook.Worksheets.Add("Parameters");
        IXLWorksheet valuesSheet = workbook.Worksheets.Add("Values");

        FillParametersSheet(parametersSheet, result);
        FillValuesSheet(valuesSheet, result);

        workbook.SaveAs(filePath);
    }

    private static void FillParametersSheet(IXLWorksheet sheet, FunctionComputationResult result)
    {
        sheet.Cell(1, 1).Value = "Author";
        sheet.Cell(1, 2).Value = "Шунин Михаил Дмитриевич";
        sheet.Cell(2, 1).Value = "Function";
        sheet.Cell(2, 2).Value = "y = a / 2 * (e^(x / a) + e^(-x / a))";
        sheet.Cell(3, 1).Value = "Left boundary";
        sheet.Cell(3, 2).Value = result.Parameters.LeftBoundary;
        sheet.Cell(4, 1).Value = "Right boundary";
        sheet.Cell(4, 2).Value = result.Parameters.RightBoundary;
        sheet.Cell(5, 1).Value = "Step";
        sheet.Cell(5, 2).Value = result.Parameters.Step;
        sheet.Cell(6, 1).Value = "Coefficient a";
        sheet.Cell(6, 2).Value = result.Parameters.CoefficientA;
        sheet.Cell(7, 1).Value = "Warning";
        sheet.Cell(7, 2).Value = result.WarningMessage ?? "None";
        sheet.Columns().AdjustToContents();
    }

    private static void FillValuesSheet(IXLWorksheet sheet, FunctionComputationResult result)
    {
        sheet.Cell(1, 1).Value = "X";
        sheet.Cell(1, 2).Value = "Y";

        for (int index = 0; index < result.Points.Count; index++)
        {
            sheet.Cell(index + 2, 1).Value = result.Points[index].X;
            sheet.Cell(index + 2, 2).Value = result.Points[index].Y;
        }

        sheet.Columns().AdjustToContents();
    }
}
