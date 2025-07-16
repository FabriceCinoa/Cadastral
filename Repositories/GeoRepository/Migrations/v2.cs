
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoRepository.DBContext.Migration;

    internal class v2 : IDBMigation
{
        public string VersionType => $"Update v{m_Version}";
        public int   m_Version =>int.Parse( $"{nameof(v2).Replace("v","")}");

        public bool Update(GeoPostGreDBContext context)
        {
            try
            {
            BdVersionEntity e = new BdVersionEntity()
                {
                    Date = DateTime.Now.ToUniversalTime(),
                    @Type = VersionType,
                    Version = this.m_Version
                };

            context.Database.ExecuteSql(@$"CREATE EXTENSION IF NOT EXISTS postgis;
                                CREATE SCHEMA IF NOT EXISTS geo;

                                CREATE TABLE geo.""Zone"" (
                                    uid UUID PRIMARY KEY,
                                    gid INTEGER NOT NULL,
                                    gpu_docId TEXT,
                                    gpu_timestamp TIMESTAMP WITHOUT TIME ZONE,
                                    code_insee TEXT,
                                    zone TEXT,
                                    ""zone-label"" TEXT,
                                    ""type-zone"" TEXT,
                                    geometry geometry(MULTIPOLYGON, 4326)  
                                );

                                CREATE INDEX ON geo.""Zone"" USING gist(""geometry"");

	                            CREATE INDEX zone_docid_idx ON geo.""Zone"" (gpu_docid) WITH (deduplicate_items = off);
");
        

            context.Add(e);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
