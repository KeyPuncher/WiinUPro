using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NintrollerLib
{
    internal class DataHandler
    {
        public enum HandlerState
        {
            Init = 0,   // Initializing
            Ready,      // Not currently reading or writing, but ready to start
            Reading,    // Actively reading from the controller
            Busy,       // Internally Paused, and not ready for commands
            Stopped     // Reading has been stopped due to error or disposing
        }

        public HandlerState state = HandlerState.Init;
        protected int _reportSize = Constants.REPORT_LENGTH;
        protected FileStream _stream;
        public FileStream DataStream
        {
            get { return _stream; }
            private set { _stream = value; }
        }

        // Reading variables
        protected delegate void ReaderDelegate();
        protected ReaderDelegate readerDelegate;
        protected IAsyncResult readerResult;

        private bool useOutputReport = false;

        public DataHandler(FileStream stream, bool alternateWrite = false)
        {
            _stream = stream;
            readerDelegate = new ReaderDelegate(AsyncRead);
            useOutputReport = alternateWrite;
        }

        public void BeginReading()
        {
            // TODO: Check if we can begin doing reading operations
            if (state == HandlerState.Ready)
            {
                readerResult = readerDelegate.BeginInvoke(ReadingStopped, null);
            }
        }

        public void WriteData(byte[] report)
        {
            HandlerState prevState = state;
            state = HandlerState.Busy;

            if (_stream != null)
            {
                try
                {
                    _stream.Write(report, 0, report.Length);
                    // TODO: use HID_SetOutputReport (needs a handle)
                }
                catch (Exception e)
                {
                    #if DEBUG
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    #endif
                }
            }

            state = prevState;
        }

        // Use this if acknowledgement is needed
        public byte[] WriteRead(byte[] report)
        {
            byte[] results = new byte[_reportSize];

            if (_stream == null)
                return results;

            // TODO: Write data and return the immediate result
            // Pause Reading
            // Send Bytes
            // Read Synchronously
            // Continue async reading

            HandlerState prevState = state;
            state = HandlerState.Busy;

            WriteData(report);
            try
            {
                _stream.Read(results, 0, results.Length);
            }
            catch (Exception e)
            {
                #if DEBUG
                System.Diagnostics.Debug.WriteLine(e.Message);
                #endif
            }

            state = prevState;

            return results;
        }

        private void AsyncRead()
        {
            while (true)
            {
                if (_stream == null)
                    break;

                if (_stream.CanRead && state == HandlerState.Reading)
                {
                    byte[] buff = new byte[_reportSize];

                    try
                    {
                        _stream.Read(buff, 0, buff.Length);
                    }
                    catch (Exception e)
                    {
                        #if DEBUG
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        #endif
                        break;
                    }

                    // TODO: do something with the read bytes
                    // Check Report Type/Category
                    // Pass Report to appropraite destination for Parsing
                    // Fire off any necessary events
                }
                else
                {
                    // TODO: Wait 10ms
                }
            }
        }

        private void ReadingStopped(IAsyncResult ar)
        {
            // TODO: change status and fire any necessary events
        }
    }
}
