using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpaceGame.Data.Models
{
    public class Group
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsInGame { get; set; }

        [Required]
        public Player HostPlayer { get; set; }
        [JsonIgnore]
        public List<Player> Clients { get; set; }

        public override bool Equals(object obj)
        {
            Group group = obj as Group;
            if (group == null)
            {
                return false;
            }

            bool areClientsEqual = false;
            if (Clients.Count == group.Clients.Count)
            {
                areClientsEqual = true;
                for (int i = 0; i < Clients.Count; i++)
                {
                    Player client = Clients[i];
                    Player testClient = group.Clients[i];
                    if (client == null && testClient == null)
                    {
                        continue;
                    }

                    if (client == null || testClient == null || !client.Equals(testClient))
                    {
                        areClientsEqual = false;
                        break;
                    }
                }
            }

            return areClientsEqual
                && Id == group.Id
                && Name == group.Name
                && StartTime == group.StartTime
                && IsActive == group.IsActive
                && IsInGame == group.IsInGame
                && HostPlayer.Equals(group.HostPlayer);
        }
    }
}
