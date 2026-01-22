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
    public class HostelRepository : IHostelRepository
    {
        private readonly HostelDbContext _db;

        public HostelRepository ( HostelDbContext db )
        {
            _db = db;
        }

        public async Task<Hostel> AddAsync ( Hostel hostel )
        {
            await _db.Hostels.AddAsync ( hostel );
            await _db.SaveChangesAsync ();
            return hostel;
        }

        public async Task<int> CountAsync ( string? search = null )
        {
            var q = _db.Hostels.AsQueryable ();
            if (!string.IsNullOrWhiteSpace ( search ))
                q = q.Where ( h => h.HostelName.Contains ( search ) || h.City.Contains ( search ) );
            return await q.CountAsync ();
        }

        public async Task DeleteAsync ( int id )
        {
            var entity = await _db.Hostels.FindAsync ( id );
            if (entity == null) return;
            // Soft-delete by default could be:
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            _db.Hostels.Update ( entity );
            await _db.SaveChangesAsync ();
        }

        public async Task<IEnumerable<Hostel>> GetAllAsync ( )
        {
            return await _db.Hostels
                .AsNoTracking ()
                .Include ( h => h.Rooms )
                .ToListAsync ();
        }

        public async Task<Hostel?> GetByIdAsync ( int id )
        {
            return await _db.Hostels
                            .AsNoTracking ()
                            .Include ( h => h.Rooms )
                            .FirstOrDefaultAsync ( h => h.Id == id );
        }

        public async Task<IEnumerable<Hostel>> GetWithRoomsAsync ( int hostelId )
        {
            // Example: raw SQL projection (advanced)
            var sql = @"SELECT * FROM Hostels WHERE Id = {0}";
            var hostels = await _db.Hostels.FromSqlRaw ( sql, hostelId ).Include ( h => h.Rooms ).ToListAsync ();
            return hostels;
        }

        public async Task UpdateAsync ( Hostel hostel )
        {
            hostel.UpdatedAt = DateTime.UtcNow;
            _db.Hostels.Update ( hostel );
            await _db.SaveChangesAsync ();
        }

        public async Task<bool> RoomExistsInHostelAsync ( int hostelId, string roomNumber )
        {
            return await _db.Rooms
                .AnyAsync ( r => r.HostelId == hostelId && r.RoomNumber == roomNumber );
        }

        public async Task<bool> IsContactNumberUsedAsync ( string contactNumber, int? excludeHostelId = null )
        {
            if (string.IsNullOrWhiteSpace ( contactNumber ))
                return false;

            // Normalize: trim and remove extra whitespace. You may also normalize formatting (e.g. remove +91, dashes) if you accept those.
            var normalized = contactNumber.Trim ();

            // Query any hostels that have this contact number but optionally exclude a specific hostel (useful during update)
            return await _db.Hostels
                .AsNoTracking ()
                .AnyAsync ( h => h.ContactNumber == normalized && (excludeHostelId == null || h.Id != excludeHostelId.Value) );
        }

        public async Task<Hostel?> GetByNameAsync ( string hostelName )
        {
            if (string.IsNullOrWhiteSpace ( hostelName ))
                return null;

            var normalized = hostelName.Trim ();

            // Case-insensitive comparison: convert both sides to lower-case.
            // NOTE: This will be translated to SQL LOWER(...) so it is server-side.
            var nameLower = normalized.ToLower ();

            return await _db.Hostels
                .AsNoTracking ()
                .FirstOrDefaultAsync ( h => EF.Functions.Like ( h.HostelName.ToLower (), nameLower ) );
        }
    }
}
