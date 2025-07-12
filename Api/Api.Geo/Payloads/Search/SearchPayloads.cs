using Api.Geo.Models;
using Common.Library.DataServiceApi;

namespace Api.Geo.Payloads;

    public partial class SearchPayload
    {
        public string  SearchString { get; set; }
        public double Precision { get; set; } = 0.5;

    public short MaxResults { get; set; } = 10;
    }


    public partial class SearchResult: Response<IList<City>>
    {

    }
