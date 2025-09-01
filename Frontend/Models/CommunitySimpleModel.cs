using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class CommunitySimpleModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Topic { get; set; }
        public string? BackgroundColor { get; set; }
        public int? MemberCount { get; set; }
    }
}
