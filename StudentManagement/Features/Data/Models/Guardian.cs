using System;
using System.Collections.Generic;

namespace StudentManagement.Features.Data.Models
{
    public class Guardian
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Relationship { get; set; }
        public required string ContactNumber { get; set; }
        public string? Email { get; set; }
        public required string Address { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
