SportShopDemo

SportShopDemo is a demo e-commerce web application for sporting goods built with ASP.NET Core MVC.
The project includes product browsing, filtering, favorites, shopping cart functionality, authentication with ASP.NET Identity, and a demo checkout flow.

========================================
OVERVIEW
========================================

This project is designed as a sports shop demo where users can:

- Browse products
- Search products by name or description
- Filter by gender, sport, subcategory, and price range
- View product details and photos
- Add products to favorites
- Add products to cart with size selection
- Change cart quantities
- Complete a demo checkout form

The application also includes seeded data and a default admin account for testing.

========================================
TECH STACK
========================================

- ASP.NET Core MVC
- .NET 8
- Entity Framework Core
- SQL Server / LocalDB
- ASP.NET Core Identity
- Razor Pages for Identity UI
- HTML / CSS / C#

========================================
MAIN FEATURES
========================================

1. Product Catalog
- Displays products ordered by name
- Supports search by keyword
- Supports filters:
  - Gender
  - Sport
  - Subcategory
  - Minimum price
  - Maximum price
- Loads product photos for display

2. Product Details
- Shows detailed information for a selected product
- Shows whether the product is in the user’s favorites

3. Favorites
- Authenticated users can add or remove products from favorites
- Users can view a personal favorites list

4. Shopping Cart
- Authenticated users can add products to cart
- Size validation is applied before adding items
- Users can decrease quantity or remove items
- Cart total is calculated automatically

5. Demo Checkout
- Users submit:
  - Full name
  - Address
  - Phone
- Orders are saved as .txt files in:
  wwwroot/orders
- This is a demo flow only
- No real payment or email sending is implemented

6. Authentication and Roles
- ASP.NET Identity is enabled
- Default roles are seeded:
  - Admin
  - Client
- A default admin account is created automatically on startup

========================================
DEFAULT ADMIN ACCOUNT
========================================

Email:
admin@sportshop.com

Password:
Admin123!

Important:
This account is intended only for demo and local testing.
Change or remove it before using the project in any real environment.

========================================
PROJECT STRUCTURE
========================================

SportShopDemo/
│
├── Areas/
│   └── Identity/Pages
├── Controllers/
│   ├── AdminProductsController.cs
│   ├── CartController.cs
│   ├── FavoritesController.cs
│   ├── HomeController.cs
│   └── ProductsController.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── Migrations/
├── Models/
│   ├── ApplicationUser.cs
│   ├── CartItem.cs
│   ├── Favorite.cs
│   ├── Product.cs
│   └── ProductPhoto.cs
├── Views/
├── wwwroot/
├── Program.cs
├── appsettings.json
├── SportShop.csproj
└── SportShop.sln

========================================
GETTING STARTED
========================================

Prerequisites:
- .NET 8 SDK
- SQL Server LocalDB or SQL Server
- Visual Studio 2022 or VS Code

1. Clone the repository

git clone https://github.com/nersanCHO/SportShopDemo.git

2. Enter the project folder

cd SportShopDemo

3. Restore dependencies

dotnet restore

4. Update the database connection if needed

The default connection string in appsettings.json is:

Server=(localdb)\MSSQLLocalDB;Database=SportShop;Trusted_Connection=True;MultipleActiveResultSets=true

You can keep it as-is for LocalDB or replace it with your own SQL Server connection string.

5. Run the project

dotnet run

6. Open the app in your browser

Use the local URL shown in the terminal after startup.

========================================
DATABASE AND SEEDING
========================================

On application startup, the project:

- Applies pending migrations automatically
- Creates roles if they do not exist
- Creates the default admin account if it does not exist
- Seeds sample products into the database

This behavior is handled in:
- Program.cs
- Data/SeedData.cs

========================================
ROUTING
========================================

The default route points to:

Controller: Products
Action: Index

So the application opens directly on the product listing page.

========================================
NOTES
========================================

- This is a demo project
- Checkout is simulated
- Orders are written as text files instead of being sent or processed
- Some product descriptions are in Bulgarian
- The project includes sample products, categories, and product images

========================================
FUTURE IMPROVEMENTS
========================================

Possible next steps:
- Add real order persistence in the database
- Add payment integration
- Add order history for users
- Add product categories page
- Improve admin product management
- Add image upload for products
- Add stock tracking
- Add localization / multilingual UI
- Add unit and integration tests

========================================
LICENSE
========================================

This project is for educational and demo purposes.

========================================
AUTHOR
========================================

Created by nersanCHO
GitHub repository:
https://github.com/nersanCHO/SportShopDemo
