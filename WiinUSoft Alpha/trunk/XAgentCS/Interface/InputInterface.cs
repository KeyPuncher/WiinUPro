using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**List of Capabilities we want
 * Display debugging messages on the console
 * Disconnect clients from the server
 * Get controller status update from server
 * Set controller's vibration status
*/

namespace XAgentCS.Interface
{
    #region Used for IPC Communication

    #region Delegates
    [Serializable]      // Get a message from the client
    public delegate void MessageReceivedEvent(MessageReceivedEventArgs message);

    [Serializable]      // Disconnect the client
    public delegate void DisconnectedEvent();

    [Serializable]      // Set the forcefeedback
    public delegate void RumbleSetEvent(RumbleSetEventArgs args);

    [Serializable]      // Update the state of this controller
    public delegate void StateChangedEvent(StateChangedEventArgs args);

    [Serializable]      // Tells the client when controller connections change
    public delegate void ControllerConnectedEvent(ControllersConnectedEventArgs args);
    #endregion

    [Serializable]
    public class InputInterface : MarshalByRefObject
    {
        // Client process ID
        public int ProcessID { get; set; }

        // Used to verify the channel is open
        public void Ping() { }

        #region Server Events
        // Recieved message from the client
        public event MessageReceivedEvent RemoteMessage;

        // Rumble is to be set
        public event RumbleSetEvent RumbleEvent;
        #endregion

        #region Client Events
        // Notify the client to disconnect
        public event DisconnectedEvent Disconnected;

        // Notify the client to update the controller's state
        public event StateChangedEvent StateChanged;

        // Tell the client what controllers are now connected
        public event ControllerConnectedEvent ControllerConnected;
        #endregion

        #region Public Functions
        // Message everyone
        public void Message(string message)
        {
            InvokeMessageRecevied(new MessageReceivedEventArgs(message));
        }

        // Tell the client to disconnect
        public void Disconnect()
        {
            InvokeDisconnected();
        }

        // Set the rumble for a controller
        public void SetRumble(int player, bool rumble)
        {
            InvokeSetRumble(new RumbleSetEventArgs(player, rumble));
        }

        // Update the state of the client
        public void SetState(int player, XControllers state)
        {
            InvokeStateChanged(new StateChangedEventArgs(player, state));
        }

        // Updated the conected controllers
        public void SetControllers(bool p1, bool p2, bool p3, bool p4)
        {
            InvokeControllerChanged(new ControllersConnectedEventArgs(p1, p2, p3, p4));
        }
        #endregion

        #region Invoke Handlers
        // This is the safe way to invoke these events
        // Using a generic type function would require the use of
        // DynamicInvoke(args) which is slower than Invoke(args)
        // plus we won't be able to remove the listener from the event

        private void InvokeMessageRecevied(MessageReceivedEventArgs eventArgs)
        {
            if (RemoteMessage == null)
                return;     // No Listeners

            MessageReceivedEvent listener = null;
            Delegate[] delegates = RemoteMessage.GetInvocationList();

            foreach (Delegate del in delegates)
            {
                try
                {
                    listener = (MessageReceivedEvent)del;
                    listener.Invoke(eventArgs);
                }
                catch (Exception)
                {
                    // Unreachable, so remove
                    RemoteMessage -= listener;
                }
            }
        }

        private void InvokeDisconnected()
        {
            if (Disconnected == null)
                return;     // No Listeners

            DisconnectedEvent listener = null;
            Delegate[] delegates = Disconnected.GetInvocationList();

            foreach (Delegate del in delegates)
            {
                try
                {
                    listener = (DisconnectedEvent)del;
                    listener.Invoke();
                }
                catch (Exception)
                {
                    // Unreachable, remove
                    Disconnected -= listener;
                }
            }
        }

        private void InvokeSetRumble(RumbleSetEventArgs eventArgs)
        {
            if (RumbleEvent == null)
                return;

            RumbleSetEvent listener = null;
            Delegate[] delegates = RumbleEvent.GetInvocationList();

            foreach (Delegate del in delegates)
            {
                try
                {
                    listener = (RumbleSetEvent)del;
                    listener.Invoke(eventArgs);
                }
                catch (Exception)
                {
                    RumbleEvent -= listener;
                }
            }
        }

        private void InvokeStateChanged(StateChangedEventArgs eventArgs)
        {
            if (StateChanged == null)
                return;

            StateChangedEvent listener = null;
            Delegate[] delegates = StateChanged.GetInvocationList();

            foreach (Delegate del in delegates)
            {
                try
                {
                    listener = (StateChangedEvent)del;
                    listener.Invoke(eventArgs);
                }
                catch (Exception)
                {
                    StateChanged -= listener;
                }
            }
        }

        private void InvokeControllerChanged(ControllersConnectedEventArgs eventArgs)
        {
            if (ControllerConnected == null)
                return;

            ControllerConnectedEvent listener = null;
            Delegate[] delegates = ControllerConnected.GetInvocationList();

            foreach (Delegate del in delegates)
            {
                try
                {
                    listener = (ControllerConnectedEvent)del;
                    listener.Invoke(eventArgs);
                }
                catch (Exception)
                {
                    ControllerConnected -= listener;
                }
            }
        }

        #endregion
    }

    // For Client Event Handlers
    public class ClientInputInterfaceEventProxy : MarshalByRefObject
    {
        #region Events (Client Events from Above)
        // Disconnect/exit the hook
        public DisconnectedEvent Disconnected;

        // Get state updates
        public StateChangedEvent StateChanged;

        // Get the Connected Controllers
        public ControllerConnectedEvent ControllerConnected;
        #endregion

        #region Handlers
        public void DisconnectedProxyHandler()
        {
            if (Disconnected != null)
                Disconnected();
        }

        public void StateChangedProxyHandler(StateChangedEventArgs args)
        {
            if (StateChanged != null)
                StateChanged(args);
        }

        public void ControllerConnectedProxyHandler(ControllersConnectedEventArgs args)
        {
            if (ControllerConnected != null)
                ControllerConnected(args);
        }
        #endregion

        // Keep alive
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    #endregion
}
