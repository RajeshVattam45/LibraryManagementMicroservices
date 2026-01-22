using HostelService.Application.DTOs;
using HostelService.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Application.Adapter
{
    public interface IHostelAdapter
    {
        Hostel ConvertToEntity ( CreateHostelDto dto );
        HostelDto ConvertToDto ( Hostel hostel );
    }
}
