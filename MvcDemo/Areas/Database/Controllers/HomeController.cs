using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcDemo.Data;

namespace MvcDemo.Areas.Database.Controllers
{
    public class HomeController : Controller
    {
        private readonly Db _db;

        #region Identity
        private readonly ApplicationDbContext _appDb;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        #endregion

        public HomeController(Db db, ApplicationDbContext appDb, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;

            #region Identity
            _appDb = appDb;
            _userManager = userManager;
            _roleManager = roleManager;
            #endregion
        }

        public async Task<IActionResult> Seed()
        {
            await SeedAllEntities();
            await SeedAll();

            #region Identity
            await SeedIdentities();
            #endregion

            return Content("<label style=\"color:red;\"><b>Database seed successful.</b></label>", "text/html");
        }

        private async Task SeedAllEntities()
        {
            _db.StoreEntities.RemoveRange(_db.StoreEntities.ToList());
            _db.CategoryEntities.RemoveRange(_db.CategoryEntities.ToList());

            _db.StoreEntities.Add(new StoreEntity()
            {
                Name = "Hepsiburada",
                IsVirtual = true,
                CreateDate = new DateTime(2022, 7, 18, 13, 56, 28),
                CreatedBy = "admin@admin.com"
            });
            _db.StoreEntities.Add(new StoreEntity()
            {
                Name = "Vatan",
                IsVirtual = false,
                CreateDate = new DateTime(2022, 7, 18, 23, 11, 1),
                CreatedBy = "admin@admin.com"
            });
            _db.StoreEntities.Add(new StoreEntity()
            {
                Name = "Migros",
                IsVirtual = false,
                CreateDate = new DateTime(2022, 6, 18, 1, 5, 7),
                CreatedBy = "admin@admin.com"
            });
            _db.StoreEntities.Add(new StoreEntity()
            {
                Name = "Teknosa",
                IsVirtual = false,
                CreateDate = new DateTime(2022, 1, 1, 15, 34, 0),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2022, 2, 1, 22, 11, 7),
                UpdatedBy = "admin@admin.com"
            });
            _db.StoreEntities.Add(new StoreEntity()
            {
                Name = "İtopya",
                IsVirtual = false,
                CreateDate = new DateTime(2021, 11, 11, 5, 36, 12),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2021, 11, 12, 17, 18, 19),
                UpdatedBy = "admin@admin.com"
            });
            _db.StoreEntities.Add(new StoreEntity()
            {
                Name = "Sahibinden",
                IsVirtual = true,
                CreateDate = new DateTime(2020, 3, 25, 14, 13, 12),
                CreatedBy = "admin@admin.com"
            });

            await _db.SaveChangesAsync();

            _db.CategoryEntities.Add(new CategoryEntity()
            {
                CreateDate = new DateTime(2022, 7, 18, 13, 56, 28),
                CreatedBy = "admin@admin.com",
                Name = "Computer",
                Description = "Laptops, desktops and computer peripherals",
                ProductEntities = new List<ProductEntity>()
                {
                    new ProductEntity()
                    {
                        Name = "Laptop",
                        UnitPrice = 3000.5,
                        ExpirationDate = new DateTime(2032, 1, 27),
                        StockAmount = 10,
                        ProductStoreEntities = new List<ProductStoreEntity>()
                        {
                            new ProductStoreEntity()
                            {
                                StoreEntityId = _db.StoreEntities.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            }
                        }
                    },
                    new ProductEntity()
                    {
                        Name = "Mouse",
                        UnitPrice = 20.5,
                        Description = "Computer peripheral",
                        ProductStoreEntities = new List<ProductStoreEntity>()
                        {
                            new ProductStoreEntity()
                            {
                                StoreEntityId = _db.StoreEntities.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStoreEntity()
                            {
                                StoreEntityId = _db.StoreEntities.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new ProductEntity()
                    {
                        Name = "Keyboard",
                        UnitPrice = 40,
                        StockAmount = 45,
                        Description = "Computer peripheral",
                        ProductStoreEntities = new List<ProductStoreEntity>()
                        {
                            new ProductStoreEntity()
                            {
                                StoreEntityId = _db.StoreEntities.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStoreEntity()
                            {
                                StoreEntityId = _db.StoreEntities.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new ProductEntity()
                    {
                        Name = "Monitor",
                        UnitPrice = 2500,
                        ExpirationDate = DateTime.Parse("05/19/2027"),
                        StockAmount = 20,
                        Description = "Computer peripheral",
                        ProductStoreEntities = new List<ProductStoreEntity>()
                        {
                            new ProductStoreEntity()
                            {
                                StoreEntityId = _db.StoreEntities.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    }
                }
            });

            _db.CategoryEntities.Add(new CategoryEntity()
            {
                CreateDate = new DateTime(2022, 7, 18, 23, 11, 1),
                CreatedBy = "admin@admin.com",
                Name = "Home Theater System",
                ProductEntities = new List<ProductEntity>()
                {
                    new ProductEntity()
                    {
                        Name = "Speaker",
                        UnitPrice = 2500,
                        StockAmount = 70
                    },
                    new ProductEntity()
                    {
                        Name = "Receiver",
                        UnitPrice = 5000,
                        StockAmount = 30,
                        Description = "Home theater system component",
                        ProductStoreEntities = new List<ProductStoreEntity>()
                        {
                            new ProductStoreEntity()
                            {
                                StoreEntityId = _db.StoreEntities.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new ProductEntity()
                    {
                        Name = "Equalizer",
                        UnitPrice = 1000,
                        StockAmount = 40
                    }
                }
            });

            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Phone",
                Description = "IOS and Android Phones",
                CreateDate = new DateTime(2022, 6, 18, 1, 5, 7),
                CreatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Food",
                CreateDate = new DateTime(2022, 1, 1, 15, 34, 0),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2022, 2, 1, 22, 11, 7),
                UpdatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Medicine",
                Description = "Antibiotics, Vitamins, Pain Killers, etc.",
                CreateDate = new DateTime(2021, 11, 11, 5, 36, 12),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2021, 11, 12, 17, 18, 19),
                UpdatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Software",
                Description = "Operating Systems, Antivirus Software, Office Software and Video Games",
                CreateDate = new DateTime(2020, 3, 25, 14, 13, 12),
                CreatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Vehicle",
                Description = "Cars, Trucks, SUVs, etc.",
                CreateDate = new DateTime(2020, 5, 7, 4, 6, 8),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2021, 1, 2, 21, 12, 21),
                UpdatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Pet",
                Description = "Dogs, Cats, Birds, Fishes, etc.",
                CreateDate = new DateTime(2019, 9, 8, 13, 10, 5),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2019, 9, 18, 11, 55, 44),
                UpdatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Kitchen",
                Description = "Refrigerators, Microwaves, Owens, etc.",
                CreateDate = new DateTime(2019, 9, 8, 9, 8, 7),
                CreatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Toy",
                CreateDate = new DateTime(2019, 9, 8, 10, 42, 34),
                CreatedBy = "admin@admin.com"
            });
            _db.CategoryEntities.Add(new CategoryEntity()
            {
                Name = "Garden",
                CreateDate = new DateTime(2018, 10, 20, 16, 8, 57),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2018, 10, 30, 9, 5, 14),
                UpdatedBy = "admin@admin.com"
            });

            await _db.SaveChangesAsync();
        }

        private async Task SeedAll()
        {
            _db.Stores.RemoveRange(_db.Stores.ToList());
            _db.Categories.RemoveRange(_db.Categories.ToList());

            _db.Stores.Add(new Store()
            {
                Name = "Hepsiburada",
                IsVirtual = true,
                CreateDate = new DateTime(2022, 7, 18, 13, 56, 28),
                CreatedBy = "admin@admin.com"
            });
            _db.Stores.Add(new Store()
            {
                Name = "Vatan",
                IsVirtual = false,
                CreateDate = new DateTime(2022, 7, 18, 23, 11, 1),
                CreatedBy = "admin@admin.com"
            });
            _db.Stores.Add(new Store()
            {
                Name = "Migros",
                IsVirtual = false,
                CreateDate = new DateTime(2022, 6, 18, 1, 5, 7),
                CreatedBy = "admin@admin.com"
            });
            _db.Stores.Add(new Store()
            {
                Name = "Teknosa",
                IsVirtual = false,
                CreateDate = new DateTime(2022, 1, 1, 15, 34, 0),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2022, 2, 1, 22, 11, 7),
                UpdatedBy = "admin@admin.com"
            });
            _db.Stores.Add(new Store()
            {
                Name = "İtopya",
                IsVirtual = false,
                CreateDate = new DateTime(2021, 11, 11, 5, 36, 12),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2021, 11, 12, 17, 18, 19),
                UpdatedBy = "admin@admin.com"
            });
            _db.Stores.Add(new Store()
            {
                Name = "Sahibinden",
                IsVirtual = true,
                CreateDate = new DateTime(2020, 3, 25, 14, 13, 12),
                CreatedBy = "admin@admin.com"
            });

            await _db.SaveChangesAsync();

            _db.Categories.Add(new Category()
            {
                CreateDate = new DateTime(2022, 7, 18, 13, 56, 28),
                CreatedBy = "admin@admin.com",
                Name = "Computer",
                Description = "Laptops, desktops and computer peripherals",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Laptop",
                        UnitPrice = 3000.5,
                        ExpirationDate = new DateTime(2032, 1, 27),
                        StockAmount = 10,
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Mouse",
                        UnitPrice = 20.5,
                        Description = "Computer peripheral",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Keyboard",
                        UnitPrice = 40,
                        StockAmount = 45,
                        Description = "Computer peripheral",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Hepsiburada").Id
                            },
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Monitor",
                        UnitPrice = 2500,
                        ExpirationDate = DateTime.Parse("05/19/2027"),
                        StockAmount = 20,
                        Description = "Computer peripheral",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    }
                }
            });

            _db.Categories.Add(new Category()
            {
                CreateDate = new DateTime(2022, 7, 18, 23, 11, 1),
                CreatedBy = "admin@admin.com",
                Name = "Home Theater System",
                Products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Speaker",
                        UnitPrice = 2500,
                        StockAmount = 70
                    },
                    new Product()
                    {
                        Name = "Receiver",
                        UnitPrice = 5000,
                        StockAmount = 30,
                        Description = "Home theater system component",
                        ProductStores = new List<ProductStore>()
                        {
                            new ProductStore()
                            {
                                StoreId = _db.Stores.SingleOrDefault(s => s.Name == "Vatan").Id
                            }
                        }
                    },
                    new Product()
                    {
                        Name = "Equalizer",
                        UnitPrice = 1000,
                        StockAmount = 40
                    }
                }
            });

            _db.Categories.Add(new Category()
            {
                Name = "Phone",
                Description = "IOS and Android Phones",
                CreateDate = new DateTime(2022, 6, 18, 1, 5, 7),
                CreatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Food",
                CreateDate = new DateTime(2022, 1, 1, 15, 34, 0),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2022, 2, 1, 22, 11, 7),
                UpdatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Medicine",
                Description = "Antibiotics, Vitamins, Pain Killers, etc.",
                CreateDate = new DateTime(2021, 11, 11, 5, 36, 12),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2021, 11, 12, 17, 18, 19),
                UpdatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Software",
                Description = "Operating Systems, Antivirus Software, Office Software and Video Games",
                CreateDate = new DateTime(2020, 3, 25, 14, 13, 12),
                CreatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Vehicle",
                Description = "Cars, Trucks, SUVs, etc.",
                CreateDate = new DateTime(2020, 5, 7, 4, 6, 8),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2021, 1, 2, 21, 12, 21),
                UpdatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Pet",
                Description = "Dogs, Cats, Birds, Fishes, etc.",
                CreateDate = new DateTime(2019, 9, 8, 13, 10, 5),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2019, 9, 18, 11, 55, 44),
                UpdatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Kitchen",
                Description = "Refrigerators, Microwaves, Owens, etc.",
                CreateDate = new DateTime(2019, 9, 8, 9, 8, 7),
                CreatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Toy",
                CreateDate = new DateTime(2019, 9, 8, 10, 42, 34),
                CreatedBy = "admin@admin.com"
            });
            _db.Categories.Add(new Category()
            {
                Name = "Garden",
                CreateDate = new DateTime(2018, 10, 20, 16, 8, 57),
                CreatedBy = "admin@admin.com",
                UpdateDate = new DateTime(2018, 10, 30, 9, 5, 14),
                UpdatedBy = "admin@admin.com"
            });

            await _db.SaveChangesAsync();
        }

        private async Task SeedIdentities()
        {
            #region Identity
            _appDb.UserRoles.RemoveRange(_appDb.UserRoles.ToList());
            _appDb.Roles.RemoveRange(_appDb.Roles.ToList());
            _appDb.Users.RemoveRange(_appDb.Users.ToList());

            await _db.SaveChangesAsync();

            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "admin"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "user"
            });

            var admin = new IdentityUser()
            {
                UserName = "admin@admin.com",
                EmailConfirmed = true
            };
            var user = new IdentityUser()
            {
                UserName = "user@user.com",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(admin, "Admin123!");
            await _userManager.CreateAsync(user, "User123!");

            await _userManager.AddToRoleAsync(admin, "admin");
            await _userManager.AddToRoleAsync(user, "user");
            #endregion
        }
    }
}
