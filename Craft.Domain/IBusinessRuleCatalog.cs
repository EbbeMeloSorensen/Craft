//namespace Craft.Domain
//{
//    public interface IBusinessRuleCatalog
//    {
//        // Register an atomic rule
//        void RegisterAtomicRule<T>(IBusinessRule<T> rule);

//        // Register a cross-entity rule
//        void RegisterCrossEntityRule<T>(IBusinessRule<IEnumerable<T>> rule);

//        // Validate a single entity in isolation
//        Dictionary<string, string> ValidateAtomic<T>(T entity);

//        // Validate a set of related entities
//        Dictionary<string, string> ValidateCrossEntity<T>(IEnumerable<T> entities);
//    }
//}

namespace Craft.Domain;

/// <summary>
/// Registers and evaluates rules for individual entities or related sets.
/// </summary>
public interface IBusinessRuleCatalog
{
    /// <summary>
    /// Register an atomic rule.
    /// </summary>
    void RegisterAtomicRule<T>(IBusinessRule<T> rule);

    /// <summary>
    /// Register a cross-entity rule.
    /// </summary>
    void RegisterCrossEntityRule<T>(IBusinessRule<IEnumerable<T>> rule);

    /// <summary>
    /// Validate a single entity in isolation.
    /// </summary>
    IReadOnlyList<BusinessRuleViolation> ValidateAtomic<T>(T entity);

    /// <summary>
    /// Validate a set of related entities.
    /// </summary>
    IReadOnlyList<BusinessRuleViolation> ValidateCrossEntity<T>(IEnumerable<T> entities);
}
