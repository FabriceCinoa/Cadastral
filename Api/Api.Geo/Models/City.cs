using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Geo.Models
{
    public class City
    {

        public string CodeInsee { get; set; }

        public string CityName { get; set; }

        public string PostalCode { get; set; }

        public string? Complement { get; set; }


    }
}
