using System;
using System.Collections.Generic;
using Godot;
using Lidgren.Network;
using Steamworks;

namespace Gareo.Scripts.Lobby
{
    public class Globals : Node
    {
        public int MaxPlayers { get; set; } = 8;
        public string HostIpAddress { get; set; }
        public ushort Port { get; set; } = 40050;
        public bool IsGameStarted { get; set; }
        public bool PlayingAsHost { get; set; }

        public NetServer Server { get; set; }

        public NetPeer Client { get; set; }
        public KinematicBody OwnPlayerBody { get; set; }
        public readonly Dictionary<string, KinematicBody> PlayerBodies = new Dictionary<string, KinematicBody>();
        public CSteamID LobbyId { get; set; } = CSteamID.Nil;
        public CSteamID HostId { get; set; } = CSteamID.Nil;
        public CSteamID OwnId { get; set; } = CSteamID.Nil;
        public readonly HashSet<CSteamID> UserIds = new HashSet<CSteamID>();

        public void Cleanup()
        {
            HostIpAddress = string.Empty;
            IsGameStarted = false;
            PlayingAsHost = false;
            OwnPlayerBody = null;
            LobbyId = CSteamID.Nil;
            HostId = CSteamID.Nil;
            PlayerBodies.Clear();
            UserIds.Clear();
        }
    }
}