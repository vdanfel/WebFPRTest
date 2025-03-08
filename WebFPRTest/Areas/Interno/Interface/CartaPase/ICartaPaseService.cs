using WebFPRTest.Areas.Interno.Models.CartaPase;

namespace WebFPRTest.Areas.Interno.Interface.CartaPase
{
    public interface ICartaPaseService
    {
        Task<CartaPaseViewModel> Jugador_Datos(int Id_Jugador);
    }
}
