using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAgendaRepository _agendaRepository;

        public AgendaService(IAgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _agendaRepository.GetAllAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _agendaRepository.GetByIdAsync(id);
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            // Verificar conflito de horário para o mesmo profissional
            var conflicts = await _agendaRepository.GetByStaffAndTimeRangeAsync(appointment.StaffId, appointment.StartTime, appointment.EndTime);
            if (conflicts.Any())
            {
                throw new InvalidOperationException("Já existe um agendamento para este profissional neste horário.");
            }
            return await _agendaRepository.CreateAsync(appointment);
        }

        public async Task<Appointment?> UpdateAsync(int id, Appointment appointment)
        {
            return await _agendaRepository.UpdateAsync(id, appointment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _agendaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Appointment>> SearchAsync(string searchTerm)
        {
            return await _agendaRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Implementação básica - retorna todos os agendamentos
            return await _agendaRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            // Implementação básica - retorna todos os agendamentos
            return await _agendaRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStaffAsync(int staffId)
        {
            // Implementação básica - retorna todos os agendamentos
            return await _agendaRepository.GetAllAsync();
        }

        public async Task<bool> ConfirmAppointmentAsync(int id)
        {
            var appointment = await _agendaRepository.GetByIdAsync(id);
            if (appointment == null) return false;
            
            appointment.Status = "confirmed";
            appointment.UpdatedAt = DateTime.UtcNow;
            
            var updated = await _agendaRepository.UpdateAsync(id, appointment);
            return updated != null;
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _agendaRepository.GetByIdAsync(id);
            if (appointment == null) return false;
            
            appointment.Status = "cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;
            
            var updated = await _agendaRepository.UpdateAsync(id, appointment);
            return updated != null;
        }

        public async Task<bool> CompleteAppointmentAsync(int id)
        {
            var appointment = await _agendaRepository.GetByIdAsync(id);
            if (appointment == null) return false;
            
            appointment.Status = "completed";
            appointment.UpdatedAt = DateTime.UtcNow;
            
            var updated = await _agendaRepository.UpdateAsync(id, appointment);
            return updated != null;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStaffAndDateAsync(int staffId, DateTime date)
        {
            var all = await _agendaRepository.GetAllAsync();
            return all.Where(a => a.StaffId == staffId && a.StartTime.Date == date.Date);
        }
    }
} 