using Reservoom.ViewModels;
using System.Windows;

namespace Reservoom
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Hotel hotel = new Hotel("Brad Suites");

            //try
            //{
            //    hotel.MakeReservation(new Reservation(
            //        new RoomID(1, 3),
            //        "Brad",
            //        new DateTime(2023, 06, 18),
            //        new DateTime(2023, 06, 19)));
            //    hotel.MakeReservation(new Reservation(
            //        new RoomID(1, 3),
            //        "Brad",
            //        new DateTime(2023, 06, 20),
            //        new DateTime(2023, 06, 21)));
            //}
            //catch (ReservationConflictException ex)
            //{

            //}

            //IEnumerable<Reservation> reservations = hotel.GetAllReservations();
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel()
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
