using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keeltekooli.Models
{
    public class RegistreerimineViewModel
    {
        public int Id { get; set; }

        public int KoolitusId { get; set; }
        public virtual Koolitus Koolitus { get; set; }

        [Required]
        public string Nimi { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Staatus { get; set; }
    }
}