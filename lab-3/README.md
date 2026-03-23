# Lab-3

Лабораторная работа №3 реализована как WinForms-приложение для построения графика функции
варианта 8: цепная линия

```text
y = a / 2 * (e^(x / a) + e^(-x / a))
```

Для номера в списке **22** вариант определяется как `((22 - 1) % 14) + 1 = 8`.

Что умеет программа:

- строить график функции на заданном интервале;
- выводить таблицу значений функции;
- предупреждать о некорректных параметрах, невозможности построения графика и вырождении графика в точку;
- экспортировать исходные данные и результаты расчёта в файл MS Excel (`.xlsx`).

Структура:

- `src/ChainLine.Core` — вычислительная логика и экспорт в Excel.
- `src/ChainLine.WinForms` — графический интерфейс WinForms.
- `tests/ChainLine.Tests` — модульные тесты.

Требования для запуска:

- Windows;
- .NET 8 SDK;
- Visual Studio 2022 или `dotnet build`.

Пример запуска:

```bash
cd lab-3
dotnet build ChainLine.sln
dotnet run --project src/ChainLine.WinForms/ChainLine.WinForms.csproj
```

Запуск тестов:

```bash
cd lab-3
dotnet test tests/ChainLine.Tests/ChainLine.Tests.csproj
```
