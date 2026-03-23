using StudentDirectory;
using Xunit;

namespace StudentDirectory.Tests;

public sealed class StudentExportServiceTests
{
    [Fact]
    public async Task ExportAsync_WritesCsvFile()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");

        try
        {
            IStudentExportService exportService = new StudentExportService();
            await exportService.ExportAsync(filePath, new[]
            {
                new StudentRecord
                {
                    Id = 1,
                    FullName = "Петров Петр Петрович",
                    GroupName = "P3208",
                    Faculty = "ФКТИ",
                    EnrollmentYear = 2022,
                    Notes = "Отличник",
                },
            });

            string content = await File.ReadAllTextAsync(filePath);
            Assert.Contains("Петров Петр Петрович", content);
            Assert.Contains("EnrollmentYear", content);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
