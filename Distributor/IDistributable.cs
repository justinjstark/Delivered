using System;

namespace Distributor
{
    public interface IDistributable
    {
        Guid Id { get; }
        string RecipientName { get; }
    }
}
