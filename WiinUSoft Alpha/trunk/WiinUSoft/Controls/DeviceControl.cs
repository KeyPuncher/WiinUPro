using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using NintrollerLib;
using ScpControl;

namespace WiinUSoft
{
    public partial class DeviceControl : UserControl
    {
        public static Dictionary<string, string> defaultAssignments = new Dictionary<string, string>()
        {
            {"A"     , "A"},
            {"B"     , "B"},
            {"X"     , "X"},
            {"Y"     , "Y"},
            {"Up"    , "Up"},
            {"Down"  , "Down"},
            {"Left"  , "Left"},
            {"Right" , "Right"},
            {"Start" , "Start"},
            {"Select", "Back"},
            {"Home"  , "Guide"},
            {"L"     , "LBumper"},
            {"R"     , "RBumper"},
            {"ZL"    , "LTrigger"},
            {"ZR"    , "RTrigger"},
            {"LS"    , "LStick"},
            {"RS"    , "RStick"},
            {"LX"    , "LX"},
            {"LY"    , "LY"},
            {"RX"    , "RX"},
            {"RY"    , "RY"}
        };

        public bool connected = false;
        public Nintroller controller;
        public int playerNum = 0;
        public Dictionary<string, string> assignments;
        public Configure config = new Configure();

        private delegate void UpdateStateDelegate(NintrollerLib.StateChangeEventArgs args);
        private XBus bus;
        private XReport report;
//        private XDevice device;
//        private XHandler handler;
        private bool xConnected = false;

        public DeviceControl()
        {
            InitializeComponent();
            assignments = new Dictionary<string, string>(defaultAssignments);
        }
        public DeviceControl(Nintroller nc) : this()
        {
            cConfigButton.Text = "Connect";
            controller = nc;
        }

            // Connect or configure the controller
        private void cConfigButton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                config.ShowDialog();
                if (config.DialogResult == DialogResult.OK)
                {
                    assignments["A"]      = (string) config.comboA.SelectedItem;
                    assignments["B"]      = (string) config.comboB.SelectedItem;
                    assignments["X"]      = (string) config.comboX.SelectedItem;
                    assignments["Y"]      = (string) config.comboY.SelectedItem;
                    assignments["L"]      = (string) config.comboL.SelectedItem;
                    assignments["R"]      = (string) config.comboR.SelectedItem;
                    assignments["ZL"]     = (string) config.comboZL.SelectedItem;
                    assignments["ZR"]     = (string) config.comboZR.SelectedItem;
                    assignments["LS"]     = (string) config.comboLS.SelectedItem;
                    assignments["RS"]     = (string) config.comboRS.SelectedItem;
                    assignments["Up"]     = (string) config.comboUp.SelectedItem;
                    assignments["Down"]   = (string) config.comboDown.SelectedItem;
                    assignments["Left"]   = (string) config.comboLeft.SelectedItem;
                    assignments["Right"]  = (string) config.comboRight.SelectedItem;
                    assignments["Start"]  = (string) config.comboStart.SelectedItem;
                    assignments["Select"] = (string) config.comboSelect.SelectedItem;
                }
            }
            else
            {
                // Connect the controller
                if (controller.Connect())
                {
                    connected = true;
                    cConfigButton.Text = "Configure";
                    cUpButton.Enabled = true;
                    cDownButton.Enabled = true;
                    cPlayerLabel.Text = "Player";
                    playerNum = ((Main)Parent.Parent).ControllerConnected(this);

                    #region SCP_Driver
                    if (ConnectXInput(playerNum + 1))
                    {
                        Debug.WriteLine("Xinput Connected");
                    }
                    else
                    {
                        Debug.WriteLine("Xinput not connected");
                    }
                    #endregion

                    cPlayerNum.Text = (playerNum + 1).ToString();
                    controller.SetPlayerLED(playerNum + 1);
                    controller.StateChange += controller_StateChange;
                    Console.WriteLine(playerNum.ToString());
                }
            }
        }

            // Safely call the controller update method
        void controller_StateChange(object sender, NintrollerLib.StateChangeEventArgs e)
        {
            try
            {
                BeginInvoke(new UpdateStateDelegate(ControllerUpdate), e);
            }
            catch
            {
                return;
            }
        }

            // read the controller state
        private void ControllerUpdate(NintrollerLib.StateChangeEventArgs args)
        {
            if (playerNum > 3) return;

            #region SCP_Driver
            if (report != null)
            {
                // TODO: update xinput handler
                report.SetButton(assignments["A"], args.ProController.A);
                report.SetButton(assignments["B"], args.ProController.B);
                report.SetButton(assignments["X"], args.ProController.X);
                report.SetButton(assignments["Y"], args.ProController.Y);
                report.SetButton(assignments["L"], args.ProController.L);
                report.SetButton(assignments["R"], args.ProController.R);
                report.SetButton(assignments["ZL"], args.ProController.ZL);
                report.SetButton(assignments["ZR"], args.ProController.ZR);
                report.SetButton(assignments["LS"], args.ProController.LS);
                report.SetButton(assignments["RS"], args.ProController.RS);
                report.SetButton(assignments["Up"], args.ProController.Up);
                report.SetButton(assignments["Down"], args.ProController.Down);
                report.SetButton(assignments["Left"], args.ProController.Left);
                report.SetButton(assignments["Right"], args.ProController.Right);
                report.SetButton(assignments["Start"], args.ProController.Start);
                report.SetButton(assignments["Select"], args.ProController.Select);
                report.SetButton(assignments["Home"], args.ProController.Home);

                report.LX = args.ProController.LeftJoy.X;
                report.LY = args.ProController.LeftJoy.Y;
                report.RX = args.ProController.RightJoy.X;
                report.RY = args.ProController.RightJoy.Y;

                UpdateXInput(report);
            }

            // Test 2
            /*handler.StartUpdate(); // does nothing really

            handler.SetButton(assignments["A"], args.ProController.A);
            handler.SetButton(assignments["B"], args.ProController.B);
            handler.SetButton(assignments["X"], args.ProController.X);
            handler.SetButton(assignments["Y"], args.ProController.Y);
            handler.SetButton(assignments["L"], args.ProController.L);
            handler.SetButton(assignments["R"], args.ProController.R);
            handler.SetButton(assignments["ZL"], args.ProController.ZL);
            handler.SetButton(assignments["ZR"], args.ProController.ZR);
            handler.SetButton(assignments["LS"], args.ProController.LS);
            handler.SetButton(assignments["RS"], args.ProController.RS);
            handler.SetButton(assignments["Up"], args.ProController.Up);
            handler.SetButton(assignments["Down"], args.ProController.Down);
            handler.SetButton(assignments["Left"], args.ProController.Left);
            handler.SetButton(assignments["Right"], args.ProController.Right);
            handler.SetButton(assignments["Start"], args.ProController.Start);
            handler.SetButton(assignments["Select"], args.ProController.Select);
            handler.SetButton(assignments["Home"], args.ProController.Home);

            handler.SetAxis("lx", (args.ProController.LeftJoy.X + 1) * 0.5);
            handler.SetAxis("ly", (args.ProController.LeftJoy.Y + 1) * 0.5);
            handler.SetAxis("rx", (args.ProController.RightJoy.X + 1) * 0.5);
            handler.SetAxis("ry", (args.ProController.RightJoy.Y + 1) * 0.5);

            handler.EndUpdate();*/
            #endregion
        }

            // Move this controller up in the list
        private void cUpButton_Click(object sender, EventArgs e)
        {
            if (connected && playerNum > 0)
            {
                ((Main)Parent.Parent).MoveUp(this);
            }
        }

            // Move this controller down in the list
        private void cDownButton_Click(object sender, EventArgs e)
        {
            if (connected && playerNum < 3)
            {
                ((Main)Parent.Parent).MoveDown(this);
            }
        }

        public void SetPlayerNum (int num)
        {
            int previousNum = playerNum;

            playerNum = num;
            cPlayerNum.Text = (playerNum + 1).ToString();
            controller.SetPlayerLED(playerNum + 1);

            // TODO: change handle device
            if (xConnected)
            {
                RemoveXInput(previousNum + 1);
                ConnectXInput(playerNum + 1);
            }
        }

        #region SCP_Driver
        public bool UpdateXInput(XReport r)
        {
            if (report != null)
            {
                Byte[] input = r.ToBytes();
                Byte[] rumble = new Byte[8];
                Byte[] reportB = new Byte[28];

                bus.Parse(input, reportB);

                if (bus.Report(reportB, rumble))
                {
                    if (rumble[1] == 0x08)
                    {
                        Byte big = (Byte)rumble[3];
                        Byte small = (Byte)rumble[4];

                        // TODO: Rumble
                        Debug.WriteLine("Big Rumble: " + big.ToString());
                        Debug.WriteLine("Small Rumble: " + small.ToString());
                    }

                    return true;
                }
            }

            return false;
        }

        public bool ConnectXInput(int id)
        {
            bus = XBus.Default;

            if (xConnected)
            {
                bus.Unplug(id);
            }

            report = new XReport(id);
            // TODO: set rumble event

            xConnected = bus.Plugin(id);
            return xConnected;
        }

        public bool RemoveXInput(int id)
        {
            if (bus.Unplug(id))
            {
                xConnected = false;
                return true;
            }

            return false;
        }
        #endregion
    }
}
