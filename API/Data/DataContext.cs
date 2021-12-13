using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ////-----------------------UserLike-------------------------------------------//
            //creating PK with combination of SoruceUserId,LikeduserID
            builder.Entity<UserLike>()
                   .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            //source user can  make like many other users.OnDelete()//this,if we delete a user,we delete the related entities.
            //For sql server set no migration if you using SQL server else you will get errors during add migration
            //SourceUser => LikedUsers
            builder.Entity<UserLike>()
                   .HasOne(s => s.SourceUser)
                   .WithMany(x => x.LikedUsers)
                   .HasForeignKey(s => s.SourceUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            //here for LikedUser =>  LikedByUsers
            builder.Entity<UserLike>()
                  .HasOne(s => s.LikedUser)
                  .WithMany(x => x.LikedByUsers)
                  .HasForeignKey(s => s.LikedUserId)
                  .OnDelete(DeleteBehavior.NoAction);
            ////-----------------------UserLike-------------------------------------------//
            ///////-----------------------Message-------------------------------------------//
            //here Message entity
            builder.Entity<Message>()
                   .HasOne(u => u.Recipient)
                   .WithMany(r => r.MessagesReceived)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                  .HasOne(u => u.Sender)
                  .WithMany(r => r.MessagesSent)
                  .OnDelete(DeleteBehavior.Restrict);
            ////----------------------------Message--------------------------------------//


        }
    }
}
