using HostelService.Domain.Entites;
using HostelService.Domain.Interfaces;
using HostelService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Infrastructure.Repositories
{
    public class HostelStudentRepository : GenericRepository<HostelStudent>, IHostelStudentRepository
    {
        private readonly HostelDbContext _db;
        public HostelStudentRepository ( HostelDbContext db ) : base ( db )
        {
            _db = db;
        }


        public async Task DeleteAsync ( int id )
        {
            var e = await _db.HostelStudents.FindAsync ( id );
            if (e == null) return;
            e.IsActive = false;
            _db.HostelStudents.Update ( e );
            await _db.SaveChangesAsync ();
        }

        public override async Task<HostelStudent?> GetByIdAsync ( int id )
        {
            return await _db.HostelStudents
                            .AsNoTracking ()
                            .Include ( hs => hs.Hostel )
                            .Include ( hs => hs.Room )
                            .FirstOrDefaultAsync ( hs => hs.Id == id );
        }

        public async Task<IEnumerable<HostelStudent>> GetByHostelAsync ( int hostelId )
        {
            return await _db.HostelStudents
                            .AsNoTracking ()
                            .Where ( hs => hs.HostelId == hostelId )
                            .OrderByDescending ( hs => hs.JoinDate )
                            .ToListAsync ();
        }


        public async Task<bool> IsStudentAssignedAsync ( int studentId )
        {
            return await _db.HostelStudents
                .AnyAsync ( hs => hs.StudentId == studentId && hs.IsActive == true );
        }
    }
}
