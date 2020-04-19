using Gareo.Scripts.Lobby;
using Godot;
using NetworkPacket;
using Steamworks;

namespace Gareo.Scenes.Player.Scripts
{
    public class Player : KinematicBody
    {
        private readonly Vector3 UP = Vector3.Up;
        private readonly float MAX_SPEED = 20f;
        private readonly float ACCELERATION = 5f;
        private readonly float GRAVITY = 10f;
        private readonly float JUMP_HEIGHT = 35f;
        private readonly float INERTIA = 0.2f;

        private Vector3 _motion;

        private Globals _globals;
        private Lobby _lobby;

        private bool _sendPacketReady;

        public override void _Ready()
        {
            _globals = GetNode<Globals>("/root/Globals");
            _lobby = GetNode<Lobby>("/root/SteamLobby");
        }

        public override void _PhysicsProcess(float delta)
        {
            _motion.y = Mathf.Min(_motion.y - GRAVITY, MAX_SPEED);
            bool friction = false;

            if (Input.IsActionPressed("ui_left"))
            {
                _motion.x = Mathf.Min(_motion.x + ACCELERATION, MAX_SPEED);
            }
            else if (Input.IsActionPressed("ui_right"))
            {
                _motion.x = Mathf.Max(_motion.x - ACCELERATION, -MAX_SPEED);
            }
            else if (Input.IsActionPressed("ui_up"))
            {
                _motion.z = Mathf.Min(_motion.z + ACCELERATION, MAX_SPEED);
            }
            else if (Input.IsActionPressed("ui_down"))
            {
                _motion.z = Mathf.Max(_motion.z - ACCELERATION, -MAX_SPEED);
            }
            else
                friction = true;

            if (IsOnFloor())
            {
                if (Input.IsActionPressed("space_bar"))
                {
                    _motion.y = JUMP_HEIGHT;
                }

                if (friction)
                {
                    _motion.x = Mathf.Lerp(_motion.x, 0, INERTIA);
                    _motion.z = Mathf.Lerp(_motion.z, 0, INERTIA);
                }
            }
            else
            {
                if (friction)
                {
                    _motion.x = Mathf.Lerp(_motion.x, 0, INERTIA * 0.25f);
                    _motion.z = Mathf.Lerp(_motion.z, 0, INERTIA * 0.25f);
                }
            }

            _motion = MoveAndSlide(_motion, UP);
        }

        public override void _Process(float delta) => TransferPlayerMovement();

        private void TransferPlayerMovement()
        {
            if (!_sendPacketReady) return;

            var packet = Utils.GetPlayerMovementAndRotationBytes(false, Name, Transform.origin,
                Rotation);

            if (_globals.PlayingAsHost)
            {
                foreach (CSteamID id in _globals.UserIds)
                {
                    SteamNetworking.SendP2PPacket(id, packet, (uint) packet.Length, EP2PSend.k_EP2PSendUnreliable);
                }
            }
            else
            {
                SteamNetworking.SendP2PPacket(_globals.HostId, packet, (uint) packet.Length,
                    EP2PSend.k_EP2PSendUnreliable);
            }

            _sendPacketReady = false;
        }

        public void _on_TickrateSend_timeout() => _sendPacketReady = true;
    }
}