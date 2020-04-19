using System;
using Godot;

namespace Gareo.Scenes.Menu
{
    public class StartGameButton : Button
    {
        public event EventHandler StartGameButtonPressed;

        public override void _Pressed()
        {
            OnStartGameButtonPressed();
        }

        private void OnStartGameButtonPressed()
        {
            StartGameButtonPressed?.Invoke(this, EventArgs.Empty);
        }
    }
}
