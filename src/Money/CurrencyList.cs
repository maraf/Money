using Neptuo.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo;
using Neptuo.Events.Handlers;
using Money.Events;

namespace Money
{
    /// <summary>
    /// All currencies.
    /// </summary>
    public partial class CurrencyList : AggregateRoot,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyExchangeRateSet>,
        IEventHandler<CurrencyExchangeRateRemoved>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDeleted>
    {
        private readonly Dictionary<IKey, UserModel> storage = new Dictionary<IKey, UserModel>();

        public CurrencyList()
        { }

        public CurrencyList(IKey key, IEnumerable<IEvent> events)
            : base(key, events)
        { }

        private UserModel GetUserModel(IKey userKey)
        {
            if (!storage.TryGetValue(userKey, out UserModel model))
                storage[userKey] = model = new UserModel();

            return model;
        }

        private void EnsureUnique(IKey userKey, string uniqueCode, bool isDeletedIncluded = false)
        {
            uniqueCode = uniqueCode.ToLowerInvariant();

            UserModel userModel = GetUserModel(userKey);
            if (userModel.UniqueCodes.Contains(uniqueCode))
                throw new CurrencyAlreadyExistsException();

            if (isDeletedIncluded && userModel.DeletedUniqueCodes.Contains(uniqueCode))
                throw new CurrencyAlreadyExistsException();
        }

        private void EnsureExists(IKey userKey, string uniqueCode, bool isDeletedIncluded = false)
        {
            uniqueCode = uniqueCode.ToLowerInvariant();

            UserModel userModel = GetUserModel(userKey);
            if (userModel.UniqueCodes.Contains(uniqueCode))
                return;

            if (isDeletedIncluded && userModel.DeletedUniqueCodes.Contains(uniqueCode))
                return;

            throw new CurrencyDoesNotExistException();
        }

        public void Add(IKey userKey, string uniqueCode, string symbol)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            Ensure.NotNullOrEmpty(symbol, "symbol");
            EnsureUnique(userKey, uniqueCode);
            Publish(new CurrencyCreated(uniqueCode, symbol) { UserKey = userKey });

            UserModel userModel = GetUserModel(userKey);
            if (userModel.DefaultName == null)
                Publish(new CurrencyDefaultChanged(uniqueCode) { UserKey = userKey });
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            UserModel userModel = GetUserModel(payload.UserKey);
            userModel.UniqueCodes.Add(payload.UniqueCode.ToLowerInvariant());
            return Task.CompletedTask;
        }

        public void SetAsDefault(IKey userKey, string uniqueCode)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            EnsureExists(userKey, uniqueCode);

            UserModel userModel = GetUserModel(userKey);
            if (userModel.DefaultName == uniqueCode.ToLowerInvariant())
                throw new CurrencyAlreadyAsDefaultException();

            Publish(new CurrencyDefaultChanged(uniqueCode) { UserKey = userKey });
        }

        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            UserModel userModel = GetUserModel(payload.UserKey);
            userModel.DefaultName = payload.UniqueCode.ToLowerInvariant();
            return Task.CompletedTask;
        }

        public void ChangeSymbol(IKey userKey, string uniqueCode, string symbol)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            EnsureExists(userKey, uniqueCode);

            Publish(new CurrencySymbolChanged(uniqueCode, symbol) { UserKey = userKey });
        }

        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload)
        {
            return Task.CompletedTask;
        }

        public void SetExchangeRate(IKey userKey, string sourceUniqueCode, string targetUniqueCode, DateTime validFrom, double rate)
        {
            Ensure.NotNullOrEmpty(sourceUniqueCode, "sourceUniqueCode");
            Ensure.NotNullOrEmpty(targetUniqueCode, "targetUniqueCode");
            EnsureExists(userKey, sourceUniqueCode);
            EnsureExists(userKey, targetUniqueCode);
            Ensure.Positive(rate, "rate");

            UserModel userModel = GetUserModel(userKey);
            CurrencyExchangeRateSet payload = new CurrencyExchangeRateSet(sourceUniqueCode, targetUniqueCode, validFrom, rate) { UserKey = userKey };
            if (userModel.ExchangeRateHashCodes.Contains(payload.GetHashCode()))
                throw new CurrencyExchangeRateAlreadyExistsException();

            Publish(payload);
        }

        Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            UserModel userModel = GetUserModel(payload.UserKey);
            userModel.ExchangeRateHashCodes.Add(payload.GetHashCode());
            return Task.CompletedTask;
        }

        public void RemoveExchangeRate(IKey userKey, string sourceUniqueCode, string targetUniqueCode, DateTime validFrom, double rate)
        {
            Ensure.NotNullOrEmpty(sourceUniqueCode, "sourceUniqueCode");
            Ensure.NotNullOrEmpty(targetUniqueCode, "targetUniqueCode");
            EnsureExists(userKey, sourceUniqueCode);
            EnsureExists(userKey, targetUniqueCode);
            Ensure.Positive(rate, "rate");

            UserModel userModel = GetUserModel(userKey);
            CurrencyExchangeRateSet payload = new CurrencyExchangeRateSet(sourceUniqueCode, targetUniqueCode, validFrom, rate) { UserKey = userKey };
            if (!userModel.ExchangeRateHashCodes.Contains(payload.GetHashCode()))
                throw new CurrencyExchangeRateDoesNotExistException();

            Publish(new CurrencyExchangeRateRemoved(sourceUniqueCode, targetUniqueCode, validFrom, rate) { UserKey = userKey });
        }

        Task IEventHandler<CurrencyExchangeRateRemoved>.HandleAsync(CurrencyExchangeRateRemoved payload)
        {
            UserModel userModel = GetUserModel(payload.UserKey);
            userModel.ExchangeRateHashCodes.Remove(new CurrencyExchangeRateSet(payload.SourceUniqueCode, payload.TargetUniqueCode, payload.ValidFrom, payload.Rate) { UserKey = payload.UserKey }.GetHashCode());
            return Task.CompletedTask;
        }

        public void Delete(IKey userKey, string uniqueCode)
        {
            Ensure.NotNullOrEmpty(uniqueCode, "uniqueCode");
            EnsureExists(userKey, uniqueCode, true);

            UserModel userModel = GetUserModel(userKey);
            if (userModel.DefaultName == uniqueCode.ToLowerInvariant())
                throw new CantDeleteDefaultCurrencyException();

            if (userModel.UniqueCodes.Count == 1)
                throw new CantDeleteLastCurrencyException();

            Publish(new CurrencyDeleted(uniqueCode) { UserKey = userKey });
        }

        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
        {
            UserModel userModel = GetUserModel(payload.UserKey);
            string uniqueCode = payload.UniqueCode.ToLowerInvariant();
            userModel.UniqueCodes.Remove(uniqueCode);
            userModel.DeletedUniqueCodes.Remove(uniqueCode);
            return Task.CompletedTask;
        }
    }
}
