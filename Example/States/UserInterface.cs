using Californium;

namespace Example.States
{
    class UserInterface : State
    {
        // This state will be rendered above the Example state (see Program.cs).
        // It doesn't need to do anything special but the Example state is zoomed
        // in and we don't want our HUD to be the same, so using another state will
        // give us a different camera.
        public UserInterface()
        {
            InactiveMode = UpdateMode.Draw;
            IsOverlay = true;

            // Create and register our fancy score display
            Entities.Add(new Entities.ScorePanel());
        }
    }
}
