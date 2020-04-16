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
        public static byte[] GetObjectMovementAndRotationBytes(string objName, Vector3 pos, Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            ObjectMovementAndRotation.StartObjectMovementAndRotation(builder);
            ObjectMovementAndRotation.AddObj(builder, builder.CreateString(objName));
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            ObjectMovementAndRotation.AddPos(builder, objPosOffset);
            var objRotOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            ObjectMovementAndRotation.AddRot(builder, objRotOffset);
            var objPacketOffset = ObjectMovementAndRotation.EndObjectMovementAndRotation(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.ObjectMovementAndRotation);
        }


        public static byte[] GetObjectMovementBytes(string objName, Vector3 pos)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            ObjectMovement.StartObjectMovement(builder);
            ObjectMovement.AddObj(builder, builder.CreateString(objName));
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            ObjectMovement.AddPos(builder, objPosOffset);
            var objPacketOffset = ObjectMovement.EndObjectMovement(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.ObjectMovement);
        }

        public static byte[] GetObjectRotationBytes(string objName, Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            ObjectRotation.StartObjectRotation(builder);
            ObjectRotation.AddObj(builder, builder.CreateString(objName));
            var objPosOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            ObjectRotation.AddRot(builder, objPosOffset);
            var objPacketOffset = ObjectRotation.EndObjectRotation(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.ObjectRotation);
        }
        public static byte[] GetPlayerMovementAndRotationBytes(string playerName, Vector3 pos, Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            PlayerMovementAndRotation.StartPlayerMovementAndRotation(builder);
            PlayerMovementAndRotation.AddObj(builder, builder.CreateString(playerName));
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            PlayerMovementAndRotation.AddPos(builder, objPosOffset);
            var objRotOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            PlayerMovementAndRotation.AddRot(builder, objRotOffset);
            var objPacketOffset = PlayerMovementAndRotation.EndPlayerMovementAndRotation(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.ObjectMovementAndRotation);
        }


        public static byte[] GetPlayerMovementBytes(string playerName, Vector3 pos)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            PlayerMovement.StartPlayerMovement(builder);
            PlayerMovement.AddObj(builder, builder.CreateString(playerName));
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            PlayerMovement.AddPos(builder, objPosOffset);
            var objPacketOffset = PlayerMovement.EndPlayerMovement(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.ObjectMovement);
        }

        public static byte[] GetPlayerRotationBytes(string playerName, Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            PlayerRotation.StartPlayerRotation(builder);
            PlayerRotation.AddObj(builder, builder.CreateString(playerName));
            var objPosOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            PlayerRotation.AddRot(builder, objPosOffset);
            var objPacketOffset = ObjectRotation.EndObjectRotation(builder);

            return GetPacketHolderBytes(builder, objPacketOffset.Value, Packets.ObjectRotation);
        }

        public static byte[] GetPlayerIdsBytes(IEnumerable<CSteamID> playerIds)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(16);

            PlayerIds.StartPlayerIds(builder);
            var convertedIds = playerIds.ToList().ConvertAll(item => (ulong) item);
            var idsVector = PlayerIds.CreateIdsVector(builder, convertedIds.ToArray());
            PlayerIds.AddIds(builder, idsVector);
            var idsOffset = PlayerIds.EndPlayerIds(builder);

            return GetPacketHolderBytes(builder, idsOffset.Value, Packets.PlayerIds);
        }

        public static byte[] GetSpawnBytes(string name, ActionType action, Vector3 pos)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            Spawn.StartSpawn(builder);
            Spawn.AddName(builder, builder.CreateString(name));
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

            SceneChange.StartSceneChange(builder);
            SceneChange.AddScene(builder, builder.CreateString(sceneName));
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