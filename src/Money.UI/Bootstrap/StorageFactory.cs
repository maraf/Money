using Money.Data;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Bootstrap
{
    internal class StorageFactory : IFactory<EventSourcingContext>, IFactory<ReadModelContext>, IFactory<ApplicationDataContainer>
    {
        EventSourcingContext IFactory<EventSourcingContext>.Create()
        {
            return new EventSourcingContext("Filename=EventSourcing.db");
        }

        ReadModelContext IFactory<ReadModelContext>.Create()
        {
            return new ReadModelContext("Filename=ReadModel.db");
        }

        ApplicationDataContainer IFactory<ApplicationDataContainer>.Create()
        {
            return ApplicationData.Current.LocalSettings;
        }
    }
}
