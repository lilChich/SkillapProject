using MediatR;
using Skillap.BLL.DTO;
using Skillap.DAL.Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skillap.BLL.Interfaces.JWT;
using Skillap.BLL.Exceptions;
using System.Net;

namespace Skillap.BLL.User.Login
{
    public class LoginHandler : IRequestHandler<LoginQuery, UserDTO>
    {
        private readonly UserManager<ApplicationUsers> userManager;
        private readonly IJwtGenerator jwtGenerator;
        private readonly SignInManager<ApplicationUsers> signInManager;

        public LoginHandler(UserManager<ApplicationUsers> UserManager, 
            IJwtGenerator JwtGenerator, 
            SignInManager<ApplicationUsers> SignInManager)
        {
            userManager = UserManager;
            jwtGenerator = JwtGenerator;
            signInManager = SignInManager;
        }

        public async Task<UserDTO> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new RestException(HttpStatusCode.Unauthorized);
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                return new UserDTO
                {
                    FirstName = user.FirstName,
                    SecondName = user.SecondName,
                    NickName = user.NickName,
                    DateOfBirth = user.DateOfBirth,
                    Country = user.Country,
                    Education = user.Education,
                    Token = jwtGenerator.CreateToken(user),
                    Image = user.Image
                };
            }

            throw new RestException(HttpStatusCode.Unauthorized);
        }
    }
}
