using GiftShuffle.Domain.Entities;
using GiftShuffle.Infraestructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GiftShuffle.Infraestructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }
        public DbSet<Friend> Friends => Set<Friend>();
        public DbSet<ShuffleHistory> ShuffleHistories => Set<ShuffleHistory>();
    }
}
