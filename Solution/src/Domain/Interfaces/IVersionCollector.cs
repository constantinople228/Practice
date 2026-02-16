using Domain.Models;

namespace Domain.Interfaces;

public interface IVersionCollector
{
    string ProductName { get; }
    Task<IReadOnlyCollection<CollectedVersion>> CollectAsync();
}