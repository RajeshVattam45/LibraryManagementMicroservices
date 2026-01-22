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
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly HostelDbContext _db;
        protected readonly DbSet<T> _set;

        public GenericRepository ( HostelDbContext db )
        {
            _db = db;
            _set = db.Set<T> ();
        }

        public async Task<T> AddAsync ( T entity )
        {
            await _set.AddAsync ( entity );
            await _db.SaveChangesAsync ();
            return entity;
        }

        public virtual async Task<T?> GetByIdAsync ( int id )
        {
            return await _set.FindAsync ( id );
        }

        public async Task UpdateAsync ( T entity )
        {
            _set.Update ( entity );
            await _db.SaveChangesAsync ();
        }

        public virtual async Task DeleteAsync ( int id )
        {
            var entity = await _set.FindAsync ( id );
            if (entity != null)
            {
                _set.Remove ( entity );
                await _db.SaveChangesAsync ();
            }
        }

    }

}
