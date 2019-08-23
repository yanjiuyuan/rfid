namespace DingTalk.Models.DingModelsHs
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DDContextHs : DbContext
    {
        public DDContextHs()
            : base("name=DDContextHs")
        {
        }

        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<ProjectInfo> ProjectInfo { get; set; }
        
    }
}
