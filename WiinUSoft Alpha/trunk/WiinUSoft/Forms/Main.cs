using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using NintrollerLib;

// TODO: Must disconnect from a process before creating a new one

namespace WiinUSoft
{
    public partial class Main : Form
    {
        private List<DeviceControl> availableDevices;
        private static int connectedControllers = 0;

            // When this object is created
        public Main()
        {
            InitializeComponent();
        }

        public int ControllerConnected (DeviceControl c)
        {
            flowPanelConnected.Controls.Add(c);
            return connectedControllers++;
        }

        public void MoveUp(DeviceControl c)
        {
            int location = flowPanelConnected.Controls.IndexOf(c);
            if (location > 0)
                flowPanelConnected.Controls.SetChildIndex(c, location - 1);

            foreach (DeviceControl d in flowPanelConnected.Controls)
            {
                d.SetPlayerNum(flowPanelConnected.Controls.IndexOf(d));
            }
        }

        public void MoveDown(DeviceControl c)
        {
            int location = flowPanelConnected.Controls.IndexOf(c);
            if (location < flowPanelConnected.Controls.Count - 1)
                flowPanelConnected.Controls.SetChildIndex(c, location + 1);

            foreach (DeviceControl d in flowPanelConnected.Controls)
            {
                d.SetPlayerNum(flowPanelConnected.Controls.IndexOf(d));
            }
        }

            // When the Main form is loaded
        private void Main_Load(object sender, EventArgs e)
        {
            // List supported paired BT devices
            List<string> supportedDevices = Nintroller.FindControllers();
            availableDevices = new List<DeviceControl>();

            // Form a list of those that are connected
            for (int i = 0; i < supportedDevices.Count; i++)
            {
                Nintroller n = new Nintroller(supportedDevices[i]);
                if (n.ConnectTest())
                {
                    availableDevices.Add(new DeviceControl(n));
                    flowPanelAvailable.Controls.Add(availableDevices[availableDevices.Count - 1]);
                }
            }
        }

            // When the refresh controllers list button is clicked
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // TODO: Refresh device list
        }

            // Make sure the xinput interface is stopped and disconnected
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (DeviceControl d in availableDevices)
            {
                d.RemoveXInput(d.playerNum + 1);
            }
        }
    }
}
