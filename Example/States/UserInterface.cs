using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.States
{
    class UserInterface : State
    {
        public UserInterface()
        {
            Entities.Add(new Entities.Panel());
        }
    }
}
