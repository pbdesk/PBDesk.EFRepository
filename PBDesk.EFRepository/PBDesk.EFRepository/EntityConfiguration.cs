using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDesk.EFRepository
{
    public class EntityConfiguration<T> : EntityTypeConfiguration<T>     where T:  Entity
    {
        public EntityConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.IsActive).HasColumnName("IsActive"); ;
            Property(x => x.LastUpdBy).HasColumnName("LastUpdBy");
            Property(x => x.LastUpdDate).HasColumnName("LastUpdDate");
        }
    }
}
