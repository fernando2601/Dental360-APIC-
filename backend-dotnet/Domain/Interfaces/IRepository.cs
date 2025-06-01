using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }

    public interface IUserRepository : IRepository<Entities.User>
    {
        Task<Entities.User?> GetByUsernameAsync(string username);
        Task<Entities.User?> GetByEmailAsync(string email);
        Task<Entities.User?> GetByResetTokenAsync(string token);
    }

    public interface IClientRepository : IRepository<Entities.Client>
    {
        Task<IEnumerable<Entities.Client>> SearchAsync(string searchTerm);
        Task<Entities.Client?> GetByEmailAsync(string email);
    }

    public interface IAppointmentRepository : IRepository<Entities.Appointment>
    {
        Task<IEnumerable<Entities.Appointment>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<Entities.Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Entities.Appointment>> GetByStaffIdAsync(int staffId);
        Task<IEnumerable<Entities.Appointment>> GetTodayAppointmentsAsync();
    }

    public interface IServiceRepository : IRepository<Entities.Service>
    {
        Task<IEnumerable<Entities.Service>> GetByCategoryAsync(string category);
        Task<IEnumerable<Entities.Service>> GetActiveServicesAsync();
    }

    public interface IStaffRepository : IRepository<Entities.Staff>
    {
        Task<IEnumerable<Entities.Staff>> GetActiveStaffAsync();
        Task<Entities.Staff?> GetByUserIdAsync(int userId);
    }

    public interface IInventoryRepository : IRepository<Entities.Inventory>
    {
        Task<IEnumerable<Entities.Inventory>> GetLowStockItemsAsync();
        Task<IEnumerable<Entities.Inventory>> GetExpiringSoonItemsAsync();
        Task<IEnumerable<Entities.Inventory>> GetByCategoryAsync(string category);
    }

    public interface IFinancialTransactionRepository : IRepository<Entities.FinancialTransaction>
    {
        Task<IEnumerable<Entities.FinancialTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Entities.FinancialTransaction>> GetByTypeAsync(string type);
        Task<decimal> GetTotalIncomeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalExpenseAsync(DateTime startDate, DateTime endDate);
    }

    public interface IPackageRepository : IRepository<Entities.Package>
    {
        Task<IEnumerable<Entities.Package>> GetActivePackagesAsync();
    }

    public interface IClientPackageRepository : IRepository<Entities.ClientPackage>
    {
        Task<IEnumerable<Entities.ClientPackage>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<Entities.ClientPackage>> GetActivePackagesAsync();
    }

    public interface IBeforeAfterRepository : IRepository<Entities.BeforeAfter>
    {
        Task<IEnumerable<Entities.BeforeAfter>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<Entities.BeforeAfter>> GetPublicGalleryAsync();
    }

    public interface ILearningAreaRepository : IRepository<Entities.LearningArea>
    {
        Task<IEnumerable<Entities.LearningArea>> GetByCategoryAsync(string category);
        Task<IEnumerable<Entities.LearningArea>> GetPublishedAsync();
    }

    public interface IClinicInfoRepository : IRepository<Entities.ClinicInfo>
    {
        Task<Entities.ClinicInfo?> GetClinicInfoAsync();
    }
}