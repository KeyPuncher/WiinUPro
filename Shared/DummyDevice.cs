using System;
using System.Collections.Generic;
using System.IO;
using NintrollerLib;

namespace Shared
{
    public class DummyDevice : Stream
    {
        #region Properties
        public ControllerType DeviceType { get; private set; }

        public INintrollerState State { get; set; }

        public InputReport NextReport { get; set; } = InputReport.BtnsOnly;

        public byte RumbleByte { get; set; }

        public byte BatteryLevel { get; set; } = 0xFF;

        public bool BatteryLow { get; set; }

        public bool LED_1 { get; set; }
        public bool LED_2 { get; set; }
        public bool LED_3 { get; set; }
        public bool LED_4 { get; set; }
        #endregion

        protected byte[] _lastReport;
        protected InputReport DataReportMode = InputReport.BtnsOnly;
        protected Queue<InputReport> _nextQueue;
        protected Queue<byte[]> _reportQueue;

        public DummyDevice(INintrollerState state)
        {
            _nextQueue = new Queue<InputReport>();
            _reportQueue = new Queue<byte[]>();
            
            if (state is ProController)
            {
                DeviceType = ControllerType.ProController;
                ConfigureProController((ProController)state);
            }
            else if (state is Wiimote)
            {
                DeviceType = ControllerType.Wiimote;
                ConfigureWiimote((Wiimote)state);
            }
            else if (state is GameCubeAdapter)
            {
                DeviceType = ControllerType.Other;
                ConfigureGameCubeAdapter((GameCubeAdapter)state);
            }
            else
            {
                State = state;
            }
        }

        public void ChangeExtension(ControllerType type)
        {
            if (DeviceType == type)
                return;

            switch (type)
            {
                case ControllerType.Wiimote:
                    if (State is IWiimoteExtension)
                    {
                        ConfigureWiimote(((IWiimoteExtension)State).wiimote);
                    }
                    else
                    {
                        ConfigureWiimote(new Wiimote());
                    }
                    break;

                case ControllerType.ClassicControllerPro:
                    var ccpState = new ClassicControllerPro();
                    ccpState.SetCalibration(Calibrations.CalibrationPreset.Default);
                    ConfigureClassicControllerProController(ccpState);
                    break;

                default:
                    // Invalid
                    return;
            }

            DeviceType = type;
            _nextQueue.Enqueue(InputReport.Status);
        }

        #region State Configs
        public void ConfigureProController(ProController pState)
        {
            pState.LJoy.rawX = (short)pState.LJoy.centerX;
            pState.LJoy.rawY = (short)pState.LJoy.centerY;
            pState.RJoy.rawX = (short)pState.RJoy.centerX;
            pState.RJoy.rawY = (short)pState.RJoy.centerY;
            State = pState;
        }

        public void ConfigureWiimote(Wiimote wState)
        {
            wState.accelerometer.rawX = (short)wState.accelerometer.centerX;
            wState.accelerometer.rawY = (short)wState.accelerometer.centerY;
            wState.accelerometer.rawZ = (short)wState.accelerometer.centerZ;
            State = wState;
        }

        public void ConfigureClassicControllerProController(ClassicControllerPro ccpState)
        {
            ccpState.LJoy.rawX = (short)ccpState.LJoy.centerX;
            ccpState.LJoy.rawY = (short)ccpState.LJoy.centerY;
            ccpState.RJoy.rawX = (short)ccpState.RJoy.centerX;
            ccpState.RJoy.rawY = (short)ccpState.RJoy.centerY;
            State = ccpState;
        }

        public void ConfigureGameCubeAdapter(GameCubeAdapter gState)
        {
            gState.port1.joystick.rawX = gState.port1.joystick.centerX;
            gState.port1.joystick.rawY = gState.port1.joystick.centerY;
            gState.port1.cStick.rawX = gState.port1.cStick.centerX;
            gState.port1.cStick.rawY = gState.port1.cStick.centerY;
            gState.port2.joystick.rawX = gState.port2.joystick.centerX;
            gState.port2.joystick.rawY = gState.port2.joystick.centerY;
            gState.port2.cStick.rawX = gState.port2.cStick.centerX;
            gState.port2.cStick.rawY = gState.port2.cStick.centerY;
            gState.port3.joystick.rawX = gState.port3.joystick.centerX;
            gState.port3.joystick.rawY = gState.port3.joystick.centerY;
            gState.port3.cStick.rawX = gState.port3.cStick.centerX;
            gState.port3.cStick.rawY = gState.port3.cStick.centerY;
            gState.port4.joystick.rawX = gState.port4.joystick.centerX;
            gState.port4.joystick.rawY = gState.port4.joystick.centerY;
            gState.port4.cStick.rawX = gState.port4.cStick.centerX;
            gState.port4.cStick.rawY = gState.port4.cStick.centerY;
            State = gState;
        }
        #endregion

        #region System.IO.Stream Implimentation
        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
            
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (DeviceType == ControllerType.Other)
            {
                GameCubeAdapter gcn = (GameCubeAdapter)State;

                Array.Copy(GetGCNController(gcn.port1), 0, buffer, 1, 9);
                Array.Copy(GetGCNController(gcn.port2), 0, buffer, 10, 9);
                Array.Copy(GetGCNController(gcn.port3), 0, buffer, 19, 9);
                Array.Copy(GetGCNController(gcn.port4), 0, buffer, 28, 9);

                buffer[1] |= (byte)(gcn.port1Connected ? 0x10 : 0x00);
                buffer[10] |= (byte)(gcn.port2Connected ? 0x10 : 0x00);
                buffer[19] |= (byte)(gcn.port3Connected ? 0x10 : 0x00);
                buffer[28] |= (byte)(gcn.port4Connected ? 0x10 : 0x00);

                return buffer.Length;
            }

            int value = -1;

            // This won't block since Read is call asynchronously
            while (_reportQueue.Count == 0 && (_nextQueue.Count != 0 && (byte)_nextQueue.Peek() < 0x30)) ;

            // Set Error
            buffer[4] = 0x00;

            // Assuming we are sending these bytes (# read)
            value = buffer.Length;

            byte[] coreBtns = GetCoreButtons();

            if (_nextQueue.Count > 0)
                NextReport = _nextQueue.Dequeue();
            else
                NextReport = DataReportMode;

            if (_reportQueue.Count > 0)
                _lastReport = _reportQueue.Dequeue();
            else
                _lastReport = null;

            switch (NextReport)
            {
                case InputReport.Status: // 20 BB BB LF 00 00 VV
                    // 20 - Status type
                    buffer[0] = (byte)InputReport.Status;

                    // BB BB - Core Buttons
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];

                    // F - Flags: Battery Low, Extension, Speaker Enabled, IR Sensor Enabled
                    buffer[3] = (byte)(BatteryLow ? 0x01 : 0x00);
                    buffer[3] += (byte)(DeviceType != ControllerType.Wiimote ? 0x02 : 0x00);
                    //buffer[3] += (byte)(Speaker ? 0x04 : 0x00);
                    //buffer[3] += (byte)(IRSensor ? 0x08 : 0x00);

                    // L - LEDs
                    buffer[3] += (byte)(LED_1 ? 0x10 : 0x00);
                    buffer[3] += (byte)(LED_2 ? 0x20 : 0x00);
                    buffer[3] += (byte)(LED_3 ? 0x40 : 0x00);
                    buffer[3] += (byte)(LED_4 ? 0x80 : 0x00);

                    // VV - Battery Level
                    buffer[6] = BatteryLevel;
                    break;

                case InputReport.ReadMem: // 21 BB BB SE AA AA DD DD DD DD DD DD DD DD DD DD DD DD DD DD DD DD
                    // 21 - Read Memory
                    buffer[0] = 0x21;

                    // BB BB - Core Buttons
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];

                    // E - Error Code
                    buffer[3] = 0x00;

                    // S - Data Size
                    buffer[3] += 0xF0;

                    // AA AA - Last 4 bytes of the requested address
                    //buffer[4] = _lastReport[3];
                    //buffer[5] = _lastReport[4];

                    // DD - Data bytes padded to 16

                    var typeBytes = BitConverter.GetBytes((long)DeviceType);
                    buffer[6] = typeBytes[5];
                    buffer[7] = typeBytes[4];
                    buffer[8] = typeBytes[3];
                    buffer[9] = typeBytes[2];
                    buffer[10] = typeBytes[1];
                    buffer[11] = typeBytes[0];
                    
                    if (_lastReport.Length >= 4 && _lastReport[4] == 250)
                    {
                        //NextReport = DataReportMode;
                        _nextQueue.Enqueue(DataReportMode);
                    }
                    break;

                case InputReport.Acknowledge: // 22 BB BB RR EE
                    // 22 - Acknowledge Report
                    buffer[0] = 0x22;

                    // BB BB - Core Buttons
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];

                    // RR - Output Report that is being acknowledged

                    // EE - Error Code
                    buffer[4] = 0x00;
                    break;

                case InputReport.BtnsOnly: // 30 BB BB
                    buffer[0] = 0x30;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    break;

                case InputReport.BtnsAcc: // 31 BB BB AA AA AA
                    buffer[0] = 0x31;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    break;

                case InputReport.BtnsExt: // 32 BB BB EE EE EE EE EE EE EE EE
                    buffer[0] = 0x32;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    Array.Copy(GetExtension(), 0, buffer, 3, 8);
                    break;

                case InputReport.BtnsAccIR: // 33 BB BB AA AA AA II II II II II II II II II II II II
                    buffer[0] = 0x33;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    break;

                case InputReport.BtnsExtB: // 34 BB BB EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE 
                    buffer[0] = 0x34;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    Array.Copy(GetExtension(), 0, buffer, 3, 19);
                    break;

                case InputReport.BtnsAccExt: // 35 BB BB AA AA AA EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE
                    buffer[0] = 0x35;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    Array.Copy(GetExtension(), 0, buffer, 6, 16);
                    break;

                case InputReport.BtnsIRExt: // 36 BB BB II II II II II II II II II II EE EE EE EE EE EE EE EE EE
                    buffer[0] = 0x36;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    Array.Copy(GetExtension(), 0, buffer, 13, 9);
                    break;

                case InputReport.BtnsAccIRExt: // 37 BB BB AA AA AA II II II II II II II II II II EE EE EE EE EE EE
                    buffer[0] = 0x37;
                    buffer[1] = coreBtns[0];
                    buffer[2] = coreBtns[1];
                    Array.Copy(GetExtension(), 0, buffer, 16, 6);
                    break;

                case InputReport.ExtOnly: // 3d EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE
                    buffer[0] = 0x3D;
                    Array.Copy(GetExtension(), 0, buffer, 1, 21);
                    break;
            }

            _lastReport = null;

            return value;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //_lastReport = buffer;
            _reportQueue.Enqueue(buffer);
            OutputReport output = (OutputReport)buffer[0];

            switch (output)
            {
                case OutputReport.StatusRequest:
                    //NextReport = InputReport.Status;
                    _nextQueue.Enqueue(InputReport.Status);
                    RumbleByte = buffer[1];
                    break;

                case OutputReport.ReadMemory: // 21 BB BB SE AA AA DD DD DD DD DD DD DD DD DD DD DD DD DD DD DD DD
                    //if (NextReport != InputReport.ReadMem)
                    if (!_nextQueue.Contains(InputReport.ReadMem))
                    {
                        // Extension Step A
                        //NextReport = InputReport.ReadMem;
                        _nextQueue.Enqueue(InputReport.ReadMem);
                    }
                    else
                    {
                        // Extension Step B
                    }
                    break;

                case OutputReport.LEDs: // 11 LL
                    //NextReport = InputReport.Acknowledge;
                    _nextQueue.Enqueue(InputReport.Acknowledge);
                    LED_1 = (buffer[1] & 0x10) != 0x00;
                    LED_2 = (buffer[1] & 0x20) != 0x00;
                    LED_3 = (buffer[1] & 0x30) != 0x00;
                    LED_4 = (buffer[1] & 0x40) != 0x00;
                    break;

                case OutputReport.DataReportMode: // 12 TT MM
                    DataReportMode = (InputReport)buffer[2];
                    //if ((byte)NextReport >= 0x30)
                    //{
                    //    NextReport = DataReportMode;
                    //}
                    if (_nextQueue.Count == 0 || (byte)_nextQueue.Peek() >= 0x30)
                    {
                        _nextQueue.Enqueue(DataReportMode);
                    }
                    break;
            }
        }
        #endregion

        #region Get Input Bytes
        protected byte[] GetCoreButtons()
        {
            byte[] buf = new byte[2];

            var buttons = new CoreButtons();

            switch (DeviceType)
            {
                case ControllerType.ProController:
                    var p = (ProController)State;
                    buttons.A = p.A;
                    buttons.B = p.B;
                    buttons.Plus = p.Plus;
                    buttons.Minus = p.Minus;
                    buttons.Home = p.Home;
                    buttons.Up = p.Up;
                    buttons.Down = p.Down;
                    buttons.Left = p.Left;
                    buttons.Right = p.Right;
                    break;

                case ControllerType.Wiimote:
                    buttons = ((Wiimote)State).buttons;
                    break;

                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                case ControllerType.ClassicController:
                case ControllerType.ClassicControllerPro:
                case ControllerType.MotionPlus:
                case ControllerType.MotionPlusCC:
                case ControllerType.MotionPlusNunchuk:
                //case ControllerType.Drums:
                //case ControllerType.Guitar:
                //case ControllerType.TaikoDrum:
                //case ControllerType.TurnTable:
                //case ControllerType.DrawTablet:
                    buttons = ((IWiimoteExtension)State).wiimote.buttons;
                    break;
            }

            buf[0] |= (byte)(buttons.Left  ? 0x01 : 0x00);
            buf[0] |= (byte)(buttons.Right ? 0x02 : 0x00);
            buf[0] |= (byte)(buttons.Down  ? 0x04 : 0x00);
            buf[0] |= (byte)(buttons.Up    ? 0x08 : 0x00);
            buf[0] |= (byte)(buttons.Plus  ? 0x10 : 0x00);

            buf[1] |= (byte)(buttons.Two   ? 0x01 : 0x00);
            buf[1] |= (byte)(buttons.One   ? 0x02 : 0x00);
            buf[1] |= (byte)(buttons.B     ? 0x04 : 0x00);
            buf[1] |= (byte)(buttons.A     ? 0x08 : 0x00);
            buf[1] |= (byte)(buttons.Minus ? 0x10 : 0x00);
            buf[1] |= (byte)(buttons.Home  ? 0x80 : 0x00);

            return buf;
        }

        protected byte[] GetAccelerometer()
        {
            byte[] buf = new byte[3];

            return buf;
        }

        protected byte[] Get9ByteIR()
        {
            byte[] buf = new byte[9];

            return buf;
        }

        protected byte[] Get10ByteIR()
        {
            byte[] buf = new byte[10];

            return buf;
        }

        protected byte[] GetExtension()
        {
            byte[] buf = new byte[21];
            
            if (DeviceType == ControllerType.ProController)
            {
                var pro = (ProController)State;

                var lx = BitConverter.GetBytes(pro.LJoy.rawX);
                var ly = BitConverter.GetBytes(pro.LJoy.rawY);
                var rx = BitConverter.GetBytes(pro.RJoy.rawX);
                var ry = BitConverter.GetBytes(pro.RJoy.rawY);

                buf[0] = lx[0];
                buf[1] = lx[1];
                buf[2] = rx[0];
                buf[3] = rx[1];
                buf[4] = ly[0];
                buf[5] = ly[1];
                buf[6] = ry[0];
                buf[7] = ry[1];

                buf[8] = 0x00;
                buf[8] += (byte)(!pro.R     ? 0x02 : 0x00);
                buf[8] += (byte)(!pro.Plus  ? 0x04 : 0x00);
                buf[8] += (byte)(!pro.Home  ? 0x08 : 0x00);
                buf[8] += (byte)(!pro.Minus ? 0x10 : 0x00);
                buf[8] += (byte)(!pro.L     ? 0x20 : 0x00);
                buf[8] += (byte)(!pro.Down  ? 0x40 : 0x00);
                buf[8] += (byte)(!pro.Right ? 0x80 : 0x00);

                buf[9] = 0x00;
                buf[9] += (byte)(!pro.Up   ? 0x01 : 0x00);
                buf[9] += (byte)(!pro.Left ? 0x02 : 0x00);
                buf[9] += (byte)(!pro.ZR   ? 0x04 : 0x00);
                buf[9] += (byte)(!pro.X    ? 0x08 : 0x00);
                buf[9] += (byte)(!pro.A    ? 0x10 : 0x00);
                buf[9] += (byte)(!pro.Y    ? 0x20 : 0x00);
                buf[9] += (byte)(!pro.B    ? 0x40 : 0x00);
                buf[9] += (byte)(!pro.ZL   ? 0x80 : 0x00);

                buf[10] = 0x00;
                buf[10] += (byte)(!pro.RStick       ? 0x01 : 0x00);
                buf[10] += (byte)(!pro.LStick       ? 0x02 : 0x00);
                buf[10] += (byte)(!pro.charging     ? 0x04 : 0x00);
                buf[10] += (byte)(!pro.usbConnected ? 0x08 : 0x00);
            }
            else if (DeviceType == ControllerType.Nunchuk || DeviceType == ControllerType.NunchukB)
            {
                
            }
            else if (DeviceType == ControllerType.ClassicController)
            {

            }
            else if (DeviceType == ControllerType.ClassicControllerPro)
            {
                var ccp = (ClassicControllerPro)State;

                var lx = BitConverter.GetBytes(ccp.LJoy.rawX);
                var ly = BitConverter.GetBytes(ccp.LJoy.rawY);
                var rx = BitConverter.GetBytes(ccp.RJoy.rawX);
                var ry = BitConverter.GetBytes(ccp.RJoy.rawY);

                buf[0] = (byte)(lx[0] + (rx[0] << 3 & 0xC0));
                buf[1] = (byte)(ly[0] + (rx[0] << 5 & 0xC0));
                buf[2] = (byte)(ry[0] + (rx[0] << 7 & 0x80));

                buf[4] = 0x01;
                buf[4] += (byte)(!ccp.R ? 0x02 : 0x00);
                buf[4] += (byte)(!ccp.Plus ? 0x04 : 0x00);
                buf[4] += (byte)(!ccp.Home ? 0x08 : 0x00);
                buf[4] += (byte)(!ccp.Minus ? 0x10 : 0x00);
                buf[4] += (byte)(!ccp.L ? 0x20 : 0x00);
                buf[4] += (byte)(!ccp.Down ? 0x40 : 0x00);
                buf[4] += (byte)(!ccp.Right ? 0x80 : 0x00);

                buf[5] = 0x00;
                buf[5] += (byte)(!ccp.Up ? 0x01 : 0x00);
                buf[5] += (byte)(!ccp.Left ? 0x02 : 0x00);
                buf[5] += (byte)(!ccp.ZR ? 0x04 : 0x00);
                buf[5] += (byte)(!ccp.X ? 0x08 : 0x00);
                buf[5] += (byte)(!ccp.A ? 0x10 : 0x00);
                buf[5] += (byte)(!ccp.Y ? 0x20 : 0x00);
                buf[5] += (byte)(!ccp.B ? 0x40 : 0x00);
                buf[5] += (byte)(!ccp.ZL ? 0x80 : 0x00);
            }

            return buf;
        }

        protected byte[] GetGCNController(GameCubeController controller)
        {
            byte[] buf = new byte[9];

            buf[1] |= (byte)(controller.A ? 0x01 : 0x00);
            buf[1] |= (byte)(controller.B ? 0x02 : 0x00);
            buf[1] |= (byte)(controller.X ? 0x04 : 0x00);
            buf[1] |= (byte)(controller.Y ? 0x08 : 0x00);
            buf[1] |= (byte)(controller.Left ? 0x10 : 0x00);
            buf[1] |= (byte)(controller.Right ? 0x20 : 0x00);
            buf[1] |= (byte)(controller.Down ? 0x40 : 0x00);
            buf[1] |= (byte)(controller.Up ? 0x80 : 0x00);

            buf[2] |= (byte)(controller.Start ? 0x01 : 0x00);
            buf[2] |= (byte)(controller.Z ? 0x02 : 0x00);
            buf[2] |= (byte)(controller.R.full ? 0x04 : 0x00);
            buf[2] |= (byte)(controller.L.full ? 0x08 : 0x00);

            buf[3] = (byte)controller.joystick.rawX;
            buf[4] = (byte)controller.joystick.rawY;
            buf[5] = (byte)controller.cStick.rawX;
            buf[6] = (byte)controller.cStick.rawY;
            buf[7] = (byte)controller.L.rawValue;
            buf[8] = (byte)controller.R.rawValue;

            return buf;
        }
        #endregion
    }
}
