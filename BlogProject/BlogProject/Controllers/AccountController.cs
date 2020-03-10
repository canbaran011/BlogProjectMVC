using BlogProject.Models;
using BlogProject.Models.Manager;
using BlogProject.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BlogProject.Controllers
{
    public class AccountController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.ForgotPass = TempData["ForgotPassword"];

            return View();
        }

        [HttpPost]
        public ActionResult Login(Users model)
        {
            Users user = new Users();
            bool password = false; ;
            user = db.Users.Where(x => x.Email == model.Email || x.UserName == model.Email).FirstOrDefault();
            if (user != null)
            {
                password = Crypto.VerifyHashedPassword(user.Password, model.Password);
            }
            if (user != null)//üye bulunduysa
            {
                //user = null;
                //user = db.Users.Where(x => (x.Email == model.Email || x.NickName == model.Email) && x.Password == password).FirstOrDefault();
                if (password != false)
                {
                    AdminList admin = db.AdminList.Where(x => x.Users.ID == user.ID).FirstOrDefault();
                    if (admin == null)
                    {
                        Session["USER"] = user.UserName;//üye yönetici değil ise
                        Session["ID"] = user.ID;
                    }
                    else
                    {
                        Session["USER"] = user.UserName + "(" + admin.AuthName + ")";
                        Session["ID"] = user.ID;

                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Warning = "Hatalı şifre.Lütfen yeniden deneyiniz.";
                    ViewBag.Status = "danger";
                }
            }
            else
            {
                ViewBag.Warning = "Böyle bir Email/Nickname bulunamadı.";
                ViewBag.Status = "danger";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Users model)
        {
            Users user = new Users();
            Users username = new Users();
            user = db.Users.Where(x => x.Email == model.Email).FirstOrDefault();
            username = db.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();
            if (user != null)
            {
                ViewBag.Warning = "E-posta adresi başka bir hesaba ait.";
                ViewBag.Status = "danger";
                return View();
            }
            else if (username != null)
            {
                ViewBag.Warning = "Kullanıcı adı başka bir hesaba ait.Lütfen farklı bir kullanıcı adı oluşturun.";
                ViewBag.Status = "danger";
                return View();
            }
            else
            {
                bool digit = model.Password.Any(Char.IsDigit);//password enaz 1 sayı ve 1 karakter içermesi için kontrol
                bool letter = model.Password.Any(Char.IsLetter);

                if (model.Password.Length < 8 || !digit || !letter)//ters mantık kullanıldı
                {
                    ViewBag.Warning = "Güvenliğiniz için parolanızı en az 8 karakter, en az 1 sayı ve 1 rakam içerecek şekilde oluşturun.";
                    ViewBag.Status = "danger";
                    return View();
                }
                else
                {
                    #region mail gönderme

                    SmtpClient smtp = new SmtpClient();//gideceği yer server
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;

                    NetworkCredential auth = new NetworkCredential();//nasıl gidecek
                    auth.UserName = "infoprojects01@gmail.com";
                    auth.Password = "Aa123456*";
                    smtp.Credentials = auth;
                    //buraya kadar ayarlar yapıldı alt yapı hazırlandı
                    MailMessage msg = new MailMessage();
                    msg.Subject = "Blog Sayfasına Kayıt";
                    msg.Body = @"<strong> Email: [email] </strong><br/>".Replace("[email]", model.Email);
                    msg.Body += @"<strong> Password: [password] </strong><br/>".Replace("[password]", model.Password);
                    msg.Body += @"<strong> Kullanici adiniz ve sifreniz basariyla olusturulmustur. </strong><br/>";
                    msg.IsBodyHtml = true;//body içerisinde yazılan html tagları aktif hale getiriyor
                    msg.To.Add(model.Email);//mailin gideceği adres
                    msg.From = new MailAddress("infoprojects01@gmail.com", "Blog Üyelik Bilgileri");

                    try
                    {
                        smtp.Send(msg);
                    }
                    catch (Exception)
                    {
                        ViewBag.Warning = "Lütfen geçerli bir Email adresi girin.";
                        ViewBag.Status = "danger";
                        return RedirectToAction("Register", "Account");
                    }
                    #endregion

                    #region password hashing
                    string password = Crypto.HashPassword(model.Password);
                    #endregion

                    try
                    {
                        #region database kayıt işlemleri
                        user = new Users();
                        user.Name = model.Name;
                        user.Surname = model.Surname;
                        user.Email = model.Email;
                        user.UserName = model.UserName;
                        user.Password = password;
                        user.Adress = model.Adress;
                        user.Phone = model.Phone;
                        db.Users.Add(user);
                        db.SaveChanges();
                        #endregion
                        Session["USER"] = user.UserName;
                        user = db.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                        Session["ID"] = user.ID;
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult UpdateInformation()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("Login");
            }

            Users user;
            int? id = Convert.ToInt32(Session["ID"]);
            if (id != null)
            {
                user = db.Users.Where(x => x.ID == id).FirstOrDefault();
                return View(user);
            }



            return View();
        }

        [HttpPost]
        public ActionResult UpdateInformation(Users model)
        {
            int? id = Convert.ToInt32(Session["ID"]);
            Users user = new Users();
            bool record = false;
            user = db.Users.Where(x => x.ID == id).FirstOrDefault();
            Users userMailchk = db.Users.Where(x => x.Email == model.Email).FirstOrDefault();
            Users userUserNamechk = db.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();
            //email adresi ve kullanıcı adı tabloda 
            //eğer girilen mail adresi tabloda varsa ve idler aynı ise mail adresi güncellenmek istenmiyordur devam edilebilir kayıt için. veya girilen mail adresi veritabanında bulunamamıştır yine kayıt için yeterli koşulu sağlar. null referans hatası verdiği için şartları uzunca yazmam gerekiyor. tek if içerisinde null gelirse id ler karşılaştırılamıyor ve hata veriyor.
            if ((userMailchk.Email == null))//veritanında bulunamadıysa
            {
                //nickname yada e mail in null olması yukarıdaki sorgularda bulunamamsı demek.
                if ((userUserNamechk == null))//aynı mantık
                {
                    record = true;//kullanıcı adı ve email kullanımda değil kayıt edilebilir.
                }
                else //boş değilse kendimi kullanıyor başkası mı, kontrol ediliyor
                {
                    if (userUserNamechk.ID == id)//kendisi kullanıyor diğer bilgilerde kayıt edilebilir
                    {
                        record = true;
                    }
                    else
                    {
                        record = false;
                        ViewBag.Warning = "Girdiğiniz kullanıcı adı şuan kullanımda";
                        ViewBag.Status = "danger";
                    }
                }
            }
            else if (userMailchk.Email != null)//aynı mantık mail kullanımda
            {
                if (userMailchk.ID == id)//maili kendi mi kullanıyor
                {
                    if (userUserNamechk == null)//mail kendine ait ise girdiği user name tablo da var, mı kullanımda mı?
                    {
                        record = true;

                    }
                    else//kullanıcı adı boşta ise kayıt yapılabilir
                    {
                        if (userUserNamechk.ID == id)//var ise kendine mi ait
                        {
                            record = true;
                        }
                        else//kendine iat değilse başkası tarafından kullanılıyordur ekrana hata ver
                        {
                            record = false;
                            ViewBag.Warning = "Girdiğiniz kullanıcı adı şuan kullanımda";
                            ViewBag.Status = "danger";
                        }
                    }
                }
                else //mail kullanılıyor ve kendisi değil ise
                {
                    record = false;
                    ViewBag.Warning = "Girdiğiniz mail adresi başka bir kullanıcı tarafında kullanımda.";
                    ViewBag.Status = "danger";

                }
            }


            if (record)
            {
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Adress = model.Adress;
                user.Phone = model.Phone;
                db.SaveChanges();
                ViewBag.Warning = "Bilgileriniz başarıyla güncelleştirilmiştir.";
                ViewBag.Status = "success";
                Session["USER"] = user.UserName;
            }

            /*
             * Devamı gelecek
             * Email adresi kontrolü yapılacak
             */
            /*
            #region mail gönderme

            SmtpClient smtp = new SmtpClient();//gideceği yer server
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential auth = new NetworkCredential();//nasıl gidecek
            auth.UserName = "infoprojects01@gmail.com";
            auth.Password = "Aa123456*";
            smtp.Credentials = auth;
            //buraya kadar ayarlar yapıldı alt yapı hazırlandı
            MailMessage msg = new MailMessage();
            msg.Subject = "Blog Sayfasına Kayıt";
            msg.Body = @"<strong> Email: [email] </strong><br/>".Replace("[email]", model.Email);
            msg.Body += @"<strong> Password: [password] </strong><br/>".Replace("[password]", model.Password);
            msg.Body += @"<strong> Kullanici adiniz ve sifreniz basariyla olusturulmustur. </strong><br/>";
            msg.IsBodyHtml = true;//body içerisinde yazılan html tagları aktif hale getiriyor
            msg.To.Add(model.Email);//mailin gideceği adres
            msg.From = new MailAddress("infoprojects01@gmail.com", "Blog Üyelik Bilgileri");

            try
            {
                smtp.Send(msg);
            }
            catch (Exception)
            {
                ViewBag.Warning = "Lütfen geçerli bir Email adresi girin.";
                ViewBag.Status = "danger";
                return RedirectToAction("Register", "Account");
            }
            #endregion
            */




            return View();
        }

        public ActionResult ChangePassword()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(AccountViewModel model)
        {
            model.ID = Convert.ToInt32(Session["ID"]);

            int result = string.Compare(model.Password1, model.Password2);
            if (result == 0)
            {
                bool digit = model.Password1.Any(Char.IsDigit);//password enaz 1 sayı ve 1 karakter içermesi için kontrol
                bool letter = model.Password1.Any(Char.IsLetter);//pass2 yi kontrol etmedim çünkü üstte ikisi aynıysa bu alana girecek

                if (model.Password1.Length < 8 || !digit || !letter)//ters mantık kullanıldı
                {
                    ViewBag.Warning = "Güvenliğiniz için parolanızı en az 8 karakter, en az 1 sayı ve 1 rakam içerecek şekilde oluşturun.";
                    ViewBag.Status = "danger";
                    return View();
                }
                else
                {
                    Users user = new Users();
                    user = db.Users.Where(x => x.ID == model.ID).FirstOrDefault();
                    user.Password = Crypto.HashPassword(model.Password1);
                    db.SaveChanges(); ViewBag.Warning = "Şifreniz başarıyla değiştirilmiştir.";
                    ViewBag.Status = "success";
                }
            }
            else
            {
                db.SaveChanges(); ViewBag.Warning = "Girdiğiniz şifreler aynı değil.";
                ViewBag.Status = "danger";
            }

            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(Users model)
        {
            Users user = new Users();
            user = db.Users.Where(x => x.Email == model.Email || x.UserName == model.Email || x.Phone == model.Email).FirstOrDefault();
            if (user != null)
            {
                #region sifre olusturma
                int count = 10;
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random rnd = new Random();
                while (0 < count)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                    count--;
                }

                string newPassword = res.ToString();

                #endregion
                #region Mail ayarları
                SmtpClient smtp = new SmtpClient();//gideceği yer
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                NetworkCredential auth = new NetworkCredential();//nasıl gidecek
                auth.UserName = "infoprojects01@gmail.com";
                auth.Password = "Aa123456*";
                smtp.Credentials = auth;
                //buraya kadar ayarlar yapıldı alt yapı hazırlandı
                MailMessage msg = new MailMessage();
                msg.Subject = "Blog Şifre Yenileme";
                //msg.Body = txtMesaj.Text;
                msg.Body = @"<strong> Email: [email] </strong><br/>".Replace("[email]", user.Email);
                msg.Body += "<strong> Yeni Sifreniz: [sifre] </strong><br/>".Replace("[sifre]", newPassword);
                msg.IsBodyHtml = true;//body içerisinde yazılan html tagları aktif hale getiriyor
                msg.To.Add(user.Email);
                msg.From = new MailAddress("infoprojects01@gmail.com", "Şifre Yenileme");
                #endregion
                try
                {
                    smtp.Send(msg);

                    string CryptoSifre = Crypto.HashPassword(newPassword);
                    user.Password = CryptoSifre;
                    //db.Users.Attach(user);
                    //db.Entry(user).State=System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Warning = "Yeni şifreniz başarıyla oluşturulmuş.Lütfen mail adresiniz kontrol edin.";
                    ViewBag.Status = "success";


                }
                catch (Exception)
                {
                    ViewBag.Warning = "Güncelleme sırasında bir hata meydana geldi.Lütfen tekrar deneyiniz.";
                    ViewBag.Status = "danger";
                }

                TempData["ForgotPassword"] = "Şifreniz başarıyla oluşturulmuştur.Mail adresinizi kontrol edin.";

                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Warning = "Bu isimde kayıtlı bir mail adresi, yada kullaniciAdi bulunamamıştır.Lütfen yeniden deneyin.";
                ViewBag.Status = "danger";
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}