using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Entities.Commons;
using App.Core.Entities.Identity;

namespace App.Core.Entities
{
    public class Logs:BaseEntity,IAuditedEntity
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public string IPAddress { get; set; }
        public string OS { get; set; }
        public string Browser { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
