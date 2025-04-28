using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Business.DTOs;

namespace App.Business.Services.InternalServices.Interfaces
{
    public interface ILogsService
    {
        Task<IEnumerable<LogsDTO>> GetAllAsync();
        
        Task<LogsDTO> CreateAsync(CreateLogsDTO createDTO);
        Task<LogsDTO> UpdateAsync(UpdateLogsDTO updateDTO);
        Task<LogsDTO> DeleteAsync(DeleteLogsDTO deleteDTO);
    }
}
