using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skillap.BLL.Validation;

namespace Skillap.BLL.User.Registration
{
    public class RegistrationValidation : AbstractValidator<RegistrationCommand>
    {
        public RegistrationValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.SecondName).NotEmpty();
            RuleFor(x => x.NickName).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.Education).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Password();
            RuleFor(x => x.DateOfBirth).NotEmpty();
        }
    }
}
