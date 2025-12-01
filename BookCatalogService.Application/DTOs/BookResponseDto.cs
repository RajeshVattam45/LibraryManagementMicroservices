using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.DTOs
{
    public class BookResponseDto
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string ISBN13 { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string AuthorName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;

        public int PublishedYear { get; set; }
        public string Edition { get; set; } = string.Empty;
        public string Language { get; set; } = "English";
        public string Format { get; set; } = "Paperback";
        public decimal Price { get; set; }

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string ShelfLocation { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
    }

}
