using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Business.DTOs.Commons;
using App.Core.Entities.Commons;
using App.Core.Entities.Identity;

namespace App.Business.DTOs
{
    public class LogsDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string IPAddress { get; set; }
        public string OS { get; set; }
        public string Browser { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CreateLogsDTO
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string IPAddress { get; set; }
        public string Location { get; set; }
        public string Device { get; set; }
    }

    public class UpdateLogsDTO:BaseEntityDTO
    {
        public string Username { get; set; }

        public string Message { get; set; }
    }
    public class DeleteLogsDTO : BaseEntityDTO
    { }
}
