using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barricade.Utilities
{
    class Waiter
    {
        private TaskCompletionSource<bool> _completion;
        public Waiter()
        {
            _completion = new TaskCompletionSource<bool>();
        }

        public void Return()
        {
            _completion.TrySetResult(true);
        }

        public async Task Wait()
        {
            await _completion.Task;
            _completion = new TaskCompletionSource<bool>();
        }
    }


    class Waiter<T>
    {
        private TaskCompletionSource<T> _completion;
        public Waiter()
        {
            _completion = new TaskCompletionSource<T>();
        }

        public void Return(T result)
        {
            _completion.TrySetResult(result);
        }

        public async Task<T> Wait()
        {
            var result = await _completion.Task;
            _completion = new TaskCompletionSource<T>();
            return result;
        }
    }
}
