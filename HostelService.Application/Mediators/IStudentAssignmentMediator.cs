using HostelService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Application.Mediators
{
    public interface IStudentAssignmentMediator
    {
        Task<HostelStudentDto> AssignStudentAsync ( CreateHostelStudentDto dto );
    }
}
