using CargoTrack.DTO.DTOs.UserDtos;

namespace CargoTrack.Business.Services.Auth
{
    public interface IAuthService
    {
        Task<IList<string>> LoginAsync(LoginUserDto loginUserDto);
        Task RegisterAsync(RegisterUserDto registerUserDto);
        Task LogoutAsync();
    }
}