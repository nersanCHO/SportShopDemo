using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportShop.Models;

namespace SportShop.Data;

public static class SeedData
{


    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        // Roles
        string[] roles = { "Admin", "Client" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Admin user
        var adminEmail = "admin@sportshop.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        var seedProducts = new List<Product>
        {
            new Product
            {
                Name ="Nike Air Zoom Pegasus 40",
                Sport = "Running",
                SubCategory = "Shoes",
                Price = 129.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "39,40,41,42,43,44,45"),
                Description = "Responsive running shoes built for daily training.",
                ImagePath = "/images/NikeAirZoomPegasus40/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NikeAirZoomPegasus40/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeAirZoomPegasus40/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeAirZoomPegasus40/4.jpg" }
                }
            },
            new Product
            {
                Name ="Adidas Predator Edge",
                Sport = "Football",
                SubCategory = "Cleats",
                Price = 199.99m,
                Gender = GenderTarget.Men,
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "40,41,42,43,44,45,46"),
                Description = "Precision football boots for maximum control.",
                ImagePath = "/images/AdidasPredatorEdge/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/AdidasPredatorEdge/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/AdidasPredatorEdge/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/AdidasPredatorEdge/4.jpg" }
                }
            },
            new Product
            {
                Name ="Training T-Shirt DryFit",
                Sport = "Fitness",
                SubCategory = "Apparel",
                Price = 24.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Clothing,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Clothing, "XS,S,M,L,XL"),
                Description = "Lightweight training tee for workouts.",
                ImagePath = "/images/TrainingTShirtDryFit/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/TrainingTShirtDryFit/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/TrainingTShirtDryFit/3.jpg" }
                }
            },
            new Product
            {
                Name = "Sport Socks Pro",
                Sport = "Running",
                SubCategory = "Accessories",
                Price = 14.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal, "Universal"),
                Description = "Breathable socks for everyday running.",
                ImagePath = "/images/SportSocksPro/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/SportSocksPro/2.jpg" }
                }
            },
            new Product
            {
                Name = "Outdoor Wind Jacket",
                Sport = "Outdoor",
                SubCategory = "Apparel",
                Price = 59.99m,
                Gender = GenderTarget.Men,
                SizeType = ProductSizeType.Clothing,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Clothing, "M,L,XL"),
                Description = "Lightweight jacket for windy weather.",
                ImagePath = "/images/OutdoorWindJacket/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/OutdoorWindJacket/2.jpg" }
                }
            },
            new Product
            {
                Name = "MC Kinley Helmet",
                Sport = "Outdoor",
                SubCategory = "Apparel",
                Price = 30.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal, "Universal"),
                Description = "Каска за ски и сноуборд за възрастни от TECNO PRO.",
                ImagePath = "/images/MCKinleyHelmet/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/MCKinleyHelmet/2.webp" },
                    new ProductPhoto { ImagePath = "/images/MCKinleyHelmet/3.webp" }
                }
            },
            new Product
            {
                Name = "Adidas 3-Stripe Beige Hat",
                Sport = "Everyday",
                SubCategory = "Apparel",
                Price = 22.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal, "Universal"),
                Description = "Шапка Адидас Бежава",
                ImagePath = "/images/Adidas3-StripesBeigeHat/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/Adidas3-StripesBeigeHat/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/Adidas3-StripesBeigeHat/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/Adidas3-StripesBeigeHat/4.jpg" }
                }
            },new Product
            {
                Name = "Adidas 3-Stripe Greeen Hat",
                Sport = "Everyday",
                SubCategory = "Apparel",
                Price = 22.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal, "Universal"),
                Description = "Шапка Адидас Зелена",
                ImagePath = "/images/Adidas3-StripesGreenHat/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/Adidas3-StripesGreenHat/2.jpg" },
                }
            },
            new Product
            {
                Name ="New Balance 9060",
                Sport = "Running",
                SubCategory = "Shoes",
                Price = 90.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "39,40,41,42,43,44,45"),
                Description = "Responsive running shoes built for daily training.",
                ImagePath = "/images/NewBalance9060/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NewBalance9060/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/NewBalance9060/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/NewBalance9060/4.jpg" },
                    new ProductPhoto { ImagePath = "/images/NewBalance9060/5.jpg" }
                }
            },
            new Product
            {
                Name ="Nike Jordan Luka 77",
                Sport = "Bascketball",
                SubCategory = "Shoes",
                Price = 105.99m,
                Gender = GenderTarget.Men,
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "39,40,41,42,43,44,45"),
                Description = "С мрежа с висока устойчивост на износване и издръжлива гумена външна подметка, мъжките баскетболни обувки Nike Jordan Luka 77 са проектирани, за да ви помогнат да доминирате на открити игрища с груба настилка.",
                ImagePath = "/images/NikeJordanLuka77/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NikeJordanLuka77/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeJordanLuka77/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeJordanLuka77/4.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeJordanLuka77/5.jpg" }
                }
            },
            new Product
            {
                Name ="Nike V5 RNR",
                Sport = "Running",
                SubCategory = "Shoes",
                Price = 93.99m,
                Gender = GenderTarget.Men,
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "39,40,41,42,43,44,45"),
                Description = "Responsive running shoes built for daily training.",
                ImagePath = "/images/NikeV5RNR/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NikeV5RNR/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeV5RNR/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeV5RNR/4.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeV5RNR/5.jpg" }
                }
            },
            new Product
            {
                Name ="Adidas Anthony Edwards 1 Low",
                Sport = "Basketball",
                SubCategory = "Shoes",
                Price = 110.99m,
                Gender = GenderTarget.Men,
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "39,40,41,42,43,44,45"),
                Description = "Тези юношески баскетболни маратонки Anthony Edwards 1 Low на adidas са създадени за потвърдените майстори на коша.",
                ImagePath = "/images/AdidasAnthonyEdwards1Low/main.png",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/AdidasAnthonyEdwards1Low/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/AdidasAnthonyEdwards1Low/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/AdidasAnthonyEdwards1Low/4.jpg" },
                    new ProductPhoto { ImagePath = "/images/AdidasAnthonyEdwards1Low/5.jpg" }
                }
            },
            new Product
            {
                Name = "Jordan Basketball",
                Sport = "Basketball",
                SubCategory = "Apparel",
                Price = 30.67m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "Издръжливата гумена външна част помага да преминеш на следващо ниво в играта на открито, а дълбоките канали осигуряват оптимален контрол. Нищо не може да те спре: излез навън и играй с топка.",
                ImagePath = "/images/JordanBasketball/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/JordanBasketball/2.webp" }
                }
            },
            new Product
            {
                Name = "NBA Bottle",
                Sport = "Everyday",
                SubCategory = "Apparel",
                Price = 14.31m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "Stainless steel.",
                ImagePath = "/images/NBABottle/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NBABottle/2.webp" },
                    new ProductPhoto { ImagePath = "/images/NBABottle/3.webp" }
                }
            },
            ///////////////////guirmej ->
            
            new Product
            {
                Name = "Nike Elemental Backpack",
                Sport = "Everyday",
                SubCategory = "Apparel",
                Price = 20.75m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "",
                ImagePath = "/images/NikeElementalBackpack/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NikeElementalBackpack/main.webp" }

                }
            },
            new Product
            {
                Name = "Nike Jordan Pump",
                Sport = "Everyday",
                SubCategory = "Apparel",
                Price = 18.91m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "Бързо напомпае любимата си топка с помпата за топки Jordan Essentials.\r\nКомпактният ѝ дизайн я прави лесна за пренасяне и идеална за надуване на топки.",
                ImagePath = "/images/NikeJordanPump/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NikeJordanPump/main.webp" }

                }
            },
            new Product
            {
                Name = "Cars Backpack",
                Sport = "Everyday",
                SubCategory = "Apparel",
                Price = 17.89m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "Ученическа чанта, която може да се използва като количка, подходяща за деца в детската градина от Back Me Up в червен цвят с тема Cars.",
                ImagePath = "/images/KachouBackpack/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/KachouBackpack/2.webp" },

                }
            },
            new Product
            {
                Name = "MC Kinley Ski Mask",
                Sport = "Everyday",
                SubCategory = "Apparel",
                Price = 20.45m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "Детска Маска за ски Freeze 3.0",
                ImagePath = "/images/MCKinleyMask/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NikeJordanPump/main.webp" }

                }
            }/*
            ,new Product
            {
                Name = "Energetics Olimpic EZ Bar 150cm",
                Sport = "Fitness",
                SubCategory = "Apparel",
                Price = 109.41m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "Олимпийски крив лост от ENERGETICS, ексклузивната марка на INTERSPORT.\r\nТози уред за фитнес е подходящ за трениращи с всякакво ниво на физическа подготовка. Извитата му W-образна форма предпазва най-вече китките на ръцете. Може да се използва и в хотели, студиа и спортни зали.\r\nПодходящ за високоинтензивни тренировки с максимално натоварване до 220 кг и може да поема дискове с тежести с диаметър 50 мм. Изработен от висококачествена стомана Q235 за максимална здравина и издръжливост, с неплъзгащи се дръжки с диаметър 28 мм за максимална стабилност по време на тренировка. Има система за завинтване с медни гайки. Подходящ както за тренировки с тежести, така и за използване върху лежанка и ви дава възможност за упражнения за укрепване и оформяне на гърдите, гърба, раменете, бицепсите, трицепсите, гърба и др.\r\n",
                ImagePath = "/images/EnergeticsOlimpicEZBar/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/EnergeticsOlimpicEZBar/2.webp" },
                    new ProductPhoto { ImagePath = "/images/EnergeticsOlimpicEZBar/3.webp" },

                }
            },new Product
            {
                Name = "Energetics Olimpic Bar 200cm",
                Sport = "Fitness",
                SubCategory = "Apparel",
                Price = 139.07m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal,"Universal"),
                Description = "Олимпийски лост от ENERGETICS, ексклузивна марка на INTERSPORT.\r\nТози уред за фитнес е подходящ за трениращи с всякакво ниво на физическа подготовка. Може да се използва и в хотели, студиа и спортни зали.\r\nПодходящ за високоинтензивни тренировки с максимално натоварване до 220 кг и може да поема дискове с тежести с диаметър 50 мм. Изработен от висококачествена стомана Q235 за максимална здравина и издръжливост, с неплъзгащи се дръжки с диаметър 28 мм за максимална стабилност по време на тренировка. Има система за завинтване с медни гайки. Подходящ както за тренировки с тежести, така и за използване върху лежанка и ви дава възможност за упражнения за укрепване и оформяне на гърдите, гърба, раменете, бицепсите, трицепсите, гърба и др.",
                ImagePath = "/images/EnergeticsOlimpicBar/main.webp",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/EnergeticsOlimpicBar/main.webp" }


                }
            }
            */

        };

        foreach (var seedProduct in seedProducts)
        {
            await UpsertSeedProductAsync(context, seedProduct);
        }

        await context.SaveChangesAsync();
    }

    private static async Task UpsertSeedProductAsync(ApplicationDbContext context, Product seedProduct)
    {
        var existing = await context.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Name == seedProduct.Name);

        if (existing == null)
        {
            context.Products.Add(seedProduct);
            return;
        }

        existing.Sport = seedProduct.Sport;
        existing.SubCategory = seedProduct.SubCategory;
        existing.Price = seedProduct.Price;
        existing.Gender = seedProduct.Gender;
        existing.SizeType = seedProduct.SizeType;
        existing.AvailableSizes = seedProduct.AvailableSizes;
        existing.Description = seedProduct.Description;
        existing.ImagePath = seedProduct.ImagePath;

        if (existing.Photos != null && existing.Photos.Count > 0)
        {
            context.ProductPhotos.RemoveRange(existing.Photos);
        }

        existing.Photos = seedProduct.Photos
            .Select(photo => new ProductPhoto
            {
                ProductId = existing.Id,
                ImagePath = photo.ImagePath
            })
            .ToList();
    }
}