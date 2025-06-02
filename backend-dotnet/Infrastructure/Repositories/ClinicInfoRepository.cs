using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class ClinicInfoRepository : IClinicInfoRepository
    {
        private readonly DentalSpaDbContext _context;

        public ClinicInfoRepository(DentalSpaDbContext context)
        {
            _context = context;
        }

        public async Task<ClinicInfo?> GetClinicInfoAsync()
        {
            return await _context.ClinicInfos.FirstOrDefaultAsync();
        }

        public async Task<ClinicInfo> CreateOrUpdateClinicInfoAsync(ClinicInfo clinicInfo)
        {
            var existing = await _context.ClinicInfos.FirstOrDefaultAsync();
            
            if (existing == null)
            {
                clinicInfo.CreatedAt = DateTime.UtcNow;
                clinicInfo.UpdatedAt = DateTime.UtcNow;
                _context.ClinicInfos.Add(clinicInfo);
            }
            else
            {
                existing.Name = clinicInfo.Name;
                existing.Description = clinicInfo.Description;
                existing.Address = clinicInfo.Address;
                existing.Phone = clinicInfo.Phone;
                existing.Email = clinicInfo.Email;
                existing.Website = clinicInfo.Website;
                existing.OpeningTime = clinicInfo.OpeningTime;
                existing.ClosingTime = clinicInfo.ClosingTime;
                existing.WorkingDays = clinicInfo.WorkingDays;
                existing.City = clinicInfo.City;
                existing.State = clinicInfo.State;
                existing.ZipCode = clinicInfo.ZipCode;
                existing.WhatsApp = clinicInfo.WhatsApp;
                existing.Instagram = clinicInfo.Instagram;
                existing.Facebook = clinicInfo.Facebook;
                existing.Logo = clinicInfo.Logo;
                existing.IsActive = clinicInfo.IsActive;
                existing.UpdatedAt = DateTime.UtcNow;
                clinicInfo = existing;
            }

            await _context.SaveChangesAsync();
            return clinicInfo;
        }

        public async Task<bool> DeleteClinicInfoAsync(int id)
        {
            var clinicInfo = await _context.ClinicInfos.FindAsync(id);
            if (clinicInfo == null) return false;

            _context.ClinicInfos.Remove(clinicInfo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}