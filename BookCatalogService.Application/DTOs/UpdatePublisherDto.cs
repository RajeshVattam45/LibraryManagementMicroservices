using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.DTOs
{
    public class UpdatePublisherDto : CreatePublisherDto
    {
        public bool IsActive { get; set; }
    }
}
