using System.ComponentModel.DataAnnotations;

namespace Keeltekooli.Models
{
    public class EmailViewModel
    {
        public int Id { get; set; }

        public string To { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}

