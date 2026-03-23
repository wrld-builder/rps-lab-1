using Autofac;
using StudentDirectory;

namespace StudentDirectory.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        ContainerBuilder builder = new();
        builder.RegisterInstance(AppPaths.GetDatabasePath()).Named<string>("databasePath");
        builder.Register(ctx => new StudentRepository(ctx.ResolveNamed<string>("databasePath")))
            .As<IStudentRepository>()
            .SingleInstance();
        builder.RegisterType<StudentService>().As<IStudentService>().SingleInstance();
        builder.RegisterType<StudentExportService>().As<IStudentExportService>().SingleInstance();
        builder.RegisterType<StudentListForm>().As<IStudentListView>().AsSelf();
        builder.RegisterType<StudentEditorForm>().As<IStudentEditorView>();
        builder.RegisterType<StudentListPresenter>().AsSelf();

        using IContainer container = builder.Build();
        StudentListForm mainForm = container.Resolve<StudentListForm>();
        StudentListPresenter presenter = container.Resolve<StudentListPresenter>();
        mainForm.AttachPresenter(presenter);

        Application.Run(mainForm);
    }
}
