using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class Community
    {
        public int? Id { get; set; }
        public string? Topic { get; set; }
        public string? Name { get; set; }
        public int? AuthorId { get; set; }
        public string? Description { get; set; }
    }
}
