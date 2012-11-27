using Californium;

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
