using Neptuo;
using Neptuo.Activators;
using Neptuo.Formatters;
using Neptuo.Formatters.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    public class CompositeTypeFormatterFactory : IFactory<IFormatter, ICompositeStorage>
    {
        private readonly ICompositeTypeProvider typeProvider;

        public CompositeTypeFormatterFactory(ICompositeTypeProvider typeProvider)
        {
            Ensure.NotNull(typeProvider, "typeProvider");
            this.typeProvider = typeProvider;
        }

        public IFormatter Create(ICompositeStorage storage)
        {
            return new CompositeTypeFormatter(typeProvider, Factory.Instance(storage));
        }
    }
}
