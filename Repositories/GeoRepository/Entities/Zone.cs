using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoRepository.Entities
{

    [Table("Zone", Schema = "geo")]
    public class Zone
    {
        [Key]
        [Column("uid")]
        public Guid Uid { get; set; } = Guid.NewGuid();

        [Column("gid")]
        [JsonProperty("gid")]
        public int Gid { get; set; }

        [Column("gpu_docid")]
        [JsonProperty("gpu_doc_id")]
        public string GpuDocId { get; set; }

        [Column("gpu_timestamp")]
        [JsonProperty("gpu_timestamp")]
        public DateTime GpuTimestamp { get; set; }

        [Column("code_insee")]
        [JsonProperty("insee")]
        public string CodeInsee { get; set; }

        [Column("zone")]
        [JsonProperty("partition")]
        public string ZoneCode { get; set; }

        [Column("zone-label")]
        [JsonProperty("libelong")]
        public string Libelong { get; set; }

        [Column("type-zone")]
        [JsonProperty("typezone")]
        public string TypeZone { get; set; }

        [Column("geometry")]
        public Geometry Geometry { get; set; }
    }
}