using System.ComponentModel.DataAnnotations;

namespace KomisSamochodowy.Models
{
    public class Marka
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Nazwa jest wymagane.")]
        [StringLength(50)]
        public string Nazwa { get; set; }
    }
}