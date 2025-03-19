using FluentValidation;
using App.Business.DTOs;
using App.Business.Validators.Commons;
using System.Text.RegularExpressions;

namespace App.Business.Validators
{
    public class CreateGraduateDTOValidator : AbstractValidator<CreateGraduateDTO>
    {
        public CreateGraduateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Ad maksimum 50 simvol ola bilər.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Soyad maksimum 50 simvol ola bilər.");

            RuleFor(x => x.FatherName)
                .NotEmpty().WithMessage("Ata adı daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Ata adı maksimum 50 simvol ola bilər.");

            RuleFor(x => x.Telephone)
                .NotEmpty().WithMessage("Telefon daxil edilməlidir.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Telefon nömrəsi düzgün formatda deyil.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-poçt daxil edilməlidir.")
                .EmailAddress().WithMessage("E-poçt ünvanı düzgün formatda deyil.");

            RuleFor(x => x.Faculty)
                .NotEmpty().WithMessage("Fakültə daxil edilməlidir.")
                .MaximumLength(100).WithMessage("Fakültə maksimum 100 simvol ola bilər.");

            RuleFor(x => x.Group)
                .NotEmpty().WithMessage("Qrup daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Qrup maksimum 50 simvol ola bilər.");

            RuleFor(x => x.Photo)
                .NotEmpty().WithMessage("Profil şəkili boş ola bilməz.");

            RuleFor(x => x.ImageId)
                .NotEmpty().WithMessage("Tələbə kartı şəkili boş ola bilməz.");

            RuleFor(x => x.EducationLevel)
                .NotEmpty().WithMessage("Təhsil səviyyəsi daxil edilməlidir.");
        }
    }


    public class UpdateGraduateDTOValidator : AbstractValidator<UpdateGraduateDTO>
    {
        public UpdateGraduateDTOValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID düzgün daxil edilməlidir.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Ad maksimum 50 simvol ola bilər.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Soyad maksimum 50 simvol ola bilər.");

            RuleFor(x => x.FatherName)
                .NotEmpty().WithMessage("Ata adı daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Ata adı maksimum 50 simvol ola bilər.");

            RuleFor(x => x.Telephone)
                .NotEmpty().WithMessage("Telefon daxil edilməlidir")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Telefon nömrəsi düzgün formatda deyil.");


            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-poçt daxil edilməlidir.")
                .EmailAddress().WithMessage("E-poçt ünvanı düzgün formatda deyil.");

            RuleFor(x => x.Faculty)
                .MaximumLength(100).WithMessage("Fakültə maksimum 100 simvol ola bilər.")
                .When(x => !string.IsNullOrEmpty(x.Faculty));

            RuleFor(x => x.Group)
                .MaximumLength(50).WithMessage("Qrup maksimum 50 simvol ola bilər.")
                .When(x => !string.IsNullOrEmpty(x.Group));

            RuleFor(x => x.Photo)
                .NotEmpty().WithMessage("Profil şəkili boş ola bilməz.");
                 

            RuleFor(x => x.ImageId)
                .NotEmpty().WithMessage("Tələbə kartı şəkili boş ola bilməz.");
        }
    }

    public class GetByIdGraduateDTOValidator : BaseEntityValidator<GetByIdGraduateDTO>
    {
        public GetByIdGraduateDTOValidator()
        {
        }
    }

    public class DeleteGraduateDTOValidator : BaseEntityValidator<DeleteGraduateDTO>
    {
        public DeleteGraduateDTOValidator()
        {
        }
    }
}
