﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorDB_BLL.Models
{
    public class ReactorBO
    {
        public int ReactorID { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public bool IsThermal { get; set; }

        public int ModeratorID { get; set; }

        public string PrimaryCoolant { get; set; }

        public string Fuel { get; set; }

        public float ThermalPower { get; set; }

        public float ElectricPower { get; set; }

        public float Efficiency { get; set; }

        public int Year { get; set; }

        public string Generation { get; set; }

        public string CountryOfOrigin { get; set; }

        public int NumberActive { get; set; }

        public string Notes { get; set; }
    }
}