using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomisSamochodowy.Models
{
    public class Rezerwacja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string KlientId { get; set; }

        [Required]
        public int SamochodId { get; set; }

        [ForeignKey("SamochodId")]
        public Samochod? Samochod { get; set; }

        public DateTime DataRezerwacji { get; set; } = DateTime.Now;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Aktywna"; // Aktywna, Anulowana, Zrealizowana
    }
}