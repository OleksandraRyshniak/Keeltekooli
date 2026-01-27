using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keeltekooli.Models
{
    public class OpetajaViewModel
    {
        [Required]
        public string Nimi { get; set; }

        public string Kvalifikatsioon { get; set; }

        public string FotoPath { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}