-- phpMyAdmin SQL Dump
-- version 5.1.3
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Erstellungszeit: 24. Jul 2022 um 23:21
-- Server-Version: 10.4.24-MariaDB
-- PHP-Version: 7.4.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `sibauirplol`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `area`
--

CREATE TABLE `area` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `bank`
--

CREATE TABLE `bank` (
  `Id` int(11) NOT NULL,
  `Name` varchar(64) CHARACTER SET latin1 NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `BankTypeId` int(11) NOT NULL,
  `CurrentMoney` int(11) DEFAULT NULL,
  `MaxMoney` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `banktype`
--

CREATE TABLE `banktype` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `bank_data`
--

CREATE TABLE `bank_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(64) CHARACTER SET latin1 NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `BankTypeId` int(11) NOT NULL,
  `Class` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `bank_type_data`
--

CREATE TABLE `bank_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `WithdrawFee` float NOT NULL COMMENT 'Prozentuale Gebühren beim Abheben beim Geldautomaten eines anderen Instituts',
  `AccountFee` float NOT NULL COMMENT 'Prozentuale Kontoführungsgebühren',
  `DepositFee` float NOT NULL COMMENT 'Prozentuale Gebühren beim Einzahlen beim Geldautomaten eines anderen Instituts',
  `WithdrawFeeMinimum` int(11) NOT NULL COMMENT 'Minimale Gebühren beim Abheben beim Geldautomaten eines anderen Instituts',
  `DepositFeeMinimum` int(11) NOT NULL COMMENT 'Minimale Gebühren beim Einzahlen beim Geldautomaten eines anderen Instituts',
  `WithdrawFeeMaximum` int(11) NOT NULL COMMENT 'Maximale Gebühren beim Abheben beim Geldautomaten eines anderen Instituts',
  `AccountFeeMaximum` int(11) NOT NULL COMMENT 'Maximale Kontoführungsgebühren',
  `DepositFeeMaximum` int(11) NOT NULL COMMENT 'Maximale Gebühren beim Einzahlen beim Geldautomaten eines anderen Instituts'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `cloth_data`
--

CREATE TABLE `cloth_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `Gender` tinyint(3) UNSIGNED NOT NULL,
  `ClothTypeData_Id` int(11) NOT NULL,
  `Value` smallint(6) NOT NULL,
  `ClothShopData_Id` int(11) NOT NULL,
  `Price` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `cloth_shop_data`
--

CREATE TABLE `cloth_shop_data` (
  `Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `WarderobeX` float NOT NULL,
  `WarderobeY` float NOT NULL,
  `WarderobeZ` float NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL,
  `IsActive` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `cloth_type_data`
--

CREATE TABLE `cloth_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `Value` tinyint(3) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `crime_category_data`
--

CREATE TABLE `crime_category_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(64) CHARACTER SET latin1 NOT NULL,
  `Order` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `crime_data`
--

CREATE TABLE `crime_data` (
  `Id` int(11) NOT NULL,
  `CrimeCategoryData_Id` int(11) NOT NULL,
  `Name` varchar(64) CHARACTER SET latin1 NOT NULL,
  `Description` varchar(128) CHARACTER SET latin1 NOT NULL,
  `Jailtime` int(11) NOT NULL,
  `Cost` int(11) NOT NULL,
  `TakeGunLic` int(11) NOT NULL,
  `TakeDriverLic` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `door_data`
--

CREATE TABLE `door_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `ModelHash` int(11) NOT NULL,
  `Locked` tinyint(1) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `Range` int(11) NOT NULL,
  `Teams` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `drug_camper_type_data`
--

CREATE TABLE `drug_camper_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `drug_camper_type_item_data`
--

CREATE TABLE `drug_camper_type_item_data` (
  `Id` int(11) NOT NULL,
  `DrugCamperTypeData_Id` int(11) NOT NULL,
  `IsInput` int(11) NOT NULL DEFAULT 1,
  `ItemData_Id` int(11) NOT NULL,
  `Amount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `drug_export_container_data`
--

CREATE TABLE `drug_export_container_data` (
  `Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `farm_field_data`
--

CREATE TABLE `farm_field_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `MinimumObjects` int(11) NOT NULL,
  `MaximumObjects` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `farm_field_object_data`
--

CREATE TABLE `farm_field_object_data` (
  `Id` int(11) NOT NULL,
  `FarmFieldDataId` int(11) NOT NULL,
  `FarmObjectDataId` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `RotationRoll` float NOT NULL,
  `RotationPitch` float NOT NULL,
  `RotationYaw` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `farm_object_data`
--

CREATE TABLE `farm_object_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `ObjectName` varchar(50) CHARACTER SET utf8 NOT NULL,
  `Capacity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `farm_object_loot_data`
--

CREATE TABLE `farm_object_loot_data` (
  `Id` int(11) NOT NULL,
  `FarmObjectDataId` int(11) NOT NULL,
  `ItemDataId` int(11) NOT NULL,
  `MinimumAmount` int(11) NOT NULL,
  `MaximumAmount` int(11) NOT NULL,
  `Chance` float NOT NULL COMMENT 'Wahrscheinlichkeit in % (Wert von 10 bedeutet 10%)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `fuelstation_data`
--

CREATE TABLE `fuelstation_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(128) CHARACTER SET latin1 NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `InfoPositionX` float NOT NULL,
  `InfoPositionY` float NOT NULL,
  `InfoPositionZ` int(11) NOT NULL,
  `Range` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `fuelstation_gaspump_data`
--

CREATE TABLE `fuelstation_gaspump_data` (
  `Id` int(11) NOT NULL,
  `FuelstationData_Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `garagespawn_data`
--

CREATE TABLE `garagespawn_data` (
  `Id` int(11) NOT NULL,
  `Garage_Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `RotationX` float NOT NULL,
  `RotationY` float NOT NULL,
  `RotationZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `garage_data`
--

CREATE TABLE `garage_data` (
  `Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `Rotation` float NOT NULL,
  `Type` int(11) NOT NULL,
  `HasMarker` tinyint(1) NOT NULL,
  `Name` varchar(64) CHARACTER SET latin1 NOT NULL,
  `PedHash` varchar(128) CHARACTER SET latin1 NOT NULL,
  `Radius` int(11) NOT NULL,
  `VehicleClassifications` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `house_appearance_data`
--

CREATE TABLE `house_appearance_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `Points` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `house_area_data`
--

CREATE TABLE `house_area_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `Points` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `house_data`
--

CREATE TABLE `house_data` (
  `Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `RotationRoll` float NOT NULL,
  `RotationPitch` float NOT NULL,
  `RotationYaw` float NOT NULL,
  `RentalPlaces` tinyint(4) NOT NULL,
  `Price` int(11) NOT NULL,
  `InteriorData_Id` int(11) NOT NULL,
  `HouseAreaData_Id` int(11) NOT NULL,
  `HouseAppearanceData_Id` int(11) NOT NULL,
  `HouseSizeData_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `house_size_data`
--

CREATE TABLE `house_size_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `Points` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `injury_death_cause_data`
--

CREATE TABLE `injury_death_cause_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `Hash` int(11) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `injury_type_data`
--

CREATE TABLE `injury_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `InjuryDeathCauseData_Id` int(11) NOT NULL,
  `Time` int(11) NOT NULL,
  `AdditionalTime` int(11) NOT NULL,
  `TreatmentType` int(11) NOT NULL,
  `Percentage` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `interior_data`
--

CREATE TABLE `interior_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `interior_position_data`
--

CREATE TABLE `interior_position_data` (
  `Id` int(11) NOT NULL,
  `InteriorDataId` int(11) NOT NULL,
  `InteriorPositionDataType_Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `interior_position_type_data`
--

CREATE TABLE `interior_position_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `interior_warderobe_data`
--

CREATE TABLE `interior_warderobe_data` (
  `Id` int(11) NOT NULL,
  `InteriorDataId` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `inventory`
--

CREATE TABLE `inventory` (
  `Id` int(11) NOT NULL,
  `InventoryTypeData_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `inventory_type_data`
--

CREATE TABLE `inventory_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `MaxSlots` int(11) NOT NULL,
  `MaxWeight` int(11) NOT NULL,
  `Type_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `item_data`
--

CREATE TABLE `item_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `Weight` int(11) NOT NULL,
  `Stacksize` int(11) NOT NULL,
  `RemoveOnUse` tinyint(1) NOT NULL,
  `Illegal` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `parcel_delivery_points`
--

CREATE TABLE `parcel_delivery_points` (
  `Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `AreaId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `plant`
--

CREATE TABLE `plant` (
  `Id` int(11) NOT NULL,
  `PlantTypeDataId` int(11) NOT NULL,
  `GrowState` int(11) NOT NULL,
  `LootFactor` float NOT NULL COMMENT 'LootFactor * BaseAmount = LootAmount',
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `PlanterPlayerId` int(11) NOT NULL,
  `ActualWater` int(11) NOT NULL,
  `ActualFertilizer` int(11) NOT NULL,
  `PlantDate` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `HarvestDate` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `HarvestPlayerId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `plant_type_data`
--

CREATE TABLE `plant_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `SeedItemId` int(11) NOT NULL,
  `TimeToGrow` int(11) NOT NULL COMMENT 'In Minuten',
  `ObjectStageOne` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `ObjectStageTwo` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT '0',
  `MaximumWater` int(11) NOT NULL DEFAULT 20,
  `MaximumFertilizer` int(11) NOT NULL DEFAULT 20
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `plant_type_loot_data`
--

CREATE TABLE `plant_type_loot_data` (
  `Id` int(11) NOT NULL,
  `PlantTypeDataId` int(11) NOT NULL,
  `ItemDataId` int(11) NOT NULL,
  `BaseAmount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `police_computer_data`
--

CREATE TABLE `police_computer_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `rank`
--

CREATE TABLE `rank` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `Power` int(11) NOT NULL,
  `Payday` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `saveposition`
--

CREATE TABLE `saveposition` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `RotationX` float NOT NULL,
  `RotationY` float NOT NULL,
  `RotationZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `server_scenario_data`
--

CREATE TABLE `server_scenario_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `shop_data`
--

CREATE TABLE `shop_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(128) CHARACTER SET latin1 NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `Rotation` float NOT NULL,
  `PedHash` varchar(128) CHARACTER SET latin1 NOT NULL,
  `Marker` int(11) NOT NULL DEFAULT 1,
  `RobPositionX` float NOT NULL,
  `RobPositionY` float NOT NULL,
  `RobPositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `sms_chat`
--

CREATE TABLE `sms_chat` (
  `Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `storageroom_data`
--

CREATE TABLE `storageroom_data` (
  `Id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `team_type_data`
--

CREATE TABLE `team_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `ticket_machine_data`
--

CREATE TABLE `ticket_machine_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `vehicle_classification_data`
--

CREATE TABLE `vehicle_classification_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `vehicle_shop_data`
--

CREATE TABLE `vehicle_shop_data` (
  `id` int(11) NOT NULL,
  `PositionX` float NOT NULL,
  `PositionY` float NOT NULL,
  `PositionZ` float NOT NULL,
  `SPositionX` float NOT NULL,
  `SPositionY` float NOT NULL,
  `SPositionZ` float NOT NULL,
  `Activated` tinyint(1) NOT NULL,
  `HasMarker` tinyint(1) NOT NULL,
  `Description` varchar(128) CHARACTER SET latin1 NOT NULL,
  `PedHash` varchar(128) CHARACTER SET latin1 NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `vehicle_tuning_data`
--

CREATE TABLE `vehicle_tuning_data` (
  `Id` tinyint(3) UNSIGNED NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `ware_export_data`
--

CREATE TABLE `ware_export_data` (
  `Id` int(11) NOT NULL,
  `Item_Id` int(11) NOT NULL,
  `MinimumPrice` int(11) NOT NULL,
  `ActualPrice` int(11) NOT NULL,
  `MaximumPrice` int(11) NOT NULL,
  `Setpoint` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `weapon_ammunition_data`
--

CREATE TABLE `weapon_ammunition_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `weapon_tint_data`
--

CREATE TABLE `weapon_tint_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0',
  `Value` tinyint(3) UNSIGNED NOT NULL,
  `IsMkII` tinyint(3) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `weapon_type_data`
--

CREATE TABLE `weapon_type_data` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) CHARACTER SET latin1 NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(95) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `area`
--
ALTER TABLE `area`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `bank`
--
ALTER TABLE `bank`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_bank_banktype` (`BankTypeId`);

--
-- Indizes für die Tabelle `banktype`
--
ALTER TABLE `banktype`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `bank_data`
--
ALTER TABLE `bank_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `BankType_Id` (`BankTypeId`);

--
-- Indizes für die Tabelle `bank_type_data`
--
ALTER TABLE `bank_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `cloth_data`
--
ALTER TABLE `cloth_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `ClothShopData_Id` (`ClothShopData_Id`),
  ADD KEY `ClothTypeData_Id` (`ClothTypeData_Id`);

--
-- Indizes für die Tabelle `cloth_shop_data`
--
ALTER TABLE `cloth_shop_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `cloth_type_data`
--
ALTER TABLE `cloth_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `crime_category_data`
--
ALTER TABLE `crime_category_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `crime_data`
--
ALTER TABLE `crime_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `CrimeCategoryData_Id` (`CrimeCategoryData_Id`);

--
-- Indizes für die Tabelle `door_data`
--
ALTER TABLE `door_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `drug_camper_type_data`
--
ALTER TABLE `drug_camper_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `drug_camper_type_item_data`
--
ALTER TABLE `drug_camper_type_item_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_drug_camper_type_item_data_drug_camper_type_data` (`DrugCamperTypeData_Id`),
  ADD KEY `FK_drug_camper_type_data_item_data` (`ItemData_Id`);

--
-- Indizes für die Tabelle `drug_export_container_data`
--
ALTER TABLE `drug_export_container_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `farm_field_data`
--
ALTER TABLE `farm_field_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `farm_field_object_data`
--
ALTER TABLE `farm_field_object_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FarmFieldDataId` (`FarmFieldDataId`),
  ADD KEY `FarmObjectDataId` (`FarmObjectDataId`);

--
-- Indizes für die Tabelle `farm_object_data`
--
ALTER TABLE `farm_object_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `farm_object_loot_data`
--
ALTER TABLE `farm_object_loot_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FarmObjectDataId2` (`FarmObjectDataId`),
  ADD KEY `ItemDataId` (`ItemDataId`);

--
-- Indizes für die Tabelle `fuelstation_data`
--
ALTER TABLE `fuelstation_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `fuelstation_gaspump_data`
--
ALTER TABLE `fuelstation_gaspump_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FuelstationData_Id` (`FuelstationData_Id`);

--
-- Indizes für die Tabelle `garagespawn_data`
--
ALTER TABLE `garagespawn_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Garage_Id` (`Garage_Id`);

--
-- Indizes für die Tabelle `garage_data`
--
ALTER TABLE `garage_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `house_appearance_data`
--
ALTER TABLE `house_appearance_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `house_area_data`
--
ALTER TABLE `house_area_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `house_data`
--
ALTER TABLE `house_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `HouseAppearanceData_Id` (`HouseAppearanceData_Id`),
  ADD KEY `HouseAreaData_Id` (`HouseAreaData_Id`),
  ADD KEY `HouseSizeData_Id` (`HouseSizeData_Id`),
  ADD KEY `InteriorData_Id2` (`InteriorData_Id`);

--
-- Indizes für die Tabelle `house_size_data`
--
ALTER TABLE `house_size_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `injury_death_cause_data`
--
ALTER TABLE `injury_death_cause_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `injury_type_data`
--
ALTER TABLE `injury_type_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InjuryDeathCauseData_Id` (`InjuryDeathCauseData_Id`);

--
-- Indizes für die Tabelle `interior_data`
--
ALTER TABLE `interior_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `interior_position_data`
--
ALTER TABLE `interior_position_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InteriorData_Id` (`InteriorDataId`),
  ADD KEY `InteriorPositionDataType_Id` (`InteriorPositionDataType_Id`);

--
-- Indizes für die Tabelle `interior_position_type_data`
--
ALTER TABLE `interior_position_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `interior_warderobe_data`
--
ALTER TABLE `interior_warderobe_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `interior_warderobe_data_ibfk_1` (`InteriorDataId`);

--
-- Indizes für die Tabelle `inventory`
--
ALTER TABLE `inventory`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InventoryTypeData_Id` (`InventoryTypeData_Id`);

--
-- Indizes für die Tabelle `inventory_type_data`
--
ALTER TABLE `inventory_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `item_data`
--
ALTER TABLE `item_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `parcel_delivery_points`
--
ALTER TABLE `parcel_delivery_points`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `AreaId_Key` (`AreaId`);

--
-- Indizes für die Tabelle `plant`
--
ALTER TABLE `plant`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PlantTypeDataId2` (`PlantTypeDataId`);

--
-- Indizes für die Tabelle `plant_type_data`
--
ALTER TABLE `plant_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `plant_type_loot_data`
--
ALTER TABLE `plant_type_loot_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PlantLootItemDataId` (`ItemDataId`),
  ADD KEY `PlantTypeDataId` (`PlantTypeDataId`);

--
-- Indizes für die Tabelle `police_computer_data`
--
ALTER TABLE `police_computer_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `rank`
--
ALTER TABLE `rank`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `saveposition`
--
ALTER TABLE `saveposition`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `server_scenario_data`
--
ALTER TABLE `server_scenario_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `shop_data`
--
ALTER TABLE `shop_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `sms_chat`
--
ALTER TABLE `sms_chat`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `storageroom_data`
--
ALTER TABLE `storageroom_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `team_type_data`
--
ALTER TABLE `team_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `ticket_machine_data`
--
ALTER TABLE `ticket_machine_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `vehicle_classification_data`
--
ALTER TABLE `vehicle_classification_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `vehicle_shop_data`
--
ALTER TABLE `vehicle_shop_data`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `vehicle_tuning_data`
--
ALTER TABLE `vehicle_tuning_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `ware_export_data`
--
ALTER TABLE `ware_export_data`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `WareItemId` (`Item_Id`);

--
-- Indizes für die Tabelle `weapon_ammunition_data`
--
ALTER TABLE `weapon_ammunition_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `weapon_tint_data`
--
ALTER TABLE `weapon_tint_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `weapon_type_data`
--
ALTER TABLE `weapon_type_data`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `area`
--
ALTER TABLE `area`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `bank`
--
ALTER TABLE `bank`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `banktype`
--
ALTER TABLE `banktype`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `bank_data`
--
ALTER TABLE `bank_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `bank_type_data`
--
ALTER TABLE `bank_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `cloth_data`
--
ALTER TABLE `cloth_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `cloth_shop_data`
--
ALTER TABLE `cloth_shop_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `cloth_type_data`
--
ALTER TABLE `cloth_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `crime_category_data`
--
ALTER TABLE `crime_category_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `crime_data`
--
ALTER TABLE `crime_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `door_data`
--
ALTER TABLE `door_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `drug_camper_type_data`
--
ALTER TABLE `drug_camper_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `drug_camper_type_item_data`
--
ALTER TABLE `drug_camper_type_item_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `drug_export_container_data`
--
ALTER TABLE `drug_export_container_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `farm_field_data`
--
ALTER TABLE `farm_field_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `farm_field_object_data`
--
ALTER TABLE `farm_field_object_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `farm_object_data`
--
ALTER TABLE `farm_object_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `farm_object_loot_data`
--
ALTER TABLE `farm_object_loot_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `fuelstation_data`
--
ALTER TABLE `fuelstation_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `fuelstation_gaspump_data`
--
ALTER TABLE `fuelstation_gaspump_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `garagespawn_data`
--
ALTER TABLE `garagespawn_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `garage_data`
--
ALTER TABLE `garage_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `house_appearance_data`
--
ALTER TABLE `house_appearance_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `house_area_data`
--
ALTER TABLE `house_area_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `house_data`
--
ALTER TABLE `house_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `house_size_data`
--
ALTER TABLE `house_size_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `injury_death_cause_data`
--
ALTER TABLE `injury_death_cause_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `injury_type_data`
--
ALTER TABLE `injury_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `interior_data`
--
ALTER TABLE `interior_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `interior_position_data`
--
ALTER TABLE `interior_position_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `interior_position_type_data`
--
ALTER TABLE `interior_position_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `interior_warderobe_data`
--
ALTER TABLE `interior_warderobe_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `inventory`
--
ALTER TABLE `inventory`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `inventory_type_data`
--
ALTER TABLE `inventory_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `item_data`
--
ALTER TABLE `item_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `parcel_delivery_points`
--
ALTER TABLE `parcel_delivery_points`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `plant`
--
ALTER TABLE `plant`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `plant_type_data`
--
ALTER TABLE `plant_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `plant_type_loot_data`
--
ALTER TABLE `plant_type_loot_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `police_computer_data`
--
ALTER TABLE `police_computer_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `rank`
--
ALTER TABLE `rank`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `saveposition`
--
ALTER TABLE `saveposition`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `server_scenario_data`
--
ALTER TABLE `server_scenario_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `shop_data`
--
ALTER TABLE `shop_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `sms_chat`
--
ALTER TABLE `sms_chat`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `storageroom_data`
--
ALTER TABLE `storageroom_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `team_type_data`
--
ALTER TABLE `team_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `ticket_machine_data`
--
ALTER TABLE `ticket_machine_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `vehicle_classification_data`
--
ALTER TABLE `vehicle_classification_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `vehicle_shop_data`
--
ALTER TABLE `vehicle_shop_data`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `vehicle_tuning_data`
--
ALTER TABLE `vehicle_tuning_data`
  MODIFY `Id` tinyint(3) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `ware_export_data`
--
ALTER TABLE `ware_export_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `weapon_ammunition_data`
--
ALTER TABLE `weapon_ammunition_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `weapon_tint_data`
--
ALTER TABLE `weapon_tint_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `weapon_type_data`
--
ALTER TABLE `weapon_type_data`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints der exportierten Tabellen
--

--
-- Constraints der Tabelle `bank`
--
ALTER TABLE `bank`
  ADD CONSTRAINT `FK_bank_banktype` FOREIGN KEY (`BankTypeId`) REFERENCES `banktype` (`Id`);

--
-- Constraints der Tabelle `bank_data`
--
ALTER TABLE `bank_data`
  ADD CONSTRAINT `BankType_Id` FOREIGN KEY (`BankTypeId`) REFERENCES `bank_type_data` (`Id`);

--
-- Constraints der Tabelle `cloth_data`
--
ALTER TABLE `cloth_data`
  ADD CONSTRAINT `ClothShopData_Id` FOREIGN KEY (`ClothShopData_Id`) REFERENCES `cloth_shop_data` (`Id`),
  ADD CONSTRAINT `ClothTypeData_Id` FOREIGN KEY (`ClothTypeData_Id`) REFERENCES `cloth_type_data` (`Id`);

--
-- Constraints der Tabelle `crime_data`
--
ALTER TABLE `crime_data`
  ADD CONSTRAINT `CrimeCategoryData_Id` FOREIGN KEY (`CrimeCategoryData_Id`) REFERENCES `crime_category_data` (`Id`);

--
-- Constraints der Tabelle `drug_camper_type_item_data`
--
ALTER TABLE `drug_camper_type_item_data`
  ADD CONSTRAINT `FK_drug_camper_type_data_item_data` FOREIGN KEY (`ItemData_Id`) REFERENCES `item_data` (`Id`),
  ADD CONSTRAINT `FK_drug_camper_type_item_data_drug_camper_type_data` FOREIGN KEY (`DrugCamperTypeData_Id`) REFERENCES `drug_camper_type_data` (`Id`);

--
-- Constraints der Tabelle `farm_field_object_data`
--
ALTER TABLE `farm_field_object_data`
  ADD CONSTRAINT `FarmFieldDataId` FOREIGN KEY (`FarmFieldDataId`) REFERENCES `farm_field_data` (`Id`),
  ADD CONSTRAINT `FarmObjectDataId` FOREIGN KEY (`FarmObjectDataId`) REFERENCES `farm_object_data` (`Id`);

--
-- Constraints der Tabelle `farm_object_loot_data`
--
ALTER TABLE `farm_object_loot_data`
  ADD CONSTRAINT `FarmObjectDataId2` FOREIGN KEY (`FarmObjectDataId`) REFERENCES `farm_object_data` (`Id`),
  ADD CONSTRAINT `ItemDataId` FOREIGN KEY (`ItemDataId`) REFERENCES `item_data` (`Id`);

--
-- Constraints der Tabelle `fuelstation_gaspump_data`
--
ALTER TABLE `fuelstation_gaspump_data`
  ADD CONSTRAINT `FuelstationData_Id` FOREIGN KEY (`FuelstationData_Id`) REFERENCES `fuelstation_data` (`Id`);

--
-- Constraints der Tabelle `garagespawn_data`
--
ALTER TABLE `garagespawn_data`
  ADD CONSTRAINT `Garage_Id` FOREIGN KEY (`Garage_Id`) REFERENCES `garage_data` (`Id`);

--
-- Constraints der Tabelle `house_data`
--
ALTER TABLE `house_data`
  ADD CONSTRAINT `HouseAppearanceData_Id` FOREIGN KEY (`HouseAppearanceData_Id`) REFERENCES `house_appearance_data` (`Id`),
  ADD CONSTRAINT `HouseAreaData_Id` FOREIGN KEY (`HouseAreaData_Id`) REFERENCES `house_area_data` (`Id`),
  ADD CONSTRAINT `HouseSizeData_Id` FOREIGN KEY (`HouseSizeData_Id`) REFERENCES `house_size_data` (`Id`),
  ADD CONSTRAINT `InteriorData_Id2` FOREIGN KEY (`InteriorData_Id`) REFERENCES `interior_data` (`Id`);

--
-- Constraints der Tabelle `injury_type_data`
--
ALTER TABLE `injury_type_data`
  ADD CONSTRAINT `InjuryDeathCauseData_Id` FOREIGN KEY (`InjuryDeathCauseData_Id`) REFERENCES `injury_death_cause_data` (`Id`);

--
-- Constraints der Tabelle `interior_position_data`
--
ALTER TABLE `interior_position_data`
  ADD CONSTRAINT `InteriorData_Id` FOREIGN KEY (`InteriorDataId`) REFERENCES `interior_data` (`Id`),
  ADD CONSTRAINT `InteriorPositionDataType_Id` FOREIGN KEY (`InteriorPositionDataType_Id`) REFERENCES `interior_position_type_data` (`Id`);

--
-- Constraints der Tabelle `interior_warderobe_data`
--
ALTER TABLE `interior_warderobe_data`
  ADD CONSTRAINT `interior_warderobe_data_ibfk_1` FOREIGN KEY (`InteriorDataId`) REFERENCES `interior_data` (`Id`);

--
-- Constraints der Tabelle `inventory`
--
ALTER TABLE `inventory`
  ADD CONSTRAINT `InventoryTypeData_Id` FOREIGN KEY (`InventoryTypeData_Id`) REFERENCES `inventory_type_data` (`Id`);

--
-- Constraints der Tabelle `parcel_delivery_points`
--
ALTER TABLE `parcel_delivery_points`
  ADD CONSTRAINT `AreaId_Key` FOREIGN KEY (`AreaId`) REFERENCES `area` (`Id`);

--
-- Constraints der Tabelle `plant`
--
ALTER TABLE `plant`
  ADD CONSTRAINT `PlantTypeDataId2` FOREIGN KEY (`PlantTypeDataId`) REFERENCES `plant_type_data` (`Id`);

--
-- Constraints der Tabelle `plant_type_loot_data`
--
ALTER TABLE `plant_type_loot_data`
  ADD CONSTRAINT `PlantLootItemDataId` FOREIGN KEY (`ItemDataId`) REFERENCES `item_data` (`Id`),
  ADD CONSTRAINT `PlantTypeDataId` FOREIGN KEY (`PlantTypeDataId`) REFERENCES `plant_type_data` (`Id`);

--
-- Constraints der Tabelle `ware_export_data`
--
ALTER TABLE `ware_export_data`
  ADD CONSTRAINT `WareItemId` FOREIGN KEY (`Item_Id`) REFERENCES `item_data` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
