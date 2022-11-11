using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab3_.Models
{
    public class Genre
    {
        [DisplayName("#")]
        public int Id { get; set; }

        [DisplayName("Наименование")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; }

        [DisplayName("Описание")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Description { get; set; }


        public List<Track> Tracks { get; set; }
    }
}
