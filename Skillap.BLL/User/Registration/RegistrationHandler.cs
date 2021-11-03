using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skillap.BLL.DTO;
using Skillap.BLL.Exceptions;
using Skillap.BLL.Interfaces.JWT;
using Skillap.DAL.EF;
using Skillap.DAL.Entities;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Skillap.BLL.User.Registration
{
    public class RegistrationHandler : IRequestHandler<RegistrationCommand, UserDTO>
    {
        private readonly UserManager<ApplicationUsers> userManager;
        private readonly IJwtGenerator jwtGenerator;
        private readonly DataContext context;

        public RegistrationHandler(DataContext Context, UserManager<ApplicationUsers> UserManager, IJwtGenerator JwtGenerator)
        {
            context = Context;
            userManager = UserManager;
            jwtGenerator = JwtGenerator;
        }


        public async Task<UserDTO> Handle(RegistrationCommand request, CancellationToken cancellationToken)
        {
            if (await context.Users.Where(x => x.Email == request.Email).AnyAsync())
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });
            }

            if (await context.Users.Where(x => x.UserName == request.NickName).AnyAsync())
            {
                throw new RestException(HttpStatusCode.BadRequest, new { UserName = "Nickname already exists" });
            }

            var user = new ApplicationUsers
            {
                FirstName = request.FirstName,
                SecondName = request.SecondName,
                NickName = request.NickName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                Gender = request.Gender,
                Country = request.Country,
                Education = request.Education,
                Image = null

            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return new UserDTO
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    NickName = user.NickName,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Country = user.Country,
                    Education = user.Education,
                    Image = user.Image,
                    Token = jwtGenerator.CreateToken(user)

                };
            }

            throw new Exception("User registration failed");
        }
    }
}
