using Reservoom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservoom.Services.ReservationProviders
{
    /// <summary>
    /// DB에 의존하지 않고 모델 전체에서 인터페이스에 의존할 수 있도록 하고 단위 테스트를 가능하게 하기 위한 인터페이스
    /// </summary>
    public interface IReservationProvider
    {
        Task<IEnumerable<Reservation>> GetAllReservations();
    }
}
