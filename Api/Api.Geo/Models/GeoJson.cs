namespace Api.Geo.Models;

public class Geometry
{
    public string Type { get; set; }
    public List<double> Coordinates { get; set; }
}

public class Properties
{
    public string Label { get; set; }
    public double Score { get; set; }
    public string Housenumber { get; set; }
    public string Id { get; set; }
    public string BanId { get; set; }
    public string Name { get; set; }
    public string Postcode { get; set; }
    public string Citycode { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string City { get; set; }
    public string Context { get; set; }
    public string Type { get; set; }
    public double Importance { get; set; }
    public string Street { get; set; }
    public string _Type { get; set; }
}

public class Feature
{
    public string Type { get; set; }
    public Geometry Geometry { get; set; }
    public Properties Properties { get; set; }
}

public class FeatureCollection
{
    public string Type { get; set; }
    public List<Feature> Features { get; set; }
    public string Query { get; set; }
}


