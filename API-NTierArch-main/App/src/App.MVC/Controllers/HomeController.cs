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

namespace App.MVC.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly IGraduateRepository _graduateRepository;
        private readonly IGraduateService _graduateService;
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
            IGraduateRepository graduateRepository)
        {
            _graduateService = graduateService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _deleteValidator = deleteValidator;
            _getByIdValidator = getByIdValidator;
            _graduateRepository = graduateRepository;
        }
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> Index()
        {
            var graduates = await _graduateService.GetAllAsync();
            var orderedGraduates = graduates.OrderByDescending(g => g.CreatedOn).ToList();
            return View(orderedGraduates);
        }


        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult Register()
        {
            var model = new CreateGraduateDTO();
            return View(model);
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
                bool sameFaculty = data.All(d => d.Faculty == data[0].Faculty);
                bool sameGroup = data.All(d => d.Group == data[0].Group);
                bool sameEduLevel = data.All(d => d.EducationLevel == data[0].EducationLevel);

                if (sameFaculty && sameGroup && sameEduLevel)
                    fileName = $"Graduates_{data[0].Faculty}_{data[0].Group}_{data[0].EducationLevel}.xlsx";
                else if (sameFaculty && sameEduLevel)
                    fileName = $"Graduates_{data[0].Faculty}_{data[0].EducationLevel}.xlsx";
                else if (sameGroup && sameEduLevel)
                    fileName = $"Graduates_{data[0].Group}_{data[0].EducationLevel}.xlsx";
                else if (sameFaculty)
                    fileName = $"Graduates_{data[0].Faculty}.xlsx";
                else if (sameGroup)
                    fileName = $"Graduates_{data[0].Group}.xlsx";
                else if (sameEduLevel)
                    fileName = $"Graduates_{data[0].EducationLevel}.xlsx";

                var stream = new MemoryStream();
                await package.SaveAsAsync(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}
