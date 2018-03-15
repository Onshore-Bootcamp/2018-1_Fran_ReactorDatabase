using ReactorDB_BLL.Models;
using System;
using System.Reflection;

namespace ReactorDB_BLL
{
    public class ModeratorBLO
    {
        public ModeratorBLO(string logPath)
        {
            _logger = new LoggerBLL(logPath);
        }

        private LoggerBLL _logger;

        public double CalculateEnergyDecrement(int atomicMass)
        {
            double energyDecrement;
            try
            {
                _logger.LogMessage("Info", "Calculate Energy Decrement request", MethodBase.GetCurrentMethod().ToString(),
                                "Attempting to calculate Energy Decrement " +
                                "for atomic mass of " + atomicMass);
                
                energyDecrement = 1D + (Math.Pow((atomicMass - 1D), 2D) / (2D * atomicMass)) * Math.Log((atomicMass - 1D) / (atomicMass + 1D));
                _logger.LogMessage("Energy Decrement calculation completed.");
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
                throw ex;
            }
            finally { }
            return energyDecrement;
        }

        public int CalculateCollisions(float energyDecrement)
        {
            int collisions;
            int e0 = 2000000; //Average initial fission neutron energy in electron-volts
            int e1 = 1; //Desired final, thermal neutron energy in electron-volts

            try
            {
                _logger.LogMessage("Info", "Calculate Collisions request", MethodBase.GetCurrentMethod().ToString(),
                                    "Attempting to calculate number of collisions " +
                                    "for energy decrement of " + energyDecrement);
                collisions = Convert.ToInt32((float)(Math.Log(e0) - Math.Log(e1)) / energyDecrement);
                _logger.LogMessage("Collision calculation completed.");

            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
                throw ex;
            }
            finally { }
            return collisions;
        }

        public float CalculateModerationEfficiency(float energyDecrement, float scattering, float absorption)
        {
            float efficiency;
            try
            {
                _logger.LogMessage("Info", "Calculate Efficiency request", MethodBase.GetCurrentMethod().ToString(),
                                    "Attempting to calculate Moderation Efficiency.");
                efficiency = (energyDecrement * scattering / absorption);
                _logger.LogMessage("Efficiency calculation completed.");
            }
            catch (Exception ex)
            {
                _logger.LogMessage(ex, "Fatal");
                throw ex;
            }
            finally { }
            return efficiency;
        }
    }
}
