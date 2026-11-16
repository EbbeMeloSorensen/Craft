//namespace Craft.Domain
//{
//    public abstract class BusinessRuleCatalogBase : IBusinessRuleCatalog
//    {
//        private readonly Dictionary<Type, List<object>> _atomicRules = new Dictionary<Type, List<object>>();
//        private readonly List<object> _crossEntityRules = new List<object>();

//        public void RegisterAtomicRule<T>(
//            IBusinessRule<T> rule)
//        {
//            if (!_atomicRules.ContainsKey(typeof(T)))
//            {
//                _atomicRules[typeof(T)] = new List<object>();
//            }

//            _atomicRules[typeof(T)].Add(rule);
//        }

//        public void RegisterCrossEntityRule<T>(
//            IBusinessRule<IEnumerable<T>> rule)
//        {
//            _crossEntityRules.Add(rule);
//        }

//        public Dictionary<string, string> ValidateAtomic<T>(
//            T entity)
//        {
//            if (!_atomicRules.ContainsKey(typeof(T))) return new Dictionary<string, string>();

//            return _atomicRules[typeof(T)]
//                .Cast<IBusinessRule<T>>()
//                .Where(rule => !rule.Validate(entity))
//                .ToDictionary(rule => rule.RuleName, rule => rule.ErrorMessage);
//        }

//        public Dictionary<string, string> ValidateCrossEntity<T>(
//            IEnumerable<T> entities)
//        {
//            return _crossEntityRules
//                .OfType<IBusinessRule<IEnumerable<T>>>()
//                .Where(rule => !rule.Validate(entities))
//                .ToDictionary(rule => rule.RuleName, rule => rule.ErrorMessage);
//        }
//    }
//}

namespace Craft.Domain;

/// <summary>
/// Evaluates rules registered by a domain catalog.
/// </summary>
public abstract class BusinessRuleCatalogBase : IBusinessRuleCatalog
{
    private readonly Dictionary<Type, List<object>> _atomicRules = new Dictionary<Type, List<object>>();

    private readonly List<object> _crossEntityRules = new List<object>();

    /// <inheritdoc />
    public void RegisterAtomicRule<T>(
        IBusinessRule<T> rule)
    {
        if (!_atomicRules.ContainsKey(typeof(T)))
        {
            _atomicRules[typeof(T)] = new List<object>();
        }

        _atomicRules[typeof(T)].Add(rule);
    }

    /// <inheritdoc />
    public void RegisterCrossEntityRule<T>(
        IBusinessRule<IEnumerable<T>> rule)
    {
        _crossEntityRules.Add(rule);
    }

    /// <inheritdoc />
    public IReadOnlyList<BusinessRuleViolation> ValidateAtomic<T>(
        T entity)
    {
        if (!_atomicRules.ContainsKey(typeof(T)))
        {
            return [];
        }

        return _atomicRules[typeof(T)]
            .Cast<IBusinessRule<T>>()
            .Select(rule => rule.Validate(entity))
            .OfType<BusinessRuleViolation>()
            .ToArray();
    }

    /// <inheritdoc />
    public IReadOnlyList<BusinessRuleViolation> ValidateCrossEntity<T>(
        IEnumerable<T> entities)
    {
        return _crossEntityRules
            .OfType<IBusinessRule<IEnumerable<T>>>()
            .Select(rule => rule.Validate(entities))
            .OfType<BusinessRuleViolation>()
            .ToArray();
    }
}
