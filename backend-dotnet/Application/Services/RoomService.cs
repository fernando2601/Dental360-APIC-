using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repository;
        public RoomService(IRoomRepository repository)
        {
            _repository = repository;
        }
        public Task<IEnumerable<Room>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Room?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Room> CreateAsync(Room room) => _repository.CreateAsync(room);
        public Task<Room?> UpdateAsync(int id, Room room) => _repository.UpdateAsync(id, room);
        public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
} 