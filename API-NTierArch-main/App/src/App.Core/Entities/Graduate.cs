using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Entities.Commons;

namespace App.Core.Entities
{
    public class Graduate : BaseEntity, IAuditedEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FatherName { get; set; }
        public bool IsReference { get; set; }
        public string? Telephone { get; set; }
        public string PhotoUrl { get; set; }
        public string IdUrl { get; set; }
        public string Email { get; set; }
        public string? QrCodeUrl { get; set; }
        public string EducationLevel { get; set; }
        public bool IsInside { get; set; } = false; 
        public string Faculty { get; set; }
        public string? Group { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
