using Reservoom.Exceptions;
using System.Collections.Generic;

namespace Reservoom.Models
{
    public class ReservationBook
    {
        private readonly List<Reservation> _reservations;

        public ReservationBook()
        {
            _reservations = new List<Reservation>();
        }

        /// <summary>
        /// 모든 예약들을 가져옵니다. 
        /// </summary>
        /// <returns>예약 목록에 있는 모든 예약들</returns>
        public IEnumerable<Reservation> GetAllReservations()
        {
            return _reservations;
        }

        /// <summary>
        /// 예약 목록에 새로운 예약을 추가합니다.
        /// </summary>
        /// <param name="reservation">새로운 예약</param>
        /// <exception cref="ReservationConflictException">이미 존재한 예약과 새로운 예약이 서로 충돌한다면 예외를 던집니다.</exception>
        public void AddReservation(Reservation reservation)
        {
            foreach (Reservation existingReservation in _reservations)
            {
                if (existingReservation.Conflicts(reservation))
                {
                    throw new ReservationConflictException(existingReservation, reservation);
                }
            }

            _reservations.Add(reservation);
        }


    }
}
