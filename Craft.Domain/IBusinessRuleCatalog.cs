namespace Craft.Domain
{
    public interface IBusinessRuleCatalog
    {
        // Register an atomic rule
        void RegisterAtomicRule<T>(IBusinessRule<T> rule);

        // Register a cross-entity rule
        void RegisterCrossEntityRule<T>(IBusinessRule<IEnumerable<T>> rule);

        // Validate a single entity in isolation
        Dictionary<string, string> ValidateAtomic<T>(T entity);

        // Validate a set of related entities
        Dictionary<string, string> ValidateCrossEntity<T>(IEnumerable<T> entities);
    }
}