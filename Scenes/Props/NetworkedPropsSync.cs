using System.Collections.Generic;
using FlatBuffers;
using Gareo.Scripts.Lobby;
using Godot;
using NetworkPacket;
using Steamworks;

namespace Gareo.Scenes.Props
{
    public class NetworkedPropsSync : Node
    {
        [Export(PropertyHint.File, "*.tscn")] public string NetworkBoxPath;

        [Export(PropertyHint.Range, "Vector3")]
        public Vector3 DefaultSpawnPos;

        private Globals _globals;
        public Dictionary<string, KinematicBody> Props;

        public override void _Ready()
        {
            DefaultSpawnPos = Vector3.Zero;
            Props = new Dictionary<string, KinematicBody>();
            _globals = GetNode<Globals>("/root/Globals");
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
                case Packets.Spawn:
                {
                    ProcessIncomingSpawn(remoteId, packetHolder);
                    break;
                }
                case Packets.ObjectMovementAndRotation:
                case Packets.ObjectMovement:
                case Packets.ObjectRotation:
                    ProcessIncomingObject(packetHolder);
                    break;
            }
        }

        //------------    Process packet related to spawning prop    ------------
        private void ProcessIncomingSpawn(CSteamID remoteId, PacketHolder packetHolder)
        {
            var packet = packetHolder.Packet<Spawn>();
            if (packet == null) return;

            switch (packet.Value.Action)
            {
                case ActionType.PropSpawn:
                {
                    PackedScene propScene = ResourceLoader.Load<PackedScene>(NetworkBoxPath);
                    KinematicBody prop = propScene?.Instance() as KinematicBody;
                    if (prop == null) return;

                    AddChild(prop);
                    Props.Add(prop.Name, prop);
                    prop.Name = packet.Value.Name;
                    if (packet.Value.Pos != null)
                    {
                        Transform propTransform = prop.Transform;
                        propTransform.origin.x = packet.Value.Pos.Value.X;
                        propTransform.origin.y = packet.Value.Pos.Value.Y;
                        propTransform.origin.z = packet.Value.Pos.Value.Z;
                        prop.Transform = propTransform;
                    }
                    else
                    {
                        Transform propTransform = prop.Transform;
                        propTransform.origin.x = DefaultSpawnPos.x;
                        propTransform.origin.y = DefaultSpawnPos.y;
                        propTransform.origin.z = DefaultSpawnPos.z;
                        prop.Transform = propTransform;
                    }

                    break;
                }
            }
        }

        //------------    Process packet related to any object movement    ------------
        private void ProcessIncomingObject(PacketHolder packetHolder)
        {
            switch (packetHolder.PacketType)
            {
                case Packets.ObjectMovementAndRotation:
                {
                    ProcessIncomingObjectPositionAndRotationChange(packetHolder);
                    break;
                }
                case Packets.ObjectMovement:
                {
                    ProcessIncomingObjectMovementChange(packetHolder);
                    break;
                }
                case Packets.ObjectRotation:
                {
                    ProcessIncomingObjectRotationChange(packetHolder);
                    break;
                }
            }
        }

        //------------    Process packet related to both changing position and rotation at once    ------------

        private void ProcessIncomingObjectPositionAndRotationChange(PacketHolder packetHolder)
        {
            var packet = packetHolder.Packet<ObjectMovementAndRotation>();
            if (packet == null) return;

            KinematicBody updatingObj = Props[packet.Value.Obj];
            if (updatingObj == null) return;

            Transform newObjTransform = updatingObj.Transform;
            if (packet.Value.Pos != null)
            {
                newObjTransform.origin.x = packet.Value.Pos.Value.X;
                newObjTransform.origin.y = packet.Value.Pos.Value.Y;
                newObjTransform.origin.z = packet.Value.Pos.Value.Z;
            }

            updatingObj.Transform = newObjTransform;

            Vector3 newObjRotation = updatingObj.Rotation;
            if (packet.Value.Rot != null)
            {
                newObjRotation.x = packet.Value.Rot.Value.X;
                newObjRotation.y = packet.Value.Rot.Value.Y;
                newObjRotation.z = packet.Value.Rot.Value.Z;
            }

            updatingObj.Rotation = newObjRotation;
        }

        //------------    Process packet related to only changing object position    ------------
        private void ProcessIncomingObjectMovementChange(PacketHolder packetHolder)
        {
            var packet = packetHolder.Packet<ObjectMovement>();
            if (packet == null) return;

            KinematicBody updatingObj = Props[packet.Value.Obj];
            if (updatingObj == null) return;

            Transform newObjTransform = updatingObj.Transform;
            if (packet.Value.Pos != null)
            {
                newObjTransform.origin.x = packet.Value.Pos.Value.X;
                newObjTransform.origin.y = packet.Value.Pos.Value.Y;
                newObjTransform.origin.z = packet.Value.Pos.Value.Z;
            }

            updatingObj.Transform = newObjTransform;
        }

        //------------    Process packet related to only changing object rotation    ------------
        private void ProcessIncomingObjectRotationChange(PacketHolder packetHolder)
        {
            var packet = packetHolder.Packet<ObjectRotation>();
            if (packet == null) return;

            KinematicBody updatingObj = Props[packet.Value.Obj];
            if (updatingObj == null) return;

            Vector3 newObjRotation = updatingObj.Rotation;
            if (packet.Value.Rot != null)
            {
                newObjRotation.x = packet.Value.Rot.Value.X;
                newObjRotation.y = packet.Value.Rot.Value.Y;
                newObjRotation.z = packet.Value.Rot.Value.Z;
            }

            updatingObj.Rotation = newObjRotation;
        }
    }
}