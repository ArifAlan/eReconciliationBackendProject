using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constans
{
    public class Messages
    {
        public static string AddedCompany = "Kayıt işlemi başarıyla gerçekleşti";
        public static string UserNotFound = "Kullanıcı bulunamadı";

        public static string PasswordError = "Şifre Yanlış";
        public static string SuccessfulLogin = "Giriş Başarılı";

        public static string UserForRegistered = "Kullanıcı Kaydı başarılı";

        public static string UserAlreadyExits = "Bu Kullanıcı mevcut";

        public static string CompanyAlreadyExists = "Bu şirket daha önce kaydedilmiş.";

        public static string MailParameterAdded = "Mail parametreleri başarıyla eklendi";

        public static string MailSendSucessful = "Mail başarıyla göderildi";

        public static string MailTemplateAdded = " MailTemplate başarıyla eklendi";

        public static string MailTemplateUpdated = "MailTemplate güncellendi";

        public static string MailTemplateDeleted = "MailTemplate silindi";
    }
}
