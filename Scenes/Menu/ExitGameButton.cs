using System;
using Godot;

namespace Gareo.Scenes.Menu
{
    public class ExitGameButton : Button
    {
        public event EventHandler ExitGameButtonPressed;

        public override void _Pressed()
        {
            OnExitGameButtonPressed();
        }

        private void OnExitGameButtonPressed()
        {
            ExitGameButtonPressed?.Invoke(this, EventArgs.Empty);
        }
    }
}