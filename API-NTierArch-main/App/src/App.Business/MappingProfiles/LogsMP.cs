using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Business.DTOs;
using App.Core.Entities;
using AutoMapper;

namespace App.Business.MappingProfiles
{
    public class LogsMP:Profile
    {
        public LogsMP()
        {
            CreateMap<Logs, LogsDTO>().ReverseMap();
            CreateMap<CreateLogsDTO, Logs>();
            CreateMap<UpdateLogsDTO, Logs>();
            CreateMap<DeleteLogsDTO, Logs>();
        }
    }
}
