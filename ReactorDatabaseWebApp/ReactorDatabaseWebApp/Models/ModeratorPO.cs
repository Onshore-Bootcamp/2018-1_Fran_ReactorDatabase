using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReactorDatabaseWebApp.Models
{
    public class ModeratorPO
    {
        public int ModeratorID { get; set; }

        [DisplayName("Moderator Name")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Chemical Formula")]
        [StringLength(20)]
        public string ChemicalFormula { get; set; }

        [DisplayName("Nucleus of Interest")]
        [Required]
        public string Nucleus { get; set; }

        [DisplayName("Atomic mass, A")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numbers only.")]
        [Range(1, 300, ErrorMessage = "Input was out of range.")]
        [Required]
        public int AtomicMass { get; set; }

        public float EnergyDecrement { get; set; }

        public int Collisions { get; set; }

        [DisplayName("Macroscopic Neutron Scattering Cross Section, Σs")]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$",
            ErrorMessage = "Integer or decimal values only. Do not input commas or symbols.")]
        [Range(0D, 100000000D, ErrorMessage = "Input was out of range.")]
        public float ScatteringXS { get; set; }

        [DisplayName("Macroscopic Neutron Absorption Cross Section, Σa")]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$",
            ErrorMessage = "Integer or decimal values only. Do not input commas or symbols.")]
        [Range(0D, 100000000D, ErrorMessage = "Input was out of range.")]
        public float AbsorptionXS { get; set; }

        [DisplayName("Moderation Efficiency")]
        public float ModerationEfficiency { get; set; }
    }
}