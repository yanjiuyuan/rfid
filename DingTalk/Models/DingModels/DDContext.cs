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
        public virtual DbSet<ContextError> ContextError { get; set; }
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

        public virtual DbSet<CreateProject> CreateProject { get; set; }

        public virtual DbSet<TasksState> TasksState { get; set; }

        public virtual DbSet<Cooperate> Cooperate { get; set; }
        public virtual DbSet<DrawingChange> DrawingChange { get; set; }

        public virtual DbSet<FileChange> FileChange { get; set; }

        public virtual DbSet<Gift> Gift { get; set; }

        public virtual DbSet<TechnicalSupport> TechnicalSupport { get; set; }

        public virtual DbSet<MaterialRelease> MaterialRelease { get; set; }

        public virtual DbSet<IntellectualProperty> IntellectualProperty { get; set; }

        public virtual DbSet<Borrow> Borrow { get; set; }

        public virtual DbSet<Maintain> Maintain { get; set; }

        public virtual DbSet<TasksQuery> TasksQuery { get; set; }

        public virtual DbSet<ProjectFunding> ProjectFunding { get; set; }

        public virtual DbSet<ApplicationUnit> ApplicationUnit { get; set; }

        public virtual DbSet<DetailedList> DetailedList { get; set; }

        public virtual DbSet<ProjectClosure> ProjectClosure { get; set; }

        public virtual DbSet<LongitudinalProject> LongitudinalProject { get; set; }

        public virtual DbSet<ProcessingProgress> ProcessingProgress { get; set; }

        public virtual DbSet<Role> Role { get; set; }

        public DbSet<TableInfo> TableInfo { get; set; }
        public DbSet<Tables> Tables { get; set; }
        public DbSet<SqlExe> SqlExe { get; set; }

        public DbSet<Pks> Pks { get; set; }
        
    }
}
