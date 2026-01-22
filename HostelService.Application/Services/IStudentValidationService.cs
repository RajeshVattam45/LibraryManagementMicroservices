using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Application.Services
{
    public interface IStudentValidationService
    {
        Task<bool> StudentExistsAsync ( int studentId );
    }
}
