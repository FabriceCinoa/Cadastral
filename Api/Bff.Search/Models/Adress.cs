using Api.Geo.Models;
using System.Drawing;

namespace Bff.Search.Models
{
    public class Adress
    {
        public City City  { get; set; }

        public LatLong Position { get; set; }
        public string Name { get; set; }

    }

    public struct  LatLong 
    {
        public LatLong(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
    }
}
