using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using NintrollerLib;
using XAgentCS.Interface;
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
        private XDevice device;
        private XHandler handler;

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
                    /*if (ConnectXInput())
                    {
                        Debug.WriteLine("Xinput Connected");
                    }
                    else
                    {
                        Debug.WriteLine("Xinput not connected");
                    }

                    report = new XReport(playerNum + 1);*/

                    // Test 2
                    handler = new XHandler(playerNum + 1);

                    if (handler.Connect())
                    {
                        Debug.WriteLine("XInput Handle Connected!");
                        handler.StartUpdate();
                        handler.Reset();
                        handler.EndUpdate();
                    }
                    else
                    {
                        Debug.WriteLine("Failed to connect handler");
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

            #region IPC
            if (Main.useIPC)
            {
                XControllers newState = new XControllers();

                newState.SetValueOR(assignments["A"], args.ProController.A);
                newState.SetValueOR(assignments["B"], args.ProController.B);
                newState.SetValueOR(assignments["X"], args.ProController.X);
                newState.SetValueOR(assignments["Y"], args.ProController.Y);
                newState.SetValueOR(assignments["L"], args.ProController.L);
                newState.SetValueOR(assignments["R"], args.ProController.R);
                newState.SetValueOR(assignments["ZL"], args.ProController.ZL);
                newState.SetValueOR(assignments["ZR"], args.ProController.ZR);
                newState.SetValueOR(assignments["LS"], args.ProController.LS);
                newState.SetValueOR(assignments["RS"], args.ProController.RS);
                newState.SetValueOR(assignments["Up"], args.ProController.Up);
                newState.SetValueOR(assignments["Down"], args.ProController.Down);
                newState.SetValueOR(assignments["Left"], args.ProController.Left);
                newState.SetValueOR(assignments["Right"], args.ProController.Right);
                newState.SetValueOR(assignments["Start"], args.ProController.Start);
                newState.SetValueOR(assignments["Select"], args.ProController.Select);
                newState.SetValueOR(assignments["Home"], args.ProController.Home);

                newState.LX = (short)((args.ProController.LeftJoy.X > 1 ? 1 : (args.ProController.LeftJoy.X < -1 ? -1 : args.ProController.LeftJoy.X)) * (float)xValue.Axis);
                newState.LY = (short)((args.ProController.LeftJoy.Y > 1 ? 1 : (args.ProController.LeftJoy.Y < -1 ? -1 : args.ProController.LeftJoy.Y)) * (float)xValue.Axis);
                newState.RX = (short)((args.ProController.RightJoy.X > 1 ? 1 : (args.ProController.RightJoy.X < -1 ? -1 : args.ProController.RightJoy.X)) * (float)xValue.Axis);
                newState.RY = (short)((args.ProController.RightJoy.Y > 1 ? 1 : (args.ProController.RightJoy.Y < -1 ? -1 : args.ProController.RightJoy.Y)) * (float)xValue.Axis);

                newState.rumble = Main.xDevices[playerNum].rumble;

                Main.xDevices[playerNum] = newState;

                if (Main.serverInterface != null)
                    Main.serverInterface.SetState(playerNum, newState);

                if (Main.xDevices[playerNum].rumble != args.ProController.Rumble)
                    controller.SetRumble(Main.xDevices[playerNum].rumble);
            }
            #endregion

            #region SCP_Driver
            /*if (report != null)
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
            }*/

            // Test 2
            handler.StartUpdate(); // does nothing really

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

            handler.EndUpdate();
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
            playerNum = num;
            cPlayerNum.Text = (playerNum + 1).ToString();
            controller.SetPlayerLED(playerNum + 1);

            // TODO: change handle device
        }

        #region SCP_Driver
        public bool UpdateXInput(XReport r)
        {
            Byte[] input = r.ToBytes();
            Byte[] rumble = new Byte[8];
            Byte[] report = new Byte[28];

            bus.Parse(input, report);

            if (bus.Report(input, rumble))
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

            return false;
        }

        public bool ConnectXInput()
        {
            return bus.Plugin(playerNum);
        }

        public bool RemoveXInput()
        {
            return handler.Disconnect();

            bus.Stop();
            bus.Close();
            return bus.Unplug(playerNum);
        }
        #endregion
    }
}
