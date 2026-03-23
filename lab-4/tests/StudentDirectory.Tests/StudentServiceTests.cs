using StudentDirectory;
using Xunit;

namespace StudentDirectory.Tests;

public sealed class StudentServiceTests
{
    [Fact]
    public async Task SaveAsync_WithValidInput_PersistsStudent()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db3");

        try
        {
            IStudentRepository repository = new StudentRepository(databasePath);
            IStudentService service = new StudentService(repository);

            StudentRecord student = await service.SaveAsync(new StudentRecordInput
            {
                FullName = "Иванов Иван Иванович",
                GroupName = "P3210",
                Faculty = "ФИТиП",
                EnrollmentYear = 2023,
                Notes = "Староста",
            });

            IReadOnlyList<StudentRecord> students = await service.GetAllAsync();

            Assert.NotEqual(0, student.Id);
            Assert.Single(students);
            Assert.Equal("Иванов Иван Иванович", students[0].FullName);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Fact]
    public async Task SaveAsync_WithEmptyFullName_Throws()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db3");

        try
        {
            IStudentService service = new StudentService(new StudentRepository(databasePath));

            InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.SaveAsync(new StudentRecordInput
                {
                    FullName = " ",
                    GroupName = "P3210",
                    Faculty = "ФИТиП",
                    EnrollmentYear = 2023,
                }));

            Assert.Contains("ФИО", exception.Message);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }
}
