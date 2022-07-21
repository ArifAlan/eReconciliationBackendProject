﻿using Business.Abstract;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.CrossCuttingConcerns.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;
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
        private readonly IMailService _mailService;
        private readonly IMailParameterService _mailParameterService;
        private readonly IMailTemplateService _mailTemplateService;


        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService,
            IMailService mailService, IMailParameterService mailParameterService, IMailTemplateService mailTemplateService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;
            _mailService = mailService;
            _mailParameterService = mailParameterService;
            _mailTemplateService = mailTemplateService;
        }

        public IResult CompanyExists(Company company)
        {
            var result = _companyService.CompanyExists(company);

            if (result.Success ==false)
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

        public IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password, Company company)
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

            UserCompanyDto userCompanyDto = new UserCompanyDto()
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
            string subject = "Kullanıcı Onay Maili";
            string body = "Kullanıcı sisteme kayıt oldu.Kaydınızı tanımlamak için  aşağıdaki linke tıklamanız gerekmektedir.";
            string link = "https://localhost:44395/api/Auth/confirmuser?value="+user.MailConfirmValue;
            string linkDescription = "Kaydı Onaylamak için tıklayınız";

            var mailTemplate = _mailTemplateService.GetByTemplateName("Kayıt", 2);
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkDescription);




            var mailParameter = _mailParameterService.Get(2);
            SendMailDto sendMailDto = new SendMailDto()
            {
                mailParameter = mailParameter.Data,
                email = user.Email,
                subject="Kullanıcı kayıt onay maili",
                body= templateBody
            };
            _mailService.SendMail(sendMailDto);

            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, Messages.UserForRegistered);
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
