using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.DTOs
{
    public class CreateAuthorDto
    {
        public string Name { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
    }

}
