using HostelService.Application.Adapter;
using HostelService.Application.DTOs;
using HostelService.Domain.Entites;
using HostelService.Domain.Enums;
using HostelService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Application.Services
{
    public class HostelAppService : IHostelAppService
    {
        private readonly IHostelRepository _repo;
        private readonly IStudentValidationService _studentValidator;
        private readonly IHostelAdapter _adapter;

        public HostelAppService ( IHostelRepository repo, IStudentValidationService studentValidator, IHostelAdapter adapter )
        {
            _repo = repo;
            _studentValidator = studentValidator;
            _adapter = adapter;
        }

        public async Task<HostelDto?> GetByIdAsync ( int id )
        {
            var entity = await _repo.GetByIdAsync ( id );
            return entity == null ? null : _adapter.ConvertToDto ( entity );
        }


        public async Task<IEnumerable<HostelDto>> GetAllAsync ( )
        {
            var hostels = await _repo.GetAllAsync ();
            return hostels.Select ( _adapter.ConvertToDto );
        }

        public async Task<HostelDto> CreateAsync ( CreateHostelDto dto )
        {
            await ValidateCreateHostel ( dto );

            if (await _repo.GetByNameAsync ( dto.HostelName ) != null)
                throw new InvalidOperationException ( "Hostel name already exists." );

            if (await _repo.IsContactNumberUsedAsync ( dto.ContactNumber ))
                throw new InvalidOperationException ( "Contact number already used." );

            // Use Adapter instead of manually mapping
            var hostel = _adapter.ConvertToEntity ( dto );

            var added = await _repo.AddAsync ( hostel );

            // Convert Entity → DTO via Adapter
            return _adapter.ConvertToDto ( added );
        }

        public async Task UpdateAsync ( UpdateHostelDto dto )
        {
            var entity = await _repo.GetByIdAsync ( dto.Id );
            if (entity == null)
                throw new Exception ( "Hostel not found" );

            if (dto.TotalFloors.HasValue && dto.TotalFloors <= 0)
                throw new ArgumentException ( "Total floors must be greater than 0." );

            if (dto.ContactNumber?.Length > 0 && dto.ContactNumber.Length != 10)
                throw new ArgumentException ( "Invalid contact number." );

            // patch update
            entity.HostelName = dto.HostelName ?? entity.HostelName;
            entity.HostelType = dto.HostelType ?? entity.HostelType;
            entity.Description = dto.Description ?? entity.Description;

            entity.AddressLine1 = dto.AddressLine1 ?? entity.AddressLine1;
            entity.AddressLine2 = dto.AddressLine2 ?? entity.AddressLine2;
            entity.City = dto.City ?? entity.City;
            entity.State = dto.State ?? entity.State;
            entity.Pincode = dto.Pincode ?? entity.Pincode;

            entity.TotalFloors = dto.TotalFloors ?? entity.TotalFloors;

            entity.WardenName = dto.WardenName ?? entity.WardenName;
            entity.ContactNumber = dto.ContactNumber ?? entity.ContactNumber;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;

            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync ( entity );
        }

        public async Task DeleteAsync ( int id )
        {
            await _repo.DeleteAsync ( id );
        }

        // Validator method
        private async Task ValidateCreateHostel ( CreateHostelDto dto )
        {
            // Business validations
            if (string.IsNullOrWhiteSpace ( dto.HostelName ))
                throw new ArgumentException ( "Hostel Name is required." );

            if (dto.TotalFloors <= 0)
                throw new ArgumentException ( "Total floors must be greater than 0." );

            if (!Enum.TryParse<HostelTypeEnum> ( dto.HostelType, true, out var hostelType ))
                throw new ArgumentException ( "Invalid hostel type." );

            if (!dto.ContactNumber.All ( char.IsDigit ))
                throw new ArgumentException ( "Contact number must contain only digits." );

            if (dto.Pincode.Length != 6 || !dto.Pincode.All ( char.IsDigit ))
                throw new ArgumentException ( "Invalid pincode." );

            if (string.IsNullOrWhiteSpace ( dto.WardenName ) || dto.WardenName.Length < 3)
                throw new ArgumentException ( "Warden name is invalid." );

            if (dto.TotalFloors > 20)
                throw new ArgumentException ( "Total floors seems unrealistic." );

            if (dto.Description?.Length > 500)
                throw new ArgumentException ( "Description cannot exceed 500 characters." );
        }
    }
}
