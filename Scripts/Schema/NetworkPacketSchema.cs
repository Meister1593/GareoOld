// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace NetworkPacket
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public enum ActionType : byte
{
  PlayerIds = 0,
  PlayerSpawn = 1,
  PropSpawn = 2,
};

public enum ObjectType : byte
{
  Player = 0,
  Prop = 1,
};

public enum Packets : byte
{
  NONE = 0,
  MovementAndRotation = 1,
  Movement = 2,
  Rotation = 3,
  PlayerIds = 4,
  Spawn = 5,
  UserAction = 6,
  SceneChange = 7,
};

public struct Vec3 : IFlatbufferObject
{
  private Struct __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public void __init(int _i, ByteBuffer _bb) { __p = new Struct(_i, _bb); }
  public Vec3 __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public float X { get { return __p.bb.GetFloat(__p.bb_pos + 0); } }
  public float Y { get { return __p.bb.GetFloat(__p.bb_pos + 4); } }
  public float Z { get { return __p.bb.GetFloat(__p.bb_pos + 8); } }

  public static Offset<NetworkPacket.Vec3> CreateVec3(FlatBufferBuilder builder, float X, float Y, float Z) {
    builder.Prep(4, 12);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<NetworkPacket.Vec3>(builder.Offset);
  }
};

public struct MovementAndRotation : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static MovementAndRotation GetRootAsMovementAndRotation(ByteBuffer _bb) { return GetRootAsMovementAndRotation(_bb, new MovementAndRotation()); }
  public static MovementAndRotation GetRootAsMovementAndRotation(ByteBuffer _bb, MovementAndRotation obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public MovementAndRotation __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public NetworkPacket.ObjectType ObjType { get { int o = __p.__offset(4); return o != 0 ? (NetworkPacket.ObjectType)__p.bb.Get(o + __p.bb_pos) : NetworkPacket.ObjectType.Player; } }
  public bool SentByHost { get { int o = __p.__offset(6); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public string Obj { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetObjBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
  public ArraySegment<byte>? GetObjBytes() { return __p.__vector_as_arraysegment(8); }
#endif
  public byte[] GetObjArray() { return __p.__vector_as_array<byte>(8); }
  public NetworkPacket.Vec3? Pos { get { int o = __p.__offset(10); return o != 0 ? (NetworkPacket.Vec3?)(new NetworkPacket.Vec3()).__assign(o + __p.bb_pos, __p.bb) : null; } }
  public NetworkPacket.Vec3? Rot { get { int o = __p.__offset(12); return o != 0 ? (NetworkPacket.Vec3?)(new NetworkPacket.Vec3()).__assign(o + __p.bb_pos, __p.bb) : null; } }

  public static void StartMovementAndRotation(FlatBufferBuilder builder) { builder.StartTable(5); }
  public static void AddObjType(FlatBufferBuilder builder, NetworkPacket.ObjectType objType) { builder.AddByte(0, (byte)objType, 0); }
  public static void AddSentByHost(FlatBufferBuilder builder, bool sentByHost) { builder.AddBool(1, sentByHost, false); }
  public static void AddObj(FlatBufferBuilder builder, StringOffset objOffset) { builder.AddOffset(2, objOffset.Value, 0); }
  public static void AddPos(FlatBufferBuilder builder, Offset<NetworkPacket.Vec3> posOffset) { builder.AddStruct(3, posOffset.Value, 0); }
  public static void AddRot(FlatBufferBuilder builder, Offset<NetworkPacket.Vec3> rotOffset) { builder.AddStruct(4, rotOffset.Value, 0); }
  public static Offset<NetworkPacket.MovementAndRotation> EndMovementAndRotation(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.MovementAndRotation>(o);
  }
};

public struct Movement : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Movement GetRootAsMovement(ByteBuffer _bb) { return GetRootAsMovement(_bb, new Movement()); }
  public static Movement GetRootAsMovement(ByteBuffer _bb, Movement obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Movement __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public NetworkPacket.ObjectType ObjType { get { int o = __p.__offset(4); return o != 0 ? (NetworkPacket.ObjectType)__p.bb.Get(o + __p.bb_pos) : NetworkPacket.ObjectType.Player; } }
  public bool SentByHost { get { int o = __p.__offset(6); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public string Obj { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetObjBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
  public ArraySegment<byte>? GetObjBytes() { return __p.__vector_as_arraysegment(8); }
#endif
  public byte[] GetObjArray() { return __p.__vector_as_array<byte>(8); }
  public NetworkPacket.Vec3? Pos { get { int o = __p.__offset(10); return o != 0 ? (NetworkPacket.Vec3?)(new NetworkPacket.Vec3()).__assign(o + __p.bb_pos, __p.bb) : null; } }

  public static void StartMovement(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddObjType(FlatBufferBuilder builder, NetworkPacket.ObjectType objType) { builder.AddByte(0, (byte)objType, 0); }
  public static void AddSentByHost(FlatBufferBuilder builder, bool sentByHost) { builder.AddBool(1, sentByHost, false); }
  public static void AddObj(FlatBufferBuilder builder, StringOffset objOffset) { builder.AddOffset(2, objOffset.Value, 0); }
  public static void AddPos(FlatBufferBuilder builder, Offset<NetworkPacket.Vec3> posOffset) { builder.AddStruct(3, posOffset.Value, 0); }
  public static Offset<NetworkPacket.Movement> EndMovement(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.Movement>(o);
  }
};

public struct Rotation : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Rotation GetRootAsRotation(ByteBuffer _bb) { return GetRootAsRotation(_bb, new Rotation()); }
  public static Rotation GetRootAsRotation(ByteBuffer _bb, Rotation obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Rotation __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public NetworkPacket.ObjectType ObjType { get { int o = __p.__offset(4); return o != 0 ? (NetworkPacket.ObjectType)__p.bb.Get(o + __p.bb_pos) : NetworkPacket.ObjectType.Player; } }
  public bool SentByHost { get { int o = __p.__offset(6); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public string Obj { get { int o = __p.__offset(8); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetObjBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
  public ArraySegment<byte>? GetObjBytes() { return __p.__vector_as_arraysegment(8); }
#endif
  public byte[] GetObjArray() { return __p.__vector_as_array<byte>(8); }
  public NetworkPacket.Vec3? Rot { get { int o = __p.__offset(10); return o != 0 ? (NetworkPacket.Vec3?)(new NetworkPacket.Vec3()).__assign(o + __p.bb_pos, __p.bb) : null; } }

  public static void StartRotation(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddObjType(FlatBufferBuilder builder, NetworkPacket.ObjectType objType) { builder.AddByte(0, (byte)objType, 0); }
  public static void AddSentByHost(FlatBufferBuilder builder, bool sentByHost) { builder.AddBool(1, sentByHost, false); }
  public static void AddObj(FlatBufferBuilder builder, StringOffset objOffset) { builder.AddOffset(2, objOffset.Value, 0); }
  public static void AddRot(FlatBufferBuilder builder, Offset<NetworkPacket.Vec3> rotOffset) { builder.AddStruct(3, rotOffset.Value, 0); }
  public static Offset<NetworkPacket.Rotation> EndRotation(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.Rotation>(o);
  }
};

public struct PlayerIds : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static PlayerIds GetRootAsPlayerIds(ByteBuffer _bb) { return GetRootAsPlayerIds(_bb, new PlayerIds()); }
  public static PlayerIds GetRootAsPlayerIds(ByteBuffer _bb, PlayerIds obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public PlayerIds __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public bool SentByHost { get { int o = __p.__offset(4); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public ulong Ids(int j) { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUlong(__p.__vector(o) + j * 8) : (ulong)0; }
  public int IdsLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<ulong> GetIdsBytes() { return __p.__vector_as_span<ulong>(6, 8); }
#else
  public ArraySegment<byte>? GetIdsBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public ulong[] GetIdsArray() { return __p.__vector_as_array<ulong>(6); }

  public static Offset<NetworkPacket.PlayerIds> CreatePlayerIds(FlatBufferBuilder builder,
      bool sentByHost = false,
      VectorOffset idsOffset = default(VectorOffset)) {
    builder.StartTable(2);
    PlayerIds.AddIds(builder, idsOffset);
    PlayerIds.AddSentByHost(builder, sentByHost);
    return PlayerIds.EndPlayerIds(builder);
  }

  public static void StartPlayerIds(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddSentByHost(FlatBufferBuilder builder, bool sentByHost) { builder.AddBool(0, sentByHost, false); }
  public static void AddIds(FlatBufferBuilder builder, VectorOffset idsOffset) { builder.AddOffset(1, idsOffset.Value, 0); }
  public static VectorOffset CreateIdsVector(FlatBufferBuilder builder, ulong[] data) { builder.StartVector(8, data.Length, 8); for (int i = data.Length - 1; i >= 0; i--) builder.AddUlong(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateIdsVectorBlock(FlatBufferBuilder builder, ulong[] data) { builder.StartVector(8, data.Length, 8); builder.Add(data); return builder.EndVector(); }
  public static void StartIdsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 8); }
  public static Offset<NetworkPacket.PlayerIds> EndPlayerIds(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.PlayerIds>(o);
  }
};

public struct Spawn : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static Spawn GetRootAsSpawn(ByteBuffer _bb) { return GetRootAsSpawn(_bb, new Spawn()); }
  public static Spawn GetRootAsSpawn(ByteBuffer _bb, Spawn obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Spawn __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public bool SentByHost { get { int o = __p.__offset(4); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetNameBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public byte[] GetNameArray() { return __p.__vector_as_array<byte>(6); }
  public NetworkPacket.ActionType Action { get { int o = __p.__offset(8); return o != 0 ? (NetworkPacket.ActionType)__p.bb.Get(o + __p.bb_pos) : NetworkPacket.ActionType.PlayerIds; } }
  public NetworkPacket.Vec3? Pos { get { int o = __p.__offset(10); return o != 0 ? (NetworkPacket.Vec3?)(new NetworkPacket.Vec3()).__assign(o + __p.bb_pos, __p.bb) : null; } }

  public static void StartSpawn(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddSentByHost(FlatBufferBuilder builder, bool sentByHost) { builder.AddBool(0, sentByHost, false); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddAction(FlatBufferBuilder builder, NetworkPacket.ActionType action) { builder.AddByte(2, (byte)action, 0); }
  public static void AddPos(FlatBufferBuilder builder, Offset<NetworkPacket.Vec3> posOffset) { builder.AddStruct(3, posOffset.Value, 0); }
  public static Offset<NetworkPacket.Spawn> EndSpawn(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.Spawn>(o);
  }
};

public struct UserAction : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static UserAction GetRootAsUserAction(ByteBuffer _bb) { return GetRootAsUserAction(_bb, new UserAction()); }
  public static UserAction GetRootAsUserAction(ByteBuffer _bb, UserAction obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public UserAction __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public NetworkPacket.ActionType Action { get { int o = __p.__offset(4); return o != 0 ? (NetworkPacket.ActionType)__p.bb.Get(o + __p.bb_pos) : NetworkPacket.ActionType.PlayerIds; } }

  public static Offset<NetworkPacket.UserAction> CreateUserAction(FlatBufferBuilder builder,
      NetworkPacket.ActionType action = NetworkPacket.ActionType.PlayerIds) {
    builder.StartTable(1);
    UserAction.AddAction(builder, action);
    return UserAction.EndUserAction(builder);
  }

  public static void StartUserAction(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddAction(FlatBufferBuilder builder, NetworkPacket.ActionType action) { builder.AddByte(0, (byte)action, 0); }
  public static Offset<NetworkPacket.UserAction> EndUserAction(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.UserAction>(o);
  }
};

public struct SceneChange : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static SceneChange GetRootAsSceneChange(ByteBuffer _bb) { return GetRootAsSceneChange(_bb, new SceneChange()); }
  public static SceneChange GetRootAsSceneChange(ByteBuffer _bb, SceneChange obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public SceneChange __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Scene { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetSceneBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetSceneBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetSceneArray() { return __p.__vector_as_array<byte>(4); }

  public static Offset<NetworkPacket.SceneChange> CreateSceneChange(FlatBufferBuilder builder,
      StringOffset sceneOffset = default(StringOffset)) {
    builder.StartTable(1);
    SceneChange.AddScene(builder, sceneOffset);
    return SceneChange.EndSceneChange(builder);
  }

  public static void StartSceneChange(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddScene(FlatBufferBuilder builder, StringOffset sceneOffset) { builder.AddOffset(0, sceneOffset.Value, 0); }
  public static Offset<NetworkPacket.SceneChange> EndSceneChange(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.SceneChange>(o);
  }
};

public struct PacketHolder : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static PacketHolder GetRootAsPacketHolder(ByteBuffer _bb) { return GetRootAsPacketHolder(_bb, new PacketHolder()); }
  public static PacketHolder GetRootAsPacketHolder(ByteBuffer _bb, PacketHolder obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public PacketHolder __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public NetworkPacket.Packets PacketType { get { int o = __p.__offset(4); return o != 0 ? (NetworkPacket.Packets)__p.bb.Get(o + __p.bb_pos) : NetworkPacket.Packets.NONE; } }
  public TTable? Packet<TTable>() where TTable : struct, IFlatbufferObject { int o = __p.__offset(6); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }

  public static Offset<NetworkPacket.PacketHolder> CreatePacketHolder(FlatBufferBuilder builder,
      NetworkPacket.Packets packet_type = NetworkPacket.Packets.NONE,
      int packetOffset = 0) {
    builder.StartTable(2);
    PacketHolder.AddPacket(builder, packetOffset);
    PacketHolder.AddPacketType(builder, packet_type);
    return PacketHolder.EndPacketHolder(builder);
  }

  public static void StartPacketHolder(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddPacketType(FlatBufferBuilder builder, NetworkPacket.Packets packetType) { builder.AddByte(0, (byte)packetType, 0); }
  public static void AddPacket(FlatBufferBuilder builder, int packetOffset) { builder.AddOffset(1, packetOffset, 0); }
  public static Offset<NetworkPacket.PacketHolder> EndPacketHolder(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<NetworkPacket.PacketHolder>(o);
  }
  public static void FinishPacketHolderBuffer(FlatBufferBuilder builder, Offset<NetworkPacket.PacketHolder> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedPacketHolderBuffer(FlatBufferBuilder builder, Offset<NetworkPacket.PacketHolder> offset) { builder.FinishSizePrefixed(offset.Value); }
};


}
