using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib.New
{
    public struct Wiimote : INintrollerState
    {
        public CoreButtons buttons;
        public Accelerometer accelerometer;
        public IR irSensor;
        //INintrollerState extension;

        public Wiimote(byte[] rawData)
        {
            buttons = new CoreButtons();
            accelerometer = new Accelerometer();
            irSensor = new IR();
            //extension = null;

            buttons.Parse(rawData, 1);

            //InputReport reportType = (InputReport)rawData[0];
            accelerometer.Parse(rawData, 3);
            irSensor.Parse(rawData, 3);
        }

        public void Update(byte[] data)
        {
            buttons.Parse(data, 1);
            accelerometer.Parse(data, 3);
            irSensor.Parse(data, 3);

            accelerometer.Normalize();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    accelerometer.Calibrate(Calibrations.Defaults.WiimoteDefault.accelerometer);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            System.Diagnostics.Debug.WriteLine(from.GetType());
        }
    }

    public struct Nunchuk : INintrollerState
    {
        public Wiimote wiimote;
        public Accelerometer accelerometer;
        public Joystick joystick;
        public bool C, Z;

        public Nunchuk(byte[] rawData)
        {
            wiimote = new Wiimote(rawData);
            accelerometer = new Accelerometer();
            joystick = new Joystick();

            C = Z = false;
            // TODO: New: Parse
        }

        public void Update(byte[] data)
        {
            int offset = 0;
            switch((InputReport)data[0])
            {
                case InputReport.BtnsExt:
                case InputReport.BtnsExtB:
                    offset = 3;
                    break;
                case InputReport.BtnsAccExt:
                    offset = 6;
                    break;
                case InputReport.BtnsIRExt:
                    offset = 13;
                    break;
                case InputReport.BtnsAccIRExt:
                    offset = 16;
                    break;
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                default:
                    return;
            }

            // Buttons
            C = (data[offset + 5] & 0x02) == 0;
            Z = (data[offset + 5] & 0x01) == 0;

            // Joystick
            joystick.rawX = data[offset];
            joystick.rawY = data[offset + 1];

            // Accelerometer
            accelerometer.Parse(data, offset + 2);

            // Normalize
            joystick.Normalize();
            accelerometer.Normalize();

            wiimote.Update(data);
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    joystick.Calibrate(Calibrations.Defaults.NunchukDefault.joystick);
                    accelerometer.Calibrate(Calibrations.Defaults.NunchukDefault.accelerometer);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            System.Diagnostics.Debug.WriteLine(from.GetType());
        }
}

    public struct ClassicController : INintrollerState
    {
        public Wiimote wiimote;
        public Joystick LJoy, RJoy;
        public Trigger L, R;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool ZL, ZR, LFull, RFull;
        public bool Plus, Minus, Home;

        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        public void Update(byte[] data)
        {
            int offset = 0;
            switch ((InputReport)data[0])
            {
                case InputReport.BtnsExt:
                case InputReport.BtnsExtB:
                    offset = 3;
                    break;
                case InputReport.BtnsAccExt:
                    offset = 6;
                    break;
                case InputReport.BtnsIRExt:
                    offset = 13;
                    break;
                case InputReport.BtnsAccIRExt:
                    offset = 16;
                    break;
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                default:
                    return;
            }

            // Buttons
            A     = (data[offset + 5] & 0x10) == 0;
            B     = (data[offset + 5] & 0x40) == 0;
            X     = (data[offset + 5] & 0x08) == 0;
            Y     = (data[offset + 5] & 0x20) == 0;
            LFull = (data[offset + 4] & 0x20) == 0;  // Until the Click
            RFull = (data[offset + 4] & 0x02) == 0;  // Until the Click
            ZL    = (data[offset + 5] & 0x80) == 0;
            ZR    = (data[offset + 5] & 0x04) == 0;
            Plus  = (data[offset + 4] & 0x04) == 0;
            Minus = (data[offset + 4] & 0x10) == 0;
            Home  = (data[offset + 4] & 0x08) == 0;

            // Dpad
            Up    = (data[offset + 5] & 0x01) == 0;
            Down  = (data[offset + 4] & 0x40) == 0;
            Left  = (data[offset + 5] & 0x02) == 0;
            Right = (data[offset + 4] & 0x80) == 0;

            // Joysticks
            LJoy.rawX = (byte)(data[offset    ] & 0x3F);
            LJoy.rawY = (byte)(data[offset + 1] & 0x03F);
            RJoy.rawX = (byte)(data[offset + 2] >> 7 | (data[offset + 1] & 0xC0) >> 5 | (data[offset] & 0xC0) >> 3);
            RJoy.rawY = (byte)(data[offset + 2] & 0x1F);

            // Triggers
            L.rawValue = (byte)(((data[offset + 2] & 0x60) >> 2) | (data[offset + 3] >> 5));
            R.rawValue = (byte)(data[offset + 3] & 0x1F);

            // Normalize
            LJoy.Normalize();
            RJoy.Normalize();
            L.Normalize();
            R.Normalize();

            wiimote.Update(data);
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    LJoy.Calibrate(Calibrations.Defaults.ClassicControllerDefault.LJoy);
                    RJoy.Calibrate(Calibrations.Defaults.ClassicControllerDefault.RJoy);
                    L.Calibrate(Calibrations.Defaults.ClassicControllerDefault.L);
                    R.Calibrate(Calibrations.Defaults.ClassicControllerDefault.R);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            System.Diagnostics.Debug.WriteLine(from.GetType());
        }
    }

    public struct ClassicControllerPro : INintrollerState
    {
        public Wiimote wiimote;
        public Joystick LJoy, RJoy;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool L, R, ZL, ZR;
        public bool Plus, Minus, Home;

        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        public void Update(byte[] data)
        {
            int offset = 0;
            switch ((InputReport)data[0])
            {
                case InputReport.BtnsExt:
                case InputReport.BtnsExtB:
                    offset = 3;
                    break;
                case InputReport.BtnsAccExt:
                    offset = 6;
                    break;
                case InputReport.BtnsIRExt:
                    offset = 13;
                    break;
                case InputReport.BtnsAccIRExt:
                    offset = 16;
                    break;
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                default:
                    return;
            }

            // Buttons
            A     = (data[offset + 5] & 0x10) == 0;
            B     = (data[offset + 5] & 0x40) == 0;
            X     = (data[offset + 5] & 0x08) == 0;
            Y     = (data[offset + 5] & 0x20) == 0;
            L     = (data[offset + 4] & 0x20) == 0;
            R     = (data[offset + 4] & 0x02) == 0;
            ZL    = (data[offset + 5] & 0x80) == 0;
            ZR    = (data[offset + 5] & 0x04) == 0;
            Plus  = (data[offset + 4] & 0x04) == 0;
            Minus = (data[offset + 4] & 0x10) == 0;
            Home  = (data[offset + 4] & 0x08) == 0;

            // Dpad
            Up    = (data[offset + 5] & 0x01) == 0;
            Down  = (data[offset + 4] & 0x40) == 0;
            Left  = (data[offset + 5] & 0x02) == 0;
            Right = (data[offset + 4] & 0x80) == 0;

            // Joysticks
            LJoy.rawX = (byte)(data[offset] & 0x3F);
            LJoy.rawY = (byte)(data[offset + 1] & 0x03F);
            RJoy.rawX = (byte)(data[offset + 2] >> 7 | (data[offset + 1] & 0xC0) >> 5 | (data[offset] & 0xC0) >> 3);
            RJoy.rawY = (byte)(data[offset + 2] & 0x1F);

            // Normalize
            LJoy.Normalize();
            RJoy.Normalize();

            wiimote.Update(data);
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    LJoy.Calibrate(Calibrations.Defaults.ClassicControllerProDefault.LJoy);
                    RJoy.Calibrate(Calibrations.Defaults.ClassicControllerProDefault.RJoy);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            System.Diagnostics.Debug.WriteLine(from.GetType());
        }
    }

    public struct ProController : INintrollerState
    {
        public Joystick LJoy, RJoy;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool L, R, ZL, ZR;
        public bool Plus, Minus, Home;
        public bool LStick, RStick;
        public bool charging, usbConnected;

        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        public void Update(byte[] data)
        {
            int offset = 0;

            switch ((InputReport)data[0])
            {
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                case InputReport.BtnsExt:
                case InputReport.BtnsExtB:
                    offset = 3;
                    break;
                case InputReport.BtnsAccExt:
                    offset = 6;
                    break;
                case InputReport.BtnsIRExt:
                    offset = 13;
                    break;
                case InputReport.BtnsAccIRExt:
                    offset = 16;
                    break;
                default:
                    break;
            }

            // Buttons
            A      = (data[offset +  9] & 0x10) == 0;
            B      = (data[offset +  9] & 0x40) == 0;
            X      = (data[offset +  9] & 0x08) == 0;
            Y      = (data[offset +  9] & 0x20) == 0;
            L      = (data[offset +  8] & 0x20) == 0;
            R      = (data[offset +  8] & 0x02) == 0;
            ZL     = (data[offset +  9] & 0x80) == 0;
            ZR     = (data[offset +  9] & 0x04) == 0;
            Plus   = (data[offset +  8] & 0x04) == 0;
            Minus  = (data[offset +  8] & 0x10) == 0;
            Home   = (data[offset +  8] & 0x08) == 0;
            LStick = (data[offset + 10] & 0x02) == 0;
            RStick = (data[offset + 10] & 0x01) == 0;

            // DPad
            Up    = (data[offset + 9] & 0x01) == 0;
            Down  = (data[offset + 8] & 0x40) == 0;
            Left  = (data[offset + 9] & 0x02) == 0;
            Right = (data[offset + 8] & 0x80) == 0;

            // Joysticks
            LJoy.rawX = BitConverter.ToInt16(data, offset);
            LJoy.rawY = BitConverter.ToInt16(data, offset + 4);
            RJoy.rawX = BitConverter.ToInt16(data, offset + 2);
            RJoy.rawY = BitConverter.ToInt16(data, offset + 6);

            // Other
            charging     = (data[offset + 10] & 0x04) == 0;
            usbConnected = (data[offset + 10] & 0x08) == 0;

            // Normalize
            LJoy.Normalize();
            RJoy.Normalize();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    LJoy.Calibrate(Calibrations.Defaults.ProControllerDefault.LJoy);
                    RJoy.Calibrate(Calibrations.Defaults.ProControllerDefault.RJoy);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            System.Diagnostics.Debug.WriteLine(from.GetType());
        }
    }

    public struct BalanceBoard : INintrollerState
    {

        public void Update(byte[] data)
        {
            throw new NotImplementedException();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            System.Diagnostics.Debug.WriteLine(from.GetType());
        }
    }

    public struct WiimotePlus : INintrollerState
    {
        Wiimote wiimote;
        //gyro

        public void Update(byte[] data)
        {
            throw new NotImplementedException();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            System.Diagnostics.Debug.WriteLine(from.GetType());
        }
    }
}
