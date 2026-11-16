//namespace Craft.Domain
//{
//    public interface IBusinessRule<T>
//    {
//        string RuleName { get; }
//        bool Validate(T entity);
//        string ErrorMessage { get; }
//    }
//}

namespace Craft.Domain;

/// <summary>
/// Evaluates a business policy for a candidate value.
/// </summary>
public interface IBusinessRule<T>
{
    /// <summary>
    /// Returns the violation for this evaluation, or null when the rule passes.
    /// </summary>
    BusinessRuleViolation? Validate(T entity);
}
