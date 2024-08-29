using System.Collections.Concurrent;

namespace ShiftPad.Core.Utility
{
    public class ResponseBuffer<TargetType, ReturnType>
        where TargetType : notnull
        where ReturnType : class?
    {
        private ConcurrentDictionary<TargetType, ReturnType?> _dictionary = new();
        private ReturnType _defaultValue;

        public ResponseBuffer(ReturnType defulatValue)
        {
            _defaultValue = defulatValue;
        }

        public async Task<ReturnType> MakeRequest(TargetType target, CancellationToken cancellationToken)
        {
            // Yield if another request is waiting for this response.
            while (_dictionary.ContainsKey(target))
            {
                await Task.Yield();
                if (cancellationToken.IsCancellationRequested)
                {
                    _dictionary.Remove(target, out _);
                    return _defaultValue;
                }
            }

            ReturnType? value;
            while (!_dictionary.TryGetValue(target, out value) || value == null)
            {
                await Task.Yield();
                if (cancellationToken.IsCancellationRequested)
                {
                    _dictionary.Remove(target, out _);
                    return _defaultValue;
                }
            }

            _ = _dictionary.Remove(target, out _);
            return value;
        }

        public bool SetResponse(TargetType target, ReturnType value)
        {
            return _dictionary.TryUpdate(target, value, null);
        }
    }
}
