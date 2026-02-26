namespace DealBite.Application.Common.Exceptions
{
    /// <summary>
    /// Validációs hiba (pl. hibás input, üzleti szabály megsértése).
    /// → 400 Bad Request
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    /// <summary>
    /// Autentikációs hiba (pl. hibás jelszó, nem létező felhasználó).
    /// → 401 Unauthorized
    /// </summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message) { }
    }

    /// <summary>
    /// Nem található erőforrás.
    /// → 404 Not Found
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string entity, Guid id)
            : base($"{entity} nem található: {id}") { }
    }

    /// <summary>
    /// Jogosultsági hiba (pl. más user listáját próbálja módosítani).
    /// → 403 Forbidden
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Nincs jogosultságod ehhez a művelethez.") : base(message) { }
    }
}