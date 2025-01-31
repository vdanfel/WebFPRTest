using WebFPRTest.Models;
using WebFPRTest.Result;

namespace WebFPRTest.Interface
{
    public interface ILoginService
    {
        Task<LoginResult> ValidarLogin(LoginViewModel login);
    }
}
