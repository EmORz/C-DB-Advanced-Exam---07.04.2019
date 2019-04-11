using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class ImportCustomerTicketsDto
    {

        [Required]
        [MinLength(3), MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [Range(12, 110)]
        public int Age { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [XmlArray("Tickets")]
        public TicketsDto[] Tickets { get; set; }
    }
    [XmlType("Ticket")]
    public class TicketsDto
    {
        [Required]
        public int ProjectionId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
/*  <Customer>
    <FirstName>Randi</FirstName>
    <LastName>Ferraraccio</LastName>
    <Age>20</Age>
    <Balance>59.44</Balance>
    <Tickets>
      <Ticket>
        <ProjectionId>1</ProjectionId>
        <Price>7</Price>
      </Ticket>
      <Ticket>
        <ProjectionId>1</ProjectionId>
        <Price>15</Price>
      </Ticket>
      <Ticket>
        <ProjectionId>1</ProjectionId>
        <Price>12.13</Price>
      </Ticket>
      <Ticket>
        <ProjectionId>1</ProjectionId>
        <Price>11</Price>
      </Ticket>
      <Ticket>
        <ProjectionId>1</ProjectionId>
        <Price>9.13</Price>
      </Ticket>
      <Ticket>
        <ProjectionId>1</ProjectionId>
        <Price>9.13</Price>
      </Ticket>
    </Tickets>
  </Customer>
*/
