using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class PostCreated
    {
        public Guid Id { get; set; }
        public Guid Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
