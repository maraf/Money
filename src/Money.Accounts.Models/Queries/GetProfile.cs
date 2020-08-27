using Money.Models;
using Money.Models.Queries;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// Gets a user profile information.
    /// </summary>
    public class GetProfile : UserQuery, IQuery<ProfileModel>
    { }
}
