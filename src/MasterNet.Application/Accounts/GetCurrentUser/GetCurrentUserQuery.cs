using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Accounts.GetCurrentUser;

public class GetCurrentUser
{
    public record GetCurrentUserQueryRequest(GetCurrentUserRequest getCurrentUserRequest):IRequest<Result<Profile>>;

    internal class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQueryRequest, Result<Profile>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public GetCurrentUserQueryHandler(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<Profile>> Handle(GetCurrentUserQueryRequest request, CancellationToken cancellationToken)
        {
            var user =  await _userManager.Users.FirstOrDefaultAsync(x => x.Email == request.getCurrentUserRequest.Email);

            if(user is null)
            {
                return Result<Profile>.Failure("No se encontro el usuario");
            }

            var profile = new Profile
            {
                Email = user.Email,
                NombreCompleto = user.NombreCompleto,
                Token = await  _tokenService.CreateToken(user),
                Username = user.UserName
            };

            return  Result<Profile>.Success(profile);
        }
    }
}