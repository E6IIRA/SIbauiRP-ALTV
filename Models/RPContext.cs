using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class RPContext : DbContext
    {
        public RPContext()
        {
        }

        public RPContext(DbContextOptions<RPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<BankData> BankData { get; set; }
        public virtual DbSet<BankTypeData> BankTypeData { get; set; }
        public virtual DbSet<Banktype> Banktype { get; set; }
        public virtual DbSet<ClothData> ClothData { get; set; }
        public virtual DbSet<ClothShopData> ClothShopData { get; set; }
        public virtual DbSet<ClothTypeData> ClothTypeData { get; set; }
        public virtual DbSet<ClothVariationData> ClothVariationData { get; set; }
        public virtual DbSet<CrimeCategoryData> CrimeCategoryData { get; set; }
        public virtual DbSet<CrimeData> CrimeData { get; set; }
        public virtual DbSet<DoorData> DoorData { get; set; }
        public virtual DbSet<DrugCamper> DrugCamper { get; set; }
        public virtual DbSet<DrugCamperTypeData> DrugCamperTypeData { get; set; }
        public virtual DbSet<DrugCamperTypeItemData> DrugCamperTypeItemData { get; set; }
        public virtual DbSet<DrugExportContainer> DrugExportContainer { get; set; }
        public virtual DbSet<DrugExportContainerData> DrugExportContainerData { get; set; }
        public virtual DbSet<FarmFieldData> FarmFieldData { get; set; }
        public virtual DbSet<FarmFieldObjectData> FarmFieldObjectData { get; set; }
        public virtual DbSet<FarmObjectData> FarmObjectData { get; set; }
        public virtual DbSet<FarmObjectLootData> FarmObjectLootData { get; set; }
        public virtual DbSet<FuelstationData> FuelstationData { get; set; }
        public virtual DbSet<FuelstationGaspumpData> FuelstationGaspumpData { get; set; }
        public virtual DbSet<GarageData> GarageData { get; set; }
        public virtual DbSet<GaragespawnData> GaragespawnData { get; set; }
        public virtual DbSet<House> House { get; set; }
        public virtual DbSet<HouseAppearanceData> HouseAppearanceData { get; set; }
        public virtual DbSet<HouseAreaData> HouseAreaData { get; set; }
        public virtual DbSet<HouseData> HouseData { get; set; }
        public virtual DbSet<HouseGarageData> HouseGarageData { get; set; }
        public virtual DbSet<HouseInteriorPosition> HouseInteriorPosition { get; set; }
        public virtual DbSet<HouseSizeData> HouseSizeData { get; set; }
        public virtual DbSet<InjuryDeathCauseData> InjuryDeathCauseData { get; set; }
        public virtual DbSet<InjuryTypeData> InjuryTypeData { get; set; }
        public virtual DbSet<InteriorData> InteriorData { get; set; }
        public virtual DbSet<InteriorPositionData> InteriorPositionData { get; set; }
        public virtual DbSet<InteriorPositionTypeData> InteriorPositionTypeData { get; set; }
        public virtual DbSet<InteriorWarderobeData> InteriorWarderobeData { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<InventoryTypeData> InventoryTypeData { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemData> ItemData { get; set; }
        public virtual DbSet<ParcelDeliveryPoints> ParcelDeliveryPoints { get; set; }
        public virtual DbSet<Plant> Plant { get; set; }
        public virtual DbSet<PlantTypeData> PlantTypeData { get; set; }
        public virtual DbSet<PlantTypeLootData> PlantTypeLootData { get; set; }
        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<PlayerAttributes> PlayerAttributes { get; set; }
        public virtual DbSet<PlayerClothEquipped> PlayerClothEquipped { get; set; }
        public virtual DbSet<PlayerClothOwned> PlayerClothOwned { get; set; }
        public virtual DbSet<PlayerCrime> PlayerCrime { get; set; }
        public virtual DbSet<PlayerHouseOwned> PlayerHouseOwned { get; set; }
        public virtual DbSet<PlayerHouseRent> PlayerHouseRent { get; set; }
        public virtual DbSet<PlayerInventories> PlayerInventories { get; set; }
        public virtual DbSet<PlayerLicence> PlayerLicence { get; set; }
        public virtual DbSet<PlayerPhoneContact> PlayerPhoneContact { get; set; }
        public virtual DbSet<PlayerStorageroomOwned> PlayerStorageroomOwned { get; set; }
        public virtual DbSet<PlayerTeamPermission> PlayerTeamPermission { get; set; }
        public virtual DbSet<PlayerVehicleKey> PlayerVehicleKey { get; set; }
        public virtual DbSet<PlayerWeapon> PlayerWeapon { get; set; }
        public virtual DbSet<PlayerWeaponComponent> PlayerWeaponComponent { get; set; }
        public virtual DbSet<PoliceComputerData> PoliceComputerData { get; set; }
        public virtual DbSet<Rank> Rank { get; set; }
        public virtual DbSet<Saveposition> Saveposition { get; set; }
        public virtual DbSet<ServerScenarioData> ServerScenarioData { get; set; }
        public virtual DbSet<ServerScenarioLootData> ServerScenarioLootData { get; set; }
        public virtual DbSet<ServerScenarioPropData> ServerScenarioPropData { get; set; }
        public virtual DbSet<ShopData> ShopData { get; set; }
        public virtual DbSet<ShopItemData> ShopItemData { get; set; }
        public virtual DbSet<SmsChat> SmsChat { get; set; }
        public virtual DbSet<SmsChatMessage> SmsChatMessage { get; set; }
        public virtual DbSet<SmsChatParticipant> SmsChatParticipant { get; set; }
        public virtual DbSet<Storageroom> Storageroom { get; set; }
        public virtual DbSet<StorageroomData> StorageroomData { get; set; }
        public virtual DbSet<StorageroomInteriorPosition> StorageroomInteriorPosition { get; set; }
        public virtual DbSet<TeamData> TeamData { get; set; }
        public virtual DbSet<TeamKeyStorage> TeamKeyStorage { get; set; }
        public virtual DbSet<TeamKeyStorageData> TeamKeyStorageData { get; set; }
        public virtual DbSet<TeamTypeData> TeamTypeData { get; set; }
        public virtual DbSet<TicketMachineData> TicketMachineData { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }
        public virtual DbSet<VehicleClassificationData> VehicleClassificationData { get; set; }
        public virtual DbSet<VehicleData> VehicleData { get; set; }
        public virtual DbSet<VehicleShopData> VehicleShopData { get; set; }
        public virtual DbSet<VehicleShopVehicle> VehicleShopVehicle { get; set; }
        public virtual DbSet<VehicleTuning> VehicleTuning { get; set; }
        public virtual DbSet<VehicleTuningData> VehicleTuningData { get; set; }
        public virtual DbSet<WareExportData> WareExportData { get; set; }
        public virtual DbSet<WareExportDataHistory> WareExportDataHistory { get; set; }
        public virtual DbSet<WeaponAmmunitionData> WeaponAmmunitionData { get; set; }
        public virtual DbSet<WeaponComponentData> WeaponComponentData { get; set; }
        public virtual DbSet<WeaponData> WeaponData { get; set; }
        public virtual DbSet<WeaponTintData> WeaponTintData { get; set; }
        public virtual DbSet<WeaponTypeData> WeaponTypeData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=db.sibauirp.de;database=sibauirp;user=Sibaui;password=XPYfMKEUMN9wqXcS!yDtHAw4qc?Nh?Bz3wF7r-SxgnXQ-q8Yy-faDMd8F7C_8BV6;treattinyasboolean=true", x => x.ServerVersion("10.4.12-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.HasIndex(e => e.RankId)
                    .HasName("Rank_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Creationdate)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.RankId)
                    .HasColumnName("Rank_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.SocialClubName)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Rank)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.RankId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Rank_Id");
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("area");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("bank");

                entity.HasIndex(e => e.BankTypeId)
                    .HasName("BankType_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BankTypeId).HasColumnType("int(11)");

                entity.Property(e => e.CurrentMoney).HasColumnType("int(11)");

                entity.Property(e => e.MaxMoney).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.BankType)
                    .WithMany(p => p.Bank)
                    .HasForeignKey(d => d.BankTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bank_banktype");
            });

            modelBuilder.Entity<BankData>(entity =>
            {
                entity.ToTable("bank_data");

                entity.HasIndex(e => e.BankTypeId)
                    .HasName("BankType_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BankTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Class).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.BankType)
                    .WithMany(p => p.BankData)
                    .HasForeignKey(d => d.BankTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BankType_Id");
            });

            modelBuilder.Entity<BankTypeData>(entity =>
            {
                entity.ToTable("bank_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AccountFee).HasComment("Prozentuale Kontoführungsgebühren");

                entity.Property(e => e.AccountFeeMaximum)
                    .HasColumnType("int(11)")
                    .HasComment("Maximale Kontoführungsgebühren");

                entity.Property(e => e.DepositFee).HasComment("Prozentuale Gebühren beim Einzahlen beim Geldautomaten eines anderen Instituts");

                entity.Property(e => e.DepositFeeMaximum)
                    .HasColumnType("int(11)")
                    .HasComment("Maximale Gebühren beim Einzahlen beim Geldautomaten eines anderen Instituts");

                entity.Property(e => e.DepositFeeMinimum)
                    .HasColumnType("int(11)")
                    .HasComment("Minimale Gebühren beim Einzahlen beim Geldautomaten eines anderen Instituts");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.WithdrawFee).HasComment("Prozentuale Gebühren beim Abheben beim Geldautomaten eines anderen Instituts");

                entity.Property(e => e.WithdrawFeeMaximum)
                    .HasColumnType("int(11)")
                    .HasComment("Maximale Gebühren beim Abheben beim Geldautomaten eines anderen Instituts");

                entity.Property(e => e.WithdrawFeeMinimum)
                    .HasColumnType("int(11)")
                    .HasComment("Minimale Gebühren beim Abheben beim Geldautomaten eines anderen Instituts");
            });

            modelBuilder.Entity<Banktype>(entity =>
            {
                entity.ToTable("banktype");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<ClothData>(entity =>
            {
                entity.ToTable("cloth_data");

                entity.HasIndex(e => e.ClothShopDataId)
                    .HasName("ClothShopData_Id");

                entity.HasIndex(e => e.ClothTypeDataId)
                    .HasName("ClothTypeData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClothShopDataId)
                    .HasColumnName("ClothShopData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ClothTypeDataId)
                    .HasColumnName("ClothTypeData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Gender).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.Value).HasColumnType("smallint(6)");

                entity.HasOne(d => d.ClothShopData)
                    .WithMany(p => p.ClothData)
                    .HasForeignKey(d => d.ClothShopDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClothShopData_Id");

                entity.HasOne(d => d.ClothTypeData)
                    .WithMany(p => p.ClothData)
                    .HasForeignKey(d => d.ClothTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClothTypeData_Id");
            });

            modelBuilder.Entity<ClothShopData>(entity =>
            {
                entity.ToTable("cloth_shop_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.IsActive).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<ClothTypeData>(entity =>
            {
                entity.ToTable("cloth_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Value).HasColumnType("tinyint(3) unsigned");
            });

            modelBuilder.Entity<ClothVariationData>(entity =>
            {
                entity.ToTable("cloth_variation_data");

                entity.HasIndex(e => e.ClothDataId)
                    .HasName("ClothData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClothDataId)
                    .HasColumnName("ClothData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Value).HasColumnType("smallint(6)");

                entity.HasOne(d => d.ClothData)
                    .WithMany(p => p.ClothVariationData)
                    .HasForeignKey(d => d.ClothDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClothData_Id");
            });

            modelBuilder.Entity<CrimeCategoryData>(entity =>
            {
                entity.ToTable("crime_category_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Order).HasColumnType("int(11)");
            });

            modelBuilder.Entity<CrimeData>(entity =>
            {
                entity.ToTable("crime_data");

                entity.HasIndex(e => e.CrimeCategoryDataId)
                    .HasName("CrimeCategoryData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Cost).HasColumnType("int(11)");

                entity.Property(e => e.CrimeCategoryDataId)
                    .HasColumnName("CrimeCategoryData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Jailtime).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TakeDriverLic).HasColumnType("int(11)");

                entity.Property(e => e.TakeGunLic).HasColumnType("int(11)");

                entity.HasOne(d => d.CrimeCategoryData)
                    .WithMany(p => p.CrimeData)
                    .HasForeignKey(d => d.CrimeCategoryDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CrimeCategoryData_Id");
            });

            modelBuilder.Entity<DoorData>(entity =>
            {
                entity.ToTable("door_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ModelHash).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Range).HasColumnType("int(11)");

                entity.Property(e => e.Teams)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DrugCamper>(entity =>
            {
                entity.ToTable("drug_camper");

                entity.HasIndex(e => e.DrugCamperTypeDataId)
                    .HasName("FK_drug_camper_drug_camper_type_data");

                entity.HasIndex(e => e.TeamDataId)
                    .HasName("FK_drug_camper_team_data");

                entity.HasIndex(e => e.VehicleId)
                    .HasName("FK_drug_camper_vehicle");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Dildogroeße).HasColumnType("int(11)");

                entity.Property(e => e.DrugCamperTypeDataId)
                    .HasColumnName("DrugCamperTypeData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SecurityUpgrade).HasColumnType("int(11)");

                entity.Property(e => e.TeamDataId)
                    .HasColumnName("TeamData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehicleId)
                    .HasColumnName("Vehicle_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ventilation).HasColumnType("int(11)");

                entity.HasOne(d => d.DrugCamperTypeData)
                    .WithMany(p => p.DrugCamper)
                    .HasForeignKey(d => d.DrugCamperTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_drug_camper_drug_camper_type_data");

                entity.HasOne(d => d.TeamData)
                    .WithMany(p => p.DrugCamper)
                    .HasForeignKey(d => d.TeamDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_drug_camper_team_data");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.DrugCamper)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_drug_camper_vehicle");
            });

            modelBuilder.Entity<DrugCamperTypeData>(entity =>
            {
                entity.ToTable("drug_camper_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DrugCamperTypeItemData>(entity =>
            {
                entity.ToTable("drug_camper_type_item_data");

                entity.HasIndex(e => e.DrugCamperTypeDataId)
                    .HasName("FK_drug_camper_type_item_data_drug_camper_type_data");

                entity.HasIndex(e => e.ItemDataId)
                    .HasName("FK_drug_camper_type_data_item_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasColumnType("int(11)");

                entity.Property(e => e.DrugCamperTypeDataId)
                    .HasColumnName("DrugCamperTypeData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsInput)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.ItemDataId)
                    .HasColumnName("ItemData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.DrugCamperTypeData)
                    .WithMany(p => p.DrugCamperTypeItemData)
                    .HasForeignKey(d => d.DrugCamperTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_drug_camper_type_item_data_drug_camper_type_data");

                entity.HasOne(d => d.ItemData)
                    .WithMany(p => p.DrugCamperTypeItemData)
                    .HasForeignKey(d => d.ItemDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_drug_camper_type_data_item_data");
            });

            modelBuilder.Entity<DrugExportContainer>(entity =>
            {
                entity.ToTable("drug_export_container");

                entity.HasIndex(e => e.DrugExportContainerDataId)
                    .HasName("DrugExportContainerDataId");

                entity.HasIndex(e => e.DrugItemId)
                    .HasName("DrugItem_Id");

                entity.HasIndex(e => e.TeamId)
                    .HasName("DrugExportContainerTeamId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CallTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1000-01-01 00:00:00'");

                entity.Property(e => e.DrugAmount).HasColumnType("int(11)");

                entity.Property(e => e.DrugExportContainerDataId)
                    .HasColumnName("DrugExportContainerData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DrugItemId)
                    .HasColumnName("DrugItem_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1000-01-01 00:00:00'");

                entity.Property(e => e.Money).HasColumnType("int(11)");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.SendTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1000-01-01 00:00:00'");

                entity.Property(e => e.TeamId)
                    .HasColumnName("Team_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.DrugExportContainerData)
                    .WithMany(p => p.DrugExportContainer)
                    .HasForeignKey(d => d.DrugExportContainerDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DrugExportContainerDataId");

                entity.HasOne(d => d.DrugItem)
                    .WithMany(p => p.DrugExportContainer)
                    .HasForeignKey(d => d.DrugItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DrugItem_Id");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.DrugExportContainer)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DrugExportContainerTeamId");
            });

            modelBuilder.Entity<DrugExportContainerData>(entity =>
            {
                entity.ToTable("drug_export_container_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            modelBuilder.Entity<FarmFieldData>(entity =>
            {
                entity.ToTable("farm_field_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.MaximumObjects).HasColumnType("int(11)");

                entity.Property(e => e.MinimumObjects).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<FarmFieldObjectData>(entity =>
            {
                entity.ToTable("farm_field_object_data");

                entity.HasIndex(e => e.FarmFieldDataId)
                    .HasName("FarmFieldDataId");

                entity.HasIndex(e => e.FarmObjectDataId)
                    .HasName("FarmObjectDataId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FarmFieldDataId).HasColumnType("int(11)");

                entity.Property(e => e.FarmObjectDataId).HasColumnType("int(11)");

                entity.HasOne(d => d.FarmFieldData)
                    .WithMany(p => p.FarmFieldObjectData)
                    .HasForeignKey(d => d.FarmFieldDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FarmFieldDataId");

                entity.HasOne(d => d.FarmObjectData)
                    .WithMany(p => p.FarmFieldObjectData)
                    .HasForeignKey(d => d.FarmObjectDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FarmObjectDataId");
            });

            modelBuilder.Entity<FarmObjectData>(entity =>
            {
                entity.ToTable("farm_object_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Capacity).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ObjectName)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<FarmObjectLootData>(entity =>
            {
                entity.ToTable("farm_object_loot_data");

                entity.HasIndex(e => e.FarmObjectDataId)
                    .HasName("FarmObjectDataId2");

                entity.HasIndex(e => e.ItemDataId)
                    .HasName("ItemDataId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Chance).HasComment("Wahrscheinlichkeit in % (Wert von 10 bedeutet 10%)");

                entity.Property(e => e.FarmObjectDataId).HasColumnType("int(11)");

                entity.Property(e => e.ItemDataId).HasColumnType("int(11)");

                entity.Property(e => e.MaximumAmount).HasColumnType("int(11)");

                entity.Property(e => e.MinimumAmount).HasColumnType("int(11)");

                entity.HasOne(d => d.FarmObjectData)
                    .WithMany(p => p.FarmObjectLootData)
                    .HasForeignKey(d => d.FarmObjectDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FarmObjectDataId2");

                entity.HasOne(d => d.ItemData)
                    .WithMany(p => p.FarmObjectLootData)
                    .HasForeignKey(d => d.ItemDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ItemDataId");
            });

            modelBuilder.Entity<FuelstationData>(entity =>
            {
                entity.ToTable("fuelstation_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InfoPositionZ).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Range).HasColumnType("int(11)");
            });

            modelBuilder.Entity<FuelstationGaspumpData>(entity =>
            {
                entity.ToTable("fuelstation_gaspump_data");

                entity.HasIndex(e => e.FuelstationDataId)
                    .HasName("FuelstationData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FuelstationDataId)
                    .HasColumnName("FuelstationData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.FuelstationData)
                    .WithMany(p => p.FuelstationGaspumpData)
                    .HasForeignKey(d => d.FuelstationDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FuelstationData_Id");
            });

            modelBuilder.Entity<GarageData>(entity =>
            {
                entity.ToTable("garage_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PedHash)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Radius).HasColumnType("int(11)");

                entity.Property(e => e.Type).HasColumnType("int(11)");

                entity.Property(e => e.VehicleClassifications)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<GaragespawnData>(entity =>
            {
                entity.ToTable("garagespawn_data");

                entity.HasIndex(e => e.GarageId)
                    .HasName("Garage_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.GarageId)
                    .HasColumnName("Garage_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Garage)
                    .WithMany(p => p.GaragespawnData)
                    .HasForeignKey(d => d.GarageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Garage_Id");
            });

            modelBuilder.Entity<House>(entity =>
            {
                entity.ToTable("house");

                entity.HasIndex(e => e.HouseDataId)
                    .HasName("HouseData_Id2");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DoorbellSign)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.HouseDataId)
                    .HasColumnName("HouseData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Money).HasColumnType("int(11)");

                entity.HasOne(d => d.HouseData)
                    .WithMany(p => p.House)
                    .HasForeignKey(d => d.HouseDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HouseData_Id2");
            });

            modelBuilder.Entity<HouseAppearanceData>(entity =>
            {
                entity.ToTable("house_appearance_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Points).HasColumnType("int(11)");
            });

            modelBuilder.Entity<HouseAreaData>(entity =>
            {
                entity.ToTable("house_area_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Points).HasColumnType("int(11)");
            });

            modelBuilder.Entity<HouseData>(entity =>
            {
                entity.ToTable("house_data");

                entity.HasIndex(e => e.HouseAppearanceDataId)
                    .HasName("HouseAppearanceData_Id");

                entity.HasIndex(e => e.HouseAreaDataId)
                    .HasName("HouseAreaData_Id");

                entity.HasIndex(e => e.HouseSizeDataId)
                    .HasName("HouseSizeData_Id");

                entity.HasIndex(e => e.InteriorDataId)
                    .HasName("InteriorData_Id2");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.HouseAppearanceDataId)
                    .HasColumnName("HouseAppearanceData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HouseAreaDataId)
                    .HasColumnName("HouseAreaData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HouseSizeDataId)
                    .HasColumnName("HouseSizeData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InteriorDataId)
                    .HasColumnName("InteriorData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.RentalPlaces).HasColumnType("tinyint(4)");

                entity.HasOne(d => d.HouseAppearanceData)
                    .WithMany(p => p.HouseData)
                    .HasForeignKey(d => d.HouseAppearanceDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HouseAppearanceData_Id");

                entity.HasOne(d => d.HouseAreaData)
                    .WithMany(p => p.HouseData)
                    .HasForeignKey(d => d.HouseAreaDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HouseAreaData_Id");

                entity.HasOne(d => d.HouseSizeData)
                    .WithMany(p => p.HouseData)
                    .HasForeignKey(d => d.HouseSizeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HouseSizeData_Id");

                entity.HasOne(d => d.InteriorData)
                    .WithMany(p => p.HouseData)
                    .HasForeignKey(d => d.InteriorDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InteriorData_Id2");
            });

            modelBuilder.Entity<HouseGarageData>(entity =>
            {
                entity.ToTable("house_garage_data");

                entity.HasIndex(e => e.GarageDataId)
                    .HasName("GarageData_Id2");

                entity.HasIndex(e => e.HouseDataId)
                    .HasName("HouseData_Id3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.GarageDataId)
                    .HasColumnName("GarageData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HouseDataId)
                    .HasColumnName("HouseData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.GarageData)
                    .WithMany(p => p.HouseGarageData)
                    .HasForeignKey(d => d.GarageDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("GarageData_Id2");

                entity.HasOne(d => d.HouseData)
                    .WithMany(p => p.HouseGarageData)
                    .HasForeignKey(d => d.HouseDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("HouseData_Id3");
            });

            modelBuilder.Entity<HouseInteriorPosition>(entity =>
            {
                entity.ToTable("house_interior_position");

                entity.HasIndex(e => e.HouseId)
                    .HasName("House_Id");

                entity.HasIndex(e => e.InteriorPositionDataId)
                    .HasName("InteriorPositionData_Id");

                entity.HasIndex(e => e.InventoryId)
                    .HasName("Inventory_Id4");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.HouseId)
                    .HasColumnName("House_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InteriorPositionDataId)
                    .HasColumnName("InteriorPositionData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InventoryId)
                    .HasColumnName("Inventory_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.House)
                    .WithMany(p => p.HouseInteriorPosition)
                    .HasForeignKey(d => d.HouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("House_Id");

                entity.HasOne(d => d.InteriorPositionData)
                    .WithMany(p => p.HouseInteriorPosition)
                    .HasForeignKey(d => d.InteriorPositionDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InteriorPositionData_Id");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.HouseInteriorPosition)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Inventory_Id4");
            });

            modelBuilder.Entity<HouseSizeData>(entity =>
            {
                entity.ToTable("house_size_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Points).HasColumnType("int(11)");
            });

            modelBuilder.Entity<InjuryDeathCauseData>(entity =>
            {
                entity.ToTable("injury_death_cause_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Hash).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<InjuryTypeData>(entity =>
            {
                entity.ToTable("injury_type_data");

                entity.HasIndex(e => e.InjuryDeathCauseDataId)
                    .HasName("InjuryDeathCauseData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AdditionalTime).HasColumnType("int(11)");

                entity.Property(e => e.InjuryDeathCauseDataId)
                    .HasColumnName("InjuryDeathCauseData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Percentage).HasColumnType("int(11)");

                entity.Property(e => e.Time).HasColumnType("int(11)");

                entity.Property(e => e.TreatmentType).HasColumnType("int(11)");

                entity.HasOne(d => d.InjuryDeathCauseData)
                    .WithMany(p => p.InjuryTypeData)
                    .HasForeignKey(d => d.InjuryDeathCauseDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InjuryDeathCauseData_Id");
            });

            modelBuilder.Entity<InteriorData>(entity =>
            {
                entity.ToTable("interior_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<InteriorPositionData>(entity =>
            {
                entity.ToTable("interior_position_data");

                entity.HasIndex(e => e.InteriorDataId)
                    .HasName("houseInteriorId");

                entity.HasIndex(e => e.InteriorPositionDataTypeId)
                    .HasName("InteriorPositionDataType_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InteriorDataId).HasColumnType("int(11)");

                entity.Property(e => e.InteriorPositionDataTypeId)
                    .HasColumnName("InteriorPositionDataType_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.InteriorData)
                    .WithMany(p => p.InteriorPositionData)
                    .HasForeignKey(d => d.InteriorDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InteriorData_Id");

                entity.HasOne(d => d.InteriorPositionDataType)
                    .WithMany(p => p.InteriorPositionData)
                    .HasForeignKey(d => d.InteriorPositionDataTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InteriorPositionDataType_Id");
            });

            modelBuilder.Entity<InteriorPositionTypeData>(entity =>
            {
                entity.ToTable("interior_position_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<InteriorWarderobeData>(entity =>
            {
                entity.ToTable("interior_warderobe_data");

                entity.HasIndex(e => e.InteriorDataId)
                    .HasName("InteriorDataId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InteriorDataId).HasColumnType("int(11)");

                entity.HasOne(d => d.InteriorData)
                    .WithMany(p => p.InteriorWarderobeData)
                    .HasForeignKey(d => d.InteriorDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("interior_warderobe_data_ibfk_1");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventory");

                entity.HasIndex(e => e.InventoryTypeDataId)
                    .HasName("InventoryTypeData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InventoryTypeDataId)
                    .HasColumnName("InventoryTypeData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.InventoryTypeData)
                    .WithMany(p => p.Inventory)
                    .HasForeignKey(d => d.InventoryTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InventoryTypeData_Id");
            });

            modelBuilder.Entity<InventoryTypeData>(entity =>
            {
                entity.ToTable("inventory_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.MaxSlots).HasColumnType("int(11)");

                entity.Property(e => e.MaxWeight).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TypeId)
                    .HasColumnName("Type_Id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("item");

                entity.HasIndex(e => e.InventoryId)
                    .HasName("Inventory_Id");

                entity.HasIndex(e => e.ItemDataId)
                    .HasName("ItemData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasColumnType("int(11)");

                entity.Property(e => e.CustomItemData)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.InventoryId)
                    .HasColumnName("Inventory_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ItemDataId)
                    .HasColumnName("ItemData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Slot).HasColumnType("int(11)");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Inventory_Id");

                entity.HasOne(d => d.ItemData)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.ItemDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ItemData_Id");
            });

            modelBuilder.Entity<ItemData>(entity =>
            {
                entity.ToTable("item_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Stacksize).HasColumnType("int(11)");

                entity.Property(e => e.Weight).HasColumnType("int(11)");
            });

            modelBuilder.Entity<ParcelDeliveryPoints>(entity =>
            {
                entity.ToTable("parcel_delivery_points");

                entity.HasIndex(e => e.AreaId)
                    .HasName("AreaId_Key");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AreaId).HasColumnType("int(11)");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.ParcelDeliveryPoints)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AreaId_Key");
            });

            modelBuilder.Entity<Plant>(entity =>
            {
                entity.ToTable("plant");

                entity.HasIndex(e => e.PlantTypeDataId)
                    .HasName("PlantTypeDataId2");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActualFertilizer).HasColumnType("int(11)");

                entity.Property(e => e.ActualWater).HasColumnType("int(11)");

                entity.Property(e => e.GrowState).HasColumnType("int(11)");

                entity.Property(e => e.HarvestDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1970-01-01 00:00:00'");

                entity.Property(e => e.HarvestPlayerId).HasColumnType("int(11)");

                entity.Property(e => e.LootFactor).HasComment("LootFactor * BaseAmount = LootAmount");

                entity.Property(e => e.PlantDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1970-01-01 00:00:00'");

                entity.Property(e => e.PlantTypeDataId).HasColumnType("int(11)");

                entity.Property(e => e.PlanterPlayerId).HasColumnType("int(11)");

                entity.HasOne(d => d.PlantTypeData)
                    .WithMany(p => p.Plant)
                    .HasForeignKey(d => d.PlantTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PlantTypeDataId2");
            });

            modelBuilder.Entity<PlantTypeData>(entity =>
            {
                entity.ToTable("plant_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.MaximumFertilizer)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'20'");

                entity.Property(e => e.MaximumWater)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'20'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ObjectStageOne)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ObjectStageTwo)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SeedItemId).HasColumnType("int(11)");

                entity.Property(e => e.TimeToGrow)
                    .HasColumnType("int(11)")
                    .HasComment("In Minuten");
            });

            modelBuilder.Entity<PlantTypeLootData>(entity =>
            {
                entity.ToTable("plant_type_loot_data");

                entity.HasIndex(e => e.ItemDataId)
                    .HasName("PlantLootItemDataId");

                entity.HasIndex(e => e.PlantTypeDataId)
                    .HasName("PlantTypeDataId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BaseAmount).HasColumnType("int(11)");

                entity.Property(e => e.ItemDataId).HasColumnType("int(11)");

                entity.Property(e => e.PlantTypeDataId).HasColumnType("int(11)");

                entity.HasOne(d => d.ItemData)
                    .WithMany(p => p.PlantTypeLootData)
                    .HasForeignKey(d => d.ItemDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PlantLootItemDataId");

                entity.HasOne(d => d.PlantTypeData)
                    .WithMany(p => p.PlantTypeLootData)
                    .HasForeignKey(d => d.PlantTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PlantTypeDataId");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("player");

                entity.HasIndex(e => e.AccountId)
                    .HasName("Account_Id");

                entity.HasIndex(e => e.InjuryTypeDataId)
                    .HasName("InjuryTypeData_Id");

                entity.HasIndex(e => e.InventoryId)
                    .HasName("Inventory_Id");

                entity.HasIndex(e => e.TeamId)
                    .HasName("Team_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AccountId)
                    .HasColumnName("Account_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Armor).HasColumnType("smallint(5) unsigned");

                entity.Property(e => e.BankMoney).HasColumnType("int(11)");

                entity.Property(e => e.BankType)
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Gender).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.Health).HasColumnType("smallint(5) unsigned");

                entity.Property(e => e.InjuryTimeLeft).HasColumnType("int(11)");

                entity.Property(e => e.InjuryTypeDataId).HasColumnType("int(11)");

                entity.Property(e => e.InventoryId)
                    .HasColumnName("Inventory_Id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.JailTime).HasColumnType("int(11)");

                entity.Property(e => e.LastSeen)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Money).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PhoneBalance).HasColumnType("int(11)");

                entity.Property(e => e.PhoneNumber).HasColumnType("int(11)");

                entity.Property(e => e.ProbationTime).HasColumnType("int(11)");

                entity.Property(e => e.TeamId)
                    .HasColumnName("Team_Id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TimePlayed).HasColumnType("int(11)");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Account_Id");

                entity.HasOne(d => d.InjuryTypeData)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.InjuryTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InjuryTypeData_Id");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Inventory_Id2");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Team_Id");
            });

            modelBuilder.Entity<PlayerAttributes>(entity =>
            {
                entity.ToTable("player_attributes");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id_FK");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Dexterity).HasColumnType("int(11)");

                entity.Property(e => e.Experience).HasColumnType("int(11)");

                entity.Property(e => e.Intelligence).HasColumnType("int(11)");

                entity.Property(e => e.Level)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.MaximumAttributes).HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Strength).HasColumnType("int(11)");

                entity.Property(e => e.Vitality).HasColumnType("int(11)");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerAttributes)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id_FK");
            });

            modelBuilder.Entity<PlayerClothEquipped>(entity =>
            {
                entity.ToTable("player_cloth_equipped");

                entity.HasIndex(e => e.ClothVariationDataId)
                    .HasName("ClothVariationData_Id");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id4");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClothVariationDataId)
                    .HasColumnName("ClothVariationData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ClothVariationData)
                    .WithMany(p => p.PlayerClothEquipped)
                    .HasForeignKey(d => d.ClothVariationDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClothVariationData_Id");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerClothEquipped)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id4");
            });

            modelBuilder.Entity<PlayerClothOwned>(entity =>
            {
                entity.ToTable("player_cloth_owned");

                entity.HasIndex(e => e.ClothVariationDataId)
                    .HasName("ClothVariationData_Id");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id4");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClothVariationDataId)
                    .HasColumnName("ClothVariationData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ClothVariationData)
                    .WithMany(p => p.PlayerClothOwned)
                    .HasForeignKey(d => d.ClothVariationDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_cloth_owned_ibfk_1");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerClothOwned)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_cloth_owned_ibfk_2");
            });

            modelBuilder.Entity<PlayerCrime>(entity =>
            {
                entity.ToTable("player_crime");

                entity.HasIndex(e => e.CrimeDataId)
                    .HasName("player_crime_fk");

                entity.HasIndex(e => e.OfficerId)
                    .HasName("player_crime_fk_2");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("player_crime_fk_1");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.CrimeDataId)
                    .HasColumnName("CrimeData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OfficerId)
                    .HasColumnName("Officer_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.CrimeData)
                    .WithMany(p => p.PlayerCrime)
                    .HasForeignKey(d => d.CrimeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_crime_fk");

                entity.HasOne(d => d.Officer)
                    .WithMany(p => p.PlayerCrimeOfficer)
                    .HasForeignKey(d => d.OfficerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_crime_fk_2");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerCrimePlayer)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_crime_fk_1");
            });

            modelBuilder.Entity<PlayerHouseOwned>(entity =>
            {
                entity.ToTable("player_house_owned");

                entity.HasIndex(e => e.HouseId)
                    .HasName("HouseData_Id");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id8");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.HouseId)
                    .HasColumnName("House_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.House)
                    .WithMany(p => p.PlayerHouseOwned)
                    .HasForeignKey(d => d.HouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_player_house_owned_sibauirp.house");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerHouseOwned)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id8");
            });

            modelBuilder.Entity<PlayerHouseRent>(entity =>
            {
                entity.ToTable("player_house_rent");

                entity.HasIndex(e => e.HouseId)
                    .HasName("HouseData_Id");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id8");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Cost).HasColumnType("int(11)");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.HouseId)
                    .HasColumnName("House_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.House)
                    .WithMany(p => p.PlayerHouseRent)
                    .HasForeignKey(d => d.HouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_player_house_rent_sibauirp.house");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerHouseRent)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_house_rent_ibfk_2");
            });

            modelBuilder.Entity<PlayerInventories>(entity =>
            {
                entity.ToTable("player_inventories");

                entity.HasIndex(e => e.InventoryId)
                    .HasName("Inventory_Id5");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id9");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InventoryId)
                    .HasColumnName("Inventory_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.PlayerInventories)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Inventory_Id5");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerInventories)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id9");
            });

            modelBuilder.Entity<PlayerLicence>(entity =>
            {
                entity.ToTable("player_licence");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id7");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Boat).HasColumnType("int(11)");

                entity.Property(e => e.Car).HasColumnType("int(11)");

                entity.Property(e => e.Helicopter).HasColumnType("int(11)");

                entity.Property(e => e.IdCard).HasColumnType("int(11)");

                entity.Property(e => e.Motorcycle).HasColumnType("int(11)");

                entity.Property(e => e.Plane).HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Truck).HasColumnType("int(11)");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerLicence)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id7");
            });

            modelBuilder.Entity<PlayerPhoneContact>(entity =>
            {
                entity.ToTable("player_phone_contact");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id10");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(25)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Number).HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerPhoneContact)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id10");
            });

            modelBuilder.Entity<PlayerStorageroomOwned>(entity =>
            {
                entity.ToTable("player_storageroom_owned");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id8");

                entity.HasIndex(e => e.StorageRoomId)
                    .HasName("storageroomdata_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StorageRoomId)
                    .HasColumnName("StorageRoom_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerStorageroomOwned)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("player_storageroom_owned_ibfk_2");

                entity.HasOne(d => d.StorageRoom)
                    .WithMany(p => p.PlayerStorageroomOwned)
                    .HasForeignKey(d => d.StorageRoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_player_storageroom_owned_sibauirp.storageroom");
            });

            modelBuilder.Entity<PlayerTeamPermission>(entity =>
            {
                entity.ToTable("player_team_permission");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id6");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Rang).HasColumnType("int(11)");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerTeamPermission)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id6");
            });

            modelBuilder.Entity<PlayerVehicleKey>(entity =>
            {
                entity.ToTable("player_vehicle_key");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id5");

                entity.HasIndex(e => e.VehicleId)
                    .HasName("Vehicle_Id2");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehicleId)
                    .HasColumnName("Vehicle_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerVehicleKey)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id5");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.PlayerVehicleKey)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Vehicle_Id2");
            });

            modelBuilder.Entity<PlayerWeapon>(entity =>
            {
                entity.ToTable("player_weapon");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id");

                entity.HasIndex(e => e.WeaponDataId)
                    .HasName("WeaponData_Id");

                entity.HasIndex(e => e.WeaponTintDataId)
                    .HasName("WeaponTintData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Ammo).HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeaponDataId)
                    .HasColumnName("WeaponData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeaponTintDataId)
                    .HasColumnName("WeaponTintData_Id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerWeapon)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id");

                entity.HasOne(d => d.WeaponData)
                    .WithMany(p => p.PlayerWeapon)
                    .HasForeignKey(d => d.WeaponDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WeaponData_Id");

                entity.HasOne(d => d.WeaponTintData)
                    .WithMany(p => p.PlayerWeapon)
                    .HasForeignKey(d => d.WeaponTintDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WeaponTintData_Id");
            });

            modelBuilder.Entity<PlayerWeaponComponent>(entity =>
            {
                entity.ToTable("player_weapon_component");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Player_Id2");

                entity.HasIndex(e => e.WeaponComponentDataId)
                    .HasName("WeaponComponentData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeaponComponentDataId)
                    .HasColumnName("WeaponComponentData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.PlayerWeaponComponent)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id2");

                entity.HasOne(d => d.WeaponComponentData)
                    .WithMany(p => p.PlayerWeaponComponent)
                    .HasForeignKey(d => d.WeaponComponentDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WeaponComponentData_Id");
            });

            modelBuilder.Entity<PoliceComputerData>(entity =>
            {
                entity.ToTable("police_computer_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Rank>(entity =>
            {
                entity.ToTable("rank");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Payday).HasColumnType("int(11)");

                entity.Property(e => e.Power).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Saveposition>(entity =>
            {
                entity.ToTable("saveposition");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<ServerScenarioData>(entity =>
            {
                entity.ToTable("server_scenario_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<ServerScenarioLootData>(entity =>
            {
                entity.ToTable("server_scenario_loot_data");

                entity.HasIndex(e => e.ItemDataId)
                    .HasName("FK_server_scenario_loot_data_item_data");

                entity.HasIndex(e => e.ServerScenarioDataId)
                    .HasName("FK_server_scenario_loot_data_server_scenario_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ItemDataId)
                    .HasColumnName("ItemData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MaximumAmount).HasColumnType("int(11)");

                entity.Property(e => e.MinimumAmount).HasColumnType("int(11)");

                entity.Property(e => e.Propname)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServerScenarioDataId)
                    .HasColumnName("ServerScenarioData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ItemData)
                    .WithMany(p => p.ServerScenarioLootData)
                    .HasForeignKey(d => d.ItemDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_server_scenario_loot_data_item_data");

                entity.HasOne(d => d.ServerScenarioData)
                    .WithMany(p => p.ServerScenarioLootData)
                    .HasForeignKey(d => d.ServerScenarioDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_server_scenario_loot_data_server_scenario_data");
            });

            modelBuilder.Entity<ServerScenarioPropData>(entity =>
            {
                entity.ToTable("server_scenario_prop_data");

                entity.HasIndex(e => e.ServerScenarioDataId)
                    .HasName("FK_server_scenario_prop_data_server_scenario_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Propname)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ServerScenarioDataId)
                    .HasColumnName("ServerScenarioData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ServerScenarioData)
                    .WithMany(p => p.ServerScenarioPropData)
                    .HasForeignKey(d => d.ServerScenarioDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_server_scenario_prop_data_server_scenario_data");
            });

            modelBuilder.Entity<ShopData>(entity =>
            {
                entity.ToTable("shop_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Marker)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PedHash)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<ShopItemData>(entity =>
            {
                entity.ToTable("shop_item_data");

                entity.HasIndex(e => e.ItemDataId)
                    .HasName("ItemData_Id2");

                entity.HasIndex(e => e.ShopDataId)
                    .HasName("ShopData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ItemDataId)
                    .HasColumnName("ItemData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.ShopDataId)
                    .HasColumnName("ShopData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ItemData)
                    .WithMany(p => p.ShopItemData)
                    .HasForeignKey(d => d.ItemDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ItemData_Id2");

                entity.HasOne(d => d.ShopData)
                    .WithMany(p => p.ShopItemData)
                    .HasForeignKey(d => d.ShopDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ShopData_Id");
            });

            modelBuilder.Entity<SmsChat>(entity =>
            {
                entity.ToTable("sms_chat");

                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            modelBuilder.Entity<SmsChatMessage>(entity =>
            {
                entity.ToTable("sms_chat_message");

                entity.HasIndex(e => e.SmsChatParticipantId)
                    .HasName("SmsChatParticipant_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SmsChatParticipantId)
                    .HasColumnName("SmsChatParticipant_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.SmsChatParticipant)
                    .WithMany(p => p.SmsChatMessage)
                    .HasForeignKey(d => d.SmsChatParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SmsChatParticipant_Id");
            });

            modelBuilder.Entity<SmsChatParticipant>(entity =>
            {
                entity.ToTable("sms_chat_participant");

                entity.HasIndex(e => e.SmsChatId)
                    .HasName("SmsChat_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Number).HasColumnType("int(11)");

                entity.Property(e => e.SmsChatId)
                    .HasColumnName("SmsChat_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.SmsChat)
                    .WithMany(p => p.SmsChatParticipant)
                    .HasForeignKey(d => d.SmsChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SmsChat_Id");
            });

            modelBuilder.Entity<Storageroom>(entity =>
            {
                entity.ToTable("storageroom");

                entity.HasIndex(e => e.InteriorDataId)
                    .HasName("FK_Storageroom_Interior_Data_Id");

                entity.HasIndex(e => e.StorageroomDataId)
                    .HasName("FK_storageroom_storageroom_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Crates).HasColumnType("int(11)");

                entity.Property(e => e.InteriorDataId)
                    .HasColumnName("InteriorData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InventoryAmount).HasColumnType("int(11)");

                entity.Property(e => e.StorageroomDataId)
                    .HasColumnName("StorageroomData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.InteriorData)
                    .WithMany(p => p.Storageroom)
                    .HasForeignKey(d => d.InteriorDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Storageroom_Interior_Data_Id");

                entity.HasOne(d => d.StorageroomData)
                    .WithMany(p => p.Storageroom)
                    .HasForeignKey(d => d.StorageroomDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_storageroom_storageroom_data");
            });

            modelBuilder.Entity<StorageroomData>(entity =>
            {
                entity.ToTable("storageroom_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            modelBuilder.Entity<StorageroomInteriorPosition>(entity =>
            {
                entity.ToTable("storageroom_interior_position");

                entity.HasIndex(e => e.InteriorPositionDataId)
                    .HasName("InteriorPositionData_Id");

                entity.HasIndex(e => e.InventoryId)
                    .HasName("Inventory_Id4");

                entity.HasIndex(e => e.StorageroomId)
                    .HasName("storageroom_interior_position_ibfk_1");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InteriorPositionDataId)
                    .HasColumnName("InteriorPositionData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InventoryId)
                    .HasColumnName("Inventory_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StorageroomId)
                    .HasColumnName("Storageroom_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.InteriorPositionData)
                    .WithMany(p => p.StorageroomInteriorPosition)
                    .HasForeignKey(d => d.InteriorPositionDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("storageroom_interior_position_ibfk_2");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.StorageroomInteriorPosition)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("storageroom_interior_position_ibfk_3");

                entity.HasOne(d => d.Storageroom)
                    .WithMany(p => p.StorageroomInteriorPosition)
                    .HasForeignKey(d => d.StorageroomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("storageroom_interior_position_ibfk_1");
            });

            modelBuilder.Entity<TeamData>(entity =>
            {
                entity.ToTable("team_data");

                entity.HasIndex(e => e.Type)
                    .HasName("FK_team_data_team_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Type).HasColumnType("int(11)");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.TeamData)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_team_data_team_type_data");
            });

            modelBuilder.Entity<TeamKeyStorage>(entity =>
            {
                entity.ToTable("team_key_storage");

                entity.HasIndex(e => e.TeamKeyStorageDataId)
                    .HasName("TeamKeyStorageData_Id");

                entity.HasIndex(e => e.VehicleId)
                    .HasName("Vehicle_Id3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.TeamKeyStorageDataId)
                    .HasColumnName("TeamKeyStorageData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehicleId)
                    .HasColumnName("Vehicle_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehicleKeyGeneration).HasColumnType("int(11)");

                entity.HasOne(d => d.TeamKeyStorageData)
                    .WithMany(p => p.TeamKeyStorage)
                    .HasForeignKey(d => d.TeamKeyStorageDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TeamKeyStorageData_Id");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.TeamKeyStorage)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Vehicle_Id3");
            });

            modelBuilder.Entity<TeamKeyStorageData>(entity =>
            {
                entity.ToTable("team_key_storage_data");

                entity.HasIndex(e => e.TeamId)
                    .HasName("Team_Id2");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TeamId)
                    .HasColumnName("Team_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamKeyStorageData)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Team_Id2");
            });

            modelBuilder.Entity<TeamTypeData>(entity =>
            {
                entity.ToTable("team_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TicketMachineData>(entity =>
            {
                entity.ToTable("ticket_machine_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicle");

                entity.HasIndex(e => e.GarageDataId)
                    .HasName("GarageData_Id");

                entity.HasIndex(e => e.InventoryId)
                    .HasName("Inventory_Id");

                entity.HasIndex(e => e.PlayerId)
                    .HasName("Owner_Id");

                entity.HasIndex(e => e.TeamDataId)
                    .HasName("TeamData_Id");

                entity.HasIndex(e => e.VehicleDataId)
                    .HasName("VehicleData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BodyHealth)
                    .HasColumnType("int(11) unsigned")
                    .HasDefaultValueSql("'1000'");

                entity.Property(e => e.ColorNeonA).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.ColorNeonB).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.ColorNeonG).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.ColorNeonR).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.ColorPearl).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.ColorPrimary).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.ColorSecondary).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Distance).HasColumnType("int(11)");

                entity.Property(e => e.EngineHealth)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1000'");

                entity.Property(e => e.GarageDataId)
                    .HasColumnName("GarageData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InventoryId)
                    .HasColumnName("Inventory_Id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.KeyGeneration).HasColumnType("int(11)");

                entity.Property(e => e.NumberPlate)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PetrolTankHealth)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1000'");

                entity.Property(e => e.PlayerId)
                    .HasColumnName("Player_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TeamDataId)
                    .HasColumnName("TeamData_Id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.VehicleDataId)
                    .HasColumnName("VehicleData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.GarageData)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.GarageDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("GarageData_Id");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Inventory_Id3");

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Player_Id3");

                entity.HasOne(d => d.TeamData)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.TeamDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TeamData_Id");

                entity.HasOne(d => d.VehicleData)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.VehicleDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VehicleData_Id");
            });

            modelBuilder.Entity<VehicleClassificationData>(entity =>
            {
                entity.ToTable("vehicle_classification_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<VehicleData>(entity =>
            {
                entity.ToTable("vehicle_data");

                entity.HasIndex(e => e.ClassificationId)
                    .HasName("Classification_Id");

                entity.HasIndex(e => e.InventoryTypeDataId)
                    .HasName("InventoryTypeData_Id2");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClassificationId)
                    .HasColumnName("Classification_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Hash).HasColumnType("int(10) unsigned");

                entity.Property(e => e.InventoryTypeDataId)
                    .HasColumnName("InventoryTypeData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MaxFuel).HasColumnType("int(11)");

                entity.Property(e => e.Multiplier).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Price).HasColumnType("int(11)");

                entity.Property(e => e.Seats).HasColumnType("tinyint(2) unsigned");

                entity.Property(e => e.Tax).HasColumnType("int(11)");

                entity.HasOne(d => d.Classification)
                    .WithMany(p => p.VehicleData)
                    .HasForeignKey(d => d.ClassificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classification_Id");

                entity.HasOne(d => d.InventoryTypeData)
                    .WithMany(p => p.VehicleData)
                    .HasForeignKey(d => d.InventoryTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("InventoryTypeData_Id2");
            });

            modelBuilder.Entity<VehicleShopData>(entity =>
            {
                entity.ToTable("vehicle_shop_data");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.PedHash)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.SpositionX).HasColumnName("SPositionX");

                entity.Property(e => e.SpositionY).HasColumnName("SPositionY");

                entity.Property(e => e.SpositionZ).HasColumnName("SPositionZ");
            });

            modelBuilder.Entity<VehicleShopVehicle>(entity =>
            {
                entity.ToTable("vehicle_shop_vehicle");

                entity.HasIndex(e => e.VehicleDataId)
                    .HasName("VehicleData_Id2");

                entity.HasIndex(e => e.VehicleShopDataId)
                    .HasName("VehicleShopData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ColorPrimary).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.ColorSecondary).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.VehicleDataId)
                    .HasColumnName("VehicleData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehicleShopDataId)
                    .HasColumnName("VehicleShopData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.VehicleData)
                    .WithMany(p => p.VehicleShopVehicle)
                    .HasForeignKey(d => d.VehicleDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VehicleData_Id2");

                entity.HasOne(d => d.VehicleShopData)
                    .WithMany(p => p.VehicleShopVehicle)
                    .HasForeignKey(d => d.VehicleShopDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VehicleShopData_Id");
            });

            modelBuilder.Entity<VehicleTuning>(entity =>
            {
                entity.ToTable("vehicle_tuning");

                entity.HasIndex(e => e.VehicleId)
                    .HasName("Vehicle_Id");

                entity.HasIndex(e => e.VehicleTuningDataId)
                    .HasName("VehicleTuningData_Id");

                entity.Property(e => e.Id)
                    .HasColumnType("tinyint(3) unsigned")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Value).HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.VehicleId)
                    .HasColumnName("Vehicle_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.VehicleTuningDataId)
                    .HasColumnName("VehicleTuningData_Id")
                    .HasColumnType("tinyint(3) unsigned");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.VehicleTuning)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Vehicle_Id");

                entity.HasOne(d => d.VehicleTuningData)
                    .WithMany(p => p.VehicleTuning)
                    .HasForeignKey(d => d.VehicleTuningDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VehicleTuningData_Id");
            });

            modelBuilder.Entity<VehicleTuningData>(entity =>
            {
                entity.ToTable("vehicle_tuning_data");

                entity.Property(e => e.Id)
                    .HasColumnType("tinyint(3) unsigned")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<WareExportData>(entity =>
            {
                entity.ToTable("ware_export_data");

                entity.HasIndex(e => e.ItemId)
                    .HasName("WareItemId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActualPrice).HasColumnType("int(11)");

                entity.Property(e => e.ItemId)
                    .HasColumnName("Item_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MaximumPrice).HasColumnType("int(11)");

                entity.Property(e => e.MinimumPrice).HasColumnType("int(11)");

                entity.Property(e => e.Setpoint).HasColumnType("int(11)");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.WareExportData)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WareItemId");
            });

            modelBuilder.Entity<WareExportDataHistory>(entity =>
            {
                entity.ToTable("ware_export_data_history");

                entity.HasIndex(e => e.WareExportDataId)
                    .HasName("FK_ware_export_data_history_ware_export_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActualPrice).HasColumnType("int(11)");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WareExportDataId)
                    .HasColumnName("WareExportData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.WareExportData)
                    .WithMany(p => p.WareExportDataHistory)
                    .HasForeignKey(d => d.WareExportDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ware_export_data_history_ware_export_data");
            });

            modelBuilder.Entity<WeaponAmmunitionData>(entity =>
            {
                entity.ToTable("weapon_ammunition_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<WeaponComponentData>(entity =>
            {
                entity.ToTable("weapon_component_data");

                entity.HasIndex(e => e.WeaponDataId)
                    .HasName("WeaponData_Id2");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Value).HasColumnType("int(10) unsigned");

                entity.Property(e => e.WeaponDataId)
                    .HasColumnName("WeaponData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.WeaponData)
                    .WithMany(p => p.WeaponComponentData)
                    .HasForeignKey(d => d.WeaponDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WeaponData_Id2");
            });

            modelBuilder.Entity<WeaponData>(entity =>
            {
                entity.ToTable("weapon_data");

                entity.HasIndex(e => e.WeaponAmmunitionDataId)
                    .HasName("WeaponAmmunitionData_Id");

                entity.HasIndex(e => e.WeaponTypeDataId)
                    .HasName("WeaponTypeData_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Hash).HasColumnType("int(10) unsigned");

                entity.Property(e => e.IsMkIi)
                    .HasColumnName("IsMkII")
                    .HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.WeaponAmmunitionDataId)
                    .HasColumnName("WeaponAmmunitionData_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeaponTypeDataId)
                    .HasColumnName("WeaponTypeData_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.WeaponAmmunitionData)
                    .WithMany(p => p.WeaponData)
                    .HasForeignKey(d => d.WeaponAmmunitionDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WeaponAmmunitionData_Id");

                entity.HasOne(d => d.WeaponTypeData)
                    .WithMany(p => p.WeaponData)
                    .HasForeignKey(d => d.WeaponTypeDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WeaponTypeData_Id");
            });

            modelBuilder.Entity<WeaponTintData>(entity =>
            {
                entity.ToTable("weapon_tint_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.IsMkIi)
                    .HasColumnName("IsMkII")
                    .HasColumnType("tinyint(3) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Value).HasColumnType("tinyint(3) unsigned");
            });

            modelBuilder.Entity<WeaponTypeData>(entity =>
            {
                entity.ToTable("weapon_type_data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
