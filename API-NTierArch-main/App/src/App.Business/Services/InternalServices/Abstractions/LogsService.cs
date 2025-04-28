using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Business.DTOs;
using App.Business.Services.ExternalServices.Abstractions;
using App.Business.Services.ExternalServices.Interfaces;
using App.Business.Services.InternalServices.Interfaces;
using App.Core.Entities;
using App.DAL.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UAParser;

namespace App.Business.Services.InternalServices.Abstractions
{
    public class LogsService:ILogsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogsRepository _repository;
        private readonly IMapper _mapper;
        public LogsService(ILogsRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetClientIp()
        {
            return _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }
        public async Task<string> GetLocationByIp(string ip)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync($"https://ipwho.is/{ip}");
                dynamic json = JsonConvert.DeserializeObject(response);

                if (json.success == true)
                {
                    return $"{json.city}, {json.region}, {json.country}";
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }


        public async Task<IEnumerable<LogsDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync(x => !x.IsDeleted);

            return _mapper.Map<IEnumerable<LogsDTO>>(entities);
        }
        public async Task<LogsDTO> CreateAsync(CreateLogsDTO createDTO)
        {
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);

            var device = c.Device.Family; 
            var os = c.OS.ToString();      
            var browser = c.UA.ToString();

            var entity = _mapper.Map<Logs>(createDTO);
            entity.OS = os;
            entity.Browser = browser;
            entity.Device = device; 
            entity.IPAddress = GetClientIp();  
            entity.Location =await GetLocationByIp(entity.IPAddress); 
            var createdEntity = await _repository.AddAsync(entity);
            return _mapper.Map<LogsDTO>(createdEntity);
        }

        public async Task<LogsDTO> UpdateAsync(UpdateLogsDTO updateDTO)
        {
            var entity = await _repository.GetByIdAsync(x => x.Id == updateDTO.Id);
            if (entity != null)
            {
                await _repository.UpdateAsync(entity);
            }
            return _mapper.Map<LogsDTO>(entity);
        }
        public async Task<LogsDTO> DeleteAsync(DeleteLogsDTO deleteDTO)
        {
            var entity = await _repository.GetByIdAsync(x => x.Id == deleteDTO.Id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
            return _mapper.Map<LogsDTO>(entity);
        }
    }
}
