#if WIINUPRO
using System;
using System.IO;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace Shared.Windows
{
    public class WinUsbStream : CommonStream
    {
        private UsbDeviceFinder _usbFinder;
        private UsbDevice _usbDevice;
        private UsbEndpointReader _reader;
        private UsbEndpointWriter _writer;
        private int _timeout = 10;

        public WinUsbStream(UsbDeviceFinder usbDeviceFinder)
        {
            _usbFinder = usbDeviceFinder;
        }

        public bool DeviceFound()
        {
            if (_usbDevice == null)
            {
                _usbDevice = UsbDevice.OpenUsbDevice(_usbFinder);
            }

            return _usbDevice != null;
        }

        #region System.IO.Stream Properties
        public override bool CanRead { get { return _usbDevice?.IsOpen ?? false && _reader != null; } }

        public override bool CanWrite { get { return _usbDevice?.IsOpen ?? false && _writer != null; } }

        public override bool CanSeek { get { return false; } }

        public override long Length { get { return _reader?.ReadBufferSize ?? 0; } }

        public override long Position
        {
            get
            {
                return 0;
            }

            set
            {
                // nothing
            }
        }
        #endregion

        public override bool OpenConnection()
        {
            if (!DeviceFound())
            {
                return false;
            }

            _reader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
            _writer = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);

            // Start receiving data
            int length;
            _writer?.Write(0x13, _timeout, out length);

            return _reader != null && _writer != null;
        }

        #region System.IO.Stream Methods
        public override void Close()
        {
            if (_reader != null)
            {
                _reader.Abort();
                _reader = null;
            }

            if (_writer != null)
            {
                _writer.Abort();
                _writer = null;
            }

            if (_usbDevice != null)
            {
                if (_usbDevice.IsOpen)
                {
                    _usbDevice.Close();
                }

                _usbDevice = null;
                UsbDevice.Exit();
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int length;
            _writer?.Write(buffer, _timeout, out length);
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            int length = 0;
            var errorCode = _reader?.Read(buffer, _timeout, out length) ?? ErrorCode.None;
            
            if (errorCode != ErrorCode.Success)
            {
                System.Diagnostics.Debug.WriteLine("Error Reading from USB Device: " + errorCode);
            }

            return length;
        }

        public override void Flush()
        {
            _reader?.Flush();
            _writer?.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
#endif