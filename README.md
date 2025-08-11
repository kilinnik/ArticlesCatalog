# API каталога статей

REST API для управления статьями и их тегами. Построено на ASP.NET Core и PostgreSQL.

## Требования

- Docker и docker-compose
- PostgreSQL (входит в `docker-compose.yml`)

## Запуск

Запустите приложение:

```bash
docker-compose up --build
```

API будет доступен по `http://localhost:8080`. Документация Swagger доступна по `http://localhost:8080/swagger` только в режиме `Development`.

## Эндпоинты

| Метод и путь | Описание | Ответ |
|--------------|----------|-------|
| `POST /api/articles` | Создать новую статью. Тело: `{ "title": "string", "tags": ["tag1", ...] }` | `201 Created` с объектом статьи |
| `GET /api/articles/{id}` | Получить статью по идентификатору | `200 OK` со статьёй или `404 Not Found` |
| `PUT /api/articles/{id}` | Обновить заголовок и теги статьи | `200 OK` с обновлённой статьёй или `404 Not Found` |
| `DELETE /api/articles/{id}` | Удалить статью | `204 No Content` |
| `GET /api/catalog/sections?skip=0&take=10` | Список разделов | `200 OK` с массивом разделов |
| `GET /api/catalog/sections/{sectionKey}/articles?skip=0&take=10` | Статьи раздела | `200 OK` с массивом статей |
