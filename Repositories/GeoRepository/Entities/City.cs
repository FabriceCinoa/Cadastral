using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GeoRepository.Entities
{

    [Table("City", Schema = "geo")]
    [PrimaryKey("Uid")]
    public class CityEntity
    {
        [Column("uid")]
        public Guid Uid { get; set; }


        [Column("code_insee")]
        public string CodeInsee { get; set; }

        [Column("city_name")]
        public string CityName { get; set; }

        [Column("postal_code")]
        public string PostalCode { get; set; }

        [Column("label")]
        public string Label { get; set; }

        [Column("complement")]
        public string? Complement { get; set; }

        [Column("Type")]
        public string? Type { get; set; }

        [Column("id_region")]
        public string? IdRegion { get; set; }

        [Column("id_division")]
        public string? IdDivision { get; set; }

        [Column("local_authority_code")]
        public string? LocalAuthorityCode { get; set; }

        [Column("city_friendly_name")]
        public string CityFriendlyName { get; set; }

        [Column("id_canton")]
        public string? IdCanton { get; set; }

        [Column("id_code_insee_parent")]
        public string? IdCodeInseeParent { get; set; }
    }
}
