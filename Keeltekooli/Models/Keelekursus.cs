using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keeltekooli.Models
{
    public class Keelekursus
    { 
        public int Id { get; set; }

        [Required]
        public string Nimetus { get; set; }

        [Required]
        public string Keel { get; set; }

        [Required]
        public string Tase { get; set; }

        public string Kirjeldus { get; set; }
    }
}