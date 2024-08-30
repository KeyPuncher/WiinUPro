using System.Collections.Concurrent;

namespace ShiftPad.Core.Utility
{
    public class ResponseBuffer<TargetType, ReturnType>
        where TargetType : notnull
        where ReturnType : class?
    {
        private ConcurrentDictionary<TargetType, ReturnType?> _dictionary = new();
        private CancellationTokenSource _internalCancellationToken;

        public ResponseBuffer()
        {
            _internalCancellationToken = new CancellationTokenSource();
        }

        public void Cancel()
        {
            _internalCancellationToken.Cancel();
        }

        public Task<(bool success, ReturnType? value)> MakeRequest(TargetType target)
        {
            return MakeRequest(target, _internalCancellationToken.Token);
        }

        public async Task<(bool success, ReturnType? value)> MakeRequest(TargetType target, CancellationToken cancellationToken)
        {
            // Yield if another request is waiting for this response.
            while (_dictionary.ContainsKey(target))
            {
                await Task.Yield();
                if (cancellationToken.IsCancellationRequested)
                {
                    _dictionary.Remove(target, out _);
                    return (false, null);
                }
            }

            ReturnType? value;
            while (!_dictionary.TryGetValue(target, out value) || value == null)
            {
                await Task.Yield();
                if (cancellationToken.IsCancellationRequested)
                {
                    _dictionary.Remove(target, out _);
                    return (false, null);
                }
            }

            _ = _dictionary.Remove(target, out _);
            return (true, value);
        }

        public bool SetResponse(TargetType target, ReturnType value)
        {
            return _dictionary.TryUpdate(target, value, null);
        }
    }
}
