using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Godch.Models
{
    public partial class GODCHContext : DbContext
    {
        public GODCHContext()
        {
        }

        public GODCHContext(DbContextOptions<GODCHContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChatGroup> ChatGroups { get; set; } = null!;
        public virtual DbSet<ChatGroupRelation> ChatGroupRelations { get; set; } = null!;
        public virtual DbSet<Forum> Forums { get; set; } = null!;
        public virtual DbSet<ForumMember> ForumMembers { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<PublishType> PublishTypes { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<ServerLog> ServerLogs { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<TagRelation> TagRelations { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRelationShip> UserRelationShips { get; set; } = null!;
        public virtual DbSet<Work> Works { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=GODCH;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatGroup>(entity =>
            {
                entity.HasKey(e => e.ChatId);

                entity.Property(e => e.ChatId).HasColumnName("ChatID");

                entity.Property(e => e.ChatTitle).HasMaxLength(64);

                entity.Property(e => e.Config).HasMaxLength(50);
            });

            modelBuilder.Entity<ChatGroupRelation>(entity =>
            {
                entity.HasKey(e => e.Rid)
                    .HasName("PK_ChatGroupsRelations");

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.ChatId).HasColumnName("ChatID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.ChatGroupRelations)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatGroupsRelations_CID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChatGroupRelations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatGroupsRelations_UID");
            });

            modelBuilder.Entity<Forum>(entity =>
            {
                entity.Property(e => e.ForumId).HasColumnName("ForumID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.ForumName).HasMaxLength(50);

                entity.HasOne(d => d.PublishTypeNavigation)
                    .WithMany(p => p.Forums)
                    .HasForeignKey(d => d.PublishType)
                    .HasConstraintName("FK_FPType");
            });

            modelBuilder.Entity<ForumMember>(entity =>
            {
                entity.HasKey(e => e.Rid)
                    .HasName("PK_ForumParticipants");

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.ForumId).HasColumnName("ForumID");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.HasOne(d => d.Forum)
                    .WithMany(p => p.ForumMembers)
                    .HasForeignKey(d => d.ForumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForumMembers_FID");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.ForumMembers)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForumMembers_FMID");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.AuthorId).HasColumnName("AuthorID");

                entity.Property(e => e.ForumId).HasColumnName("ForumID");

                entity.Property(e => e.HeadPostId).HasColumnName("HeadPostID");

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK_Posts_AUID");
            });

            modelBuilder.Entity<PublishType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_PublishType");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.Description).HasMaxLength(10);

                entity.Property(e => e.Detail).HasMaxLength(200);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Description)
                    .HasMaxLength(30)
                    .IsFixedLength();
            });

            modelBuilder.Entity<ServerLog>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Client).HasMaxLength(50);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LaunchTime).HasMaxLength(20);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.TagName).HasMaxLength(30);
            });

            modelBuilder.Entity<TagRelation>(entity =>
            {
                entity.HasKey(e => e.Rid);

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.Property(e => e.WorkId).HasColumnName("WorkID");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.TagRelations)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TagRelations_TID");

                entity.HasOne(d => d.Work)
                    .WithMany(p => p.TagRelations)
                    .HasForeignKey(d => d.WorkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TagRelations_WID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Account).HasMaxLength(32);

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.EmailAddress).HasMaxLength(64);

                entity.Property(e => e.FullName).HasMaxLength(64);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(64);
            });

            modelBuilder.Entity<UserRelationShip>(entity =>
            {
                entity.HasKey(e => e.Rid);

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserId2).HasColumnName("UserID2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRelationShipUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRelationShips_UID");

                entity.HasOne(d => d.UserId2Navigation)
                    .WithMany(p => p.UserRelationShipUserId2Navigations)
                    .HasForeignKey(d => d.UserId2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRelationShips_FID");
            });

            modelBuilder.Entity<Work>(entity =>
            {
                entity.Property(e => e.WorkId).HasColumnName("WorkID");

                entity.Property(e => e.AuthorId).HasColumnName("AuthorID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.WorkName).HasMaxLength(50);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Works)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Works_Author");

                entity.HasOne(d => d.PublishTypeNavigation)
                    .WithMany(p => p.Works)
                    .HasForeignKey(d => d.PublishType)
                    .HasConstraintName("FK_Works_Pub");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
