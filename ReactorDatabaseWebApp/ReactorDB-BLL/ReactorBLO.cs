using ReactorDB_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReactorDB_BLL
{
    public class ReactorBLO
    {
        public ReactorBLO(string logPath)
        {
            _logger = new LoggerBLL(logPath);
        }
        private LoggerBLL _logger;

        public float CalculateEfficiency(ReactorBO reactorBO)
        {
            float efficiency = 100 * reactorBO.ElectricPower / reactorBO.ThermalPower;
            return efficiency;
        }

        /// <summary>
        /// Counts thermal and fast reactors in database, and adds to BO
        /// </summary>
        public StatisticsBO SpectrumCount(List<ReactorBO> reactorBOList, StatisticsBO statisticsBO)
        {
            int thermalCount = default(int);
            int fastCount = default(int);
            int totalCount = reactorBOList.Count();

            if (totalCount > 0)
            {
                _logger.LogMessage("Info", "Spectrum Count request", MethodBase.GetCurrentMethod().ToString(),
                                    totalCount + " ReactorBOs found in list. Counting thermal and fast reactors in database.");
                foreach (ReactorBO reactorBO in reactorBOList)
                {
                    if (reactorBO.IsThermal)
                    {
                        thermalCount++;
                    }
                    else
                    {
                        fastCount++;
                    }
                }
                statisticsBO.ThermalCount = thermalCount;
                statisticsBO.FastCount = fastCount;
                statisticsBO.ReactorCount = totalCount;
            }
            else
            {
                _logger.LogMessage("Warning", "Empty list in Spectrum Count", MethodBase.GetCurrentMethod().ToString(),
                                    "No ReactorBOs found in list. Did not count thermal and fast reactors.");
            }
            return statisticsBO;
        }

        public StatisticsBO SpectrumPercentage(StatisticsBO statisticsBO)
        {
            statisticsBO.PercentThermal = (decimal)statisticsBO.ThermalCount / statisticsBO.ReactorCount * (decimal)100;
            statisticsBO.PercentFast = (decimal)statisticsBO.FastCount / statisticsBO.ReactorCount * (decimal)100;
            _logger.LogMessage(statisticsBO.PercentThermal + "% of database reactors operate in thermal spectrum.");
            _logger.LogMessage(statisticsBO.PercentFast + "% of database reactors operate in fast spectrum.");

            return statisticsBO;
        }

        /// <summary>
        /// Counts number of database reactors in each generation
        /// </summary>
        public StatisticsBO GenerationCounter(List<ReactorBO> reactorBOList, StatisticsBO statisticsBO)
        {
            int genIcount = default(int);
            int genIIcount = default(int);
            int genIIIcount = default(int);
            int genIVcount = default(int);
            int genVcount = default(int);
            int noGenCount = default(int);

            foreach (ReactorBO reactorBO in reactorBOList)
            {
                switch (reactorBO.Generation)
                {
                    case "I":
                        genIcount++;
                        break;

                    case "II":
                        genIIcount++;
                        break;

                    case "III":
                        genIIIcount++;
                        break;

                    case "IV":
                        genIVcount++;
                        break;

                    case "V":
                        genVcount++;
                        break;

                    default:
                        noGenCount++;
                        break;
                }
            }
            statisticsBO.NoGenCount = noGenCount;
            statisticsBO.GenIcount = genIcount;
            statisticsBO.GenIIcount = genIIcount;
            statisticsBO.GenIIIcount = genIIIcount;
            statisticsBO.GenIVcount = genIVcount;
            statisticsBO.GenVcount = genVcount;
            return statisticsBO;
        }

        /// <summary>
        /// Calculates percentage of database reactors in each generation
        /// </summary>
        public StatisticsBO GenerationPercentage(int[] genCount, StatisticsBO statisticsBO)
        {
            decimal[] genPercentArray = new decimal[genCount.Length];
            for(int gen = 0; gen < genCount.Length; gen++)
            {
                genPercentArray[gen] = (decimal)genCount[gen] / statisticsBO.ReactorCount * 100;
                if (gen == 0)
                {
                    _logger.LogMessage(genPercentArray[gen] + "% of database reactors have no generation listed.");
                }
                else
                {
                    _logger.LogMessage(genPercentArray[gen] + "% of database reactors in generation " + gen);
                }
            }
            statisticsBO.PercentNoGen = genPercentArray[0];
            statisticsBO.PercentGenI = genPercentArray[1];
            statisticsBO.PercentGenII = genPercentArray[2];
            statisticsBO.PercentGenIII = genPercentArray[3];
            statisticsBO.PercentGenIV = genPercentArray[4];
            statisticsBO.PercentGenV = genPercentArray[5];
            
            return statisticsBO;
        }
    }
}
