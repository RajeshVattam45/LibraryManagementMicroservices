using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.DTOs
{
    public class UpdateBookDto : CreateBookDto
    {
        public int AvailableCopies { get; set; }
    }
}
