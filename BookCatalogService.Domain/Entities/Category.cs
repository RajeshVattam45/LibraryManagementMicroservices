using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        // Basic Info
        public string Name { get; set; } = string.Empty;       // Fiction, Science, History
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;       // FIC, SCI, HIS

        // Hierarchy (optional)
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        // Status
        public bool IsActive { get; set; } = true;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public ICollection<Book> Books { get; set; } = new List<Book> ();
    }

}
