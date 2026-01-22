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
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        private readonly HostelDbContext _db;

        public RoomRepository ( HostelDbContext db ) : base ( db )
        {
            _db = db;
        }

        // Only custom method (not part of generic repo)
        public async Task<IEnumerable<Room>> GetAllByHostelAsync ( int hostelId )
        {
            return await _db.Rooms
                            .AsNoTracking ()
                            .Where ( r => r.HostelId == hostelId )
                            .OrderBy ( r => r.FloorNumber )
                            .ToListAsync ();
        }

        public override async Task<Room?> GetByIdAsync ( int id )
        {
            return await _db.Rooms.AsNoTracking ().FirstOrDefaultAsync ( r => r.Id == id );
        }

        // Override DeleteAsync because you want SOFT DELETE
        public override async Task DeleteAsync ( int id )
        {
            var r = await _db.Rooms.FindAsync ( id );
            if (r == null) return;

            r.IsActive = false;
            r.UpdatedAt = DateTime.UtcNow;

            _db.Rooms.Update ( r );
            await _db.SaveChangesAsync ();
        }
    }
}
