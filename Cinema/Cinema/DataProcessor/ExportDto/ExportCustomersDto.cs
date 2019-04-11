using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ExportDto
{
    [XmlType("Customer")]
    public class ExportCustomersDto
    {
        [XmlAttribute("FirstName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string FirstName { get; set; }
        
        [XmlAttribute("LastName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string LastName { get; set; }

        [XmlElement("SpentMoney")]
        public string SpentMoney { get; set; }

        [XmlElement("SpentTime")]
        public string SpentTime { get; set; }

    }
}

/*<Customer FirstName="Marjy" LastName="Starbeck">
   <SpentMoney>82.65</SpentMoney>
   <SpentTime>17:04:00</SpentTime>
   </Customer>
   */
