using HostelService.Application.DTOs;
using HostelService.Application.Services;
using HostelService.Domain.Entites;
using HostelService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Application.Mediators
{
    public class StudentAssignmentMediator : IStudentAssignmentMediator
    {
        private readonly IHostelStudentRepository _studentRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IStudentValidationService _studentValidator;

        public StudentAssignmentMediator (
            IHostelStudentRepository studentRepo,
            IRoomRepository roomRepo,
            IStudentValidationService studentValidator )
        {
            _studentRepo = studentRepo;
            _roomRepo = roomRepo;
            _studentValidator = studentValidator;
        }

        public async Task<HostelStudentDto> AssignStudentAsync ( CreateHostelStudentDto dto )
        {
            //// 1. Validate student exists (from external School service)
            //var studentExists = await _studentValidator.StudentExistsAsync ( dto.StudentId );
            //if (!studentExists)
            //    throw new Exception ( "Student does not exist in the School system." );

            // 2. Check if already assigned
            var alreadyExist = await _studentRepo.IsStudentAssignedAsync ( dto.StudentId );
            if (alreadyExist)
                throw new Exception ( "Student is already assigned to a hostel." );

            // 3. Room validation
            var room = await _roomRepo.GetByIdAsync ( dto.RoomId );
            if (room == null)
                throw new Exception ( "Room not found." );

            if (!room.IsActive)
                throw new Exception ( "Room is inactive." );

            if (room.HostelId != dto.HostelId)
                throw new Exception ( "Room does not belong to the selected hostel." );

            if (room.OccupiedBeds >= room.TotalBeds)
                throw new Exception ( "Room is full." );

            // 4. Create entity
            var entity = new HostelStudent
            {
                StudentId = dto.StudentId,
                HostelId = dto.HostelId,
                RoomId = dto.RoomId,
                JoinDate = dto.JoinDate,
                IsActive = true
            };

            // 5. Save to DB
            await _studentRepo.AddAsync ( entity );

            // 6. Update room capacity
            room.OccupiedBeds++;
            await _roomRepo.UpdateAsync ( room );

            // 7. Convert to DTO (normally you’d use Adapter)
            return new HostelStudentDto
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                HostelId = entity.HostelId,
                RoomId = entity.RoomId,
                JoinDate = entity.JoinDate,
                IsActive = entity.IsActive,
                RoomNumber = room.RoomNumber
            };
        }
    }
}
