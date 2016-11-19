using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The implementation of <see cref="IFormatter"/> which works on types implementing <see cref="ICompositeModel"/>.
    /// </summary>
    public class CompositeModelFormatter : IFormatter, ISerializer, IDeserializer
    {
        private readonly Func<Type, object> modelFactory;
        private readonly IFactory<ICompositeStorage> storageFactory;

        /// <summary>
        /// Creates new instance with factories from model and storages.
        /// </summary>
        /// <param name="modelFactory">The factory for empty models when deserializing.</param>
        /// <param name="storageFactory">The factory for empty composite storages.</param>
        public CompositeModelFormatter(Func<Type, object> modelFactory, IFactory<ICompositeStorage> storageFactory)
        {
            Ensure.NotNull(modelFactory, "modelFactory");
            Ensure.NotNull(storageFactory, "storageFactory");
            this.modelFactory = modelFactory;
            this.storageFactory = storageFactory;
        }

        public async Task<bool> TrySerializeAsync(object input, ISerializerContext context)
        {
            Ensure.NotNull(input, "input");
            Ensure.NotNull(context, "context");

            ICompositeModel model = input as ICompositeModel;
            if (model == null)
                return false;

            ICompositeStorage storage = storageFactory.Create();
            model.Save(storage);

            await storage.StoreAsync(context.Output).ConfigureAwait(false);
            return true;
        }

        public bool TrySerialize(object input, ISerializerContext context)
        {
            Ensure.NotNull(input, "input");
            Ensure.NotNull(context, "context");

            ICompositeModel model = input as ICompositeModel;
            if (model == null)
                return false;

            ICompositeStorage storage = storageFactory.Create();
            model.Save(storage);

            storage.Store(context.Output);
            return true;
        }

        public async Task<bool> TryDeserializeAsync(Stream input, IDeserializerContext context)
        {
            Ensure.NotNull(input, "input");
            Ensure.NotNull(context, "context");

            ICompositeModel model = modelFactory.Invoke(context.OutputType) as ICompositeModel;
            if (model == null)
                return false;

            ICompositeStorage storage = storageFactory.Create();
            await storage.LoadAsync(input).ConfigureAwait(false);

            model.Load(storage);
            context.Output = model;
            return true;
        }

        public bool TryDeserialize(Stream input, IDeserializerContext context)
        {
            Ensure.NotNull(input, "input");
            Ensure.NotNull(context, "context");

            ICompositeModel model = modelFactory.Invoke(context.OutputType) as ICompositeModel;
            if (model == null)
                return false;

            ICompositeStorage storage = storageFactory.Create();
            storage.Load(input);

            model.Load(storage);
            context.Output = model;
            return true;
        }
    }
}
