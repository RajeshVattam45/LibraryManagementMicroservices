using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Domain.Entities
{
    public class Publisher
    {
        public int Id { get; set; }

        // Basic Info
        public string Name { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Location
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        // Status
        public bool IsActive { get; set; } = true;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public ICollection<Book> Books { get; set; } = new List<Book> ();
    }

}
