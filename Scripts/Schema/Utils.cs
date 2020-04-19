using System;
using System.Collections.Generic;
using System.Linq;
using FlatBuffers;
using Godot;
using Steamworks;

namespace NetworkPacket
{
    public static class Utils
    {
        public static byte[] GetPropMovementAndRotationBytes(bool sentByHost, string objName, Vector3 pos,
            Vector3 rot)
        {
            FlatBufferBuilder builder = CreateMovementAndRotationBuilder(sentByHost, objName, pos, rot);
            MovementAndRotation.AddObjType(builder, ObjectType.Prop);
            var objPacketOffset = MovementAndRotation.EndMovementAndRotation(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.MovementAndRotation);
        }

        private static FlatBufferBuilder CreateMovementAndRotationBuilder(bool sentByHost, string objName, Vector3 pos,
            Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);
            var objNameOffset = builder.CreateString(objName);
            MovementAndRotation.StartMovementAndRotation(builder);
            MovementAndRotation.AddSentByHost(builder, sentByHost);
            MovementAndRotation.AddObj(builder, objNameOffset);
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            MovementAndRotation.AddPos(builder, objPosOffset);
            var objRotOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            MovementAndRotation.AddRot(builder, objRotOffset);
            return builder;
        }


        public static byte[] GetObjectMovementBytes(bool sentByHost, string objName, Vector3 pos)
        {
            FlatBufferBuilder builder = CreateMovementBuilder(sentByHost, objName, pos);
            Movement.AddObjType(builder, ObjectType.Prop);
            var objPacketOffset = Movement.EndMovement(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.Movement);
        }

        private static FlatBufferBuilder CreateMovementBuilder(bool sentByHost, string objName, Vector3 pos)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);
            var objNameOffset = builder.CreateString(objName);
            MovementAndRotation.StartMovementAndRotation(builder);
            MovementAndRotation.AddSentByHost(builder, sentByHost);
            MovementAndRotation.AddObj(builder, objNameOffset);
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            MovementAndRotation.AddPos(builder, objPosOffset);
            return builder;
        }

        public static byte[] GetObjectRotationBytes(bool sentByHost, string objName, Vector3 rot)
        {
            FlatBufferBuilder builder = CreateRotationBuilder(sentByHost, objName, rot);
            Rotation.AddObjType(builder, ObjectType.Prop);
            var objPacketOffset = Rotation.EndRotation(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.Rotation);
        }

        private static FlatBufferBuilder CreateRotationBuilder(bool sentByHost, string objName, Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);
            var objNameOffset = builder.CreateString(objName);
            MovementAndRotation.StartMovementAndRotation(builder);
            MovementAndRotation.AddSentByHost(builder, sentByHost);
            MovementAndRotation.AddObj(builder, objNameOffset);
            var objRotOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            MovementAndRotation.AddRot(builder, objRotOffset);
            return builder;
        }

        public static byte[] GetPlayerMovementAndRotationBytes(bool sentByHost, string playerName, Vector3 pos,
            Vector3 rot)
        {
            FlatBufferBuilder builder = CreateMovementAndRotationBuilder(sentByHost, playerName, pos, rot);
            MovementAndRotation.AddObjType(builder, ObjectType.Player);
            var playerPacketOffset = MovementAndRotation.EndMovementAndRotation(builder);

            return GetPacketHolderBytes(builder, playerPacketOffset.Value, Packets.MovementAndRotation);
        }


        public static byte[] GetPlayerMovementBytes(bool sentByHost, string playerName, Vector3 pos)
        {
            FlatBufferBuilder builder = CreateMovementBuilder(sentByHost, playerName, pos);
            Movement.AddObjType(builder, ObjectType.Player);
            var playerPacketOffset = Movement.EndMovement(builder);

            return GetPacketHolderBytes(builder, playerPacketOffset.Value, Packets.Movement);
        }

        public static byte[] GetPlayerRotationBytes(bool sentByHost, string playerName, Vector3 rot)
        {
            FlatBufferBuilder builder = CreateRotationBuilder(sentByHost, playerName, rot);
            Rotation.AddObjType(builder, ObjectType.Player);
            var playerPacketOffset = Rotation.EndRotation(builder);

            return GetPacketHolderBytes(builder, playerPacketOffset.Value, Packets.Rotation);
        }

        public static byte[] GetPlayerIdsBytes(bool sentByHost, IEnumerable<CSteamID> playerIds)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(16);
            var convertedIds = playerIds.ToList().ConvertAll(item => (ulong) item);
            var idsVector = PlayerIds.CreateIdsVector(builder, convertedIds.ToArray());
            PlayerIds.StartPlayerIds(builder);
            PlayerIds.AddSentByHost(builder, sentByHost);
            PlayerIds.AddIds(builder, idsVector);
            var idsOffset = PlayerIds.EndPlayerIds(builder);

            return GetPacketHolderBytes(builder, idsOffset.Value, Packets.PlayerIds);
        }

        public static byte[] GetSpawnBytes(bool sentByHost, string name, ActionType action, Vector3 pos)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);
            var nameOffset = builder.CreateString(name);

            Spawn.StartSpawn(builder);
            Spawn.AddSentByHost(builder, sentByHost);
            Spawn.AddName(builder, nameOffset);
            Spawn.AddAction(builder, action);
            Spawn.AddPos(builder, Vec3.CreateVec3(builder, pos.x, pos.y, pos.z));
            var spawnOffset = Spawn.EndSpawn(builder);

            return GetPacketHolderBytes(builder, spawnOffset.Value, Packets.Spawn);
        }

        public static byte[] GetUserActionBytes(ActionType action)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);
            UserAction.StartUserAction(builder);
            UserAction.AddAction(builder, action);
            var userActionOffset = UserAction.EndUserAction(builder);
            return GetPacketHolderBytes(builder, userActionOffset.Value, Packets.UserAction);
        }

        public static byte[] GetSceneChangeBytes(string sceneName)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);
            var sceneNameOffset = builder.CreateString(sceneName);
            SceneChange.StartSceneChange(builder);
            SceneChange.AddScene(builder, sceneNameOffset);
            var sceneOffset = SceneChange.EndSceneChange(builder);

            return GetPacketHolderBytes(builder, sceneOffset.Value, Packets.SceneChange);
        }


        private static byte[] GetPacketHolderBytes(FlatBufferBuilder builder, int packetOffset, Packets packetType)
        {
            PacketHolder.StartPacketHolder(builder);
            PacketHolder.AddPacketType(builder, packetType);
            PacketHolder.AddPacket(builder, packetOffset);
            var packetHolder = PacketHolder.EndPacketHolder(builder);
            PacketHolder.FinishPacketHolderBuffer(builder, packetHolder);

            return builder.SizedByteArray();
        }
    }
}