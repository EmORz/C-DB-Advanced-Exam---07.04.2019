using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Cinema.Data.Models;
using Cinema.Data.Models.Enums;
using Cinema.DataProcessor.ImportDto;
using Newtonsoft.Json;

namespace Cinema.DataProcessor
{
    using System;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2:f2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var deserializeMovies = JsonConvert.DeserializeObject<ImportMoviesDto[]>(jsonString);

            var movies = new List<Movie>();

            foreach (var dto in deserializeMovies)
            {
                var genreTemp = Enum.TryParse<Genre>(dto.Genre, true, out Genre genre);
                if (IsValid(dto) == false)
                {

                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = new Movie
                {
                    Title = dto.Title,
                    Genre = genre,
                    Duration = dto.Duration,
                    Rating = dto.Rating,
                    Director = dto.Director
                };
                var str = string.Format(SuccessfulImportMovie, dto.Title, dto.Genre, dto.Rating);
                sb.AppendLine(str);
                movies.Add(movie);
            }

            context.Movies.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();


        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {

            var sb = new StringBuilder();
            var deserializeHallsSeats = JsonConvert.DeserializeObject<ImportHallsSeats[]>(jsonString);

            var hallsSeats = new List<Hall>();


            foreach (var hall in deserializeHallsSeats)
            {
                if (IsValid(hall) == false || hall.Seats <= 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var tempHall = new Hall
                {
                    Name = hall.Name,
                    Is4Dx = hall.Is4Dx,
                    Is3D = hall.Is3D,

                };
                for (int i = 0; i < hall.Seats; i++)
                {
                    tempHall.Seats.Add(new Seat());
                }
                hallsSeats.Add(tempHall);

                var tempstr = "";
                if (hall.Is3D == true && hall.Is4Dx == false)
                {
                    tempstr = "3D";
                }

                else if (hall.Is4Dx == true && hall.Is3D == false)
                {
                    tempstr = "4Dx";
                }

                else if (hall.Is3D && hall.Is4Dx)
                {
                    tempstr = "4Dx/3D";
                }
                else
                {
                    tempstr = "Normal";
                }
                var str = string.Format(SuccessfulImportHallSeat, tempHall.Name, tempstr, hall.Seats);
                sb.AppendLine(str);

            }
            context.Halls.AddRange(hallsSeats);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {

            var sb = new StringBuilder();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectionsDto[]), new XmlRootAttribute("Projections"));

            var projectionsDtos = (ImportProjectionsDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var projections = new List<Projection>();


            foreach (var dto in projectionsDtos)
            {
                var movies = context.Movies.FirstOrDefault(x => x.Id == dto.MovieId);
                var hall = context.Halls.FirstOrDefault(x => x.Id == dto.HallId);

                var checkMovieHall = movies == null || hall == null;
                if (IsValid(dto) == false || checkMovieHall == true)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var tempProjection = new Projection
                {
                    MovieId = dto.MovieId,
                    HallId = dto.HallId,
                    DateTime = DateTime.ParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };
                var getTitleOnFilm = context.Movies.FirstOrDefault(f => f.Id == tempProjection.MovieId);

                if (getTitleOnFilm == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var str = string.Format(SuccessfulImportProjection, getTitleOnFilm.Title, tempProjection.DateTime.ToString("MM/dd/yyyy"));
                sb.AppendLine(str);
                projections.Add(tempProjection);
            }

            context.Projections.AddRange(projections);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerTicketsDto[]), new XmlRootAttribute("Customers"));

            var customerDtos = (ImportCustomerTicketsDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var customers = new List<Customer>();
            //
            foreach (var dto in customerDtos)
            {
                if (IsValid(dto) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var tempCustomer = new Customer
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    Balance = dto.Balance
                };

                foreach (var dtoTicket in dto.Tickets)
                {
                    tempCustomer.Tickets.Add(new Ticket
                    {
                        ProjectionId = dtoTicket.ProjectionId,
                        Price = dtoTicket.Price
                    });
                }
                customers.Add(tempCustomer);
                var str = string.Format(SuccessfulImportCustomerTicket, tempCustomer.FirstName, tempCustomer.LastName, tempCustomer.Tickets.Count);
                sb.AppendLine(str);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var result = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, result, true);
        }
    }
}