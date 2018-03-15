using ReactorDB_DAL.Models;
using System.Data;

namespace ReactorDB_DAL.Mapping
{
    public static class Mapper
    {
        public static ReactorDO ReactorTableRowToDO(DataRow row)
        {
            ReactorDO reactorDO = new ReactorDO();
            reactorDO.ReactorID = int.Parse(row["ReactorID"].ToString());
            reactorDO.Name = row["Name"].ToString().Trim();
            reactorDO.FullName = row["FullName"].ToString().Trim();
            reactorDO.IsThermal = bool.Parse(row["IsThermal"].ToString());
            reactorDO.ModeratorID = (int.TryParse(row["ModeratorID"].ToString(), out int moderatorID)) ? moderatorID : default(int);
            reactorDO.PrimaryCoolant = row["PrimaryCoolant"].ToString().Trim();
            reactorDO.Fuel = row["Fuel"].ToString().Trim();
            reactorDO.ThermalPower = (float.TryParse(row["ThermalPower"].ToString(), out float thermalPower)) ? thermalPower : default(float);
            reactorDO.ElectricPower = (float.TryParse(row["ElectricPower"].ToString(), out float electricPower)) ? electricPower : default(float);
            reactorDO.Efficiency = (float.TryParse(row["Efficiency"].ToString(), out float efficiency)) ? efficiency : default(float);
            reactorDO.Year = int.Parse(row["Year"].ToString());
            reactorDO.Generation = row["Generation"].ToString().Trim();
            reactorDO.CountryOfOrigin = row["CountryOfOrigin"].ToString().Trim();
            reactorDO.NumberActive = (int.TryParse(row["NumberActive"].ToString(), out int active)) ? active : default(int);
            reactorDO.Notes = row["Notes"].ToString().Trim();
            return reactorDO;
        }

        public static ModeratorDO ModeratorTableRowToDO(DataRow row)
        {
            ModeratorDO moderatorDO = new ModeratorDO();
            moderatorDO.ModeratorID = int.Parse(row["ModeratorID"].ToString());
            moderatorDO.Name = row["Name"].ToString().Trim();
            moderatorDO.ChemicalFormula = row["ChemicalFormula"].ToString().Trim();
            moderatorDO.Nucleus = row["Nucleus"].ToString().Trim();
            moderatorDO.AtomicMass = (int.TryParse(row["AtomicMass"].ToString(), out int atomicMass) ? atomicMass : default(int));
            moderatorDO.EnergyDecrement = (float.TryParse(row["EnergyDecrement"].ToString(), out float energyDecrement) ? energyDecrement : default(float));
            moderatorDO.Collisions = (int.TryParse(row["NumberOfCollisions"].ToString(), out int collisions) ? collisions : default(int));
            moderatorDO.ScatteringXS = (float.TryParse(row["ScatteringXS"].ToString(), out float sigmaS) ? sigmaS : default(float));
            moderatorDO.AbsorptionXS = (float.TryParse(row["AbsorptionXS"].ToString(), out float sigmaA) ? sigmaA : default(float));
            moderatorDO.ModerationEfficiency = (float.TryParse(row["Efficiency"].ToString(), out float efficiency) ? efficiency : default(float));
            return moderatorDO;
        }

        public static UserDO UserTableRowToDO(DataRow row)
        {
            UserDO userDO = new UserDO();
            userDO.UserID = int.Parse(row["UserID"].ToString());
            userDO.Username = row["Username"].ToString().Trim();
            userDO.Password = row["Password"].ToString();
            userDO.RoleID = int.Parse(row["RoleID"].ToString());
            userDO.FirstName = row["FirstName"].ToString().Trim();
            userDO.LastName = row["LastName"].ToString().Trim();
            userDO.Age = (int.TryParse(row["Age"].ToString(), out int age) ? age : default(int));
            userDO.Email = row["Email"].ToString().Trim();
            userDO.Subscription = bool.Parse(row["Subscription"].ToString());
            return userDO;
        }

        public static BookmarkDO BookmarkTableRowToDO(DataRow row)
        {
            BookmarkDO bookmarkDO = new BookmarkDO();
            bookmarkDO.BookmarkID = int.Parse(row["BookmarkID"].ToString());
            bookmarkDO.UserID = int.Parse(row["UserID"].ToString());
            bookmarkDO.ReactorID = int.Parse(row["ReactorID"].ToString());
            return bookmarkDO;
        }

        public static LogDO LogTableRowToDO(DataRow row)
        {
            LogDO logDO = new LogDO();
            logDO.LogID = int.Parse(row["LogID"].ToString());
            logDO.Timestamp = row["Timestamp"].ToString().Trim();
            logDO.Message = row["Message"].ToString().Trim();
            return logDO;
        }
    }
}
