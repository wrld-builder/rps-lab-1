# Lab-4

Лабораторная работа №4 выполнена по варианту 2: хранение списка студентов.

Для номера в списке **22** вариант определяется как `((22 - 1) % 10) + 1 = 2`.

Реализовано:

- WinForms-приложение;
- паттерн MVP;
- внедрение зависимостей через Autofac;
- хранение данных в SQLite;
- асинхронный доступ к БД через `SQLiteAsyncConnection`;
- добавление и редактирование студентов;
- импорт списка студентов из файла CSV;
- сохранение данных между запусками;
- экспорт списка студентов в файл CSV.

Структура:

- `src/StudentDirectory.Core` — модель, репозиторий, сервисы, экспорт.
- `src/StudentDirectory.WinForms` — интерфейс WinForms и презентеры.
- `tests/StudentDirectory.Tests` — модульные тесты.

Запуск:

```bash
cd lab-4
dotnet build StudentDirectory.sln
dotnet run --project src/StudentDirectory.WinForms/StudentDirectory.WinForms.csproj
```

Тесты:

```bash
cd lab-4
dotnet test tests/StudentDirectory.Tests/StudentDirectory.Tests.csproj
```

Файл базы данных хранится в `%LocalAppData%/StudentDirectory.WinForms/students.db3`.
