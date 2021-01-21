using GameWebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameWebAPI.Repositories
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<FriendInvite> FriendsRequests { get; set; }
        public DbSet<PlayerFriend> FriendsList { get; set; }
        public DbSet<GameInvite> GameInvites { get; set; }

        public DbSet<PlayerMatchdata> PlayersMatchdata { get; set; }

        // DbContextOptions is defined at Startup Configurate Services
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity Player properties check constraints
            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<Player>().HasKey(p => p.username);
            modelBuilder.Entity<Player>().HasIndex(u => u.email).IsUnique();

            // Entity FriendInvite
            modelBuilder.Entity<FriendInvite>().ToTable("FriendInvites");
            modelBuilder.Entity<FriendInvite>().HasKey(f => new { f.playerId, f.targetPlayerId});

            // Entity PlayerFriend
            modelBuilder.Entity<PlayerFriend>().ToTable("FriendsList");
            modelBuilder.Entity<PlayerFriend>().HasKey(f => new { f.playerId, f.friendId});

            //Entity GameRequests
            modelBuilder.Entity<GameInvite>().ToTable("GameInvites");
            modelBuilder.Entity<GameInvite>().HasKey(g => new { g.roomId, g.invitedId});

            //Entity PlayerMatchdata
            modelBuilder.Entity<PlayerMatchdata>().ToTable("PlayersMatchdata");
            modelBuilder.Entity<PlayerMatchdata>().HasKey(p => new { p.playerId, p.gameID });

            // use this line when entity has a list property
            // modelBuilder.Entity<Entity>().HasMany(e => e.propertyName).WithOne(e => e.entity);
        }
    }
}