# SportShop (ASP.NET Core MVC + Identity + EF Core)

Онлайн магазин за спортни стоки – дипломна тема.

## Функционалности (по заданието)
- Регистрация/вход (ASP.NET Core Identity)
- Два типа потребители: **Admin** и **Client** (роля)
- Админ панел: CRUD продукти + качване на снимка
- Търсене/филтри: **мъже/жени**, **спорт**, **подкатегория**, **ценови диапазон**
- Количка: добавяне/намаляване/премахване + **обща цена**
- Любими: добавяне/премахване + страница "Любими"

## Изисквания
- .NET 8 SDK
- (по желание) Visual Studio 2022 / VS Code

## Стартиране (локално)
1) Отвори папката `SportShop`.
2) В терминал:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

3) Отвори в браузър показания URL (напр. https://localhost:5001).

## Админ акаунт (seed)
- Email: **admin@sportshop.com**
- Password: **Admin123!**

> След първото стартиране смени паролата.

## Бележка за базата
Използва се SQLite файл `SportShop.db` (създава се автоматично след migrations/update).
