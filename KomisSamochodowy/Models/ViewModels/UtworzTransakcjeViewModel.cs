using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KomisSamochodowy.Models.ViewModels
{
    public class UtworzTransakcjeViewModel
    {
        [Required(ErrorMessage = "Wybierz samochód.")]
        [Display(Name = "Samochód")]
        public int SamochodId { get; set; }

        // Zmiana: KlientId nie jest już wymagane na sztywno, bo może być nowy klient
        [Display(Name = "Istniejący Klient")]
        public string? KlientId { get; set; }

        [Required(ErrorMessage = "Podaj kwotę transakcji.")]
        [Range(0, 10000000, ErrorMessage = "Kwota musi być większa od 0.")]
        [Display(Name = "Kwota końcowa (PLN)")]
        public decimal Kwota { get; set; }

        [Display(Name = "Data transakcji")]
        public DateTime DataTransakcji { get; set; } = DateTime.Now;

        // NOWE POLA DLA NOWEGO KLIENTA
        [Display(Name = "Nowy klient (utwórz konto)")]
        public bool CzyNowyKlient { get; set; }

        [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
        [Display(Name = "E-mail nowego klienta")]
        public string? NowyKlientEmail { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Hasło musi mieć min. 8 znaków.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło dla nowego klienta")]
        public string? NowyKlientHaslo { get; set; }

        // Listy rozwijane
        public List<Samochod> DostepneSamochody { get; set; } = new List<Samochod>();
        public SelectList? Klienci { get; set; }
    }
}