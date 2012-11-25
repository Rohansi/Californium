using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Californium;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            GameOptions.Width = 1280;
            GameOptions.Height = 720;

            Game.Initialize();
            Game.SetState(new States.Test());
            Game.PushState(new States.UserInterface());
            Game.Run();
        }
    }
}
