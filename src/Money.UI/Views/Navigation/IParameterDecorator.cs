using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Navigation
{
    public interface IParameterDecorator
    {
        object Decorate(object parameter);
    }
}
