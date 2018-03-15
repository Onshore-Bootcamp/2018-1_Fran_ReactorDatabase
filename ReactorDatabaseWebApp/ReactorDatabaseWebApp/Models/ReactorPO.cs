using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ReactorDatabaseWebApp.Models
{
    public class ReactorPO
    {
        public List<SelectListItem> GenerationDropDown { get; set; }

        public List<SelectListItem> ModeratorDropDown { get; set; }

        public int ReactorID { get; set; }

        [StringLength(25)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Full Name")]
        [StringLength(100)]
        public string FullName { get; set; }

        [DisplayName("Spectrum")]
        [Required]
        public bool IsThermal { get; set; }

        [DisplayName("Moderator")]
        public int ModeratorID { get; set; }

        public string ModeratorName { get; set; }

        [DisplayName("Primary Coolant")]
        [StringLength(60)]
        [Required]
        public string PrimaryCoolant { get; set; }

        [DisplayName("Fuel Type")]
        [StringLength(80)]
        [Required]
        public string Fuel { get; set; }

        [DisplayName("Thermal Power (MWth)")]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$", 
            ErrorMessage = "Integer or decimal values only. Do not input commas or symbols.")]
        [Range(0D, 9999999D, ErrorMessage = "Input was out of range.")]
        public float ThermalPower { get; set; }

        [DisplayName("Electric Power (MWe)")]
        [RegularExpression(@"^[0-9]\d*(\.\d+)?$",
            ErrorMessage = "Integer or decimal values only. Do not input commas or symbols.")]
        [Range(0D, 9999999D, ErrorMessage = "Input was out of range.")]
        public float ElectricPower { get; set; }

        //[DisplayName("Efficiency (%)")]
        //[RegularExpression(@"^[0-9]\d*(\.\d+)?$",
        //    ErrorMessage = "Integer or decimal values only. Do not input commas or symbols.")]
        public float Efficiency { get; set; }

        [DisplayName("Year Designed (yyyy)")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numbers only.")]
        [Range(1, 9999, ErrorMessage = "Input was out of range.")]
        [Required]
        public int Year { get; set; }

        [StringLength(10)]
        public string Generation { get; set; }

        [DisplayName("Country of Origin")]
        [StringLength(50)]
        public string CountryOfOrigin { get; set; }

        [DisplayName("Number in Operation")]
        [Range(0, 99999)]
        public int NumberActive { get; set; }

        [DisplayName("Additional Information")]
        public string Notes { get; set; }
    }
}