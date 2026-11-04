using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomisSamochodowy.Models
{
    public class Transakcja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataTransakcji { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Kwota { get; set; }

        [Required]
        public string SprzedawcaId { get; set; }

        [Required]
        public string KlientId { get; set; }

        [Required]
        public int SamochodId { get; set; }

        [ForeignKey("SamochodId")]
        public Samochod? Samochod { get; set; }
    }
}