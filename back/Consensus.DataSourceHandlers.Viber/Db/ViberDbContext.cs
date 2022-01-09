using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Consensus.DataSourceHandlers.Viber.Db.Entities;

namespace Consensus.DataSourceHandlers.Viber.Db
{
    public partial class ViberDbContext : DbContext
    {
        public ViberDbContext()
        {
        }

        public ViberDbContext(DbContextOptions<ViberDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChatInfo> ChatInfos { get; set; } = null!;
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("Data Source=D:/tmp/viber/viber.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatInfo>(entity =>
            {
                entity.HasKey(e => e.ChatId);

                entity.ToTable("ChatInfo");

                entity.HasIndex(e => e.Token, "IX_ChatInfo_Token")
                    .IsUnique();

                entity.HasIndex(e => e.Token, "ChatInfo_Token_Index");

                entity.Property(e => e.ChatId)
                    .HasColumnType("integer")
                    .HasColumnName("ChatID");

                entity.Property(e => e.BackgroundId)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("BackgroundID");

                entity.Property(e => e.Flags)
                    .HasColumnType("integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IconId)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("IconID");

                entity.Property(e => e.LastReadMessageId)
                    .HasColumnType("integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.LastReadMessageToken)
                    .HasColumnType("integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.LastSeenMessageToken)
                    .HasColumnType("integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.MetaData).HasColumnType("varchar(255)");

                entity.Property(e => e.Name).HasColumnType("varchar(200)");

                entity.Property(e => e.Pgcountry)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("PGCountry");

                entity.Property(e => e.PglastMessageId)
                    .HasColumnType("integer")
                    .HasColumnName("PGLastMessageID");

                entity.Property(e => e.Pglatitude)
                    .HasColumnType("integer")
                    .HasColumnName("PGLatitude");

                entity.Property(e => e.Pglongtitude)
                    .HasColumnType("integer")
                    .HasColumnName("PGLongtitude");

                entity.Property(e => e.Pgrevision)
                    .HasColumnType("integer")
                    .HasColumnName("PGRevision");

                entity.Property(e => e.PgsearchExFlags)
                    .HasColumnType("integer")
                    .HasColumnName("PGSearchExFlags");

                entity.Property(e => e.PgsearchFlags)
                    .HasColumnType("integer")
                    .HasColumnName("PGSearchFlags");

                entity.Property(e => e.PgtabLine)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("PGTabLine");

                entity.Property(e => e.Pgtags)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("PGTags");

                entity.Property(e => e.Pgtype)
                    .HasColumnType("integer")
                    .HasColumnName("PGType");

                entity.Property(e => e.Pguri)
                    .HasColumnType("varchar(255)")
                    .HasColumnName("PGUri");

                entity.Property(e => e.PgwatchersCount)
                    .HasColumnType("integer")
                    .HasColumnName("PGWatchersCount");

                entity.Property(e => e.TimeStamp).HasColumnType("longint");

                entity.Property(e => e.Token).HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.HasIndex(e => e.EncryptedMid, "IX_Contact_EncryptedMID")
                    .IsUnique();

                entity.HasIndex(e => e.EncryptedNumber, "IX_Contact_EncryptedNumber")
                    .IsUnique();

                entity.HasIndex(e => e.Mid, "IX_Contact_MID")
                    .IsUnique();

                entity.HasIndex(e => e.Number, "IX_Contact_Number")
                    .IsUnique();

                entity.HasIndex(e => e.Vid, "IX_Contact_VID")
                    .IsUnique();

                entity.HasIndex(e => e.EncryptedMid, "Contact_EncryptedMID_Index");

                entity.HasIndex(e => e.EncryptedNumber, "Contact_EncryptedNumber_Index");

                entity.HasIndex(e => e.Mid, "Contact_MID_Index");

                entity.HasIndex(e => e.Number, "Contact_Number_Index");

                entity.Property(e => e.ContactId)
                    .HasColumnType("integer")
                    .HasColumnName("ContactID");

                entity.Property(e => e.Abcontact)
                    .HasColumnType("smallint (0,1)")
                    .HasColumnName("ABContact");

                entity.Property(e => e.ContactFlags)
                    .HasColumnType("long")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DownloadId).HasColumnName("DownloadID");

                entity.Property(e => e.EncryptedMid).HasColumnName("EncryptedMID");

                entity.Property(e => e.Mid).HasColumnName("MID");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("longint")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ViberContact).HasColumnType("smallint (0,1)");

                entity.Property(e => e.Vid).HasColumnName("VID");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasIndex(e => new { e.ChatId, e.SortOrder, e.TimeStamp }, "Events_ChatID_SortOrder_TimeStamp_Index");

                entity.HasIndex(e => new { e.ChatId, e.Flags, e.Token }, "Events_ChatID_Token_Flags_Index");

                entity.HasIndex(e => e.ContactId, "Events_Number_Index");

                entity.HasIndex(e => e.Seq, "Events_Seq_Index");

                entity.HasIndex(e => e.SortOrder, "Events_SortOrder_Index");

                entity.HasIndex(e => e.Token, "Events_Token_Index");

                entity.Property(e => e.EventId)
                    .HasColumnType("integer")
                    .HasColumnName("EventID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("integer")
                    .HasColumnName("ChatID");

                entity.Property(e => e.ContactId)
                    .HasColumnType("integer")
                    .HasColumnName("ContactID");

                entity.Property(e => e.ContactLatitude)
                    .HasColumnType("signed long")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ContactLongitude)
                    .HasColumnType("signed long")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Direction).HasColumnType("unsigned integer");

                entity.Property(e => e.Flags)
                    .HasColumnType("integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsRead).HasColumnType("smallint (0,1)");

                entity.Property(e => e.IsSessionLifeTime)
                    .HasColumnType("unsigned integer (0, 1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Seq).HasColumnType("integer");

                entity.Property(e => e.SortOrder)
                    .HasColumnType("unsigned long")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.TimeStamp).HasColumnType("longint");

                entity.Property(e => e.Token).HasColumnType("unsigned long");

                entity.Property(e => e.Type).HasColumnType("smallint");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ChatId);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ContactId);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.HasIndex(e => e.ClientFlag, "Messages_ClientFlag");

                entity.HasIndex(e => e.PgmessageId, "Messages_PGMessageId_Index");

                entity.HasIndex(e => e.PttId, "Messages_PttID_Index");

                entity.HasIndex(e => e.Status, "Messages_Status_Index");

                entity.HasIndex(e => e.Type, "Messages_Type_Index");

                entity.Property(e => e.EventId)
                    .HasColumnType("integer")
                    .ValueGeneratedNever()
                    .HasColumnName("EventID");

                entity.Property(e => e.AdminsReactions).HasColumnType("varchar(255)");

                entity.Property(e => e.AppId)
                    .HasColumnType("integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Body).HasColumnType("varchar(5000)");

                entity.Property(e => e.ClientFlag)
                    .HasColumnType("unsigned integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Duration)
                    .HasColumnType("signed")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Flag)
                    .HasColumnType("unsigned integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.FollowersLikeCount)
                    .HasColumnType("unsigned integer")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Info).HasColumnType("varchar(7000)");

                entity.Property(e => e.MembersReactions).HasColumnType("varchar(255)");

                entity.Property(e => e.PayloadPath).HasColumnType("varchar(1000)");

                entity.Property(e => e.PgisLiked)
                    .HasColumnType("integer")
                    .HasColumnName("PGIsLiked");

                entity.Property(e => e.PglikeCount)
                    .HasColumnType("integer")
                    .HasColumnName("PGLikeCount");

                entity.Property(e => e.PgmessageId)
                    .HasColumnType("unsigned long")
                    .HasColumnName("PGMessageId")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.PrevReaction).HasColumnType("integer");

                entity.Property(e => e.PttId)
                    .HasColumnType("varchar(100)")
                    .HasColumnName("PttID");

                entity.Property(e => e.PttStatus)
                    .HasColumnType("unsigned short")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Status).HasColumnType("integer");

                entity.Property(e => e.StickerId)
                    .HasColumnType("unsigned long")
                    .HasColumnName("StickerID")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Subject).HasColumnType("varchar(500)");

                entity.Property(e => e.ThumbnailPath).HasColumnType("varchar(100)");

                entity.Property(e => e.Type).HasColumnType("unsigned integer");

                entity.HasOne(d => d.Event)
                    .WithOne(p => p.Message)
                    .HasForeignKey<Message>(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
