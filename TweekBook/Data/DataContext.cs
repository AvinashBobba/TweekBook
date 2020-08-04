using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TweekBook.Domain;

namespace TweekBook.Data
{
    public class DataContext : IdentityDbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
    }
}
