using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoRepository.DBContext;

    internal interface IDBMigation
    {

        string VersionType { get; }
        bool Update(GeoPostGreDBContext context);
    }

    internal static class  MigrationInfos {
            internal static int AppVersion =>1;
    }



[Table("BdVersion", Schema = "geo")]
[PrimaryKey("Id")]
internal class BdVersionEntity
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    [Column("version")]
    public int Version { get; set; }


    [Column("type")]
    public string Type { get; set; }

    [Column("date")]
    public DateTimeOffset Date { get; set; }

}
