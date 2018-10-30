namespace DingTalk.Models.KisLocalModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class KisLocalContext : DbContext
    {
        public KisLocalContext()
            : base("name=KisLocalContext")
        {
        }

        public virtual DbSet<t_ICItem> t_ICItem { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FName)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FHelpCode)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FShortNumber)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FNumber)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FModifyTime)
                .IsFixedLength();

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FBrNo)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FModel)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FItemDescription)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FAlias)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FApproveNo)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FEquipmentNum)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FFullName)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FHighLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FLowLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FSecCoefficient)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FSecInv)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FABCCls)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FKFPeriod)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FNote)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FOIHighLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FOILowLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FPlanPrice)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FPOHighPrice)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FProfitRate)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FSalePrice)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FSOHighLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FSOLowLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FSOLowPrc)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FStockPrice)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FWWHghPrc)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FBatChangeEconomy)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FBatchAppendQty)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FBatFixEconomy)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FDailyConsume)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FInHighLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FInLowLimit)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FOrderPoint)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FQtyMax)
                .HasPrecision(18, 6);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FQtyMin)
                .HasPrecision(18, 6);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FChartNumber)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FGrossWeight)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FHeight)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FLength)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FNetWeight)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FSize)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FVersion)
                .IsUnicode(false);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FWidth)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FChgFeeRate)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FOutMachFee)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FPieceRate)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FStandardCost)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FStandardManHour)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FStdFixFeeRate)
                .HasPrecision(28, 10);

            modelBuilder.Entity<t_ICItem>()
                .Property(e => e.FStdPayRate)
                .HasPrecision(28, 10);
        }
    }
}
