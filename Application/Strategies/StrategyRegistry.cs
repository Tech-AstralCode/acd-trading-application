using Domain.Strategies;

namespace Application.Strategies;

public interface IStrategyRegistry
{ IStrategy? Resolve(string name); }

public sealed class StrategyRegistry : IStrategyRegistry
{
    private readonly IEnumerable<IStrategy> _strategies;
    public StrategyRegistry(IEnumerable<IStrategy> strategies) => _strategies = strategies;
    public IStrategy? Resolve(string name) => _strategies.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}