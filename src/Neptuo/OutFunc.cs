using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptuo
{ 
    /// <summary>
    /// Function with one input parameter, one output parameter and result value.
    /// </summary>
    /// <typeparam name="T">Type of input parameter.</typeparam>
    /// <typeparam name="TOutput">Type of output parameter.</typeparam>
    /// <typeparam name="TReturn">Type of result value.</typeparam>
    /// <param name="input">Input parametervalue.</param>
    /// <param name="output">Ouput parameter.</param>
    /// <returns>Computed result.</returns>
    public delegate TReturn OutFunc<T, TOutput, TReturn>(T input, out TOutput output);
}
