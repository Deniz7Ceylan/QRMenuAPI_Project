using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Models;

namespace QRMenuAPI.Data;

public class DBInitializer
{
    public DBInitializer(ApplicationDbContext? context, RoleManager<IdentityRole>? roleManager, UserManager<ApplicationUser>? userManager)
    {
        context.Database.EnsureCreated();

        State state;
        IdentityRole identityRole;
        ApplicationUser applicationUser;
        Company? company = null;

        // State değerlerini ekleyin (sadece boş olan tabloya)
        if (!context.States.Any())
        {
            context.States.AddRange(
                new State { Id = 0, Name = "Deleted" },
                new State { Id = 1, Name = "Active" },
                new State { Id = 2, Name = "Passive" }
            );
            context.SaveChanges();
        }

        if (roleManager != null)
        {
            if (roleManager.Roles.Count() == 0)
            {
                //identityRole = new IdentityRole("Administrator");
                //roleManager.CreateAsync(identityRole).Wait();
                identityRole = new IdentityRole("CompanyAdministrator");
                roleManager.CreateAsync(identityRole).Wait();
                identityRole = new IdentityRole("BrandAdministrator");
                roleManager.CreateAsync(identityRole).Wait();
                identityRole = new IdentityRole("RestaurantAdministrator");
                roleManager.CreateAsync(identityRole).Wait();
            }
        }

        // TAB Gıda company'sini oluşturun (sadece bir kez oluşturulmalı)
        if (!context.Companies.Any(c => c.Name == "TAB Gıda"))
        {
            var tabGidaCompany = new Company
            {
                Name = "TAB Gıda",
                PostalCode = "34349",
                Address = "Dikilitaş Mahallesi Emirhan Caddesi No:109 Beşiktaş / İstanbul",
                Phone = "02123106600",
                EMail = "tabgida@tabgida.com.tr",
                RegisterDate = DateTime.Now, // Şu anki tarih
                WebAddress = "http://www.tabgida.com.tr",
                StateId = 1
            };
            context.Companies.Add(tabGidaCompany);
            context.SaveChanges();

            // TAB Gıda company'sine ait bir ApplicationUser oluşturun
            var tabGidaUser = new ApplicationUser
            {
                UserName = "TabAdmin",
                //Password = "Tabgida.123",
                Name = "TAB",
                SurName = "Gıda",
                Email = "tabgidaadmin@tabgida.com.tr",
                PhoneNumber = "02123106600",
                RegisterDate = DateTime.Now,
                CompanyId = tabGidaCompany.Id,
                StateId = 1,
            };
            userManager.CreateAsync(tabGidaUser, "Tabgida.123").Wait();
            userManager.AddToRoleAsync(tabGidaUser, "CompanyAdministrator").Wait();
            context.SaveChanges();
        }

        if (!context.Brands.Any())
        {
            // Burger King markasının eklenmesi

            var burgerKing = new Brand
            {
                Name = "Burger King",
                PostalCode = "12345",
                Address = "BKAdres",
                Phone = "1234567890",
                EMail = "info@burgerking.com",
                RegisterDate = DateTime.Now,
                TaxNumber = "1234567890",
                WebAddress = "https://www.burgerking.com",
                Company = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda"),
                StateId = 1
            };
            context.Brands.Add(burgerKing);

            //Arby's
            var arbys = new Brand
            {
                Name = "Arby's",
                PostalCode = "12345",
                Address = "ArbysAdres",
                Phone = "1234567890",
                EMail = "info@arbys.com",
                RegisterDate = DateTime.Now,
                TaxNumber = "1234567890",
                WebAddress = "https://www.arbys.com",
                Company = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda"),
                StateId = 1
            };
            context.Brands.Add(arbys);

            //Popeyes
            var popeyes = new Brand
            {
                Name = "Popeyes",
                PostalCode = "12345",
                Address = "PopeyesAdres",
                Phone = "1234567890",
                EMail = "info@popeyes.com",
                RegisterDate = DateTime.Now,
                TaxNumber = "1234567890",
                WebAddress = "https://www.popeyes.com",
                Company = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda"),
                StateId = 1
            };
            context.Brands.Add(popeyes);

            //Usta Dönerci
            var ustaDonerci = new Brand
            {
                Name = "Usta Dönerci",
                PostalCode = "12345",
                Address = "UDAdres",
                Phone = "1234567890",
                EMail = "info@ustadonerci.com",
                RegisterDate = DateTime.Now,
                TaxNumber = "1234567890",
                WebAddress = "https://www.ustadonerci.com",
                Company = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda"),
                StateId = 1
            };
            context.Brands.Add(ustaDonerci);

            //Sbarro
            var sbarro = new Brand
            {
                Name = "Sbarro",
                PostalCode = "12345",
                Address = "SbarroAdres",
                Phone = "1234567890",
                EMail = "info@sbarro.com",
                RegisterDate = DateTime.Now,
                TaxNumber = "1234567890",
                WebAddress = "https://www.sbarro.com",
                Company = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda"),
                StateId = 1
            };
            context.Brands.Add(sbarro);

            //Usta Pideci
            var ustaPideci = new Brand
            {
                Name = "Usta Pideci",
                PostalCode = "12345",
                Address = "UstaPideciAdres",
                Phone = "1234567890",
                EMail = "info@ustapideci.com",
                RegisterDate = DateTime.Now,
                TaxNumber = "1234567890",
                WebAddress = "https://www.ustapideci.com",
                Company = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda"),
                StateId = 1
            };
            context.Brands.Add(ustaPideci);

            //Subway
            var subway = new Brand
            {
                Name = "Subway",
                PostalCode = "12345",
                Address = "SubwayAdres",
                Phone = "1234567890",
                EMail = "info@subway.com",
                RegisterDate = DateTime.Now,
                TaxNumber = "1234567890",
                WebAddress = "https://www.subway.com",
                Company = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda"),
                StateId = 1
            };
            context.Brands.Add(subway);
            context.SaveChanges();


            if (!context.BrandUsers.Any())
            {
                var burgerKingAdmin = new BrandUser
                {
                    ApplicationUser = new ApplicationUser
                    {
                        Id = userManager.GetUserIdAsync(new ApplicationUser { UserName = "BurgerKingAdmin" }).Result.ToString(),
                        UserName = "BurgerKingAdmin",
                        //Password = "Burgerking.123",
                        Name = "Admin",
                        SurName = "Burger King",
                        Email = "burgerkingadmin@example.com",
                        PhoneNumber = "1234567890",
                        CompanyId = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda").Id,
                        StateId = 1,
                    },
                    BrandId = context.Brands.FirstOrDefault(s => s.Name == "Burger King").Id,
                };
                context.BrandUsers.Add(burgerKingAdmin);
                userManager.CreateAsync(burgerKingAdmin.ApplicationUser, "Burgerking.123").Wait();
                userManager.AddToRoleAsync(burgerKingAdmin.ApplicationUser, "BrandAdministrator").Wait();

                var arbysAdmin = new BrandUser
                {
                    ApplicationUser = new ApplicationUser
                    {
                        UserName = "ArbysAdmin",
                        //Password = "Arbys.123",
                        Name = "Admin",
                        SurName = "arbys",
                        Email = "arbysadmin@example.com",
                        PhoneNumber = "1234567890",
                        RegisterDate = DateTime.Now,
                        StateId = 1,
                        CompanyId = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda").Id
                    },
                    BrandId = context.Brands.FirstOrDefault(s => s.Name == "Arby's").Id,
                    UserId = userManager.GetUserIdAsync(new ApplicationUser { UserName = "ArbysAdmin" }).Result.ToString()
                };
                context.BrandUsers.Add(arbysAdmin);
                userManager.CreateAsync(arbysAdmin.ApplicationUser, "Arbys.123").Wait();
                userManager.AddToRoleAsync(arbysAdmin.ApplicationUser, "BrandAdministrator").Wait();

                var popeyesAdmin = new BrandUser
                {
                    ApplicationUser = new ApplicationUser
                    {
                        UserName = "PopeyesAdmin",
                        //Password = "Popeyes.123",
                        Name = "Admin",
                        SurName = "popeyes",
                        Email = "popeyesadmin@example.com",
                        PhoneNumber = "1234567890",
                        RegisterDate = DateTime.Now,
                        StateId = 1,
                        CompanyId = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda").Id
                    },
                    BrandId = context.Brands.FirstOrDefault(s => s.Name == "Popeyes").Id,
                };
                context.BrandUsers.Add(popeyesAdmin);
                userManager.CreateAsync(popeyesAdmin.ApplicationUser, "Popeyes.123").Wait();
                userManager.AddToRoleAsync(popeyesAdmin.ApplicationUser, "BrandAdministrator").Wait();

                var ustaDonerciAdmin = new BrandUser
                {
                    ApplicationUser = new ApplicationUser
                    {
                        UserName = "UstaDonerciAdmin",
                        //Password = "Ustadonerci.123",
                        Name = "Admin",
                        SurName = "ustadonerci",
                        Email = "ustadonerciadmin@example.com",
                        PhoneNumber = "1234567890",
                        RegisterDate = DateTime.Now,
                        StateId = 1,
                        CompanyId = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda").Id
                    },
                    BrandId = context.Brands.FirstOrDefault(s => s.Name == "Usta Dönerci").Id
                };
                context.BrandUsers.Add(ustaDonerciAdmin);
                userManager.CreateAsync(ustaDonerciAdmin.ApplicationUser, "Ustadonerci.123").Wait();
                userManager.AddToRoleAsync(ustaDonerciAdmin.ApplicationUser, "BrandAdministrator").Wait();

                var sbarroAdmin = new BrandUser
                {
                    ApplicationUser = new ApplicationUser
                    {
                        UserName = "SbarroAdmin",
                        //Password = "Sbarro.123",
                        Name = "Admin",
                        SurName = "sbarro",
                        Email = "sbarroadmin@example.com",
                        PhoneNumber = "1234567890",
                        RegisterDate = DateTime.Now,
                        StateId = 1,
                        CompanyId = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda").Id
                    },
                    BrandId = context.Brands.FirstOrDefault(s => s.Name == "Sbarro").Id
                };
                context.BrandUsers.Add(sbarroAdmin);
                userManager.CreateAsync(sbarroAdmin.ApplicationUser, "Sbarro.123").Wait();
                userManager.AddToRoleAsync(sbarroAdmin.ApplicationUser, "BrandAdministrator").Wait();

                var ustaPideciAdmin = new BrandUser
                {
                    ApplicationUser = new ApplicationUser
                    {
                        UserName = "UstaPideciAdmin",
                        //Password = "Ustapideci.123",
                        Name = "Admin",
                        SurName = "ustapideci",
                        Email = "ustapideciadmin@example.com",
                        PhoneNumber = "1234567890",
                        RegisterDate = DateTime.Now,
                        StateId = 1,
                        CompanyId = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda").Id
                    },
                    BrandId = context.Brands.FirstOrDefault(s => s.Name == "Usta Pideci").Id
                };
                context.BrandUsers.Add(ustaPideciAdmin);
                userManager.CreateAsync(ustaPideciAdmin.ApplicationUser, "Ustapideci.123").Wait();
                userManager.AddToRoleAsync(ustaPideciAdmin.ApplicationUser, "BrandAdministrator").Wait();

                var subwayAdmin = new BrandUser
                {
                    ApplicationUser = new ApplicationUser
                    {
                        UserName = "SubwayAdmin",
                        //Password = "Subway.123",
                        Name = "Admin",
                        SurName = "subway",
                        Email = "subwayadmin@example.com",
                        PhoneNumber = "1234567890",
                        RegisterDate = DateTime.Now,
                        StateId = 1,
                        CompanyId = context.Companies.FirstOrDefault(s => s.Name == "TAB Gıda").Id
                    },
                    BrandId = context.Brands.FirstOrDefault(s => s.Name == "Subway").Id
                };
                context.BrandUsers.Add(subwayAdmin);
                userManager.CreateAsync(subwayAdmin.ApplicationUser, "Subway.123").Wait();
                userManager.AddToRoleAsync(subwayAdmin.ApplicationUser, "BrandAdministrator").Wait();
            }
        }
        context.SaveChanges();
    }
}