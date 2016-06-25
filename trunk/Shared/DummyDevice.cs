using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NintrollerLib;

namespace Shared
{
    public class DummyDevice : Stream
    {
        #region Properties
        public ControllerType DeviceType { get; }

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

        public DummyDevice(INintrollerState state)
        {
            State = state;
            Type t = State.GetType();
            if (t == typeof(ProController))
            {
                DeviceType = ControllerType.ProController;
            }
        }

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
            int value = -1;

            // Set Error
            buffer[4] = 0x00;

            switch (NextReport)
            {
                case InputReport.Status: // 20 BB BB LF 00 00 VV
                    // 20 - Status type
                    buffer[0] = (byte)InputReport.Status;

                    // BB BB - Core Buttons
                    //buffer[1]
                    //buffer[2]

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

                    // E - Error Code
                    buffer[3] = 0x00;

                    // S - Data Size
                    buffer[3] += 0x00;

                    // AA AA - Last 4 bytes of the requested address

                    // DD - Data bytes padded to 16
                    break;

                case InputReport.Acknowledge: // 22 BB BB RR EE
                    // 22 - Acknowledge Report
                    buffer[0] = 0x22;

                    // BB BB - Core Buttons

                    // RR - Output Report that is being acknowledged

                    // EE - Error Code
                    buffer[4] = 0x00;
                    break;

                case InputReport.BtnsOnly: // 30 BB BB
                    buffer[0] = 0x30;
                    break;

                case InputReport.BtnsAcc: // 31 BB BB AA AA AA
                    buffer[0] = 0x31;
                    break;

                case InputReport.BtnsExt: // 32 BB BB EE EE EE EE EE EE EE EE
                    buffer[0] = 0x32;
                    break;

                case InputReport.BtnsAccIR: // 33 BB BB AA AA AA II II II II II II II II II II II II
                    buffer[0] = 0x33;
                    break;

                case InputReport.BtnsExtB: // 34 BB BB EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE 
                    buffer[0] = 0x34;
                    break;

                case InputReport.BtnsAccExt: // 35 BB BB AA AA AA EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE
                    buffer[0] = 0x35;
                    break;

                case InputReport.BtnsIRExt: // 36 BB BB II II II II II II II II II II EE EE EE EE EE EE EE EE EE
                    buffer[0] = 0x36;
                    break;

                case InputReport.BtnsAccIRExt: // 37 BB BB AA AA AA II II II II II II II II II II EE EE EE EE EE EE
                    buffer[0] = 0x37;
                    break;

                case InputReport.ExtOnly: // 3d EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE EE
                    buffer[0] = 0x3D;
                    Array.Copy(GetExtension(), 0, buffer, 1, 21);
                    break;
            }

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
            _lastReport = buffer;
            OutputReport output = (OutputReport)buffer[0];

            switch (output)
            {
                case OutputReport.StatusRequest:
                    NextReport = InputReport.Status;
                    RumbleByte = buffer[1];
                    break;

                case OutputReport.LEDs: // 11 LL
                    NextReport = InputReport.Acknowledge;
                    LED_1 = (buffer[1] & 0x10) != 0x00;
                    LED_2 = (buffer[1] & 0x20) != 0x00;
                    LED_3 = (buffer[1] & 0x30) != 0x00;
                    LED_4 = (buffer[1] & 0x40) != 0x00;
                    break;

                case OutputReport.DataReportMode: // 12 TT MM
                    NextReport = (InputReport)buffer[2];
                    break;
            }
        }
        #endregion

        #region Get Input Bytes
        protected byte[] GetCoreButtons()
        {
            byte[] buf = new byte[2];

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

            Type t = State.GetType();
            if (t == typeof(ProController))
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
                buf[8] += (byte)(pro.R     ? 0x02 : 0x00);
                buf[8] += (byte)(pro.Plus  ? 0x04 : 0x00);
                buf[8] += (byte)(pro.Home  ? 0x08 : 0x00);
                buf[8] += (byte)(pro.Minus ? 0x10 : 0x00);
                buf[8] += (byte)(pro.L     ? 0x20 : 0x00);
                buf[8] += (byte)(pro.Down  ? 0x40 : 0x00);
                buf[8] += (byte)(pro.Right ? 0x80 : 0x00);

                buf[9] = 0x00;
                buf[9] += (byte)(pro.Up   ? 0x01 : 0x00);
                buf[9] += (byte)(pro.Left ? 0x02 : 0x00);
                buf[9] += (byte)(pro.ZR   ? 0x04 : 0x00);
                buf[9] += (byte)(pro.X    ? 0x08 : 0x00);
                buf[9] += (byte)(pro.A    ? 0x10 : 0x00);
                buf[9] += (byte)(pro.Y    ? 0x20 : 0x00);
                buf[9] += (byte)(pro.B    ? 0x40 : 0x00);
                buf[9] += (byte)(pro.ZL   ? 0x80 : 0x00);

                buf[10] = 0x00;
                buf[10] += (byte)(pro.RStick       ? 0x01 : 0x00);
                buf[10] += (byte)(pro.LStick       ? 0x02 : 0x00);
                buf[10] += (byte)(pro.charging     ? 0x04 : 0x00);
                buf[10] += (byte)(pro.usbConnected ? 0x08 : 0x00);
            }
            else if (t == typeof(Nunchuk))
            {
                
            }
            else if (t == typeof(ClassicController))
            {

            }
            else if (t == typeof(ClassicController))
            {

            }

            return buf;
        }
        #endregion
    }
}
