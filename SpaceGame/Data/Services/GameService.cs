using SpaceGame.Data.Models;
using System;
using System.Collections.Generic;

namespace SpaceGame.Data.Services
{
    public class GameService
    {
        private readonly AppDbContext _context;

        public GameService(AppDbContext context)
        {
            _context = context;
        }

        public void StartGame(Group group)
        {
            group.IsInGame = true;
            _context.Groups.Update(group);

            List<Player> players = new List<Player>(group.Clients)
            {
                group.HostPlayer
            };

            Random random = new Random(DateTime.Now.Millisecond);
            int traitorIndex = random.Next(players.Count);
            int captainIndex;

            do
            {
                captainIndex = random.Next(players.Count);
            } while (traitorIndex == captainIndex);

            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];

                if (i == traitorIndex)
                {
                    player.Role = GamePlayerRole.Traitor;
                }
                else if (i == captainIndex)
                {
                    player.Role = GamePlayerRole.Captain;
                }
                else
                {
                    player.Role = GamePlayerRole.Passenger;
                }
            }

            _context.Players.UpdateRange(players);
            _context.SaveChanges();
        }
    }
}
