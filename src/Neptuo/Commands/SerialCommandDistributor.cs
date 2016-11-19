using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// The implementation of <see cref="ICommandDistributor"/> which distributes all comands to the same 'queue'.
    /// </summary>
    public class SerialCommandDistributor : ICommandDistributor
    {
        private readonly DistributionObject distributor = new DistributionObject();

        public object Distribute(object command)
        {
            return distributor;
        }

        private class DistributionObject
        {
            private readonly Guid guid = Guid.NewGuid();

            public override bool Equals(object obj)
            {
                DistributionObject other = obj as DistributionObject;
                if (other == null)
                    return false;

                return guid.Equals(other.guid);
            }

            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 23 + guid.GetHashCode();
                return hash;
            }
        }
    }
}
