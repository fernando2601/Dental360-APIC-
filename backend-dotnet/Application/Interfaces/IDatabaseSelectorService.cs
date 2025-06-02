namespace DentalSpa.Application.Interfaces
{
    public interface IDatabaseSelectorService
    {
        string GetPrimaryDatabase();
        string GetSecondaryDatabase();
        bool IsPostgreSqlAvailable();
        bool IsSqlServerAvailable();
        Task<bool> TestConnectionAsync(string connectionType);
    }
}