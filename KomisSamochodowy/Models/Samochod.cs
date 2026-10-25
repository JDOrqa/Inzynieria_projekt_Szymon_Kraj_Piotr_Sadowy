using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace KomisSamochodowy.Models
{
    public class Samochod
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Marka jest wymagane.")]
        [StringLength(50)]
        public string Marka { get; set; }

        [Required(ErrorMessage = "Pole Model jest wymagane.")]
        [StringLength(50)]
        public string Model { get; set; }

        [Required(ErrorMessage = "Pole Rok produkcji jest wymagane.")]
        [Range(1900, 2100, ErrorMessage = "Podaj poprawny rok produkcji.")]
        public int RokProdukcji { get; set; }

        [Required(ErrorMessage = "Pole Cena jest wymagane.")]
        [Range(0, 10000000, ErrorMessage = "Cena musi być większa lub równa 0.")]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Cena { get; set; }
    }
}