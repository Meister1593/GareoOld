using System;
using System.Collections.Generic;
using FlatBuffers;
using Godot;
using Lidgren.Network;
using NetworkPacket;
using Steamworks;

namespace Gareo.Scripts.Lobby
{
    public class PacketReceiveArgs : EventArgs
    {
        public byte[] Packet;
        public CSteamID RemoteId;
    }

    public class Lobby : Node
    {
        private Globals _globals;
        public event EventHandler<PacketReceiveArgs> PacketReceived;
        public event EventHandler LobbyEnteredFinished;

        public override void _Ready()
        {
            _globals = GetNode<Globals>("/root/Globals");

            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
            Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFailed);
        }

        //------------    Whenever user joins, this triggers, add PlayerID (or remove)    ------------
        private void OnLobbyChatUpdate(LobbyChatUpdate_t update)
        {
            switch (update.m_rgfChatMemberStateChange)
            {
                case 1: // User has been joined
                    // Add connected player id to globals var
                    _globals.UserIds.Add((CSteamID) update.m_ulSteamIDMakingChange);
                    GD.Print(
                        "NOTIFY: User " +
                        SteamFriends.GetFriendPersonaName((CSteamID) update.m_ulSteamIDMakingChange) +
                        "has been added to Globals.");
                    break;
                case 2: // User left the lobby
                case 4: // User disconnected without leaving lobby
                case 8: // User has been kicked
                case 10: // User has been kicked and banned
                    // Remove disconnected player id in globals var
                    if (_globals.UserIds.Remove((CSteamID) update.m_ulSteamIDMakingChange))
                    {
                        _globals.PlayerBodies.Remove(update.m_ulSteamIDMakingChange.ToString());
                        // @TODO Remove hard-coding for world name
                        if (GetTree().CurrentScene.Name.Equals("World1"))
                        {
                            GetNode($"/root/World1/Players/NetworkPlayers/{update.m_ulSteamIDMakingChange.ToString()}")
                                .QueueFree();
                        }

                        GD.Print(
                            "NOTIFY: User " +
                            SteamFriends.GetFriendPersonaName((CSteamID) update.m_ulSteamIDMakingChange) +
                            " has been removed from Globals.");
                    }

                    break;
            }
        }

        //------------    ACCEPT OR REJECT INCOMING CONNECTION    ------------
        private void OnP2PSessionRequest(P2PSessionRequest_t request)
        {
            if (request.m_steamIDRemote.Equals(_globals.HostId) || _globals.UserIds.Contains(request.m_steamIDRemote))
                SteamNetworking.AcceptP2PSessionWithUser(request.m_steamIDRemote);
            else
                GD.Print($"A connection was just rejected from {request.m_steamIDRemote}.");
        }

        private void OnP2PSessionConnectFailed(P2PSessionConnectFail_t failure)
        {
            GD.Print("P2P session failed. Error code: " + failure.m_eP2PSessionError);
        }

        //------------    When lobby was created (Callback to Host)    ------------
        private void OnLobbyCreated(LobbyCreated_t lobby)
        {
            if (lobby.m_eResult.Equals(EResult.k_EResultOK))
            {
                GD.Print($"OK: Created lobby with id: {lobby.m_ulSteamIDLobby}");

                // Assign global vars
                _globals.PlayingAsHost = true;
                _globals.HostId = SteamUser.GetSteamID();
                _globals.OwnId = _globals.HostId;
                _globals.LobbyId = (CSteamID) lobby.m_ulSteamIDLobby;
            }
            else
            {
                GD.Print("ERR: Could not create lobby.");
            }
        }

        //------------    When joining through invite or just steam   ------------ @TODO needs to be only invite
        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t joinRequested)
        {
            SteamMatchmaking.JoinLobby(joinRequested.m_steamIDLobby);
            GD.Print($"OK: Joined lobby: {joinRequested.m_steamIDLobby}");
        }

        //------------    When entering lobby (as host or user)    ------------
        private void OnLobbyEntered(LobbyEnter_t entrance)
        {
            _globals.LobbyId = (CSteamID) entrance.m_ulSteamIDLobby;
            if (_globals.PlayingAsHost)
            {
                GD.Print($"OK: Entered into a lobby with ID: {_globals.LobbyId} as host, changing scene.");
                OnLobbyEnteredFinished();
            }
            else
            {
                _globals.HostId = SteamMatchmaking.GetLobbyOwner((CSteamID) entrance.m_ulSteamIDLobby);
                _globals.OwnId = SteamUser.GetSteamID();
                GD.Print($"OK: Entered into a lobby with ID: {_globals.LobbyId} as user.");
                SendRequestToFillPlayerIds();
            }
        }

        protected virtual void OnLobbyEnteredFinished()
        {
            LobbyEnteredFinished?.Invoke(this, EventArgs.Empty);
        }

        // Send request to get all lobby user ids from host
        private void SendRequestToFillPlayerIds()
        {
            var packet = Utils.GetUserActionBytes(ActionType.PlayerIds);
            SteamNetworking.SendP2PPacket(_globals.HostId, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
            GD.Print("NOTIFY: Sent request to host to get player ids");
        }

        //------------    Receive and process P2P packets    ------------
        public override void _Process(float delta)
        {
            NetIncomingMessage msg;
            while ((msg = _globals.Server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        GD.Print($"ErrorMessage: {msg.ReadString()}");
                        break;
                    case NetIncomingMessageType.Data:
                        GD.Print();
                        msg.SenderEndPoint
                        break;
                    default:
                        GD.Print("Unhandled type: " + msg.MessageType);
                        break;
                }
                _globals.Server.Recycle(msg);
            }

            while (SteamNetworking.IsP2PPacketAvailable(out uint packetSize))
            {
                byte[] incomingPacket = new byte[packetSize];

                // Receive data of packet
                if (SteamNetworking.ReadP2PPacket(incomingPacket, packetSize,
                    out uint bytesRead, out CSteamID remoteId))
                {
                    ProcessIncomingPackets(incomingPacket, remoteId);
                    OnPacketReceived(incomingPacket, remoteId);
                }
            }
        }

        protected virtual void OnPacketReceived(byte[] incomingPacket, CSteamID remoteId)
        {
            PacketReceived?.Invoke(this, new PacketReceiveArgs()
            {
                Packet = incomingPacket,
                RemoteId = remoteId
            });
        }

        private void ProcessIncomingPackets(byte[] packet, CSteamID remoteId)
        {
            ByteBuffer buff = new ByteBuffer(packet);
            PacketHolder packetHolder = PacketHolder.GetRootAsPacketHolder(buff);
            switch (packetHolder.PacketType)
            {
                case Packets.UserAction:
                {
                    ProcessIncomingUserAction(remoteId, packetHolder);
                    break;
                }
                case Packets.PlayerIds:
                {
                    ProcessIncomingPlayerIds(packetHolder);
                    break;
                }
            }
        }

        private void ProcessIncomingUserAction(CSteamID remoteId, PacketHolder packetHolder)
        {
            var userAction = packetHolder.Packet<UserAction>();
            if (userAction == null) return;
            GD.Print($"NOTIFY: Receiving UserAction: {userAction.Value.Action}.");
            switch (userAction.Value.Action)
            {
                case ActionType.PlayerIds:
                    var sendingUsers = new HashSet<CSteamID>(_globals.UserIds);
                    sendingUsers.RemoveWhere(id => id.Equals(remoteId));

                    var packet = Utils.GetPlayerIdsBytes(true, sendingUsers);
                    SteamNetworking.SendP2PPacket(remoteId, packet, (uint) packet.Length,
                        EP2PSend.k_EP2PSendReliable);
                    GD.Print($"OK: Sent user list from host to: {SteamFriends.GetFriendPersonaName(remoteId)}");

                    break;
            }
        }

        private void ProcessIncomingPlayerIds(PacketHolder packetHolder)
        {
            var incomingPlayerIds = packetHolder.Packet<PlayerIds>();
            if (incomingPlayerIds != null)
            {
                if (!incomingPlayerIds.Value.SentByHost) return;
                var newIncomingPlayerIds =
                    Array.ConvertAll(incomingPlayerIds.Value.GetIdsArray(), item => (CSteamID) item);
                _globals.UserIds.UnionWith(newIncomingPlayerIds);
                GD.Print($"OK: Added lobby player id's from host, new count: {_globals.UserIds.Count}");
            }

            GD.Print("NOTIFY: Changing scene after getting PlayerIds packet.");
            OnLobbyEnteredFinished();
        }
    }
}