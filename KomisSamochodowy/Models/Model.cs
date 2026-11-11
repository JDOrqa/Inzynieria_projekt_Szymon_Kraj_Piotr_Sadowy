using System.ComponentModel.DataAnnotations;

namespace KomisSamochodowy.Models
{
    public class Model
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Nazwa jest wymagane.")]
        [StringLength(50)]
        public string Nazwa { get; set; }

        [Required]
        public int MarkaId { get; set; }
        public Marka? Marka { get; set; }
    }
}