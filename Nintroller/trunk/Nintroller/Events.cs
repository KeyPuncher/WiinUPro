using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintrollerLib
{
    /// <summary>
    /// Object that is returned from the StateChange Event
    /// </summary>
    public class StateChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Returned state of the device.
        /// </summary>
        public NintyState DeviceState;
        /// <summary>
        /// State of the device as if a Wiimote.
        /// (Empty values if not a Wiimote)
        /// </summary>
        public WiimoteState Wiimote;
        /// <summary>
        /// State of the device as if a WiimotePlus.
        /// (Empty values if not a WiimotePlus)
        /// </summary>
        public WiimotePlusState WiimotePlus;
        /// <summary>
        /// State of the device as if a Pro Controller.
        /// (Empty values if not a Pro Controller)
        /// </summary>
        public ProControllerState ProController;
        /// <summary>
        /// State of the device as if a Balance Board.
        /// (Empty values if not a BalanceBoard)
        /// </summary>
        public BalanceBoardState BalanceBoard;

        /// <summary>
        /// Constructor to set the State.
        /// </summary>
        /// <param name="state">State of the device.</param>
        public StateChangeEventArgs(NintyState state)
        {
            DeviceState = state;

            Wiimote = WiimoteState.Empty;
            WiimotePlus = WiimotePlusState.Empty;
            ProController = ProControllerState.Empty;
            BalanceBoard = BalanceBoardState.Empty;

            if (state.GetType() == typeof(WiimoteState))
                Wiimote = (WiimoteState)state;
            else if (state.GetType() == typeof(WiimotePlusState))
                WiimotePlus = (WiimotePlusState)state;
            else if (state.GetType() == typeof(ProControllerState))
                ProController = (ProControllerState)state;
            else if (state.GetType() == typeof(BalanceBoardState))
                BalanceBoard = (BalanceBoardState)state;
        }
    }

    /// <summary>
    /// Object that is returned from the ExtensionChange Event
    /// </summary>
    public class ExtensionChangeEventArgs : EventArgs
    {
        /// <summary>
        /// The controller this extension belongs to.
        /// </summary>
        public Guid ControllerID;
        /// <summary>
        /// The type of extension that is now attatched.
        /// </summary>
        public ControllerType Extension;

        /// <summary>
        /// Constructor to set the ID and Extension.
        /// </summary>
        /// <param name="id">ID of the controller</param>
        /// <param name="ext">Extension type</param>
        public ExtensionChangeEventArgs(Guid id, ControllerType ext)
        {
            ControllerID = id;
            Extension = ext;
        }
    }
}
