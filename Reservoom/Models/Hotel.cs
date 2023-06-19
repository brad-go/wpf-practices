using System.Collections.Generic;

namespace Reservoom.Models
{
    public class Hotel
    {
        private readonly ReservationBook _reservationBook;

        public string Name { get; }

        public Hotel(string name)
        {
            Name = name;

            _reservationBook = new ReservationBook();
        }

        /// <summary>
        /// 모든 예약들을 가져옵니다.
        /// </summary>
        /// <param name="username">유저의 이름</param>
        /// <returns>유저를 위한 예약들</returns>
        public IEnumerable<Reservation> GetAllReservations()
        {
            return _reservationBook.GetAllReservations();
        }

        /// <summary>
        /// 예약을 합니다.
        /// </summary>
        /// <param name="reservation">새로운 예약</param>
        /// <exception cref="ReservationConflictException"/>
        public void MakeReservation(Reservation reservation)
        {
            _reservationBook.AddReservation(reservation);
        }
    }
}
