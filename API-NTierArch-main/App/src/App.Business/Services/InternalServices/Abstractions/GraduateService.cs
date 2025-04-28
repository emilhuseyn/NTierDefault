using System.Collections.Generic;
using System.Threading.Tasks;
using App.Business.DTOs;
using App.Business.Services.ExternalServices.Abstractions;
using App.Business.Services.ExternalServices.Interfaces;
using App.Business.Services.InternalServices.Interfaces;
using App.Core.Entities;
using App.Core.Entities.Identity;
using App.DAL.Presistence;
using App.DAL.Repositories.Interfaces;
using App.Shared.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace App.Business.Services
{
    public class GraduateService : IGraduateService
    {
        private readonly IFileManagerService _fileManagerService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGraduateRepository _repository;
        private readonly IClaimService _claimService; 
        private readonly ILogsService _logsService;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public GraduateService(IGraduateRepository repository, IMapper mapper, IWebHostEnvironment webHostEnvironment, IFileManagerService fileManagerService, ILogsService logsService, IClaimService claimService, AppDbContext dbContext)
        {
            _repository = repository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _fileManagerService = fileManagerService;
            _logsService = logsService;
            _claimService = claimService;
             _dbContext = dbContext;
        }
        private async Task<User> GetCurrentUserAsync()
        {
            var userId = _claimService.GetUserId();
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<IEnumerable<GraduateDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync(x => !x.IsDeleted);
            
            return _mapper.Map<IEnumerable<GraduateDTO>>(entities);
        }

        public async Task<GraduateDTO> GetByIdAsync(GetByIdGraduateDTO getByIdDTO)
        {
            var entity = await _repository.GetByIdAsync(x => x.Id == getByIdDTO.Id);
            return _mapper.Map<GraduateDTO>(entity);
        }

        public async Task<GraduateDTO> CreateAsync(CreateGraduateDTO createDTO)
        {
            var entity = _mapper.Map<Graduate>(createDTO);

            if (createDTO.Photo != null)
            {
                entity.PhotoUrl = await _fileManagerService.UploadLocalAsync(createDTO.Photo, "Photos", _webHostEnvironment.WebRootPath);
            }

            if (createDTO.ImageId != null)
            {
                entity.IdUrl = await _fileManagerService.UploadLocalAsync(createDTO.ImageId, "Ids", _webHostEnvironment.WebRootPath);
            }

            var createdEntity = await _repository.AddAsync(entity);

            string detailPageUrl = $"http://digiteam.az/home/details/{createdEntity.Id}";
            string qrCodeFilePath = GenerateQRCode(detailPageUrl, createdEntity.Id);
            createdEntity.QrCodeUrl = qrCodeFilePath;
            await _repository.UpdateAsync(createdEntity);

            MailService.SendQrCode(createDTO.Email, qrCodeFilePath, entity);
            var User = await GetCurrentUserAsync();

            await _logsService.CreateAsync(new CreateLogsDTO
            {
                Message = $"{entity.Name + " " + entity.Surname + " "+entity.FatherName+ " "  } yaradıldı. {User.UserName} tərəfindən.",
                Username = User.UserName,
                UserId = User.Id,
            });

            return _mapper.Map<GraduateDTO>(createdEntity);
        }


        private string GenerateQRCode(string qrData, int graduateId)
            {
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new PngByteQRCode(qrCodeData);
                    var qrCodeBytes = qrCode.GetGraphic(20);

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "QRs");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileName = $"{graduateId}.png";
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    File.WriteAllBytes(filePath, qrCodeBytes);

                    return $"/Uploads/QRs/{fileName}"; 
                }
            }
        
    


    public async Task<GraduateDTO> UpdateAsync(UpdateGraduateDTO updateDTO)
        {
            var entity = await _repository.GetByIdAsync(x => x.Id == updateDTO.Id);
            if (entity != null)
            {
                _mapper.Map(updateDTO, entity);

                if (updateDTO.Photo != null)
                {
                    entity.PhotoUrl = await _fileManagerService.UploadLocalAsync(updateDTO.Photo, "Photos", _webHostEnvironment.WebRootPath);
                }

                if (updateDTO.ImageId != null)
                {
                    entity.IdUrl = await _fileManagerService.UploadLocalAsync(updateDTO.ImageId, "Ids", _webHostEnvironment.WebRootPath);
                }


                await _repository.UpdateAsync(entity);
            }
            return _mapper.Map<GraduateDTO>(entity);
        }

        public async Task<GraduateDTO> DeleteAsync(DeleteGraduateDTO deleteDTO)
        {
            var entity = await _repository.GetByIdAsync(x => x.Id == deleteDTO.Id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                var User = await GetCurrentUserAsync();

                await _logsService.CreateAsync(new CreateLogsDTO
                {
                    Username=User.UserName,
                    Message = $"{entity.Name + " " + entity.Surname + " " + entity.FatherName + " "} silindi. {User.UserName} tərəfindən.",
                    UserId = User.Id,

                });
            }

            return _mapper.Map<GraduateDTO>(entity);
        }

    }
}
