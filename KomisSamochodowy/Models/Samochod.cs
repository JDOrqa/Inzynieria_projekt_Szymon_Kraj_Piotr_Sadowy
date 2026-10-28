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

        [Required(ErrorMessage = "Pole Przebieg jest wymagane.")]
        [Range(0, 1000000, ErrorMessage = "Przebieg musi być większy lub równy 0.")]
        public int Przebieg { get; set; }

        [Required(ErrorMessage = "Wybierz rodzaj paliwa.")]
        [StringLength(20)]
        public string Paliwo { get; set; }

        [Required(ErrorMessage = "Podaj adres zdjęcia samochodu.")]
        [Url(ErrorMessage = "Podaj poprawny adres URL zdjęcia.")]
        [StringLength(500)]
        public string ZdjecieUrl { get; set; }

        [Required(ErrorMessage = "Wybierz status samochodu.")]
        [StringLength(30)]
        public string Status { get; set; } = "Dostępny";

        [StringLength(1000)]
        public string? Opis { get; set; }

        [StringLength(2000)]
        public string? GaleriaZdjec { get; set; }
    }
}