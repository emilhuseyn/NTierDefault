//using Microsoft.AspNetCore.Identity;

//public class CustomIdentityErrorDescriber : IdentityErrorDescriber
//{
//    public override IdentityError PasswordTooShort(int length)
//    {
//        return new IdentityError
//        {
//            Code = nameof(PasswordTooShort),
//            Description = $"Şifrə çox qısadır. Minimum uzunluq {length} simvol olmalıdır."
//        };
//    }

//    public override IdentityError PasswordRequiresNonAlphanumeric()
//    {
//        return new IdentityError
//        {
//            Code = nameof(PasswordRequiresNonAlphanumeric),
//            Description = "Şifrə ən azı bir xüsusi simvol daxil etməlidir (!, @, # və s.)."
//        };
//    }

//    public override IdentityError PasswordRequiresDigit()
//    {
//        return new IdentityError
//        {
//            Code = nameof(PasswordRequiresDigit),
//            Description = "Şifrə ən azı bir rəqəm (0-9) daxil etməlidir."
//        };
//    }

//    public override IdentityError PasswordRequiresLower()
//    {
//        return new IdentityError
//        {
//            Code = nameof(PasswordRequiresLower),
//            Description = "Şifrə ən azı bir kiçik hərf (a-z) daxil etməlidir."
//        };
//    }

//    public override IdentityError PasswordRequiresUpper()
//    {
//        return new IdentityError
//        {
//            Code = nameof(PasswordRequiresUpper),
//            Description = "Şifrə ən azı bir böyük hərf (A-Z) daxil etməlidir."
//        };
//    }

//    public override IdentityError DuplicateUserName(string userName)
//    {
//        return new IdentityError
//        {
//            Code = nameof(DuplicateUserName),
//            Description = $"'{userName}' istifadəçi adı artıq mövcuddur."
//        };
//    }

//    public override IdentityError DuplicateEmail(string email)
//    {
//        return new IdentityError
//        {
//            Code = nameof(DuplicateEmail),
//            Description = $"'{email}' e-poçt ünvanı artıq mövcuddur."
//        };
//    }

//    public override IdentityError InvalidUserName(string userName)
//    {
//        return new IdentityError
//        {
//            Code = nameof(InvalidUserName),
//            Description = $"'{userName}' istifadəçi adı etibarsızdır."
//        };
//    }

//    public override IdentityError InvalidEmail(string email)
//    {
//        return new IdentityError
//        {
//            Code = nameof(InvalidEmail),
//            Description = $"'{email}' e-poçt ünvanı etibarsızdır."
//        };
//    }
//}
