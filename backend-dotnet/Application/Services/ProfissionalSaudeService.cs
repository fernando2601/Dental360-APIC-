using DentalSpa.Application.DTOs;
using DentalSpa.Application.Interfaces;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class ProfissionalSaudeService : IProfissionalSaudeService
    {
        private readonly IProfissionalSaudeRepository _repo;
        public ProfissionalSaudeService(IProfissionalSaudeRepository repo)
        {
            _repo = repo;
        }

        public async Task<ProfissionalSaudeResponse?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<ProfissionalSaudeResponse?> GetByStaffIdAsync(int staffId)
        {
            var entity = await _repo.GetByStaffIdAsync(staffId);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<IEnumerable<ProfissionalSaudeResponse>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToResponse);
        }

        public async Task<ProfissionalSaudeResponse> CreateAsync(ProfissionalSaudeCreateRequest request)
        {
            var entity = new ProfissionalSaude
            {
                StaffId = request.StaffId,
                RegistroProfissional = request.RegistroProfissional,
                TipoRegistro = request.TipoRegistro,
                Conselho = request.Conselho,
                UF = request.UF,
                DataRegistro = request.DataRegistro,
                Especialidade = request.Especialidade
            };
            var created = await _repo.CreateAsync(entity);
            return MapToResponse(created);
        }

        public async Task<ProfissionalSaudeResponse?> UpdateAsync(int id, ProfissionalSaudeCreateRequest request)
        {
            var entity = new ProfissionalSaude
            {
                Id = id,
                StaffId = request.StaffId,
                RegistroProfissional = request.RegistroProfissional,
                TipoRegistro = request.TipoRegistro,
                Conselho = request.Conselho,
                UF = request.UF,
                DataRegistro = request.DataRegistro,
                Especialidade = request.Especialidade
            };
            var updated = await _repo.UpdateAsync(id, entity);
            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        private ProfissionalSaudeResponse MapToResponse(ProfissionalSaude entity)
        {
            return new ProfissionalSaudeResponse
            {
                Id = entity.Id,
                StaffId = entity.StaffId,
                RegistroProfissional = entity.RegistroProfissional,
                TipoRegistro = entity.TipoRegistro,
                Conselho = entity.Conselho,
                UF = entity.UF,
                DataRegistro = entity.DataRegistro,
                Especialidade = entity.Especialidade
            };
        }
    }
} 