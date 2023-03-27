using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.RequestNote;

namespace Sofco.DAL.Mappings.RequestNote
{
    public static class RequestNoteMapping
    {
        public static void MapRequestNote(this ModelBuilder builder)
        {
            //RequestNote
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasKey(t => t.Id);

            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasOne(x => x.UserApplicant).WithMany(x => x.RequestNotes).HasForeignKey(x => x.UserApplicantId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasOne(x => x.ProviderArea).WithMany(x => x.RequestNotes).HasForeignKey(x => x.ProviderAreaId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasOne(x => x.Workflow).WithMany(x => x.RequestNotes).HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasOne(x => x.Status).WithMany(x => x.RequestNotes).HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.Histories).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId);
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.ProductsServices).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId);
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.Analytics).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.Providers).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId);
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.Trainings).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId);            
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.Travels).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId);            
            builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.Attachments).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId);
            //builder.Entity<Domain.Models.RequestNote.RequestNote>().HasMany(x => x.ProvidersSugg).WithOne(x => x.RequestNote).HasForeignKey(x => x.RequestNoteId);

            //RequestNoteHistory
            builder.Entity<RequestNoteHistory>().HasKey(x => x.Id);
            builder.Entity<RequestNoteHistory>().Property(x => x.Comment).HasMaxLength(1000);
            builder.Entity<RequestNoteHistory>().Property(x => x.UserName).HasMaxLength(100);

            builder.Entity<RequestNoteHistory>().HasOne(x => x.StatusFrom).WithMany(x => x.RequestNoteHistories).HasForeignKey(x => x.StatusFromId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<RequestNoteHistory>().HasOne(x => x.StatusTo).WithMany(x => x.RequestNoteHistories2).HasForeignKey(x => x.StatusToId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RequestNoteHistory>().HasOne(x => x.RequestNote).WithMany(x => x.Histories).HasForeignKey(x => x.RequestNoteId);

            //RequestNoteTravel
            builder.Entity<RequestNoteTravel>().HasKey(x => x.Id);
            builder.Entity<RequestNoteTravel>()
                .HasOne(pt => pt.RequestNote)
                .WithMany(p => p.Travels)
                .HasForeignKey(pt => pt.RequestNoteId);

            //RequestNoteTravelEmployee
            builder.Entity<RequestNoteTravelEmployee>().HasKey(x => new { x.EmployeeId, x.RequestNoteTravelId });

            builder.Entity<RequestNoteTravel>().HasMany(x => x.Employees).WithOne(x => x.RequestNoteTravel).HasForeignKey(x => x.RequestNoteTravelId);

            //RequestNoteTraining
            builder.Entity<RequestNoteTraining>().HasKey(x => x.Id);
            builder.Entity<RequestNoteTraining>()
                .HasOne(pt => pt.RequestNote)
                .WithMany(p => p.Trainings)
                .HasForeignKey(pt => pt.RequestNoteId);

            //RequestNoteTrainingEmployee
            builder.Entity<RequestNoteTrainingEmployee>().HasKey(x => new { x.EmployeeId, x.RequestNoteTainingId });

            builder.Entity<RequestNoteTraining>().HasMany(x => x.Employees).WithOne(x => x.RequestNoteTraining).HasForeignKey(x => x.RequestNoteTainingId);

            //RequestNoteFile
            builder.Entity<RequestNoteFile>().HasKey(t => new { t.FileId, t.RequestNoteId });

            builder.Entity<RequestNoteFile>()
                .HasOne(pt => pt.RequestNote)
                .WithMany(p => p.Attachments)
                .HasForeignKey(pt => pt.RequestNoteId);

            builder.Entity<RequestNoteFile>()
                .HasOne(pt => pt.File)
                .WithMany()
                .HasForeignKey(pt => pt.FileId);

        }
    }
}
