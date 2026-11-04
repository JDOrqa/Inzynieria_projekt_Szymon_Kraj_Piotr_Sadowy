using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KomisSamochodowy.Models.ViewModels
{
    public class UtworzTransakcjeViewModel
    {
        [Required(ErrorMessage = "Wybierz samochód.")]
        [Display(Name = "Samochód")]
        public int SamochodId { get; set; }

        [Required(ErrorMessage = "Wybierz klienta.")]
        [Display(Name = "Klient")]
        public string KlientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj kwotę transakcji.")]
        [Range(0, 10000000, ErrorMessage = "Kwota musi być większa od 0.")]
        [Display(Name = "Kwota końcowa (PLN)")]
        public decimal Kwota { get; set; }

        [Display(Name = "Data transakcji")]
        public DateTime DataTransakcji { get; set; } = DateTime.Now;

       
        public List<Samochod> DostepneSamochody { get; set; } = new List<Samochod>();

       
        public SelectList? Klienci { get; set; }
    }
}