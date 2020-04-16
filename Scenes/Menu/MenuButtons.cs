using Gareo.Scripts.Lobby;
using Godot;
using Steamworks;

namespace Gareo.Scenes.Menu
{
    public class MenuButtons : Node
    {
        private Globals _globals;

        public override void _Ready()
        {
            _globals = GetNode<Globals>("/root/Globals");
        }

        // When pressed StartGame button, it tries to create Steam lobby
        private void _on_StartGame_pressed()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _globals.MaxPlayers);
            GD.Print("OK: Created Steam lobby");
        }

        // When pressed ExitGame button, it shutdowns this game SteamApi (not whole steam) and exits game.
        private void _on_ExitGame_pressed()
        {
            GD.Print("OK: Shutting down game");
            GetTree().Quit();
        }
    }
}
