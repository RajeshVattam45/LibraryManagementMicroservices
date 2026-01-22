using HostelService.Application.DTOs;
using HostelService.Domain.Entites;
using HostelService.Domain.Enums;
using HostelService.Domain.Factories;
using HostelService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Application.Services
{
    public class RoomAppService : IRoomAppService
    {
        private readonly IRoomRepository _repo;
        private readonly IHostelRepository _hostelRepo;

        public RoomAppService ( IRoomRepository repo, IHostelRepository hostelRepository )
        {
            _repo = repo;
            _hostelRepo = hostelRepository;
        }

        public async Task<RoomDto?> GetByIdAsync ( int id )
        {
            var room = await _repo.GetByIdAsync ( id );
            if (room == null)
                return null;

            return ToDto ( room );
        }

        public async Task<IEnumerable<RoomDto>> GetAllByHostelAsync ( int hostelId )
        {
            var rooms = await _repo.GetAllByHostelAsync ( hostelId );
            return rooms.Select ( r => ToDto ( r ) );
        }

        public async Task<RoomDto> AddAsync ( CreateRoomDto dto )
        {
            var roomExist = await _hostelRepo.GetByIdAsync ( dto.HostelId );
            if(roomExist == null)
                throw new Exception ( "Hostel not found" );

            if(dto.TotalBeds <= 0)
                throw new Exception ( "Total beds must be greater than zero" );

            if (dto.OccupiedBeds < 0 || dto.OccupiedBeds > dto.TotalBeds)
                throw new Exception ( "Occupied beds cannot exceed total beds." );

            if(dto.FeePerBed < 0)
                throw new Exception ( "Fee per bed cannot be negative." );

            if (await _hostelRepo.RoomExistsInHostelAsync ( dto.HostelId, dto.RoomNumber ))
                throw new Exception ( "Room number already exists in this hostel." );

            if (!Enum.TryParse ( dto.RoomType, true, out RoomTypeEnum roomTypeEnum ))
                throw new Exception ( "Invalid room type" );

            // Convert App DTO → Factory DTO
            var factoryDto = new CreateRoomDtoData
            {
                HostelId = dto.HostelId,
                RoomNumber = dto.RoomNumber,
                FloorNumber = dto.FloorNumber,
                TotalBeds = dto.TotalBeds,
                OccupiedBeds = dto.OccupiedBeds,
                FeePerBed = dto.FeePerBed
            };

            // Convert string → enum safely
            //Enum.TryParse ( dto.RoomType, out RoomTypeEnum roomTypeEnum );

            // Use Factory Method here
            var room = RoomFactory.Create ( roomTypeEnum, factoryDto );

            await _repo.AddAsync ( room );

            return ToDto ( room );
        }

        public async Task UpdateAsync ( int id, UpdateRoomDto dto )
        {
            var room = await _repo.GetByIdAsync ( id );
            if (room == null)
                throw new Exception ( "Room not found" );

            room.RoomNumber = dto.RoomNumber;
            room.FloorNumber = dto.FloorNumber;
            room.RoomType = dto.RoomType;
            room.TotalBeds = dto.TotalBeds;
            room.OccupiedBeds = dto.OccupiedBeds;
            room.FeePerBed = dto.FeePerBed;
            room.IsActive = dto.IsActive;

            await _repo.UpdateAsync ( room );
        }

        public async Task DeleteAsync ( int id )
        {
            await _repo.DeleteAsync ( id );
        }

        private RoomDto ToDto ( Room r ) =>
            new RoomDto
            {
                Id = r.Id,
                HostelId = r.HostelId,
                RoomNumber = r.RoomNumber,
                FloorNumber = r.FloorNumber,
                RoomType = r.RoomType,
                TotalBeds = r.TotalBeds,
                OccupiedBeds = r.OccupiedBeds,
                FeePerBed = r.FeePerBed,
                AvailableBeds = r.AvailableBeds,
                IsActive = r.IsActive
            };
    }
}
