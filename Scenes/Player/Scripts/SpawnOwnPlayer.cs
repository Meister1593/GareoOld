using Gareo.Scripts.Lobby;
using Godot;
using NetworkPacket;
using Steamworks;

namespace Gareo.Scenes.Player.Scripts
{
    public class SpawnOwnPlayer : Node
    {
        [Export(PropertyHint.File, "*.tscn")] private string _playerPath;

        [Export(PropertyHint.Range, "Vector3")]
        public Vector3 DefaultSpawnPos;

        private Globals _globals;

        public override void _Ready()
        {
            GD.Print("NOTIFY: SpawnOwnPlayer Ready started");
            _globals = GetNode<Globals>("/root/Globals");
            if (_globals.PlayingAsHost)
            {
                GD.Print("NOTIFY: SpawnOwnPlayer as Host");
                PackedScene playerScene = ResourceLoader.Load<PackedScene>(_playerPath);
                KinematicBody player = playerScene?.Instance() as KinematicBody;
                if (player == null) return;

                CallDeferred("add_child", player);
                _globals.OwnPlayerBody = player;

                player.Name = _globals.HostId.ToString();
                Transform playerTransform = player.Transform;
                playerTransform.origin = DefaultSpawnPos;
                player.Transform = playerTransform;
            }
            else
            {
                GD.Print("NOTIFY: SpawnOwnPlayer as Client, sending");
                var packet = Utils.GetSpawnBytes(true, _globals.OwnId.ToString(),
                    ActionType.PlayerSpawn, DefaultSpawnPos);
                SteamNetworking.SendP2PPacket(_globals.HostId, packet, (uint) packet.Length,
                    EP2PSend.k_EP2PSendReliable);
            }
        }
    }
}