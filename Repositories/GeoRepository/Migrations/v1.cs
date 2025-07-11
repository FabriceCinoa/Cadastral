
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoRepository.DBContext.Migration;

    internal class v1 : IDBMigation
{
        public string VersionType => $"Update v{m_Version}";
        public int   m_Version =>int.Parse( $"{nameof(v1).Replace("v","")}");

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

            context.Database.ExecuteSql($"CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            context.Database.ExecuteSql($" CREATE EXTENSION IF NOT EXISTS fuzzystrmatch;");
            context.Database.ExecuteSql($"CREATE EXTENSION IF NOT EXISTS unaccent;");

            

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
