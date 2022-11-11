using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab3_.ViewModels
{
    public class TrackViewModel
    {
        [DisplayName("#")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DisplayName("Наименование")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DisplayName("Исполнитель")]
        public string Performer { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DisplayName("Жанр")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DisplayName("Дата выхода")]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DisplayName("Длительность")]
        [RegularExpression(@"\d\d:\d\d", ErrorMessage = "Формат: mm:ss")]
        public string Duration { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DisplayName("Рейтинг")]
        [Range(1, 5, ErrorMessage = "От 1 до 5")]
        public int Rating { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}