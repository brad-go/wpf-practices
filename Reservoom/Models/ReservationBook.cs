using Reservoom.Exceptions;
using Reservoom.Services.ReservationConfilctValidators;
using Reservoom.Services.ReservationCreators;
using Reservoom.Services.ReservationProviders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservoom.Models
{
    public class ReservationBook
    {
        private readonly IReservationProvider _reservationProvider;
        private readonly IReservationCreator _reservationCreator;
        private readonly IReservationConflictValidator _reservationConflictValidator;

        public ReservationBook(IReservationProvider reservationProvider, IReservationCreator reservationCreator, IReservationConflictValidator reservationConflictValidator)
        {
            _reservationProvider = reservationProvider;
            _reservationCreator = reservationCreator;
            _reservationConflictValidator = reservationConflictValidator;
        }

        /// <summary>
        /// 모든 예약들을 가져옵니다. 
        /// </summary>
        /// <returns>예약 목록에 있는 모든 예약들</returns>
        public async Task<IEnumerable<Reservation>> GetAllReservations()
        {
            return await _reservationProvider.GetAllReservations();
        }

        /// <summary>
        /// 예약 목록에 새로운 예약을 추가합니다.
        /// </summary>
        /// <param name="reservation">새로운 예약</param>
        /// <exception cref="ReservationConflictException">이미 존재한 예약과 새로운 예약이 서로 충돌한다면 예외를 던집니다.</exception>
        public async Task AddReservation(Reservation reservation)
        {
            Reservation conflictingReservation = await _reservationConflictValidator.GetConflictingReservation(reservation);

            if (conflictingReservation != null)
            {
                throw new ReservationConflictException(conflictingReservation, reservation);
            }

            await _reservationCreator.CreateReservation(reservation);
        }


    }
}
