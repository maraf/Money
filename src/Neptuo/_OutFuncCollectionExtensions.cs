using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    public static class _OutFuncCollectionExtensions
    {
        /// <summary>
        /// Tries to execute all funcs in <paramref name="list"/> until first returns <c>true</c>. 
        /// </summary>
        /// <typeparam name="T">Type of func parameter.</typeparam>
        /// <typeparam name="TOutput">Type of output parameter.</typeparam>
        /// <param name="list">List to execute on.</param>
        /// <param name="parameter">Value of func parameter.</param>
        /// <param name="output">Provided output value.</param>
        /// <returns><c>true</c> if any func in <paramref name="list"/> returns <c>true</c>; otherwise <c>false</c>.</returns>
        public static bool TryExecute<T, TOutput>(this OutFuncCollection<T, TOutput, bool> list, T parameter, out TOutput output)
        {
            foreach (OutFunc<T, TOutput, bool> func in list)
            {
                if (func(parameter, out output))
                    return true;
            }

            output = default(TOutput);
            return false;
        }
    }
}
