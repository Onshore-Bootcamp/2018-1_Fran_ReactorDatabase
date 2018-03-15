using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorDB_BLL.Models
{
    public class ModeratorBO
    {
        public int ModeratorID { get; set; }

        public string Name { get; set; }

        public string ChemicalFormula { get; set; }

        public string Nucleus { get; set; }

        public int AtomicMass { get; set; }

        public float EnergyDecrement { get; set; }

        public int Collisions { get; set; }

        public float ScatteringXS { get; set; }

        public float AbsorptionXS { get; set; }

        public float ModerationEfficiency { get; set; }
    }
}
