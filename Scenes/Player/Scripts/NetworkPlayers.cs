using FlatBuffers;
using Gareo.Scripts.Lobby;
using Godot;
using Godot.Collections;
using NetworkPacket;
using Steamworks;

namespace Gareo.Scenes.Player.Scripts
{
    public class NetworkPlayers : Node
    {
        private Globals _globals;
        public Dictionary<string, KinematicBody> Players;

        public override void _Ready()
        {
            _globals = GetNode<Globals>("/root/Globals");
            Players = new Dictionary<string, KinematicBody>();
            
        }

        //------------    Receive and process P2P packets    ------------
        public override void _Process(float delta)
        {
            while (SteamNetworking.IsP2PPacketAvailable(out uint packetSize))
            {
                byte[] incomingPacket = new byte[packetSize];

                // Receive data of packet
                if (SteamNetworking.ReadP2PPacket(incomingPacket, packetSize,
                    out uint bytesRead, out CSteamID remoteId))
                {
                    ProcessIncomingPackets(incomingPacket, remoteId);
                }
            }
        }

        private void ProcessIncomingPackets(byte[] incomingPacket, CSteamID remoteId)
        {
            ByteBuffer buff = new ByteBuffer(incomingPacket);
            PacketHolder packetHolder = PacketHolder.GetRootAsPacketHolder(buff);
            switch (packetHolder.PacketType)
            {
                case Packets.PlayerMovementAndRotation:
                case Packets.PlayerMovement:
                case Packets.PlayerRotation:
                    ProcessPlayerMovement(packetHolder, remoteId);
                    break;
            }
        }

        private void ProcessPlayerMovement(PacketHolder packetHolder, CSteamID remoteId)
        {
            switch (packetHolder.PacketType)
            {
                case Packets.PlayerMovementAndRotation:
                    ProcessPlayerMovementAndRotationChange(packetHolder, remoteId);
                    break;
                case Packets.PlayerMovement:
                    ProcessPlayerMovementChange(packetHolder, remoteId);
                    break;
                case Packets.PlayerRotation:
                    ProcessPlayerRotationChange(packetHolder, remoteId);
                    break;
            }
        }

        private void ProcessPlayerMovementAndRotationChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<PlayerMovementAndRotation>();
            if (packet == null) return;

            KinematicBody updatingPlayer = Players[packet.Value.Obj];
            if (updatingPlayer == null) return;

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
        }

        private void ProcessPlayerMovementChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<PlayerMovementAndRotation>();
            if (packet == null) return;

            KinematicBody updatingPlayer = Players[packet.Value.Obj];
            if (updatingPlayer == null) return;

            Transform newPlayerTransform = updatingPlayer.Transform;
            if (packet.Value.Pos != null)
            {
                newPlayerTransform.origin.x = packet.Value.Pos.Value.X;
                newPlayerTransform.origin.y = packet.Value.Pos.Value.Y;
                newPlayerTransform.origin.z = packet.Value.Pos.Value.Z;
            }

            updatingPlayer.Transform = newPlayerTransform;
        }

        private void ProcessPlayerRotationChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<PlayerMovementAndRotation>();
            if (packet == null) return;

            KinematicBody updatingPlayer = Players[packet.Value.Obj];
            if (updatingPlayer == null) return;

            Vector3 newPlayerRotation = updatingPlayer.Rotation;
            if (packet.Value.Rot != null)
            {
                newPlayerRotation.x = packet.Value.Rot.Value.X;
                newPlayerRotation.y = packet.Value.Rot.Value.Y;
                newPlayerRotation.z = packet.Value.Rot.Value.Z;
            }

            updatingPlayer.Rotation = newPlayerRotation;
        }
    }
}