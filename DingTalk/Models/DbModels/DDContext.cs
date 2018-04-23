namespace DingTalk.Models.DbModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DDContext : DbContext
    {
        public DDContext()
            : base("name=DDContext")
        {
        }

        public virtual DbSet<Approve> Approve { get; set; }
        public virtual DbSet<FlowMan> FlowMan { get; set; }
        public virtual DbSet<FlowProgress> FlowProgress { get; set; }
        public virtual DbSet<FlowQx> FlowQx { get; set; }
        public virtual DbSet<Flows> Flows { get; set; }
        public virtual DbSet<FlowSort> FlowSort { get; set; }
        public virtual DbSet<NodeInfo> NodeInfo { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<ProjectInfo> ProjectInfo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Approve>()
                .Property(e => e.ApproveNo)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Approve>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<Approve>()
                .Property(e => e.HandleTime)
                .IsUnicode(false);

            modelBuilder.Entity<Approve>()
                .Property(e => e.ApproveTime)
                .IsUnicode(false);

            modelBuilder.Entity<Approve>()
                .Property(e => e.ApproveMan)
                .IsUnicode(false);

            modelBuilder.Entity<Approve>()
                .Property(e => e.ApproveView)
                .IsUnicode(false);

            modelBuilder.Entity<Approve>()
                .Property(e => e.FJPath)
                .IsUnicode(false);

            modelBuilder.Entity<FlowMan>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FlowMan>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<FlowMan>()
                .Property(e => e.Node)
                .IsUnicode(false);

            modelBuilder.Entity<FlowMan>()
                .Property(e => e.RoleId)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProgress>()
                .Property(e => e.TaskId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FlowProgress>()
                .Property(e => e.CurrentManId)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProgress>()
                .Property(e => e.CurrentMan)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProgress>()
                .Property(e => e.ApprovedTime)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProgress>()
                .Property(e => e.StartTime)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProgress>()
                .Property(e => e.Remaek)
                .IsUnicode(false);

            modelBuilder.Entity<FlowQx>()
                .Property(e => e.ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FlowQx>()
                .Property(e => e.FlowId)
                .IsUnicode(false);

            modelBuilder.Entity<FlowQx>()
                .Property(e => e.Qx_Range)
                .IsUnicode(false);

            modelBuilder.Entity<FlowQx>()
                .Property(e => e.User)
                .IsUnicode(false);

            modelBuilder.Entity<FlowQx>()
                .Property(e => e.Dept)
                .IsUnicode(false);

            modelBuilder.Entity<FlowQx>()
                .Property(e => e.Role)
                .IsUnicode(false);

            modelBuilder.Entity<FlowQx>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<Flows>()
                .Property(e => e.FlowId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Flows>()
                .Property(e => e.FlowName)
                .IsUnicode(false);

            modelBuilder.Entity<Flows>()
                .Property(e => e.CreateMan)
                .IsUnicode(false);

            modelBuilder.Entity<Flows>()
                .Property(e => e.CreateManId)
                .IsUnicode(false);

            modelBuilder.Entity<Flows>()
                .Property(e => e.ApplyTime)
                .IsUnicode(false);

            modelBuilder.Entity<Flows>()
                .Property(e => e.ApplyMan)
                .IsUnicode(false);

            modelBuilder.Entity<FlowSort>()
                .Property(e => e.SORT_ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FlowSort>()
                .Property(e => e.SORT_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<FlowSort>()
                .Property(e => e.DEPT_ID)
                .IsUnicode(false);

            modelBuilder.Entity<FlowSort>()
                .Property(e => e.SORT_PARENT)
                .IsUnicode(false);

            modelBuilder.Entity<FlowSort>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.FlowId)
                .IsUnicode(false);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.NodeName)
                .IsUnicode(false);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.NodePeople)
                .IsUnicode(false);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.PeopleId)
                .IsUnicode(false);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.PreNodeId)
                .IsUnicode(false);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.Condition)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.DrawingNo)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.CodeNo)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Count)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.MaterialScience)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Brand)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Sorts)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Mark)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Roles>()
                .Property(e => e.FlowId)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.NodeName)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.ApplyMan)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.ApplyManId)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.ApplyTime)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.Remark)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.FileUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.ProjectId)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.nickName)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.corpId)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.emplId)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.rightLevel)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.avatar)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.FinnalLoginTime)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.ProjectName)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.DeptName)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.ApplyMan)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.ApplyManId)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.StartTime)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.EndTime)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.ProjectId)
                .IsUnicode(false);
        }
    }
}
