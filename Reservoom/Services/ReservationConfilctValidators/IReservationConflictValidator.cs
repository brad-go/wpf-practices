using Reservoom.Models;
using System.Threading.Tasks;

namespace Reservoom.Services.ReservationConfilctValidators
{
    public interface IReservationConflictValidator
    {
        Task<Reservation> GetConflictingReservation(Reservation reservation);
    }
}
