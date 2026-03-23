# Lab-1

Материалы текущей лабораторной находятся в этом репозитории.

- Вариант по списку (до пересчёта): **22**
- Вариант задания ЛР1 (после пересчёта): **11**

Исходный код, solution, тесты и Docker-файлы находятся в этой папке.

Рекомендуемый запуск из каталога `lab-1/`:

```bash
docker compose build
docker compose run --rm app
docker compose run --rm test
```

При запуске через `docker compose` контейнеру проброшены `/Users` и `/Volumes`, поэтому
можно указывать обычные абсолютные пути macOS, например
`/Users/macbook/Desktop/input.txt`. При сохранении результата приложение автоматически
создаёт недостающие папки.

Если запускать через `docker run`, volume нужно указать вручную:

```bash
docker build -t circle-intersection .
docker run -it --rm -v /Users:/Users -v /Volumes:/Volumes circle-intersection
```
