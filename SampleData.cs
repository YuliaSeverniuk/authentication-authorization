using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskAuthenticationAuthorization.Models;

namespace TaskAuthenticationAuthorization
{
    public class SampleData
    {
        public static void Initialize(ShoppingContext context)
        {
            SeedShopData(context);
            SeedUsers(context);
        }

        private static void SeedUsers(ShoppingContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            var hasher = new PasswordHasher<User>();
            User Create(string email, string password, string role, string buyerType, Customer customer)
            {
                var user = new User
                {
                    Email = email,
                    Role = role,
                    BuyerType = buyerType,
                    CustomerId = customer?.ID
                };
                user.PasswordHash = hasher.HashPassword(user, password);
                return user;
            }

            var ostap = context.Customers.FirstOrDefault(c => c.LastName == "Bender");
            var shura = context.Customers.FirstOrDefault(c => c.LastName == "Balaganov");

            context.Users.AddRange(
                Create("admin@shop.com", "admin123", Roles.Admin, BuyerTypes.None, null),
                Create("ostap@shop.com", "buyer123", Roles.Buyer, BuyerTypes.Golden, ostap),
                Create("shura@shop.com", "buyer123", Roles.Buyer, BuyerTypes.Regular, shura)
            );
            context.SaveChanges();
        }

        private static void SeedShopData(ShoppingContext context)
        {
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }
            context.Products.AddRange(
                    new Product
                    {
                        Name = "Butter",
                        Price = 30.0
                    },
                    new Product
                    {
                        Name = "Banana",
                        Price = 20.50
                    },
                    new Product
                    {
                        Name = "Cola",
                        Price = 9.30
                    }
                );
            context.SaveChanges();
            context.Customers.AddRange(
                    new Customer
                    {
                        FirstName = "Ostap",
                        LastName = "Bender",
                        Address = "Rio de Zhmerinka",
                        Discount = Discount.O,

                    },
                    new Customer
                    {
                        FirstName = "Shura",
                        LastName = "Balaganov",
                        Address = "Odessa",
                        Discount = Discount.R,

                    }
                );
            context.SaveChanges();
            context.SuperMarkets.AddRange(
                    new SuperMarket
                    {
                        Name = "Wellmart",
                        Address = "Lviv",

                    },
                    new SuperMarket
                    {
                        Name = "Billa",
                        Address = "Odessa",

                    }
                );
            context.SaveChanges();
            context.Orders.AddRange(
                    new Order
                    {
                        CustomerId = 1,
                        SuperMarketId = 1,
                        OrderDate = DateTime.Now,
                     },
                        new Order
                        {
                            CustomerId = 1,
                            SuperMarketId = 1,
                            OrderDate = DateTime.Now,
                         }
                );
            context.SaveChanges();
            context.OrderDetails.AddRange(
                    new OrderDetail
                    {
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 2

                    },
                        new OrderDetail
                        {
                            OrderId = 2,
                            ProductId = 2,
                            Quantity = 1
                        }
                );
            context.SaveChanges();
        }
    }
 }
