using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Business.DTOs.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Business.DTOs
{
    public class CreateGraduateDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FatherName { get; set; }
        public string? Telephone { get; set; }
        public IFormFile Photo { get; set; }
        public IFormFile ImageId { get; set; }
        public string EducationLevel { get; set; }
        public bool IsReference { get; set; }

        public string Email { get; set; }
        public string Faculty { get; set; }
        public string Group { get; set; }
    }
    public class UpdateGraduateDTO:BaseEntityDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EducationLevel { get; set; }
        public bool IsReference { get; set; }

        public string FatherName { get; set; }
        public string? Telephone { get; set; }
        public IFormFile Photo { get; set; }
        public IFormFile ImageId { get; set; }
        public string Email { get; set; }
        public string Faculty { get; set; }
        public string Group { get; set; }
    }
    public class GetByIdGraduateDTO : BaseEntityDTO
    {

    }
    public class DeleteGraduateDTO : BaseEntityDTO
    {

    }
    public class GraduateDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FatherName { get; set; }
        public string EducationLevel { get; set; }
        public string? Telephone { get; set; }
        public string PhotoUrl { get; set; }
        public string IdUrl { get; set; }
        public bool IsReference { get; set; }

        public string Email { get; set; }
        public bool IsInside { get; set; }
        public string Faculty { get; set; }
        public string Group { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class UpdateIsInsideDTO
    {
        public int Id { get; set; }
        public bool IsInside { get; set; }
    }


}
