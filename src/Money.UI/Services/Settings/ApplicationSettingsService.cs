using Neptuo;
using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Services.Settings
{
    internal class ApplicationSettingsService : IUserPreferenceService
    {
        private readonly IFactory<IFormatter, ICompositeStorage> formatterFactory;
        private readonly IConverterRepository converters = Converts.Repository;
        private readonly IFactory<ApplicationDataContainer> storageContainerFactory;

        public ApplicationSettingsService(IFactory<IFormatter, ICompositeStorage> formatterFactory, IFactory<ApplicationDataContainer> storageContainerFactory)
        {
            Ensure.NotNull(formatterFactory, "formatterFactory");
            Ensure.NotNull(storageContainerFactory, "storageContainerFactory");
            this.formatterFactory = formatterFactory;
            this.storageContainerFactory = storageContainerFactory;
        }

        private bool TryGetContainer(string containerPath, bool isCreatedWhenNotExists, out ApplicationDataContainer container)
        {
            Ensure.NotNullOrEmpty(containerPath, "containerPath");

            ApplicationDataContainer result = storageContainerFactory.Create();
            string[] paths = containerPath.Split('.');
            foreach (string path in paths)
            {
                if (!result.Containers.TryGetValue(path, out ApplicationDataContainer current))
                {
                    if (isCreatedWhenNotExists)
                    {
                        current = result.CreateContainer(path, ApplicationDataCreateDisposition.Always);
                    }
                    else
                    {
                        container = null;
                        return false;
                    }
                }

                result = current;
            }

            container = result;
            return true;
        }

        public bool TryLoad<T>(string containerPath, out T model)
            where T : class
        {
            if (TryGetContainer(containerPath, false, out ApplicationDataContainer container))
            {
                IFormatter formatter = formatterFactory.Create(new ApplicationDataCompositeStorage(container));
                IDeserializerContext context = new DefaultDeserializerContext(typeof(T));
                if (formatter.TryDeserialize(new MemoryStream(), context))
                {
                    model = (T)context.Output;
                    return true;
                }
            }

            model = null;
            return false;
        }

        public bool TrySave<T>(string containerPath, T model)
        {
            if (TryGetContainer(containerPath, true, out ApplicationDataContainer container))
            {
                IFormatter formatter = formatterFactory.Create(new ApplicationDataCompositeStorage(container));
                ISerializerContext context = new DefaultSerializerContext(typeof(T), new MemoryStream());
                if (formatter.TrySerialize(model, context))
                    return true;
            }

            return false;
        }
    }
}
