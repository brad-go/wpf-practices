using System;

namespace Reservoom.Models
{
    public class Reservation
    {
        public RoomID RoomID { get; }
        public string Username { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public TimeSpan Length => EndTime.Subtract(StartTime);

        public Reservation(RoomID roomID, string username, DateTime startTime, DateTime endTime)
        {
            RoomID = roomID;
            Username = username;
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// 현재 예약과 입력받은 예약이 서로 겹치는지 확인합니다.
        /// </summary>
        /// <param name="reservation">새로운 예약</param>
        /// <returns>현재 예약과 입력받은 예약이 겹치는지에 대한 여부</returns>
        internal bool Conflicts(Reservation reservation)
        {
            if (reservation.RoomID != RoomID)
            {
                return false;
            }

            return reservation.StartTime < EndTime || reservation.EndTime > StartTime;
        }
    }
}
