using Microsoft.EntityFrameworkCore;
using Reservoom.DTOs;

namespace Reservoom.DbContexts
{
    public class ReservoomDbContext : DbContext
    {
        public ReservoomDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ReservationDTO> Reservations { get; set; }
    }
}
