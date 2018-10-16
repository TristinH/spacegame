using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SpaceGame.Data.Models
{
    public enum GamePlayerRole
    {
        Traitor,
        Captain,
        Crewmember,
        Passenger
    }

    public class Player
    {
        public long Id { get; set; }
        [Required]
        public IdentityUser User { get; set; }
        public string Name { get; set; }
        public GamePlayerRole? Role { get; set; }

        public override bool Equals(object obj)
        {
            Player player = obj as Player;
            if (player == null)
            {
                return false;
            }

            return Id == player.Id
                && User.Id == player.User.Id
                && Role == player.Role
                && Name == player.Name;
        }
    }
}
