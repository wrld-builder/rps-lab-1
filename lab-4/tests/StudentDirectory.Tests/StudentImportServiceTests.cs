using StudentDirectory;
using Xunit;

namespace StudentDirectory.Tests;

public sealed class StudentImportServiceTests
{
    [Fact]
    public async Task ImportAsync_ReadsExportedCsvFile()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");

        try
        {
            await File.WriteAllTextAsync(
                filePath,
                "Id,FullName,GroupName,Faculty,EnrollmentYear,Notes\n" +
                "1,\"Сидоров Сидор Сидорович\",\"P3201\",\"ФКТИ\",2021,\"Переведен из другой группы\"\n");

            IStudentImportService importService = new StudentImportService();
            IReadOnlyList<StudentRecordInput> students = await importService.ImportAsync(filePath);

            Assert.Single(students);
            Assert.Equal("Сидоров Сидор Сидорович", students[0].FullName);
            Assert.Equal("P3201", students[0].GroupName);
            Assert.Equal(2021, students[0].EnrollmentYear);
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
