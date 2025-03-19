using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Business.DTOs;

namespace App.Business.Services.InternalServices.Interfaces
{
    public interface IGraduateService
    {
        Task<IEnumerable<GraduateDTO>> GetAllAsync();
        Task<GraduateDTO> GetByIdAsync(GetByIdGraduateDTO getByIdDTO);
        Task<GraduateDTO> CreateAsync(CreateGraduateDTO createDTO);
        Task<GraduateDTO> UpdateAsync(UpdateGraduateDTO updateDTO);
        Task<GraduateDTO> DeleteAsync(DeleteGraduateDTO deleteDTO);
    }
}
