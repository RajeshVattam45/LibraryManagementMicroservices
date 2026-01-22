using HostelService.Application.DTOs;
using HostelService.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Application.Adapter
{
    public class HostelAdapter : IHostelAdapter
    {
        public Hostel ConvertToEntity ( CreateHostelDto dto )
        {
            return new Hostel
            {
                HostelName = dto.HostelName,
                HostelType = dto.HostelType,
                Description = dto.Description,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode,
                TotalFloors = dto.TotalFloors,
                WardenName = dto.WardenName,
                ContactNumber = dto.ContactNumber,
            };
        }

        public HostelDto ConvertToDto ( Hostel h )
        {
            return new HostelDto
            {
                Id = h.Id,
                HostelName = h.HostelName,
                HostelType = h.HostelType,
                City = h.City,
                ContactNumber = h.ContactNumber,
                IsActive = h.IsActive
            };
        }
    }
}
