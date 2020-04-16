using System.Collections.Generic;
using Godot;
using Steamworks;

namespace Gareo.Scripts.Lobby
{
    public class Globals : Node
    {
        public int MaxPlayers = 4;
        public bool IsGameStarted { get; set; }
        public bool PlayingAsHost { get; set; }

        public CSteamID LobbyId { get; set; } = CSteamID.Nil;
        public CSteamID HostId { get; set; } = CSteamID.Nil;
        public CSteamID OwnId { get; set; } = CSteamID.Nil;
        public readonly HashSet<CSteamID> UserIds = new HashSet<CSteamID>();

        public void Cleanup()
        {
            IsGameStarted = false;
            PlayingAsHost = false;
            LobbyId = CSteamID.Nil;
            HostId = CSteamID.Nil;
            UserIds.Clear();
        }
    }
}