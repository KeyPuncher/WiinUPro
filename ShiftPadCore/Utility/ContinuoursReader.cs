namespace ShiftPad.Core.Utility
{
    /// <summary>
    /// Helper class to instigate continous and repeated stream reading.
    /// </summary>
    public class ContinuoursReader
    {
        private Stream _stream;
        private int _bufferSize;
        private Action<byte[]> _callback;
        private CancellationTokenSource _cancellationToken;

        public ContinuoursReader(Stream stream, int bufferSize, Action<byte[]> callback)
        {
            _stream = stream;
            _bufferSize = bufferSize;
            _callback = callback;

            // Starting cancel to prevent multiple read starts.
            _cancellationToken = new CancellationTokenSource();
            _cancellationToken.Cancel();
        }

        public async Task StartReading()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken = new CancellationTokenSource();
                await Task.Run(PerformReads);
            }
        }

        public void StopReading()
        {
            _cancellationToken.Cancel();
        }

        private async Task PerformReads()
        {
            while (!_cancellationToken.IsCancellationRequested)
            { 
                byte[] buffer = new byte[_bufferSize];
                await _stream.ReadExactlyAsync(buffer, _cancellationToken.Token);
                _callback?.Invoke(buffer);
            }
        }
    }
}
