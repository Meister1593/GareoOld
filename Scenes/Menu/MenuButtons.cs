using System;
using Gareo.Scripts.Lobby;
using Godot;
using Lidgren.Network;
using Steamworks;

namespace Gareo.Scenes.Menu
{
    public class MenuButtons : Node
    {
        [Export(PropertyHint.File, "*.tscn")] private string WorldScenePath;

        private Globals _globals;
        private Lobby _lobby;

        [Export] private NodePath _startGameButtonPath;

        [Export] private NodePath _exitGameButtonPath;

        private StartGameButton _startGameButton;
        private ExitGameButton _exitGameButton;

        public override void _Ready()
        {
            _globals = GetNode<Globals>("/root/Globals");
            _lobby = GetNode<Lobby>("/root/SteamLobby");

            _startGameButton = GetNode<StartGameButton>(_startGameButtonPath);
            _exitGameButton = GetNode<ExitGameButton>(_exitGameButtonPath);

            _lobby.LobbyEnteredFinished += ChangeSceneToWorld;
            _startGameButton.StartGameButtonPressed += OnStartGameButtonPressed;
            _exitGameButton.ExitGameButtonPressed += OnExitGameButtonPressed;
        }

        private void ChangeSceneToWorld(object source, EventArgs args)
        {
            GetTree().ChangeScene(WorldScenePath);
        }

        // When pressed StartGame button, it tries to create server
        private void OnStartGameButtonPressed(object source, EventArgs args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("DefaultConfiguration");
            config.Port = _globals.Port;
            config.EnableUPnP = true;
            config.LocalAddress = NetUtility.Resolve("localhost");
            NetServer server = new NetServer(config);
            server.Start();
            _globals.Server = server;
            _globals.PlayingAsHost = true;
            GD.Print("OK: Created Server");
        }

        // When pressed ExitGame button, exit game.
        private void OnExitGameButtonPressed(object source, EventArgs args)
        {
            GD.Print("OK: Shutting down game");
            GetTree().Quit();
        }
    }
}