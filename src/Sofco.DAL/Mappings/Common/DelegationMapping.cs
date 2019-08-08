using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Mappings.Common
{
    public static class DelegationMapping
    {
        public static void MapDelegation(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Delegation>().HasKey(_ => _.Id);
            builder.Entity<Delegation>().HasOne(x => x.User).WithMany(x => x.Delegations1).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Delegation>().HasOne(x => x.GrantedUser).WithMany(x => x.Delegations2).HasForeignKey(x => x.GrantedUserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
