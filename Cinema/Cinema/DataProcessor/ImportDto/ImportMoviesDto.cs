using System;
using System.ComponentModel.DataAnnotations;
using Cinema.Data.Models.Enums;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportMoviesDto
    {


        [Required]
        [MinLength(3), MaxLength(20)]
        public string Title { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        [Range(1.0, 10.0)]
        public double Rating { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Director { get; set; }

        /*       /*    "Title": "Little Big Man",
           "Genre": "Western",
           "Duration": "01:58:00",
           "Rating": 28,
           "Director": "Duffie Abrahamson"
           
           * /*/
    }
}