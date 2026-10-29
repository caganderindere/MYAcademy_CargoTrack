using CargoTrack.Business.Constants;
using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Customers;
using CargoTrack.DTO.DTOs.UserDtos;
using CargoTrack.Entity.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace CargoTrack.Business.Services.Auth
{
    public class AuthService(UserManager<AppUser> _userManager,
                             SignInManager<AppUser> _signInManager,
                             ICustomerRepository _customerRepository) : IAuthService
    {
        private const string InvalidLoginMessage = "Kullanıcı adı veya şifre hatalı.";

        public async Task<IList<string>> LoginAsync(LoginUserDto loginUserDto)
        {
            var user = await _userManager.FindByNameAsync(loginUserDto.UserName);

            if (user is null)
            {
                throw new BusinessException(InvalidLoginMessage);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginUserDto.Password, false, true);

            if (result.IsLockedOut)
            {
                throw new BusinessException("Çok fazla hatalı deneme yaptınız. Lütfen birkaç dakika sonra tekrar deneyin.");
            }

            if (!result.Succeeded)
            {
                throw new BusinessException(InvalidLoginMessage);
            }

            return await _userManager.GetRolesAsync(user);
        }

        public async Task RegisterAsync(RegisterUserDto registerUserDto)
        {
            var existingCustomer = await _customerRepository.GetByPhoneNumberAsync(registerUserDto.PhoneNumber!);

            if (existingCustomer?.AppUserId is not null)
            {
                throw new BusinessException("Bu telefon numarası zaten bir kullanıcı hesabına bağlı.");
            }

            var user = registerUserDto.Adapt<AppUser>();

            var result = await _userManager.CreateAsync(user, registerUserDto.Password!);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(x => x.Description));
                throw new BusinessException(errors);
            }

            await _userManager.AddToRoleAsync(user, RoleNames.User);

            if (existingCustomer is null)
            {
                var customer = new Customer
                {
                    FirstName = registerUserDto.FirstName!,
                    LastName = registerUserDto.LastName!,
                    PhoneNumber = registerUserDto.PhoneNumber!,
                    Email = registerUserDto.Email,
                    AppUserId = user.Id
                };

                await _customerRepository.CreateAsync(customer);
                return;
            }

            existingCustomer.AppUserId = user.Id;
            existingCustomer.Email ??= registerUserDto.Email;
            await _customerRepository.UpdateAsync(existingCustomer);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
