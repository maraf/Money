using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.EntityFrameworkCore
{
    public class SchemaOptions
    {
        public string Name { get; set; }
    }

    public class SchemaOptions<TContext> : SchemaOptions
        where TContext : DbContext
    {
        public static readonly SchemaOptions<TContext> Default = new SchemaOptions<TContext>();
    }
}
