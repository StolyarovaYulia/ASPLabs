using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab3_.Models
{
    public class Translation
    {
        [DisplayName("#")]
        public int Id { get; set; }

        [DisplayName("Время")]
        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"\d\d:\d\d", ErrorMessage = "Формат: hh:mm")]
        public string Time { get; set; }

        [DisplayName("Дата")]
        [Required(ErrorMessage = "Обязательное поле")]
        public DateTime Date { get; set; }

        [DisplayName("Вещатель")]
        [Required(ErrorMessage = "Обязательное поле")]
        public int EmployeeId { get; set; }

        [DisplayName("Трек")]
        [Required(ErrorMessage = "Обязательное поле")]
        public int TrackId { get; set; }


        public Track Track { get; set; }

        public Employee Employee { get; set; }
    }
}
