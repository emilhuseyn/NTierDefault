using Microsoft.AspNetCore.Mvc;
using App.Business.Services.InternalServices.Interfaces;
using System.Threading.Tasks;
using App.Business.DTOs;
using App.DAL.Repositories.Interfaces;
using Microsoft.CodeAnalysis;
using App.DAL.Presistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.MVC.Controllers
{
    public class LogsController : Controller
    {
        private readonly ILogsRepository _logsService;
        private readonly AppDbContext _context;
        public LogsController(ILogsRepository logsService, AppDbContext context)
        {
            _logsService = logsService;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var logs = await _logsService.GetAllAsync(x => x.IsDeleted == false, x => x.User);
             var logsDTO = logs.Select(log => new LogsDTO
            {
                 Id = log.Id,
                IPAddress=log.IPAddress,
                Browser=log.Browser,
                Device=log.Device,
                Location=log.Location,
                OS=log.OS,
                Username = _context.Users.FirstOrDefault(x => x.Id ==log.UserId).UserName,
                CreatedOn = log.CreatedOn.AddHours(4) ,
                Message = log.Message,
             }).ToList();

            return View(logsDTO);
        }



      
        //public async Task<IActionResult> DeleteAllData()
        //{
        //     var allGraduates = await _context.Graduates.ToListAsync();
        //    _context.Graduates.RemoveRange(allGraduates);

        //     var allLogs = await _context.Logs.ToListAsync();
        //    _context.Logs.RemoveRange(allLogs);

        //     await _context.SaveChangesAsync();

        //    return RedirectToAction("Index");  
        //}

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _logsService.GetByIdAsync(x=>x.Id == id);
                await _logsService.DeleteAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
