using System.Collections.Generic;
using FlatBuffers;
using Gareo.Scripts.Lobby;
using Godot;
using NetworkPacket;
using Steamworks;

namespace Gareo.Scenes.Props
{
    public class NetworkedProps : Node
    {
        [Export(PropertyHint.File, "*.tscn")] public string NetworkBoxPath;

        [Export(PropertyHint.Range, "Vector3")]
        public Vector3 DefaultSpawnPos;

        private Globals _globals;
        private Dictionary<string, KinematicBody> _props;
        private Lobby _lobby;

        public override void _Ready()
        {
            DefaultSpawnPos = Vector3.Zero;
            _props = new Dictionary<string, KinematicBody>();
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
                {
                    ProcessIncomingSpawn(args.RemoteId, packetHolder);
                    break;
                }
                case Packets.MovementAndRotation:
                case Packets.Movement:
                case Packets.Rotation:
                    ProcessIncomingObjectAction(packetHolder, args.RemoteId);
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
                // When player accepted new spawn and sent back this callback
                case ActionType.PropSpawn:
                {
                    Vector3? pos = null;
                    if (packet.Value.Pos != null)
                    {
                        pos = new Vector3(packet.Value.Pos.Value.X, packet.Value.Pos.Value.Y, packet.Value.Pos.Value.Z);
                    }

                    SpawnProp(packet.Value.Name, pos);
                    // When host receives prop spawn from player
                    if (_globals.PlayingAsHost)
                        SpawnPropCallback(packet.Value.Name, pos, remoteId);

                    break;
                }
            }
        }

        private void SpawnProp(string name, Vector3? pos)
        {
            PackedScene propScene = ResourceLoader.Load<PackedScene>(NetworkBoxPath);
            KinematicBody prop = propScene?.Instance() as KinematicBody;
            if (prop == null) return;

            AddChild(prop);
            _props.Add(prop.Name, prop);
            prop.Name = name;
            if (pos != null)
            {
                Transform propTransform = prop.Transform;
                propTransform.origin.x = pos.Value.x;
                propTransform.origin.y = pos.Value.y;
                propTransform.origin.z = pos.Value.z;
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
        }

        private void SpawnPropCallback(string name, Vector3? pos, CSteamID remoteId)
        {
            if (pos == null) pos = Vector3.Zero;
            var packet = Utils.GetSpawnBytes(true, name, ActionType.PropSpawn, pos.Value);
            SteamNetworking.SendP2PPacket(remoteId, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
        }

        //------------    Process packet related to any object movement    ------------
        private void ProcessIncomingObjectAction(PacketHolder packetHolder, CSteamID remoteId)
        {
            switch (packetHolder.PacketType)
            {
                case Packets.MovementAndRotation:
                {
                    ProcessIncomingObjectPositionAndRotationChange(packetHolder, remoteId);
                    break;
                }
                case Packets.Movement:
                {
                    ProcessIncomingObjectMovementChange(packetHolder, remoteId);
                    break;
                }
                case Packets.Rotation:
                {
                    ProcessIncomingObjectRotationChange(packetHolder, remoteId);
                    break;
                }
            }
        }

        //------------    Process packet related to both changing position and rotation at once    ------------

        private void ProcessIncomingObjectPositionAndRotationChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<MovementAndRotation>();
            if (packet == null) return;
            if (packet.Value.ObjType != ObjectType.Prop) return;

            if (!_props.TryGetValue(packet.Value.Obj, out KinematicBody updatingObj)) return;

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

            if (packet.Value.SentByHost) return;
            ObjectPositionAndRotationChangeCallback(packet.Value.Obj, newObjTransform.origin, newObjRotation,
                remoteId);
        }

        private void ObjectPositionAndRotationChangeCallback(string name, Vector3 pos, Vector3 rot, CSteamID remoteId)
        {
            var packet = Utils.GetPropMovementAndRotationBytes(true, name, pos, rot);
            foreach (CSteamID id in _globals.UserIds)
            {
                SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendUnreliable);
            }
        }

        //------------    Process packet related to only changing object position    ------------
        private void ProcessIncomingObjectMovementChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<Movement>();
            if (packet == null) return;
            if (packet.Value.ObjType != ObjectType.Prop) return;

            if (!_props.TryGetValue(packet.Value.Obj, out KinematicBody updatingObj)) return;

            Transform newObjTransform = updatingObj.Transform;
            if (packet.Value.Pos != null)
            {
                newObjTransform.origin.x = packet.Value.Pos.Value.X;
                newObjTransform.origin.y = packet.Value.Pos.Value.Y;
                newObjTransform.origin.z = packet.Value.Pos.Value.Z;
            }

            updatingObj.Transform = newObjTransform;

            if (packet.Value.SentByHost) return;
            ObjectPositionChangeCallback(packet.Value.Obj, newObjTransform.origin, remoteId);
        }

        private void ObjectPositionChangeCallback(string name, Vector3 pos, CSteamID remoteId)
        {
            var packet = Utils.GetObjectMovementBytes(true, name, pos);
            foreach (CSteamID id in _globals.UserIds)
            {
                SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
            }
        }

        //------------    Process packet related to only changing object rotation    ------------
        private void ProcessIncomingObjectRotationChange(PacketHolder packetHolder, CSteamID remoteId)
        {
            var packet = packetHolder.Packet<Rotation>();
            if (packet == null) return;
            if (packet.Value.ObjType != ObjectType.Prop) return;

            if (!_props.TryGetValue(packet.Value.Obj, out KinematicBody updatingObj)) return;

            Vector3 newObjRotation = updatingObj.Rotation;
            if (packet.Value.Rot != null)
            {
                newObjRotation.x = packet.Value.Rot.Value.X;
                newObjRotation.y = packet.Value.Rot.Value.Y;
                newObjRotation.z = packet.Value.Rot.Value.Z;
            }

            updatingObj.Rotation = newObjRotation;

            if (packet.Value.SentByHost) return;
            ObjectRotationChangeCallback(packet.Value.Obj, newObjRotation, remoteId);
        }

        private void ObjectRotationChangeCallback(string name, Vector3 rot, CSteamID remoteId)
        {
            var packet = Utils.GetObjectRotationBytes(true, name, rot);
            foreach (CSteamID id in _globals.UserIds)
            {
                SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendReliable);
            }
        }
    }
}