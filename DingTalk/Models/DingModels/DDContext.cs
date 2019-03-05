namespace DingTalk.Models.DingModels
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
        public virtual DbSet<Car> Car { get; set; }
        public virtual DbSet<CarTable> CarTable { get; set; }
        public virtual DbSet<Code> Code { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<FileInfos> FileInfos { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<KisOffice> KisOffice { get; set; }
        public virtual DbSet<KisPurchase> KisPurchase { get; set; }
        public virtual DbSet<LeaveWord> LeaveWord { get; set; }
        public virtual DbSet<NewsAndCases> NewsAndCases { get; set; }
        public virtual DbSet<OfficeSupplies> OfficeSupplies { get; set; }
        public virtual DbSet<OfficeSuppliesPurchase> OfficeSuppliesPurchase { get; set; }
        public virtual DbSet<OverTime> OverTime { get; set; }
        public virtual DbSet<ProcedureInfo> ProcedureInfo { get; set; }
        public virtual DbSet<ProjectInfo> ProjectInfo { get; set; }
        public virtual DbSet<PurchaseDown> PurchaseDown { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual DbSet<PurchaseProcedureInfo> PurchaseProcedureInfo { get; set; }
        public virtual DbSet<PurchaseTable> PurchaseTable { get; set; }
        public virtual DbSet<Receiving> Receiving { get; set; }
        public virtual DbSet<Vote> Vote { get; set; }
        public virtual DbSet<Worker> Worker { get; set; }
        public virtual DbSet<WorkTime> WorkTime { get; set; }
        public virtual DbSet<GiftTable> GiftTable { get; set; }

        public virtual DbSet<PicInfo> PicInfo { get; set; }

        public virtual DbSet<MaterialCode> MaterialCode { get; set; }

        public virtual DbSet<GoDown> GoDown { get; set; }

        public virtual DbSet<Pick> Pick { get; set; }

        public virtual DbSet<ErrorLogs> ErrorLogs { get; set; }

        public virtual DbSet<Evection> Evection { get; set; }
        
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
                .Property(e => e.Id)
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

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.BackNodeId)
                .IsUnicode(false);

            modelBuilder.Entity<NodeInfo>()
                .Property(e => e.ChoseNodeId)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.BomId)
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

            modelBuilder.Entity<Purchase>()
                .Property(e => e.SingleWeight)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.AllWeight)
                .IsUnicode(false);

            modelBuilder.Entity<Purchase>()
                .Property(e => e.NeedTime)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Roles>()
                .Property(e => e.RoleName)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.CreateMan)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.CreateManId)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.UserId)
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
                .Property(e => e.Dept)
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

            modelBuilder.Entity<Tasks>()
                .Property(e => e.OldImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.OldFileUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.MediaId)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.FilePDFUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.OldFilePDFUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.MediaIdPDF)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.PdfState)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.ProjectName)
                .IsUnicode(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.counts)
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

            modelBuilder.Entity<Car>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Car>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.CarNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.CreateMan)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.Remark)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.UseMan)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.OccupyCarId)
                .IsUnicode(false);

            modelBuilder.Entity<Car>()
                .Property(e => e.UseTimes)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.PeerNumber)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.MainContent)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.PlantTravelWay)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.FactTravelWay)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.StartKilometres)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.EndKilometres)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.UseKilometres)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.CarId)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.DrivingMan)
                .IsUnicode(false);

            modelBuilder.Entity<CarTable>()
                .Property(e => e.OccupyCarId)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Code>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.CodeNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.BigCode)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.SmallCode)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.Standard)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.SurfaceTreatment)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.PerformanceLevel)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.StandardNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.Features)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.purpose)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.Remark)
                .IsUnicode(false);

            modelBuilder.Entity<Code>()
                .Property(e => e.FNote)
                .IsUnicode(false);

            modelBuilder.Entity<Contract>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Contract>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<Contract>()
                .Property(e => e.ContractNo)
                .IsUnicode(false);

            modelBuilder.Entity<Contract>()
                .Property(e => e.SignUnit)
                .IsUnicode(false);

            modelBuilder.Entity<Contract>()
                .Property(e => e.SalesManager)
                .IsUnicode(false);

            modelBuilder.Entity<Contract>()
                .Property(e => e.ContractType)
                .IsUnicode(false);

            modelBuilder.Entity<Contract>()
                .Property(e => e.Path)
                .IsUnicode(false);

            modelBuilder.Entity<Contract>()
                .Property(e => e.FilePath)
                .IsUnicode(false);

            modelBuilder.Entity<FileInfos>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FileInfos>()
                .Property(e => e.ApplyMan)
                .IsUnicode(false);

            modelBuilder.Entity<FileInfos>()
                .Property(e => e.ApplyManId)
                .IsUnicode(false);

            modelBuilder.Entity<FileInfos>()
                .Property(e => e.FilePath)
                .IsUnicode(false);

            modelBuilder.Entity<FileInfos>()
                .Property(e => e.LastModifyTime)
                .IsUnicode(false);

            modelBuilder.Entity<FileInfos>()
                .Property(e => e.LastModifyState)
                .IsUnicode(false);

            modelBuilder.Entity<FileInfos>()
                .Property(e => e.MediaId)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.JobName)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.Require)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.WorkPlace)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.Pay)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.Url)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.BigType)
                .IsUnicode(false);

            modelBuilder.Entity<Jobs>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<KisOffice>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<KisOffice>()
                .Property(e => e.FItemID)
                .IsUnicode(false);

            modelBuilder.Entity<KisOffice>()
                .Property(e => e.FNumber)
                .IsUnicode(false);

            modelBuilder.Entity<KisOffice>()
                .Property(e => e.FName)
                .IsUnicode(false);

            modelBuilder.Entity<KisOffice>()
                .Property(e => e.FModel)
                .IsUnicode(false);

            modelBuilder.Entity<KisOffice>()
                .Property(e => e.FNote)
                .IsUnicode(false);

            modelBuilder.Entity<KisPurchase>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<KisPurchase>()
                .Property(e => e.FItemID)
                .IsUnicode(false);

            modelBuilder.Entity<KisPurchase>()
                .Property(e => e.FNumber)
                .IsUnicode(false);

            modelBuilder.Entity<KisPurchase>()
                .Property(e => e.FName)
                .IsUnicode(false);

            modelBuilder.Entity<KisPurchase>()
                .Property(e => e.FModel)
                .IsUnicode(false);

            modelBuilder.Entity<KisPurchase>()
                .Property(e => e.FNote)
                .IsUnicode(false);

            modelBuilder.Entity<LeaveWord>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<LeaveWord>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<LeaveWord>()
                .Property(e => e.Content)
                .IsUnicode(false);

            modelBuilder.Entity<LeaveWord>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.Contents)
                .IsUnicode(false);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.BigType)
                .IsUnicode(false);

            modelBuilder.Entity<NewsAndCases>()
                .Property(e => e.Abstract)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.CodeNo)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Standard)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Count)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Price)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Purpose)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.UrgentDate)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.Mark)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSupplies>()
                .Property(e => e.ExpectPrice)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.CodeNo)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Standard)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Count)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Price)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.ExpectPrice)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Purpose)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.UrgentDate)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Mark)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.ApplyMan)
                .IsUnicode(false);

            modelBuilder.Entity<OfficeSuppliesPurchase>()
                .Property(e => e.Dept)
                .IsUnicode(false);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.DateTime)
                .IsUnicode(false);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.StartTime)
                .IsUnicode(false);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.EndTimeTime)
                .IsUnicode(false);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.UseTime)
                .IsUnicode(false);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.OverTimeContent)
                .IsUnicode(false);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<OverTime>()
                .Property(e => e.EffectiveTime)
                .IsUnicode(false);

            modelBuilder.Entity<ProcedureInfo>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ProcedureInfo>()
                .Property(e => e.ProcedureName)
                .IsUnicode(false);

            modelBuilder.Entity<ProcedureInfo>()
                .Property(e => e.DefaultWorkTime)
                .IsUnicode(false);

            modelBuilder.Entity<ProcedureInfo>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<ProcedureInfo>()
                .Property(e => e.ApplyMan)
                .IsUnicode(false);

            modelBuilder.Entity<ProcedureInfo>()
                .Property(e => e.ApplyManId)
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
                .Property(e => e.ProjectState)
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

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.FilePath)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.ResponsibleMan)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.ResponsibleManId)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.ProjectType)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.TeamMembers)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.TeamMembersId)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.CreateMan)
                .IsUnicode(false);

            modelBuilder.Entity<ProjectInfo>()
                .Property(e => e.CreateManId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.OldTaskId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.BomId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.DrawingNo)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.CodeNo)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.Count)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.MaterialScience)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.Brand)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.Sorts)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.Mark)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.PurchaseProcedureInfoId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseDown>()
                .Property(e => e.FlowType)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.BomId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.DrawingNo)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.CodeNo)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Count)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.MaterialScience)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Brand)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Sorts)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.Mark)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.SingleWeight)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.AllWeight)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(e => e.NeedTime)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseProcedureInfo>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PurchaseProcedureInfo>()
                .Property(e => e.DrawingNo)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseProcedureInfo>()
                .Property(e => e.ProcedureInfoId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseProcedureInfo>()
                .Property(e => e.CreateManId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseProcedureInfo>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseProcedureInfo>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.CodeNo)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Standard)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Count)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Price)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Purpose)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.UrgentDate)
                .IsUnicode(false);

            modelBuilder.Entity<PurchaseTable>()
                .Property(e => e.Mark)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.TaskId)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.ReceivingUnit)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.ReceivingNo)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.FileNo)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.ReceivingTime)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.MainIdea)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.Suggestion)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.Leadership)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.Review)
                .IsUnicode(false);

            modelBuilder.Entity<Receiving>()
                .Property(e => e.HandleImplementation)
                .IsUnicode(false);

            modelBuilder.Entity<Vote>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Vote>()
                .Property(e => e.ApplyMan)
                .IsUnicode(false);

            modelBuilder.Entity<Vote>()
                .Property(e => e.ApplyManId)
                .IsUnicode(false);

            modelBuilder.Entity<Vote>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Vote>()
                .Property(e => e.Option)
                .IsUnicode(false);

            modelBuilder.Entity<Vote>()
                .Property(e => e.VoteCount)
                .IsUnicode(false);

            modelBuilder.Entity<Vote>()
                .Property(e => e.SubmitterId)
                .IsUnicode(false);

            modelBuilder.Entity<Worker>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Worker>()
                .Property(e => e.WorkerId)
                .IsUnicode(false);

            modelBuilder.Entity<Worker>()
                .Property(e => e.WorkerName)
                .IsUnicode(false);

            modelBuilder.Entity<Worker>()
                .Property(e => e.CreateManId)
                .IsUnicode(false);

            modelBuilder.Entity<Worker>()
                .Property(e => e.CreateTime)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTime>()
                .Property(e => e.Id)
                .HasPrecision(18, 0);

            modelBuilder.Entity<WorkTime>()
                .Property(e => e.PurchaseProcedureInfoId)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTime>()
                .Property(e => e.Worker)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTime>()
                .Property(e => e.WorkerId)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTime>()
                .Property(e => e.StartTime)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTime>()
                .Property(e => e.EndTime)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTime>()
                .Property(e => e.UseTime)
                .IsUnicode(false);
        }
    }
}
