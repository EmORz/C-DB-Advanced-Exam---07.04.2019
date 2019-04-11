using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Cinema.DataProcessor.ExportDto;
using Newtonsoft.Json;

namespace Cinema.DataProcessor
{
    using System;

    using Data;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var exportTopMovies = context
                .Movies
                .Where(x => x.Rating >= rating && x.Projections.Any(t => t.Tickets.Count >= 1))
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(t => t.Projections.Sum(a => a.Tickets.Sum(pc => pc.Price)))
                .Select(r => new
                {
                    MovieName = r.Title,
                    Rating = r.Rating.ToString("F2"),
                    TotalIncomes = r.Projections.Sum(f => f.Tickets.Sum(d => d.Price)).ToString("F2"),
                    Customers = r.Projections.SelectMany(s => s.Tickets).Select(a =>
                               new
                               {
                                   FirstName = a.Customer.FirstName,
                                   LastName = a.Customer.LastName,
                                   Balance = a.Customer.Balance.ToString("F2")
                               })
                               .OrderByDescending(x => x.Balance)
                               .ThenBy(x => x.FirstName)
                               .ThenBy(x => x.LastName)
                        .ToArray()
                })
                .Take(10)
                .ToArray()
                ;

            var json = JsonConvert.SerializeObject(exportTopMovies, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var exportResult =
                 context
                .Customers
                .Where(ag => ag.Age >= age)
                     .OrderByDescending(x => x.Tickets.Sum(s => s.Price))
                     .Take(10)
                .Select(c => new ExportCustomersDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(x => x.Price).ToString("F2"),
                    SpentTime = TimeSpan.FromSeconds(c.Tickets.Sum(x => x.Projection.Movie.Duration.TotalSeconds)).ToString(@"hh\:mm\:ss")
                })
                     .ToArray();
            /*Take first 10 records and order the result by spent money in descending order.*/

            var serializer = new XmlSerializer(typeof(ExportCustomersDto[]), new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            serializer.Serialize(new StringWriter(sb), exportResult, namespaces);

            var result = sb.ToString();
            return result;

        }
    }
}