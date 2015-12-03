using System;

namespace Verdeler
{
    public interface IDistributable
    {
        Guid Id { get; }
        string RecipientName { get; }
    }
}
