using Neptuo.Activators;
using Neptuo.Collections.Specialized;
using Neptuo.Formatters.Metadata;
using Neptuo.Models;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    public class CompositeExceptionFormatter : CompositeTypeFormatter
    {
        protected new static class Name
        {
            public const string InnerStorage = "AggregateRootException";

            public const string AggregateRootKey = "AggregateRootKey";
            public const string CommandKey = "CommandKey";
            public const string SourceCommandKey = "SourceCommandKey";
        }

        public CompositeExceptionFormatter(ICompositeTypeProvider provider, IFactory<ICompositeStorage> storageFactory)
            : base(provider, storageFactory)
        { }

        protected override bool TryStore(object input, ISerializerContext context, CompositeType type, CompositeVersion typeVersion, ICompositeStorage storage)
        {
            if (base.TryStore(input, context, type, typeVersion, storage))
            {
                if (input is AggregateRootException e)
                {
                    ICompositeStorage inner = storage.Add(Name.InnerStorage);
                    inner.Add(Name.AggregateRootKey, e.Key);
                    inner.Add(Name.CommandKey, e.CommandKey);
                    inner.Add(Name.SourceCommandKey, e.SourceCommandKey);
                }

                return true;
            }

            return false;
        }

        protected override bool TryLoad(Stream input, IDeserializerContext context, CompositeType type, ICompositeStorage storage)
        {
            if (base.TryLoad(input, context, type, storage))
            {
                if (storage.TryGet("AggregateRootException", out ICompositeStorage inner))
                {
                    AggregateRootExceptionDecorator decorator = new AggregateRootExceptionDecorator((AggregateRootException)context.Output);

                    IKey key = inner.Get<IKey>(Name.AggregateRootKey);
                    if (key != null)
                        decorator.SetKey(key);

                    key = inner.Get<IKey>(Name.CommandKey);
                    if (key != null)
                        decorator.SetCommandKey(key);

                    key = inner.Get<IKey>(Name.SourceCommandKey);
                    if (key != null)
                        decorator.SetSourceCommandKey(key);
                }

                return true;
            }

            return false;
        }
    }
}
