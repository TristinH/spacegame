using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpaceGame.Controllers.Api;
using SpaceGame.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Data.Services
{
    public class LobbyService
    {
        private readonly AppDbContext _context;

        public LobbyService(AppDbContext context)
        {
            _context = context;
        }

        public Group GetGroupById(long id)
        {
            return _context.Groups
                .Include(g => g.HostPlayer)
                .Include(g => g.Clients)
                .FirstOrDefault(g => g.Id == id);
        }

        public IEnumerable<Group> GetActiveGroups()
        {
            return _context.Groups
                .Include(g => g.HostPlayer)
                .Include(g => g.Clients)
                .Where(g => g.IsActive && !g.IsInGame);
        }

        public Group StartGroup(IdentityUser user, StartGroupRequest requestInfo)
        {
            Player hostPlayer = new Player
            {
                User = user,
                Name = requestInfo.HostPlayerName,
                Role = null
            };
            _context.Players.Add(hostPlayer);

            Group group = new Group
            {
                Name = requestInfo.GroupName,
                StartTime = DateTime.Now,
                IsActive = true,
                IsInGame = false,
                HostPlayer = hostPlayer
            };
            _context.Groups.Add(group);
            _context.SaveChanges();

            return group;
        }

        public void DeactivateGroup(Group group)
        {
            group.IsActive = false;

            _context.Groups.Update(group);
            _context.SaveChanges();
        }

        public List<Player> AddPlayerToGroup(IdentityUser user, Group group, AddPlayerRequest requestInfo)
        {
            Player newPlayer = new Player
            {
                User = user,
                Name = requestInfo.PlayerName,
                Role = null
            };
            _context.Players.Add(newPlayer);

            if (group.Clients == null)
            {
                group.Clients = new List<Player>();
            }

            group.Clients.Add(newPlayer);
            _context.Groups.Update(group);
            _context.SaveChanges();

            return group.Clients;
        }

        public List<Player> RemovePlayerFromGroup(Group group, long playerId)
        {
            group.Clients.RemoveAll(p => p.Id == playerId);
            _context.Groups.Update(group);
            _context.SaveChanges();

            return group.Clients;
        }
    }
}
