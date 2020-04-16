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
        public static byte[] GetObjectMovementAndRotationBytes(string objName, Vector3 pos,
            Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            ObjectMovementAndRotation.StartObjectMovementAndRotation(builder);
            ObjectMovementAndRotation.AddObj(builder, uint.Parse(objName));
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            ObjectMovementAndRotation.AddPos(builder, objPosOffset);
            var objRotOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            ObjectMovementAndRotation.AddRot(builder, objRotOffset);
            var objPacketOffset = ObjectMovementAndRotation.EndObjectMovementAndRotation(builder);

            PacketHolder.StartPacketHolder(builder);
            PacketHolder.AddPacketType(builder, Packets.ObjectMovementAndRotation);
            PacketHolder.AddPacket(builder, objPacketOffset.Value);
            var packetHolder = PacketHolder.EndPacketHolder(builder);

            PacketHolder.FinishPacketHolderBuffer(builder, packetHolder);

            return builder.SizedByteArray();
        }

        public static byte[] GetObjectMovementBytes(string objName,Vector3 pos)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            ObjectMovement.StartObjectMovement(builder);
            ObjectMovement.AddObj(builder, uint.Parse(objName));
            var objPosOffset = Vec3.CreateVec3(builder, pos.x, pos.y, pos.z);
            ObjectMovement.AddPos(builder, objPosOffset);
            var objPacketOffset = ObjectMovement.EndObjectMovement(builder);

            PacketHolder.StartPacketHolder(builder);
            PacketHolder.AddPacketType(builder, Packets.ObjectMovement);
            PacketHolder.AddPacket(builder, objPacketOffset.Value);
            var packetHolder = PacketHolder.EndPacketHolder(builder);

            PacketHolder.FinishPacketHolderBuffer(builder, packetHolder);

            return builder.SizedByteArray();
        }

        public static byte[] GetObjectRotationBytes(string objName,Vector3 rot)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            ObjectRotation.StartObjectRotation(builder);
            ObjectRotation.AddObj(builder, uint.Parse(objName));
            var objPosOffset = Vec3.CreateVec3(builder, rot.x, rot.y, rot.z);
            ObjectRotation.AddRot(builder, objPosOffset);
            var objPacketOffset = ObjectRotation.EndObjectRotation(builder);

            PacketHolder.StartPacketHolder(builder);
            PacketHolder.AddPacketType(builder, Packets.ObjectRotation);
            PacketHolder.AddPacket(builder, objPacketOffset.Value);
            var packetHolder = PacketHolder.EndPacketHolder(builder);

            PacketHolder.FinishPacketHolderBuffer(builder, packetHolder);

            return builder.SizedByteArray();
        }

        public static byte[] GetPlayerIdsBytes(IEnumerable<CSteamID> playerIds)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(16);

            PlayerIds.StartPlayerIds(builder);
            var convertedIds = playerIds.ToList().ConvertAll(item => (ulong) item);
            var idsVector = PlayerIds.CreateIdsVector(builder, convertedIds.ToArray());
            PlayerIds.AddIds(builder, idsVector);
            var idsOffset = PlayerIds.EndPlayerIds(builder);

            PacketHolder.StartPacketHolder(builder);
            PacketHolder.AddPacketType(builder, Packets.PlayerIds);
            PacketHolder.AddPacket(builder, idsOffset.Value);
            var packetHolder = PacketHolder.EndPacketHolder(builder);

            PacketHolder.FinishPacketHolderBuffer(builder, packetHolder);

            return builder.SizedByteArray();
        }

        public static byte[] GetUserActionBytes(ActionType action)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            UserAction.StartUserAction(builder);
            UserAction.AddAction(builder, action);
            var userActionOffset = UserAction.EndUserAction(builder);

            PacketHolder.StartPacketHolder(builder);
            PacketHolder.AddPacketType(builder, Packets.UserAction);
            PacketHolder.AddPacket(builder, userActionOffset.Value);
            var packetHolder = PacketHolder.EndPacketHolder(builder);

            PacketHolder.FinishPacketHolderBuffer(builder, packetHolder);

            return builder.SizedByteArray();
        }

        public static byte[] GetSceneChangeBytes(string sceneName)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(8);

            SceneChange.StartSceneChange(builder);
            SceneChange.AddScene(builder, builder.CreateString(sceneName));
            var sceneOffset = SceneChange.EndSceneChange(builder);

            PacketHolder.StartPacketHolder(builder);
            PacketHolder.AddPacketType(builder, Packets.SceneChange);
            PacketHolder.AddPacket(builder, sceneOffset.Value);
            var packetHolder = PacketHolder.EndPacketHolder(builder);

            PacketHolder.FinishPacketHolderBuffer(builder, packetHolder);

            return builder.SizedByteArray();
        }
    }
}