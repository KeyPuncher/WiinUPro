using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NintrollerLib;

namespace Nintroller_Test_App
{
    public partial class Form1 : Form
    {
        private Nintroller N;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> deviceList = Nintroller.FindControllers();

            if (deviceList.Count > 0)
            {
                N = new Nintroller(deviceList[0]);

                if (N.ConnectTest())
                {
                    N.StateChange += N_StateChange;
                    N.ExtensionChange += N_ExtensionChange;
                    N.Connect();
                }
            }
        }

        #region State reading
        void N_StateChange(object sender, NintrollerLib.StateChangeEventArgs e)
        {
            #region Code Tests
            // old
            /*if (e.DeviceState.GetType() == typeof(ProControllerState))
            {
                ProControllerState Pro = (ProControllerState)e.DeviceState;
                Console.WriteLine(Pro.LeftJoyRaw.ToString());
            }*/
            /*
            Console.WriteLine(e.ProController.LeftJoyRaw.ToString());

            if (e.DeviceState.GetType() == typeof(WiimoteState))
            {
                if (e.Wiimote.Extension != null)
                    Console.WriteLine(e.Wiimote.Extension.GetType());
                else
                    Console.WriteLine("no extension");
            }*/

            /*e.DeviceState.Battery;
            e.DeviceState.BatteryLevel;
            e.DeviceState.BatteryRaw;
            e.DeviceState.GetRumble();
            e.DeviceState.hasLEDs;
            e.DeviceState.hasRumble;
            e.DeviceState.LED1;
            e.DeviceState.LED2;
            e.DeviceState.LED3;
            e.DeviceState.LED4;
            e.DeviceState.lowBattery;
            e.DeviceState.ParseReport();
            e.DeviceState.ResetCalibration();
            e.DeviceState.SetRumble(true);*/
            #endregion

            Update(e);
        }

        public void Update(NintrollerLib.StateChangeEventArgs e)
        {
            BeginInvoke(new UpdateFormDelegate(UpdateForm), e);
        }

        private delegate void UpdateFormDelegate(NintrollerLib.StateChangeEventArgs args);

        void UpdateForm (NintrollerLib.StateChangeEventArgs e)
        {
            // Common
            labelBattery.Text = e.DeviceState.BatteryRaw.ToString();
            labelBatteryLow.Checked = e.DeviceState.BatteryLow;

            #region Wiimote
            // Buttons
            wiimoteButtonList.SetItemChecked(0, e.Wiimote.A);
            wiimoteButtonList.SetItemChecked(1, e.Wiimote.B);
            wiimoteButtonList.SetItemChecked(2, e.Wiimote.One);
            wiimoteButtonList.SetItemChecked(3, e.Wiimote.Two);
            wiimoteButtonList.SetItemChecked(4, e.Wiimote.Plus);
            wiimoteButtonList.SetItemChecked(5, e.Wiimote.Minus);
            wiimoteButtonList.SetItemChecked(6, e.Wiimote.Home);
            wiimoteButtonList.SetItemChecked(7, e.Wiimote.Up);
            wiimoteButtonList.SetItemChecked(8, e.Wiimote.Down);
            wiimoteButtonList.SetItemChecked(9, e.Wiimote.Left);
            wiimoteButtonList.SetItemChecked(10, e.Wiimote.Right);
            // Accelerometer
            if (checkRawValues.Checked)
            {
                wiimoteLabelAccX.Text = e.Wiimote.AccRaw.X.ToString();
                wiimoteLabelAccY.Text = e.Wiimote.AccRaw.Y.ToString();
                wiimoteLabelAccZ.Text = e.Wiimote.AccRaw.Z.ToString();
            }
            else
            {
                wiimoteLabelAccX.Text = e.Wiimote.Acc.X.ToString();
                wiimoteLabelAccY.Text = e.Wiimote.Acc.Y.ToString();
                wiimoteLabelAccZ.Text = e.Wiimote.Acc.Z.ToString();
            }
            // IR
            if (checkRawValues.Checked)
            {
                wiimoteLabelIR1X.Text = e.Wiimote.IR1.RawX.ToString();
                wiimoteLabelIR1X.Text = e.Wiimote.IR1.RawY.ToString();
                wiimoteLabelIR2X.Text = e.Wiimote.IR2.RawX.ToString();
                wiimoteLabelIR2Y.Text = e.Wiimote.IR2.RawY.ToString();
                wiimoteLabelIR3X.Text = e.Wiimote.IR3.RawX.ToString();
                wiimoteLabelIR3Y.Text = e.Wiimote.IR3.RawY.ToString();
                wiimoteLabelIR4X.Text = e.Wiimote.IR4.RawX.ToString();
                wiimoteLabelIR4Y.Text = e.Wiimote.IR4.RawY.ToString();
            }
            else
            {
                wiimoteLabelIR1X.Text = e.Wiimote.IR1.X.ToString();
                wiimoteLabelIR1Y.Text = e.Wiimote.IR1.Y.ToString();
                wiimoteLabelIR2X.Text = e.Wiimote.IR2.X.ToString();
                wiimoteLabelIR2Y.Text = e.Wiimote.IR2.Y.ToString();
                wiimoteLabelIR3X.Text = e.Wiimote.IR3.X.ToString();
                wiimoteLabelIR3Y.Text = e.Wiimote.IR3.Y.ToString();
                wiimoteLabelIR4X.Text = e.Wiimote.IR4.X.ToString();
                wiimoteLabelIR4Y.Text = e.Wiimote.IR4.Y.ToString();
            }
            wiimoteLabelIRSizes.Text = string.Format("{0}, {1}, {2}, {3}",
                                        e.Wiimote.IR1.Size, e.Wiimote.IR2.Size,
                                        e.Wiimote.IR3.Size, e.Wiimote.IR4.Size);
            #endregion

            #region Motion Plus
            if (checkRawValues.Checked)
            {
                motionLabelGyroX.Text = e.Wiimote.MotionPlus.GyroRaw.X.ToString();
                motionLabelGyroY.Text = e.Wiimote.MotionPlus.GyroRaw.Y.ToString();
                motionLabelGyroZ.Text = e.Wiimote.MotionPlus.GyroRaw.Z.ToString();
            }
            else
            {
                motionLabelGyroX.Text = e.Wiimote.MotionPlus.Gyro.X.ToString();
                motionLabelGyroY.Text = e.Wiimote.MotionPlus.Gyro.Y.ToString();
                motionLabelGyroZ.Text = e.Wiimote.MotionPlus.Gyro.Z.ToString();
            }
            #endregion

            #region Pro Controller
            // Buttons
            proButtonsList.SetItemChecked(0, e.ProController.A);
            proButtonsList.SetItemChecked(1, e.ProController.B);
            proButtonsList.SetItemChecked(2, e.ProController.X);
            proButtonsList.SetItemChecked(3, e.ProController.Y);
            proButtonsList.SetItemChecked(4, e.ProController.L);
            proButtonsList.SetItemChecked(5, e.ProController.R);
            proButtonsList.SetItemChecked(6, e.ProController.ZL);
            proButtonsList.SetItemChecked(7, e.ProController.ZR);
            proButtonsList.SetItemChecked(8, e.ProController.Start);
            proButtonsList.SetItemChecked(9, e.ProController.Select);
            proButtonsList.SetItemChecked(10, e.ProController.Home);
            proButtonsList.SetItemChecked(11, e.ProController.Up);
            proButtonsList.SetItemChecked(12, e.ProController.Down);
            proButtonsList.SetItemChecked(13, e.ProController.Left);
            proButtonsList.SetItemChecked(14, e.ProController.Right);
            proButtonsList.SetItemChecked(15, e.ProController.LS);
            proButtonsList.SetItemChecked(16, e.ProController.RS);
            // Joysticks
            if (checkRawValues.Checked)
            {
                proLabelJoyLX.Text = e.ProController.LeftJoyRaw.X.ToString();
                proLabelJoyLY.Text = e.ProController.LeftJoyRaw.Y.ToString();
                proLabelJoyRX.Text = e.ProController.RightJoyRaw.X.ToString();
                proLabelJoyRY.Text = e.ProController.RightJoyRaw.Y.ToString();
            }
            else
            {
                proLabelJoyLX.Text = e.ProController.LeftJoy.X.ToString();
                proLabelJoyLY.Text = e.ProController.LeftJoy.Y.ToString();
                proLabelJoyRX.Text = e.ProController.RightJoy.X.ToString();
                proLabelJoyRY.Text = e.ProController.RightJoy.Y.ToString();
            }
            #endregion

            #region Classic Controller
            // Buttons
            classicButtonsList.SetItemChecked(0, e.Wiimote.ClassicController.A);
            classicButtonsList.SetItemChecked(1, e.Wiimote.ClassicController.B);
            classicButtonsList.SetItemChecked(2, e.Wiimote.ClassicController.X);
            classicButtonsList.SetItemChecked(3, e.Wiimote.ClassicController.Y);
            classicButtonsList.SetItemChecked(4, e.Wiimote.ClassicController.L);
            classicButtonsList.SetItemChecked(5, e.Wiimote.ClassicController.LFull);
            classicButtonsList.SetItemChecked(6, e.Wiimote.ClassicController.R);
            classicButtonsList.SetItemChecked(7, e.Wiimote.ClassicController.RFull);
            classicButtonsList.SetItemChecked(8, e.Wiimote.ClassicController.ZL);
            classicButtonsList.SetItemChecked(9, e.Wiimote.ClassicController.ZR);
            classicButtonsList.SetItemChecked(10, e.Wiimote.ClassicController.Start);
            classicButtonsList.SetItemChecked(11, e.Wiimote.ClassicController.Select);
            classicButtonsList.SetItemChecked(12, e.Wiimote.ClassicController.Home);
            classicButtonsList.SetItemChecked(13, e.Wiimote.ClassicController.Up);
            classicButtonsList.SetItemChecked(14, e.Wiimote.ClassicController.Down);
            classicButtonsList.SetItemChecked(15, e.Wiimote.ClassicController.Left);
            classicButtonsList.SetItemChecked(16, e.Wiimote.ClassicController.Right);

            // Joysticks
            if (checkRawValues.Checked)
            {
                classicLabelJoyLX.Text = e.Wiimote.ClassicController.LeftJoyRaw.X.ToString();
                classicLabelJoyLY.Text = e.Wiimote.ClassicController.LeftJoyRaw.Y.ToString();
                classicLabelJoyRX.Text = e.Wiimote.ClassicController.RightJoyRaw.X.ToString();
                classicLabelJoyRY.Text = e.Wiimote.ClassicController.RightJoyRaw.Y.ToString();
            }
            else
            {
                classicLabelJoyLX.Text = e.Wiimote.ClassicController.LeftJoy.X.ToString();
                classicLabelJoyLY.Text = e.Wiimote.ClassicController.LeftJoy.Y.ToString();
                classicLabelJoyRX.Text = e.Wiimote.ClassicController.RightJoy.X.ToString();
                classicLabelJoyRY.Text = e.Wiimote.ClassicController.RightJoy.Y.ToString();
            }

            // Triggers
            if (checkRawValues.Checked)
            {
                classicLabelTriggerL.Text = e.Wiimote.ClassicController.LTriggerRaw.ToString();
                classicLabelTriggerR.Text = e.Wiimote.ClassicController.RTriggerRaw.ToString();
            }
            else
            {
                classicLabelTriggerL.Text = e.Wiimote.ClassicController.LTrigger.ToString();
                classicLabelTriggerR.Text = e.Wiimote.ClassicController.RTrigger.ToString();
            }
            #endregion

            #region Classic Controller Pro
            // Buttons
            classicProButtonsList.SetItemChecked(0, e.Wiimote.ClassicControllerPro.A);
            classicProButtonsList.SetItemChecked(1, e.Wiimote.ClassicControllerPro.B);
            classicProButtonsList.SetItemChecked(2, e.Wiimote.ClassicControllerPro.X);
            classicProButtonsList.SetItemChecked(3, e.Wiimote.ClassicControllerPro.Y);
            classicProButtonsList.SetItemChecked(4, e.Wiimote.ClassicControllerPro.L);
            classicProButtonsList.SetItemChecked(5, e.Wiimote.ClassicControllerPro.R);
            classicProButtonsList.SetItemChecked(6, e.Wiimote.ClassicControllerPro.ZL);
            classicProButtonsList.SetItemChecked(7, e.Wiimote.ClassicControllerPro.ZR);
            classicProButtonsList.SetItemChecked(8, e.Wiimote.ClassicControllerPro.Start);
            classicProButtonsList.SetItemChecked(9, e.Wiimote.ClassicControllerPro.Select);
            classicProButtonsList.SetItemChecked(10, e.Wiimote.ClassicControllerPro.Home);
            classicProButtonsList.SetItemChecked(11, e.Wiimote.ClassicControllerPro.Up);
            classicProButtonsList.SetItemChecked(12, e.Wiimote.ClassicControllerPro.Down);
            classicProButtonsList.SetItemChecked(13, e.Wiimote.ClassicControllerPro.Left);
            classicProButtonsList.SetItemChecked(14, e.Wiimote.ClassicControllerPro.Right);

            // Joysticks
            if (checkRawValues.Checked)
            {
                classicProLabelJoyLX.Text = e.Wiimote.ClassicControllerPro.LeftJoyRaw.X.ToString();
                classicProLabelJoyLY.Text = e.Wiimote.ClassicControllerPro.LeftJoyRaw.Y.ToString();
                classicProLabelJoyRX.Text = e.Wiimote.ClassicControllerPro.RightJoyRaw.X.ToString();
                classicProLabelJoyRY.Text = e.Wiimote.ClassicControllerPro.RightJoyRaw.Y.ToString();
            }
            else
            {
                classicProLabelJoyLX.Text = e.Wiimote.ClassicControllerPro.LeftJoy.X.ToString();
                classicProLabelJoyLY.Text = e.Wiimote.ClassicControllerPro.LeftJoy.Y.ToString();
                classicProLabelJoyRX.Text = e.Wiimote.ClassicControllerPro.RightJoy.X.ToString();
                classicProLabelJoyRY.Text = e.Wiimote.ClassicControllerPro.RightJoy.Y.ToString();
            }
            #endregion

            #region Nunchuck
            // Buttons
            nunchuckButtonsList.SetItemChecked(0, e.Wiimote.Nunchuck.C);
            nunchuckButtonsList.SetItemChecked(1, e.Wiimote.Nunchuck.Z);
            // Joystick
            if (checkRawValues.Checked)
            {
                nunchuckLabelJoyX.Text = e.Wiimote.Nunchuck.JoyRaw.X.ToString();
                nunchuckLabelJoyY.Text = e.Wiimote.Nunchuck.JoyRaw.Y.ToString();
            }
            else
            {
                nunchuckLabelJoyX.Text = e.Wiimote.Nunchuck.Joy.X.ToString();
                nunchuckLabelJoyY.Text = e.Wiimote.Nunchuck.Joy.Y.ToString();
            }
            // Accelerometer
            if (checkRawValues.Checked)
            {
                nunchuckLabelAccX.Text = e.Wiimote.Nunchuck.AccRaw.X.ToString();
                nunchuckLabelAccY.Text = e.Wiimote.Nunchuck.AccRaw.Y.ToString();
                nunchuckLabelAccZ.Text = e.Wiimote.Nunchuck.AccRaw.Z.ToString();
            }
            else {
                nunchuckLabelAccX.Text = e.Wiimote.Nunchuck.Acc.X.ToString();
                nunchuckLabelAccY.Text = e.Wiimote.Nunchuck.Acc.Y.ToString();
                nunchuckLabelAccZ.Text = e.Wiimote.Nunchuck.Acc.Z.ToString();
            }
            #endregion

            #region Balance Board
            if (checkRawValues.Checked)
            {
                balanceLabelTopLeft.Text = e.BalanceBoard.Raw.TopLeft.ToString();
                balanceLabelTopRight.Text = e.BalanceBoard.Raw.TopRight.ToString();
                balanceLabelBottomLeft.Text = e.BalanceBoard.Raw.BottomLeft.ToString();
                balanceLabelBottomRight.Text = e.BalanceBoard.Raw.BottomRight.ToString();
            }
            else
            {
                balanceLabelTopLeft.Text = e.BalanceBoard.Sensor.TopLeft.ToString();
                balanceLabelTopRight.Text = e.BalanceBoard.Sensor.TopRight.ToString();
                balanceLabelBottomLeft.Text = e.BalanceBoard.Sensor.BottomLeft.ToString();
                balanceLabelBottomRight.Text = e.BalanceBoard.Sensor.BottomRight.ToString();
            }
            #endregion
        }
        #endregion

        #region Extension Reading
        private delegate void ChangedExtensionDelegate(ExtensionChangeEventArgs args);

        void N_ExtensionChange(object sender, ExtensionChangeEventArgs e)
        { BeginInvoke(new ChangedExtensionDelegate(ChangedExtension), e); }

        void ChangedExtension(ExtensionChangeEventArgs args)
        {
            // If e.ControllerID = N.ID
            ExtensionLabel.Text = args.Extension.ToString();
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (N != null)
                N.Disconnect();
        }

        private void motionButtonEnable_Click(object sender, EventArgs e)
        {
            N.StartMotionPlus();
            N.ChangeReport(InputReport.BtnsAccExt);
        }

        private void commonReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (commonReport.SelectedIndex)
            {
                case 0:
                    N.ChangeReport(InputReport.BtnsOnly);
                    break;
                case 1:
                    N.ChangeReport(InputReport.BtnsAcc);
                    break;
                case 2:
                    N.ChangeReport(InputReport.BtnsExt);
                    break;
                case 3:
                    N.ChangeReport(InputReport.BtnsAccIR);
                    break;
                case 4:
                    N.ChangeReport(InputReport.BtnsExtB);
                    break;
                case 5:
                    N.ChangeReport(InputReport.BtnsAccExt);
                    break;
                case 6:
                    N.ChangeReport(InputReport.BtnsIRExt);
                    break;
                case 7:
                    N.ChangeReport(InputReport.BtnsAccIRExt);
                    break;
                case 8:
                    N.ChangeReport(InputReport.ExtOnly);
                    break;
                default:
                    Console.WriteLine(commonReport.SelectedIndex);
                    break;
            }
        }

        private void checkLED_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                CheckBox cb = (CheckBox)sender;

                if (cb.Text == "LED 1")
                    N.SetLEDs(cb.Checked, N.State.LED2, N.State.LED3, N.State.LED4);
                else if (cb.Text == "LED 2")
                    N.SetLEDs(N.State.LED1, cb.Checked, N.State.LED3, N.State.LED4);
                else if (cb.Text == "LED 3")
                    N.SetLEDs(N.State.LED1, N.State.LED2, cb.Checked, N.State.LED4);
                else if (cb.Text == "LED 4")
                    N.SetLEDs(N.State.LED1, N.State.LED2, N.State.LED3, cb.Checked);
            }
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            if (N != null)
                N.RefreshStatus();
        }

    }
}
