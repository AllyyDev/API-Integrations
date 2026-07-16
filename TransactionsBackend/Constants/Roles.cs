namespace TransactionsBackend.Constants;

/// <summary>
/// Application role names used for JWT claims and authorization policies.
/// </summary>
public static class Roles
{
    /// <summary>Full access to all users and transactions.</summary>
    public const string Admin = "Admin";

    /// <summary>Access limited to own profile and own transactions.</summary>
    public const string User = "User";
}
