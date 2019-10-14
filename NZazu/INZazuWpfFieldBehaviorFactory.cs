using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuWpfFieldBehaviorFactory
    {
        INZazuWpfFieldBehavior CreateFieldBehavior(BehaviorDefinition behaviorDefinition);
    }
}