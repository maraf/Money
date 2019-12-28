using Microsoft.JSInterop;
using Money.Models;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class ProfileStorage : JsonLocalStorage<ProfileModel>
    {
        public ProfileStorage(Json json, IJSRuntime jsRuntime, ILogFactory logFactory) 
            : base(json, jsRuntime, logFactory, "profile")
        { }
    }
}
