using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Keeltekooli.Models
{
    public class Koolitus
    {
        public int Id { get; set; }

        [Required]
        public int KeelekursusId { get; set; }
        public virtual Keelekursus Keelekursus { get; set; }

        public int OpetajaId { get; set; }
        public virtual Opetaja Opetaja{ get; set; }

        public DateTime AlgusKuupaev { get; set; } = DateTime.Now;
        public DateTime LoppKuupaev { get; set; } = DateTime.Now;
        public float Hind { get; set; }
        public int MaxOsalejaid { get; set; }

        public virtual ICollection<Registreerimine> Registreerimised { get; set; }
    }
}