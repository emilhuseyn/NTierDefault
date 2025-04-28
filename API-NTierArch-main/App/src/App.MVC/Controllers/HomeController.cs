using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Business.DTOs;
using App.Business.Services.InternalServices.Interfaces;
using FluentValidation;
using OfficeOpenXml;
using App.DAL.Repositories.Interfaces;
using App.Core.Enums;
using Microsoft.AspNetCore.Identity;
using App.Core.Entities.Identity;
using UAParser;
using static System.Runtime.InteropServices.JavaScript.JSType;
using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using App.DAL.Presistence;

namespace App.MVC.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly IGraduateRepository _graduateRepository;
        private readonly IGraduateService _graduateService;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IValidator<CreateGraduateDTO> _createValidator;
        private readonly IValidator<UpdateGraduateDTO> _updateValidator;
        private readonly IValidator<DeleteGraduateDTO> _deleteValidator;
        private readonly IValidator<GetByIdGraduateDTO> _getByIdValidator;

        public HomeController(
            IGraduateService graduateService,
            IValidator<CreateGraduateDTO> createValidator,
            IValidator<UpdateGraduateDTO> updateValidator,
            IValidator<DeleteGraduateDTO> deleteValidator,
            IValidator<GetByIdGraduateDTO> getByIdValidator,
            IGraduateRepository graduateRepository,
            UserManager<User> userManager,
            AppDbContext appDbContext)
        {
            _graduateService = graduateService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _deleteValidator = deleteValidator;
            _getByIdValidator = getByIdValidator;
            _graduateRepository = graduateRepository;
            this.userManager = userManager;
            _appDbContext = appDbContext;
        }
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
             var allGraduates = await _graduateService.GetAllAsync();

             var graduatesList = allGraduates.ToList();

             var filteredGraduates = graduatesList;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                filteredGraduates = graduatesList.Where(g =>
                    g.Name.ToLower().Contains(search) ||
                    g.Surname.ToLower().Contains(search) ||
                    g.Email.ToLower().Contains(search)
                ).ToList();
            }

             var orderedGraduates = filteredGraduates.OrderByDescending(g => g.CreatedOn).ToList();
            int totalItems = orderedGraduates.Count;
            var pagedGraduates = orderedGraduates
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

   
            ViewBag.AllGraduates = graduatesList; 
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SearchTerm = search;

            return View(pagedGraduates);
        }


        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult Register()
        {
            var model = new CreateGraduateDTO();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFakeGraduates()
        {
            var graduates = new List<Graduate>();

            for (int i = 1; i <= 10000; i++)
            {
                var graduate = new Graduate
                {
                    Name = $"Name{i}",
                    Surname = $"Surname{i}",
                    FatherName = $"Father{i}",
                    IsReference = i % 2 == 0, // Every second graduate is a reference
                    Telephone = $"050-123-45{i:D2}",
                    PhotoUrl = "/uploads/photos/default.png", // example default
                    IdUrl = "/uploads/ids/default.png",
                    Email = $"graduate{i}@example.com",
                    QrCodeUrl = null,
                    EducationLevel = (i % 2 == 0) ? "Bakalavr" : "Magistr",
                    IsInside = i % 2 != 0,
                    Faculty = $"Faculty{i % 10}", // 10 different faculties
                    Group = $"Group{i % 5}" // 5 different groups
                };

                graduates.Add(graduate);
            }

            // Now save all graduates to DB
            await _appDbContext.Graduates.AddRangeAsync(graduates);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "100 graduates created successfully!" });
        }



        [HttpPost]
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> Register(CreateGraduateDTO createDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(createDTO);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(createDTO);
            }
             
            await _graduateService.CreateAsync(createDTO);

             
            return RedirectToAction("Index");
        }





        [Authorize(Roles = "Moderator,Admin")]  
        public async Task<IActionResult> Details(int id)
        {
            var dto = new GetByIdGraduateDTO { Id = id };
            var validationResult = await _getByIdValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return NotFound(new { message = "Məzun ID düzgün deyil!" });

            var graduate = await _graduateService.GetByIdAsync(dto);

            if (graduate == null)
                return NotFound(new { message = "Məzun tapılmadı!" });

            return View(graduate);
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Admin")]  
        public async Task<IActionResult> Update(UpdateGraduateDTO updateDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDTO);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("Details", updateDTO);
            }

            var updatedGraduate = await _graduateService.UpdateAsync(updateDTO);
            if (updatedGraduate == null)
                return NotFound();

            return RedirectToAction("Details", new { id = updatedGraduate.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Admin")]  
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new DeleteGraduateDTO { Id = id };
            var validationResult = await _deleteValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(new { message = "Xəta baş verdi!" });

            var deletedGraduate = await _graduateService.DeleteAsync(dto);
            if (deletedGraduate == null)
                return NotFound(new { message = "Məzun tapılmadı!" });

            return Ok(new { message = "Məzun uğurla silindi!" });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateIsInside([FromBody] UpdateIsInsideDTO model)
        {
            var graduate = await _graduateRepository.GetByIdAsync(x=>x.Id==model.Id);
            if (graduate == null) return NotFound(new { success = false, message = "Məzun tapılmadı" });

            graduate.IsInside = model.IsInside;
            await _graduateRepository.UpdateAsync(graduate);

            return Ok(new { success = true });
        }


        [HttpPost]
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> ExportToExcel([FromBody] List<GraduateDTO> data)
        {
            if (data == null || data.Count == 0)
                return BadRequest("No data received");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Graduates");

                // Set headers
                worksheet.Cells[1, 1].Value = "Ad";
                worksheet.Cells[1, 2].Value = "Soyad";
                worksheet.Cells[1, 3].Value = "Ata Adı";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Fakültə";
                worksheet.Cells[1, 6].Value = "Qrup";
                worksheet.Cells[1, 7].Value = "Kategoriya";

                 for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Name;
                    worksheet.Cells[i + 2, 2].Value = data[i].Surname;
                    worksheet.Cells[i + 2, 3].Value = data[i].FatherName;
                    worksheet.Cells[i + 2, 4].Value = data[i].Email;
                    worksheet.Cells[i + 2, 5].Value = data[i].Faculty;
                    worksheet.Cells[i + 2, 6].Value = data[i].Group;
                    worksheet.Cells[i + 2, 7].Value = data[i].EducationLevel;
                }

                worksheet.Cells.AutoFitColumns();

                 string fileName = "Graduates.xlsx";

                 if (data.Count > 0)
                {
                    var firstItem = data[0];
                    bool sameFaculty = data.All(d => d.Faculty == firstItem.Faculty);
                    bool sameGroup = data.All(d => d.Group == firstItem.Group);
                    bool sameEduLevel = data.All(d => d.EducationLevel == firstItem.EducationLevel);

                    if (sameFaculty && sameGroup && sameEduLevel)
                        fileName = $"Graduates_{firstItem.Faculty}_{firstItem.Group}_{firstItem.EducationLevel}.xlsx";
                    else if (sameFaculty && sameEduLevel)
                        fileName = $"Graduates_{firstItem.Faculty}_{firstItem.EducationLevel}.xlsx";
                    else if (sameGroup && sameEduLevel)
                        fileName = $"Graduates_{firstItem.Group}_{firstItem.EducationLevel}.xlsx";
                    else if (sameFaculty)
                        fileName = $"Graduates_{firstItem.Faculty}.xlsx";
                    else if (sameGroup)
                        fileName = $"Graduates_{firstItem.Group}.xlsx";
                    else if (sameEduLevel)
                        fileName = $"Graduates_{firstItem.EducationLevel}.xlsx";
                }

                var stream = new MemoryStream();
                await package.SaveAsAsync(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}
