namespace Craft.Domain
{
    public interface IBusinessRule<T>
    {
        string RuleName { get; }
        bool Validate(T entity);
        string ErrorMessage { get; }
    }
}