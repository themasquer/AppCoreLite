using AppCoreLite.Entities;
using AppCoreLite.Enums;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcDemo.Data;

namespace MvcDemo.Areas.Database.Controllers
{
    [Area("Database")]
    public class HomeController : Controller
    {
        private readonly Db _db;
        private readonly UnitDb _unitDb;

        #region Identity
        private readonly ApplicationDbContext _appDb;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        #endregion

        public HomeController(Db db, UnitDb unitDb, ApplicationDbContext appDb, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _unitDb = unitDb;

            #region Identity
            _appDb = appDb;
            _userManager = userManager;
            _roleManager = roleManager;
            #endregion
        }

        public async Task<IActionResult> Seed()
        {
            ReSeedEntities();
            await SeedAllEntities();

            ReSeed();
            await SeedAll();

            #region Identity
            await SeedIdentities();
            #endregion

            #region Account
            ReSeedAccounts();
            await SeedAccounts();
            #endregion

            #region Organization Chart
            ReSeedUnits();
            await SeedUnits();
            #endregion

            //return Content("<label style=\"color:red;\"><b>Database seed successful.</b></label>", "text/html");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        private void ReSeedEntities()
        {
            if (_db.StoreEntities.Count() > 0 || _db.CategoryEntities.Count() > 0 || _db.ProductEntities.Count() > 0)
            {
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('StoreEntities', RESEED, 0)");
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('ProductEntities', RESEED, 0)");
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('CategoryEntities', RESEED, 0)");
            }
        }

        private void ReSeed()
        {
            if (_db.Stores.Count() > 0 || _db.Categories.Count() > 0 || _db.Products.Count() > 0)
            {
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Stores', RESEED, 0)");
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Products', RESEED, 0)");
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('Categories', RESEED, 0)");
            }
        }

        private void ReSeedAccounts()
        {
            if (_db.AccountRoles.Count() > 0 || _db.AccountUsers.Count() > 0)
            {
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('AccountRoles', RESEED, 0)");
                _db.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('AccountUsers', RESEED, 0)");
            }
        }

        private void ReSeedUnits()
        {
            if (_unitDb.TreeNodes.Count() > 0 || _unitDb.TreeNodeDetails.Count() > 0)
            {
                _unitDb.Database.ExecuteSqlRaw("dbcc CHECKIDENT ('TreeNodeDetails', RESEED, 0)");
            }
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
        }

        private async Task SeedAccounts()
        {
            _db.AccountUsers.RemoveRange(_db.AccountUsers.ToList());
            _db.AccountRoles.RemoveRange(_db.AccountRoles.ToList());

            _db.AccountRoles.Add(new AccountRole()
            {
                RoleName = Roles.Admin.ToString().ToLower(),
                AccountUsers = new List<AccountUser>()
                {
                    new AccountUser()
                    {
                        UserName = "admin@admin.com",
                        Password = "Admin123!",
                        IsActive = true
                    },
                    new AccountUser()
                    {
                        UserName = "string",
                        Password = "string",
                        IsActive = true
                    }
                }
            });
            _db.AccountRoles.Add(new AccountRole()
            {
                RoleName = Roles.User.ToString().ToLower(),
                AccountUsers = new List<AccountUser>()
                {
                    new AccountUser()
                    {
                        UserName = "user@user.com",
                        Password = "User123!",
                        IsActive = true
                    }
                }
            });

            await _db.SaveChangesAsync();
        }

        private async Task SeedUnits()
        {
            _unitDb.TreeNodes.RemoveRange(_unitDb.TreeNodes.ToList());

            await _unitDb.SaveChangesAsync();

            _unitDb.TreeNodeDetails.RemoveRange(_unitDb.TreeNodeDetails.ToList());

            _unitDb.TreeNodeDetails.Add(new TreeNodeDetail()
            {
                TextTurkish = "Genel Müdür",
                TextEnglish = "General Manager",
                Level = 1
            });
            _unitDb.TreeNodeDetails.Add(new TreeNodeDetail()
            {
                TextTurkish = "Genel Müdür Yardımcısı",
                TextEnglish = "Vice Manager",
                Level = 2
            });
            _unitDb.TreeNodeDetails.Add(new TreeNodeDetail()
            {
                TextTurkish = "Yönetici",
                TextEnglish = "Director",
                Level = 3
            });
            _unitDb.TreeNodeDetails.Add(new TreeNodeDetail()
            {
                TextTurkish = "Müdür",
                TextEnglish = "Manager",
                Level = 3
            });

            await _unitDb.SaveChangesAsync();

            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 1,
                ParentId = 0,
                NameTurkish = "Yönetim",
                NameEnglish = "Management",
                TextTurkish = "Genel Müdürlük",
                TextEnglish = "General Management",
                AbbreviationTurkish = "GM",
                AbbreviationEnglish = "GM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Genel Müdür").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 2,
                ParentId = 1,
                NameTurkish = "Ürün",
                NameEnglish = "Product",
                TextTurkish = "Genel Müdür Yardımcılığı",
                TextEnglish = "Vice Management",
                AbbreviationTurkish = "ÜGMY",
                AbbreviationEnglish = "PVM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Genel Müdür Yardımcısı").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 3,
                ParentId = 1,
                NameTurkish = "Kategori",
                NameEnglish = "Category",
                TextTurkish = "Genel Müdür Yardımcılığı",
                TextEnglish = "Vice Management",
                AbbreviationTurkish = "KGMY",
                AbbreviationEnglish = "CVM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Genel Müdür Yardımcısı").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 4,
                ParentId = 1,
                NameTurkish = "Mağaza",
                NameEnglish = "Store",
                TextTurkish = "Genel Müdür Yardımcılığı",
                TextEnglish = "Vice Management",
                AbbreviationTurkish = "MGMY",
                AbbreviationEnglish = "SVM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Genel Müdür Yardımcısı").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 5,
                ParentId = 3,
                NameTurkish = "Elektronik",
                NameEnglish = "Electronics",
                TextTurkish = "Yöneticiliği",
                TextEnglish = "Management",
                AbbreviationTurkish = "EY",
                AbbreviationEnglish = "EM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Yönetici").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 6,
                ParentId = 3,
                NameTurkish = "Yiyecek ve İçecek",
                NameEnglish = "Food and Drinks",
                TextTurkish = "Yöneticiliği",
                TextEnglish = "Management",
                AbbreviationTurkish = "YİY",
                AbbreviationEnglish = "FDM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Yönetici").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 7,
                ParentId = 3,
                NameTurkish = "İlaç",
                NameEnglish = "Medicine",
                TextTurkish = "Yöneticiliği",
                TextEnglish = "Management",
                AbbreviationTurkish = "İY",
                AbbreviationEnglish = "MM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Yönetici").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 8,
                ParentId = 3,
                NameTurkish = "Evcil Hayvan",
                NameEnglish = "Pet",
                TextTurkish = "Yöneticiliği",
                TextEnglish = "Management",
                AbbreviationTurkish = "EHY",
                AbbreviationEnglish = "PM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Yönetici").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 9,
                ParentId = 4,
                NameTurkish = "Ankara Mağaza",
                NameEnglish = "Ankara Store",
                TextTurkish = "Yöneticiliği",
                TextEnglish = "Management",
                AbbreviationTurkish = "ANKMY",
                AbbreviationEnglish = "ANKSM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Müdür").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 10,
                ParentId = 4,
                NameTurkish = "İstanbul Mağaza",
                NameEnglish = "Istanbul Store",
                TextTurkish = "Yöneticiliği",
                TextEnglish = "Management",
                AbbreviationTurkish = "İSTMY",
                AbbreviationEnglish = "ISTSM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Müdür").Id
            });
            _unitDb.TreeNodes.Add(new TreeNode()
            {
                Id = 11,
                ParentId = 4,
                NameTurkish = "İzmir Mağaza",
                NameEnglish = "İzmir Store",
                TextTurkish = "Yöneticiliği",
                TextEnglish = "Management",
                AbbreviationTurkish = "İZMMY",
                AbbreviationEnglish = "IZMSM",
                IsActive = true,
                TreeNodeDetailId = _unitDb.TreeNodeDetails.SingleOrDefault(tnd => tnd.TextTurkish == "Müdür").Id
            });

            await _unitDb.SaveChangesAsync();
        }
    }
}
