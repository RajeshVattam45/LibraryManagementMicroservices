using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }

        // Identifiers
        public string ISBN { get; set; } = string.Empty;
        public string ISBN13 { get; set; } = string.Empty;

        // Basic Info
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Foreign Keys
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;

        // Publishing Info
        public int PublishedYear { get; set; }
        public string Edition { get; set; } = string.Empty;
        public string Language { get; set; } = "English";     // English, Hindi, Telugu, etc.
        public string Format { get; set; } = "Paperback";     // Paperback, Hardcover, eBook
        public decimal Price { get; set; }

        // Copies / Availability
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string ShelfLocation { get; set; } = string.Empty;  // e.g., A3-Row4

        // Search Enhancers
        public string Tags { get; set; } = string.Empty;      // comma separated

        // Visual
        public string CoverImageUrl { get; set; } = string.Empty;

        // Status
        public bool IsActive { get; set; } = true;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
