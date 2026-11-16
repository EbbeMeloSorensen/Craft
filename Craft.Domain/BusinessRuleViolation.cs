namespace Craft.Domain;

/// <summary>
/// Describes a failed business rule and identifies the affected member.
/// </summary>
public sealed record BusinessRuleViolation(string MemberName, string Message);
