# TasAndJet API

## Описание
### НАДО ИЗМЕНИТЬ СВОЙ ХОТС В FileProvider.cs при генерации ссылок на картинки

TasAndJet – это backend-сервис для обработки запросов от клиентов и управления заказами в транспортной системе.

На данный момент реализованы следующие возможности:

- Регистрация пользователей
- Логин (вход в систему)
- Назначение ролей
- Верификация по SMS
- Оформление заказа
- Отслеживание статуса заказа
- Написание отзывов


## Структура проекта

- **TasAndJet.Api** – основной API сервис
- **PostgreSQL** – база данных
- **PgAdmin** – администрирование базы данных
- **MinIO** – облачное хранилище
- **Jaeger** – трассировка запросов
- **RabbitMQ** – брокер сообщений

## Запуск проекта

1. Клонировать репозиторий:

   ```sh
   git clone https://github.com/darynku/TasAndJet.git
   cd tasandjet
   ```

2. Выполнить миграции базы данных
   ```sh
   dotnet ef migrations add MigrationName -c ApplicationDbContext -p .\TasAndJet.Infrastructure\ -s .\TasAndJet.Api
   ```
3. Собрать и запустить сервисы с помощью Docker:

   ```sh
   docker-compose up -d --build
   ```.

## Переменные окружения

- `POSTGRES_DB=tasandjet`
- `POSTGRES_USER=postgres`
- `POSTGRES_PASSWORD=123`
- `ASPNETCORE_ENVIRONMENT=Docker`
- `OTEL_EXPORTER_OTLP_ENDPOINT=http://jaeger:4317`

## Основные сервисы и их порты

- API: `http://localhost:5001`
- PostgreSQL: `localhost:5432` (логин: `postgres`, пароль: `123`)
- PgAdmin: `http://localhost:8080` (логин: `admin@admin.com`, пароль: `admin`)
- MinIO:
  - API: `http://localhost:9000`
  - Консоль: `http://localhost:9001`
- Jaeger: `http://localhost:16686`
- RabbitMQ: `http://localhost:15672` (логин: `guest`, пароль: `guest`)

## Авторизация

Пользователи проходят аутентификацию через логин (email + пароль) и верификацию по SMS. В дальнейшем планируется поддержка OAuth (Google Login).

## API Эндпоинты

Примеры некоторых доступных маршрутов:

- `POST /api/Accounts/register` – регистрация
- `POST /api/Accounts/login` – вход в систему
- `POST /api/Orders` – оформление заказа
- `GET /api/Orders/{orderId}` – получение детальной информации о заказе
- `POST /api/Reviews/create` – написание отзыва
