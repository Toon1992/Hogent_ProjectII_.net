using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DidactischeLeermiddelen.ViewModels;
using Newtonsoft.Json;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class LoginOtherService:ILogin
    {
        private IGebruikerRepository repository;
        public string Email { get; set; }
        public string Type { get; set; }

        public LoginOtherService(IGebruikerRepository repository)
        {
            this.repository = repository;
        }
        public async Task<bool> Login(LoginViewModel model)
        {
            var json = string.Empty;
            string password = HashPasswordSha256(model.Password);
            string url = "https://studservice.hogent.be/auth" + "/" + model.Email + "/" + password;
            using (HttpClient hc = new HttpClient())
            {
                json = await hc.GetStringAsync(url); //wc.DownloadString(url);
            }
            dynamic array = JsonConvert.DeserializeObject(json);
            if (array.Equals("[]"))
            {
                return true;
            }
            var name = array.NAAM.ToString();
            var vnaam = array.VOORNAAM.ToString();
            Type = array.TYPE.ToString();
            var faculteit = array.FACULTEIT.ToString();
            Email = array.EMAIL.ToString();
            Gebruiker gebruiker = repository.FindByName(model.Email);
            if (gebruiker == null)
            {
                if (Type.ToLower().Equals("student"))
                {
                    gebruiker = new Student()
                    {
                        Naam = vnaam + " " + name,
                        Email = Email,
                        Faculteit = faculteit
                    };
                }
                else
                {
                    gebruiker = new Lector
                    {
                        Naam = vnaam + " " + name,
                        Email = Email,
                        Faculteit = faculteit
                    };
                }
                gebruiker.Verlanglijst = new Verlanglijst();
                gebruiker.Reservaties = new List<Reservatie>();
                repository.AddGebruiker(gebruiker);
                repository.SaveChanges();
            }
            return false;
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public string HashPasswordSha256(string password)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}