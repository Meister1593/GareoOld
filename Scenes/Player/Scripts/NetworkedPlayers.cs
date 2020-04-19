using FlatBuffers;
using Gareo.Scripts.Lobby;
using Godot;
using Godot.Collections;
using NetworkPacket;
using Steamworks;

namespace Gareo.Scenes.Player.Scripts
{
    public class NetworkedPlayers : Node
    {
        [Export(PropertyHint.File, "*.tscn")] private string _playerPath;
        [Export(PropertyHint.File, "*.tscn")] private string _networkedPlayerPath;

        [Export(PropertyHint.Range, "Vector3")]
        public Vector3 DefaultSpawnPos;

        private Globals _globals;
        private Lobby _lobby;

        public override void _Ready()
        {
            _globals = GetNode<Globals>("/root/Globals");
            _lobby = GetNode<Lobby>("/root/SteamLobby");

            _lobby.PacketReceived += ProcessIncomingPackets;
        }

        private void ProcessIncomingPackets(object source, PacketReceiveArgs args)
        {
            ByteBuffer buff = new ByteBuffer(args.Packet);
            PacketHolder packetHolder = PacketHolder.GetRootAsPacketHolder(buff);
            switch (packetHolder.PacketType)
            {
                case Packets.Spawn:
                    ProcessSpawn(packetHolder, args.RemoteId);
                    break;
                case Packets.MovementAndRotation:
                case Packets.Movement:
                case Packets.Rotation:
                    ProcessPlayerMovement(packetHolder, args.RemoteId);
                    break;
            }
        }

        private void ProcessPlayerMovement(PacketHolder packetHolder, CSteamID remoteId)
        {
            switch (packetHolder.PacketType)
            {
                case Packets.MovementAndRotation:
                    ProcessPlayerMovementAndRotationChange(packetHolder, remoteId);
                    break;
                case Packets.Movement:
                    ProcessPlayerMovementChange(packetHolder, remoteId);
                    break;
                case Packets.Rotation:
                    ProcessPlayerRotationChange(packetHolder, remoteId);
                    break;
            }
        }

        private void ProcessSpawn(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<Spawn>();
            if (packet == null) return;

            switch (packet.Value.Action)
            {
                case ActionType.PlayerSpawn:
                    PackedScene playerScene;
                    KinematicBody player;
                    if (packet.Value.Name.Equals(_globals.OwnId.ToString()))
                    {
                        playerScene = ResourceLoader.Load<PackedScene>(_playerPath);
                        player = playerScene?.Instance() as KinematicBody;
                        if (player == null) return;
                        GetParent().AddChild(player);
                        player.Name = packet.Value.Name;
                        _globals.OwnPlayerBody = player;
                    }
                    else
                    {
                        playerScene = ResourceLoader.Load<PackedScene>(_networkedPlayerPath);
                        player = playerScene?.Instance() as KinematicBody;
                        if (player == null) return;
                        AddChild(player);
                        player.Name = packet.Value.Name;
                        _globals.PlayerBodies.Add(player.Name, player);
                    }

                    Transform propTransform;
                    if (packet.Value.Pos != null)
                    {
                        propTransform = player.Transform;
                        propTransform.origin.x = packet.Value.Pos.Value.X;
                        propTransform.origin.y = packet.Value.Pos.Value.Y;
                        propTransform.origin.z = packet.Value.Pos.Value.Z;
                        player.Transform = propTransform;
                    }
                    else
                    {
                        propTransform = player.Transform;
                        propTransform.origin.x = DefaultSpawnPos.x;
                        propTransform.origin.y = DefaultSpawnPos.y;
                        propTransform.origin.z = DefaultSpawnPos.z;
                        player.Transform = propTransform;
                    }

                    if (_globals.PlayingAsHost)
                    {
                        SpawnPlayerCallback(packet.Value.Name, propTransform.origin, remoteId);
                        SendAllExitingPlayersCallback(remoteId);
                    }

                    break;
            }
        }

        private void SpawnPlayerCallback(string name, Vector3 pos, CSteamID remoteId)
        {
            var packet = Utils.GetSpawnBytes(true, name, ActionType.PlayerSpawn, pos);
            foreach (CSteamID id in _globals.UserIds)
            {
                SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
            }
        }

        private void SendAllExitingPlayersCallback(CSteamID remoteId)
        {
            var packet = Utils.GetSpawnBytes(true, _globals.OwnPlayerBody.Name, ActionType.PlayerSpawn,
                _globals.OwnPlayerBody.Transform.origin);
            SteamNetworking.SendP2PPacket(remoteId, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
            foreach (var playerBody in _globals.PlayerBodies)
            {
                if (playerBody.Value.Name.Equals(remoteId.ToString())) continue;
                packet = Utils.GetSpawnBytes(true, playerBody.Value.Name, ActionType.PlayerSpawn,
                    playerBody.Value.Transform.origin);
                SteamNetworking.SendP2PPacket(remoteId, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
            }
        }

        private void ProcessPlayerMovementAndRotationChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<MovementAndRotation>();
            if (packet == null) return;
            if (packet.Value.ObjType != ObjectType.Player) return;
                
            if (!_globals.PlayerBodies.TryGetValue(packet.Value.Obj, out KinematicBody updatingPlayer))
            {
                if (_globals.OwnPlayerBody != null && _globals.OwnPlayerBody.Name.Equals(packet.Value.Obj))
                    updatingPlayer = _globals.OwnPlayerBody;
                else return;
            }

            Transform newPlayerTransform = updatingPlayer.Transform;
            if (packet.Value.Pos != null)
            {
                newPlayerTransform.origin.x = packet.Value.Pos.Value.X;
                newPlayerTransform.origin.y = packet.Value.Pos.Value.Y;
                newPlayerTransform.origin.z = packet.Value.Pos.Value.Z;
            }

            updatingPlayer.Transform = newPlayerTransform;

            Vector3 newPlayerRotation = updatingPlayer.Rotation;
            if (packet.Value.Rot != null)
            {
                newPlayerRotation.x = packet.Value.Rot.Value.X;
                newPlayerRotation.y = packet.Value.Rot.Value.Y;
                newPlayerRotation.z = packet.Value.Rot.Value.Z;
            }

            updatingPlayer.Rotation = newPlayerRotation;
            if (_globals.PlayingAsHost)
                PlayerMovementAndRotationChangeCallback(packet.Value.Obj, newPlayerTransform.origin, newPlayerRotation,
                    remoteId);
        }

        private void PlayerMovementAndRotationChangeCallback(string name, Vector3 pos, Vector3 rot, CSteamID remoteId)
        {
            var packet = Utils.GetPlayerMovementAndRotationBytes(true, name, pos, rot);
            foreach (CSteamID id in _globals.UserIds)
            {
                SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendUnreliable);
            }
        }

        private void ProcessPlayerMovementChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<Movement>();
            if (packet == null) return;
            if (packet.Value.ObjType != ObjectType.Player) return;

            if (!_globals.PlayerBodies.TryGetValue(packet.Value.Obj, out KinematicBody updatingPlayer))
            {
                if (_globals.OwnPlayerBody.Name.Equals(packet.Value.Obj))
                    updatingPlayer = _globals.OwnPlayerBody;
                else return;
            }

            Transform newPlayerTransform = updatingPlayer.Transform;
            if (packet.Value.Pos != null)
            {
                newPlayerTransform.origin.x = packet.Value.Pos.Value.X;
                newPlayerTransform.origin.y = packet.Value.Pos.Value.Y;
                newPlayerTransform.origin.z = packet.Value.Pos.Value.Z;
            }

            updatingPlayer.Transform = newPlayerTransform;

            if (_globals.PlayingAsHost)
                PlayerMovementChangeCallback(packet.Value.Obj, newPlayerTransform.origin, remoteId);
        }

        private void PlayerMovementChangeCallback(string name, Vector3 pos, CSteamID remoteId)
        {
            var packet = Utils.GetPlayerMovementBytes(true, name, pos);
            foreach (CSteamID id in _globals.UserIds)
            {
                SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendUnreliable);
            }
        }

        private void ProcessPlayerRotationChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<Rotation>();
            if (packet == null) return;
            if (packet.Value.ObjType != ObjectType.Player) return;

            if (!_globals.PlayerBodies.TryGetValue(packet.Value.Obj, out KinematicBody updatingPlayer))
            {
                if (_globals.OwnPlayerBody.Name.Equals(packet.Value.Obj))
                    updatingPlayer = _globals.OwnPlayerBody;
                else return;
            }

            Vector3 newPlayerRotation = updatingPlayer.Rotation;
            if (packet.Value.Rot != null)
            {
                newPlayerRotation.x = packet.Value.Rot.Value.X;
                newPlayerRotation.y = packet.Value.Rot.Value.Y;
                newPlayerRotation.z = packet.Value.Rot.Value.Z;
            }

            updatingPlayer.Rotation = newPlayerRotation;

            if (_globals.PlayingAsHost)
                PlayerRotationChangeCallback(packet.Value.Obj, newPlayerRotation, remoteId);
        }

        private void PlayerRotationChangeCallback(string name, Vector3 rot, CSteamID remoteId)
        {
            var packet = Utils.GetPlayerRotationBytes(true, name, rot);
            foreach (CSteamID id in _globals.UserIds)
            {
                SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
            }
        }
    }
}