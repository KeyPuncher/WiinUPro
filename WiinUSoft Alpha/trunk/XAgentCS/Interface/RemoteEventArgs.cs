using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAgentCS.Interface
{
    [Serializable]
    public class RumbleSetEventArgs : MarshalByRefObject
    {
        public int Player { get; set; }
        public bool Rumble { get; set; }

        public RumbleSetEventArgs(int player, bool rumble)
        {
            Player = player;
            Rumble = rumble;
        }

        public override string ToString()
        {
            return "Player " + Player.ToString() + " Rumble: " + Rumble.ToString();
        }
    }

    [Serializable]
    public class StateChangedEventArgs : MarshalByRefObject
    {
        public int Player { get; set; }
        public XControllers State { get; set; }

        public StateChangedEventArgs(int player, XControllers state)
        {
            Player = player;
            State = state;
        }
    }

    [Serializable]
    public class ControllersConnectedEventArgs : MarshalByRefObject
    {
        public bool P1Connected { get; set; }
        public bool P2Connected { get; set; }
        public bool P3Connected { get; set; }
        public bool P4Connected { get; set; }

        public ControllersConnectedEventArgs(bool p1, bool p2, bool p3, bool p4)
        {
            P1Connected = p1;
            P2Connected = p2;
            P3Connected = p3;
            P4Connected = p4;
        }
    }
}
