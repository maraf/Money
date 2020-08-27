using Money.Models.Queries;
using Neptuo;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ApiTokenValidator : ApiAuthenticationStateProvider.ITokenValidator
    {
        private readonly IQueryDispatcher queries;
        private readonly ILog log;

        public ApiTokenValidator(IQueryDispatcher queries, ILogFactory logFactory)
        {
            Ensure.NotNull(queries, "queries");
            Ensure.NotNull(logFactory, "logFactory");
            this.queries = queries;
            this.log = logFactory.Scope("TokenValidator");
        }

        public async Task<bool> ValidateAsync(string token)
        {
            try
            {
                // Unauthorized exception is processed globally, don't need to do anyting here.
                log.Debug($"Validation token using '{nameof(GetProfile)}' query.");
                var profile = await queries.QueryAsync(new GetProfile());
                log.Debug($"Result '{profile}'.");
                return profile != null;
            }
            catch (ServerNotRespondingException e)
            {
                log.Debug($"Catch '{nameof(ServerNotRespondingException)}'.");
                return !String.IsNullOrEmpty(token);
            }
        }
    }
}
