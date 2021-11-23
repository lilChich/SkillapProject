using Microsoft.AspNetCore.Identity;
using Skillap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.DAL.EF
{
    public class DataSeed
    {

        public static async Task SeedDataAsync(DataContext context, UserManager<ApplicationUsers> userManager, RoleManager<ApplicationRole> roleManager)
        {

            if (!userManager.Users.Any())
            {
                var users = new List<ApplicationUsers>
                {
                    new ApplicationUsers
                    {
                        FirstName = "Bohdan",
                        SecondName = "Butenko",
                        NickName = "Aboba",
                        Gender = true,
                        DateOfBirth = new DateTime(2001, 10, 31),
                        Country = "Ukraine",
                        Education = "Kharkiv National University of Radioelectronics 121",
                        Image = null,
                        Email = "noobbogdan@gmail.com",
                        UserName = "noobbogdan@gmail.com"

                    },

                    new ApplicationUsers
                    {
                        FirstName = "Andrei",
                        SecondName = "Abobov",
                        NickName = "Sasuke",
                        Gender = true,
                        DateOfBirth = new DateTime(2005, 06, 20),
                        Country = "Ukraine",
                        Education = "Kharkiv National University of Radioelectronics 121",
                        Image = null,
                        Email = "moderator@gmail.com",
                        UserName = "moderator@gmail.com"
                    },

                    new ApplicationUsers
                    {
                        FirstName = "Borya",
                        SecondName = "Akimbo",
                        NickName = "Akimbo69",
                        Gender = false,
                        DateOfBirth = new DateTime(1998, 08, 23),
                        Country = "USA",
                        Education = "University of San Diego",
                        Image = null,
                        Email = "user@gmail.com",
                        UserName = "user@gmail.com"
                    },

                    new ApplicationUsers
                    {
                        FirstName = "Travis",
                        SecondName = "Scott",
                        NickName = "AsapRocky",
                        Gender = true,
                        DateOfBirth = new DateTime(1991, 05, 10),
                        Country = "USA",
                        Education = "University of San Diego",
                        Image = null,
                        Email = "vip@gmail.com",
                        UserName = "vip@gmail.com"
                    }
                };

                foreach (var user in users)
                {
                    /*if (!await roleManager.RoleExistsAsync("Admin") && user.Email.Contains("noobbogdan"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "Admin" });
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                    *//*if(user.Email.Contains("noobbogdan"))
                    {

                    }*//*

                    if (!await roleManager.RoleExistsAsync("Moderator") && user.Email.Contains("moderator"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "Moderator" });
                        await userManager.AddToRoleAsync(user, "Moderator");
                    }
                    *//*else if ()
                    {
                        
                    }*//*

                    if (!await roleManager.RoleExistsAsync("User") && user.Email.Contains("user"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "User" });
                        await userManager.AddToRoleAsync(user, "User");
                    }
                    *//*else if ()
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }*//*

                    if (!await roleManager.RoleExistsAsync("VIP") && user.Email.Contains("vip"))
                    {
                        await roleManager.CreateAsync(new ApplicationRole() { Name = "VIP" });
                        await userManager.AddToRoleAsync(user, "VIP");
                    }
*//*                    else if ()
                    {
                        
                    }*/

                    await userManager.CreateAsync(user, "Qwerty7337@");
                }
            }
        }
    }
}
