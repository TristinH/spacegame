using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpaceGame.Data;
using SpaceGame.Data.Models;
using System;
using System.Collections.Generic;
using TestSupport.EfHelpers;

namespace SpaceGame.Test
{
    public class TestBase : IDisposable
    {
        public AppDbContext TestContext { get; private set; }

        public TestBase()
        {
            DbContextOptions<AppDbContext> options = SqliteInMemory.CreateOptions<AppDbContext>(); ;
            TestContext = new AppDbContext(options);
            TestContext.Database.EnsureCreated();
            TestContext.SetUpData();
        }

        public void Dispose()
        {
            TestContext.Dispose();
        }
    }

    public static class TestData
    {
        public static IdentityUser user1, user2, user3;
        public static Player player1, player2, player3;
        public static Group group1, group2;

        public static void SetUpData(this AppDbContext context)
        {
            SetUpUsers(context);
            SetUpPlayers(context);
            SetUpGroups(context);
        }

        private static void SetUpUsers(AppDbContext context)
        {
            user1 = context.Users.Add(new IdentityUser() { UserName = "User1" }).Entity;
            user2 = context.Users.Add(new IdentityUser() { UserName = "User2" }).Entity;
            user3 = context.Users.Add(new IdentityUser() { UserName = "User3" }).Entity;
            context.SaveChanges();
        }

        private static void SetUpPlayers(AppDbContext context)
        {
            player1 = context.Players.Add(new Player() { Id = 1, Name = "Player1", User = user1 }).Entity;
            player2 = context.Players.Add(new Player() { Id = 2, Name = "Player2", User = user2 }).Entity;
            player3 = context.Players.Add(new Player() { Id = 3, Name = "Player3", User = user3 }).Entity;
            context.SaveChanges();
        }

        private static void SetUpGroups(AppDbContext context)
        {
            group1 = context.Groups.Add(new Group()
            {
                Id = 1,
                Name = "Group1",
                HostPlayer = player1,
                Clients = new List<Player>() { player2 },
                IsActive = true,
                IsInGame = false,
                StartTime = DateTime.Now
            }).Entity;

            group2 = context.Groups.Add(new Group()
            {
                Id = 2,
                Name = "Group2",
                HostPlayer = player3,
                Clients = null,
                IsActive = false,
                IsInGame = false,
                StartTime = DateTime.Now
            }).Entity;
            context.SaveChanges();
        }
    }
}
