using Business.Abstract;
using Business.Constans;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICompanyService _companyService;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;   
        }

        public IResult CompanyExists(Company company)
        {
            var result = _companyService.CompanyExists(company);

            if (result != null)
            {
                return new ErrorResult(Messages.CompanyAlreadyExists);
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(User user,int companyId)
        {
            var claims = _userService.GetClaims(user,companyId);
            var accessToken=_tokenHelper.CreateToken(user,claims,companyId);
            return new SuccessDataResult<AccessToken>(accessToken); 
        }

        public IDataResult<User> Login(UserForLogin userForLogin)
        {
            var userToCheck = _userService.GetByMail(userForLogin.Email);
            if (userToCheck==null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userForLogin.Password,userToCheck.PasswordHash,userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }
            return new SuccessDataResult<User>(userToCheck,Messages.SuccessfulLogin);
        }

        public IDataResult<UserCompanyRegisterDto> Register(UserForRegister userForRegister, string password, Company company)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash,out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name,    
            };
            _userService.Add(user);

            _companyService.Add(company);

            _companyService.UserCompanyAdd(user.Id, company.Id);

            UserCompanyRegisterDto userCompanyDto = new UserCompanyRegisterDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AddedAt = user.AddedAt,
                IsActive = true,
                MailConfirm = user.MailConfirm,
                MailConfirmValue=Guid.NewGuid().ToString(),
                MailConfirmDate=user.MailConfirmDate,
                PasswordHash=user .PasswordHash,  
                PasswordSalt=user.PasswordSalt
            };

            return new SuccessDataResult<UserCompanyRegisterDto>(userCompanyDto, Messages.UserForRegistered);
        }

        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name,
            };
            _userService.Add(user);
            return new SuccessDataResult<User>(user, Messages.UserForRegistered);
        }

        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email)!=null)
            {
                return new ErrorResult(Messages.UserAlreadyExits);
            }
            return new SuccessResult();
        }
    }
}
