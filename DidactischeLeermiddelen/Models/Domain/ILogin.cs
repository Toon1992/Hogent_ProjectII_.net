using System.Threading.Tasks;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface ILogin
    {
        Task<bool> Login(LoginViewModel model);
    }
}
