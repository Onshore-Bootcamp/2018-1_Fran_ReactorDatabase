using ReactorDatabaseWebApp.Models;
using ReactorDB_DAL.Models;
using ReactorDB_BLL.Models;
using System;

namespace ReactorDatabaseWebApp.Mapping
{
    public static class Mapper
    {
        public static ReactorPO ReactorDOtoPO(ReactorDO from)
        {
            ReactorPO to = new ReactorPO();
            to.ReactorID = from.ReactorID;
            to.Name = from.Name;
            to.FullName = from.FullName;
            to.IsThermal = from.IsThermal;
            to.ModeratorID = from.ModeratorID;
            to.PrimaryCoolant = from.PrimaryCoolant;
            to.Fuel = from.Fuel;
            to.ThermalPower = from.ThermalPower;
            to.ElectricPower = from.ElectricPower;
            to.Efficiency = from.Efficiency;
            to.Year = from.Year;
            to.Generation = from.Generation;
            to.CountryOfOrigin = from.CountryOfOrigin;
            to.NumberActive = from.NumberActive;
            to.Notes = from.Notes;
            return to;
        }

        public static ReactorBO ReactorDOtoBO(ReactorDO from)
        {
            ReactorBO to = new ReactorBO();
            to.ReactorID = from.ReactorID;
            to.Name = from.Name;
            to.FullName = from.FullName;
            to.IsThermal = from.IsThermal;
            to.ModeratorID = from.ModeratorID;
            to.PrimaryCoolant = from.PrimaryCoolant;
            to.Fuel = from.Fuel;
            to.ThermalPower = from.ThermalPower;
            to.ElectricPower = from.ElectricPower;
            to.Efficiency = from.Efficiency;
            to.Year = from.Year;
            to.Generation = from.Generation;
            to.CountryOfOrigin = from.CountryOfOrigin;
            to.NumberActive = from.NumberActive;
            to.Notes = from.Notes;
            return to;
        }

        public static ReactorDO ReactorPOtoDO(ReactorPO from)
        {
            ReactorDO to = new ReactorDO();
            to.ReactorID = from.ReactorID;
            to.Name = from.Name;
            to.FullName = from.FullName;
            to.IsThermal = from.IsThermal;
            to.ModeratorID = from.ModeratorID;
            to.PrimaryCoolant = from.PrimaryCoolant;
            to.Fuel = from.Fuel;
            to.ThermalPower = from.ThermalPower;
            to.ElectricPower = from.ElectricPower;
            to.Efficiency = from.Efficiency;
            to.Year = from.Year;
            to.Generation = from.Generation;
            to.CountryOfOrigin = from.CountryOfOrigin;
            to.NumberActive = from.NumberActive;
            to.Notes = from.Notes;
            return to;
        }

        public static ReactorBO ReactorPOtoBO(ReactorPO from)
        {
            ReactorBO to = new ReactorBO();
            to.ReactorID = from.ReactorID;
            to.Name = from.Name;
            to.FullName = from.FullName;
            to.IsThermal = from.IsThermal;
            to.ModeratorID = from.ModeratorID;
            to.PrimaryCoolant = from.PrimaryCoolant;
            to.Fuel = from.Fuel;
            to.ThermalPower = from.ThermalPower;
            to.ElectricPower = from.ElectricPower;
            to.Efficiency = from.Efficiency;
            to.Year = from.Year;
            to.Generation = from.Generation;
            to.CountryOfOrigin = from.CountryOfOrigin;
            to.NumberActive = from.NumberActive;
            to.Notes = from.Notes;
            return to;
        }

        public static ModeratorPO ModeratorDOtoPO(ModeratorDO from)
        {
            ModeratorPO to = new ModeratorPO();
            to.ModeratorID = from.ModeratorID;
            to.Name = from.Name;
            to.ChemicalFormula = from.ChemicalFormula;
            to.Nucleus = from.Nucleus;
            to.AtomicMass = from.AtomicMass;
            to.EnergyDecrement = from.EnergyDecrement;
            to.Collisions = from.Collisions;
            to.ScatteringXS = from.ScatteringXS;
            to.AbsorptionXS = from.AbsorptionXS;
            to.ModerationEfficiency = from.ModerationEfficiency;
            return to;
        }

        public static ModeratorBO ModeratorPOtoBO(ModeratorPO from)
        {
            ModeratorBO to = new ModeratorBO();
            to.ModeratorID = from.ModeratorID;
            to.Name = from.Name;
            to.ChemicalFormula = from.ChemicalFormula;
            to.Nucleus = from.Nucleus;
            to.AtomicMass = from.AtomicMass;
            to.EnergyDecrement = from.EnergyDecrement;
            to.Collisions = from.Collisions;
            to.ScatteringXS = from.ScatteringXS;
            to.AbsorptionXS = from.AbsorptionXS;
            to.ModerationEfficiency = from.ModerationEfficiency;
            return to;
        }

        public static ModeratorDO ModeratorPOtoDO(ModeratorPO from)
        {
            ModeratorDO to = new ModeratorDO();
            to.ModeratorID = from.ModeratorID;
            to.Name = from.Name;
            to.ChemicalFormula = from.ChemicalFormula;
            to.Nucleus = from.Nucleus;
            to.AtomicMass = from.AtomicMass;
            to.EnergyDecrement = from.EnergyDecrement;
            to.Collisions = from.Collisions;
            to.ScatteringXS = from.ScatteringXS;
            to.AbsorptionXS = from.AbsorptionXS;
            to.ModerationEfficiency = from.ModerationEfficiency;
            return to;
        }

        public static UserPO UserDOtoPO(UserDO from)
        {
            UserPO to = new UserPO();
            to.UserID = from.UserID;
            to.Username = from.Username;
            to.Password = from.Password;
            to.RoleID = from.RoleID;
            to.FirstName = from.FirstName;
            to.LastName = from.LastName;
            to.Age = from.Age;
            to.Email = from.Email;
            to.Subscription = from.Subscription;
            return to;
        }

        public static UserDO UserPOtoDO(UserPO from)
        {
            UserDO to = new UserDO();
            to.UserID = from.UserID;
            to.Username = from.Username;
            to.Password = from.Password;
            to.RoleID = from.RoleID;
            to.FirstName = from.FirstName;
            to.LastName = from.LastName;
            to.Age = from.Age;
            to.Email = from.Email;
            to.Subscription = from.Subscription;
            return to;
        }

        public static BookmarkPO BookmarkDOtoPO(BookmarkDO from)
        {
            BookmarkPO to = new BookmarkPO();
            to.BookmarkID = from.BookmarkID;
            to.UserID = from.UserID;
            to.ReactorID = from.ReactorID;
            return to;
        }

        public static BookmarkDO BookmarkPOtoDO(BookmarkPO from)
        {
            BookmarkDO to = new BookmarkDO();
            to.BookmarkID = from.BookmarkID;
            to.UserID = from.UserID;
            to.ReactorID = from.ReactorID;
            return to;
        }

        public static StatisticsPO StatisticsBOtoPO(StatisticsBO from)
        {
            StatisticsPO to = new StatisticsPO();
            to.ReactorCount = from.ReactorCount;
            to.ThermalCount = from.ThermalCount;
            to.FastCount = from.FastCount;
            to.PercentThermal = from.PercentThermal;
            to.PercentFast = from.PercentFast;
            to.GenIcount = from.GenIcount;
            to.GenIIcount = from.GenIIcount;
            to.GenIIIcount = from.GenIIIcount;
            to.GenIVcount = from.GenIVcount;
            to.GenVcount = from.GenVcount;
            to.NoGenCount = from.NoGenCount;
            to.PercentGenI = from.PercentGenI;
            to.PercentGenII = from.PercentGenII;
            to.PercentGenIII = from.PercentGenIII;
            to.PercentGenIV = from.PercentGenIV;
            to.PercentGenV = from.PercentGenV;
            to.PercentNoGen = from.PercentNoGen;
            return to;
        }

        public static LogPO LogDOtoPO(LogDO from)
        {
            LogPO to = new LogPO();
            to.LogID = from.LogID;
            to.Timestamp = from.Timestamp;
            to.Message = from.Message;
            return to;
        }
    }
}