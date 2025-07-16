using Api.Geo.Models;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry = Api.Geo.Models.Geometry;
using Point = NetTopologySuite.Geometries.Point;

namespace Api.Geo.Helpers
{
    
    internal static class GeoHelpers
    {

        private static GeometryFactory  _geoFactory = new GeometryFactory();
        public static Geometry GetGeometry(this NetTopologySuite.Geometries.Geometry ntsGeometry)
        {
            if (ntsGeometry == null)
                return null;

            var type = ntsGeometry.GeometryType;



            var _ret  =  type switch
            {
                "Point" => new Geometry
                {
                    Type = "Point",
                    Coordinates = new List<double> { ((Point)ntsGeometry).X, ((Point)ntsGeometry).Y }
                },

                "LineString" => new Geometry
                {
                    Type = "LineString",
                    Coordinates = ((LineString)ntsGeometry)
                        .Coordinates
                        .Select(c => new List<double> { c.X, c.Y })
                        .ToList()
                },

                "Polygon" => new Geometry
                {
                    Type = "Polygon",
                    Coordinates = new List<List<List<double>>>(
                        ((Polygon)ntsGeometry).Coordinates
                            .GroupBy(c => c.Z) // dummy group to keep exterior ring together
                            .Select(group => group
                                .Select(c => new List<double> { c.X, c.Y })
                                .ToList()
                            )
                    )
                },

                "MultiPoint" => new Geometry
                {
                    Type = "MultiPoint",
                    Coordinates = ((MultiPoint)ntsGeometry)
                        .Geometries
                        .Select(g => new List<double> { ((Point)g).X, ((Point)g).Y })
                        .ToList()
                },

                "MultiLineString" => new Geometry
                {
                    Type = "MultiLineString",
                    Coordinates = ((MultiLineString)ntsGeometry)
                        .Geometries
                        .Select(line => ((LineString)line)
                            .Coordinates
                            .Select(c => new List<double> { c.X, c.Y })
                            .ToList()
                        ).ToList()
                },

                "MultiPolygon" => new Geometry
                {
                    Type = "MultiPolygon",
                    Coordinates = ((MultiPolygon)ntsGeometry)
                        .Geometries
                        .Select(polygon => ((Polygon)polygon)
                            .Coordinates
                            .GroupBy(c => c.Z) // same as above, dummy group
                            .Select(group => group
                                .Select(c => new List<double> { c.X, c.Y })
                                .ToList()
                            ).ToList()
                        ).ToList()
                },

                _ => throw new NotSupportedException($"Geometry type '{type}' not supported.")
            };

            

            _ret.Type = ntsGeometry.GeometryType;
            return _ret;
        }

        public static double GetSurfaceMeterPerSquare(this NetTopologySuite.Geometries.Geometry geometry)
        {
            if (geometry == null || !geometry.IsValid)           
                   return 0.0;
            double surfaceM2 = CalculerSurfaceGeographiqueEnMetres(geometry.Coordinates);


            return surfaceM2;



        }

        static double CalculerSurfaceGeographiqueEnMetres(Coordinate[] coordinates)
        {
            if (coordinates.Length < 4) return 0.0;

            double surface = 0.0;
            double earthRadius = 6378137.0; // Rayon de la Terre en mètres (WGS84)

            // Conversion en radians et calcul
            for (int i = 0; i < coordinates.Length - 1; i++)
            {
                double lat1 = coordinates[i].Y * Math.PI / 180.0;
                double lon1 = coordinates[i].X * Math.PI / 180.0;
                double lat2 = coordinates[i + 1].Y * Math.PI / 180.0;
                double lon2 = coordinates[i + 1].X * Math.PI / 180.0;

                surface += (lon2 - lon1) * (2 + Math.Sin(lat1) + Math.Sin(lat2));
            }

            surface = Math.Abs(surface) * earthRadius * earthRadius / 2.0;
            return surface/1000;
        }


        static double CalculerSurfaceUTM(GeometryFactory factory, NetTopologySuite.Geometries.Geometry geometry)
        {
           
            var polygon = factory.CreatePolygon(geometry.Coordinates);

            // En UTM, l'unité est le mètre
            return polygon.Area;
            
        }

        /*

        static void CalculerSurfaceLambert93()
        {
            Console.WriteLine("\n=== Calcul surface Lambert 93 (France) ===");

            // Exemple avec des coordonnées Lambert 93 (système métrique français)
            var factory = new GeometryFactory();
            var coordinatesLambert93 = new Coordinate[]
            {
                new Coordinate(652000, 6862000), // Coordonnées Lambert 93
                new Coordinate(653000, 6862000),
                new Coordinate(653000, 6863000),
                new Coordinate(652000, 6863000),
                new Coordinate(652000, 6862000)
            };

            var polygon = factory.CreatePolygon(coordinatesLambert93);

            // En Lambert 93, l'unité est déjà le mètre
            double surfaceM2 = polygon.Area;
            Console.WriteLine($"Surface Lambert 93: {surfaceM2:F2} m²");
            Console.WriteLine($"Surface en hectares: {surfaceM2 / 10000:F2} ha");
        }

    }*/


    public static class SurfaceConverter
    {
        /// <summary>
        /// Convertit une surface en mètres carrés vers d'autres unités
        /// </summary>
        public static class UnitesSurface
        {
            public static double VersHectares(double metresCarres) => metresCarres / 10000.0;
            public static double VersKilometresCarres(double metresCarres) => metresCarres / 1000000.0;
            public static double VersAres(double metresCarres) => metresCarres / 100.0;
            public static double VersCentimetresCarres(double metresCarres) => metresCarres * 10000.0;
        }

        /// <summary>
        /// Calcule la surface réelle d'un polygone selon son système de coordonnées
        /// </summary>
        public static double CalculerSurfaceReelle(Polygon polygon, string systemeCoordonnes)
        {
            switch (systemeCoordonnes.ToUpper())
            {
                case "WGS84":
                case "EPSG:4326":
                    // Coordonnées géographiques - nécessite une conversion
                    return CalculerSurfaceGeographique(polygon);

                case "LAMBERT93":
                case "EPSG:2154":
                case "UTM":
                    // Systèmes métriques - surface directe
                    return polygon.Area;

                default:
                    throw new ArgumentException($"Système de coordonnées non supporté: {systemeCoordonnes}");
            }
        }

        private static double CalculerSurfaceGeographique(Polygon polygon)
        {
            // Implémentation de la formule de surface géographique
            var coords = polygon.ExteriorRing.Coordinates;
            double surface = 0.0;
            double earthRadius = 6378137.0;

            for (int i = 0; i < coords.Length - 1; i++)
            {
                double lat1 = coords[i].Y * Math.PI / 180.0;
                double lon1 = coords[i].X * Math.PI / 180.0;
                double lat2 = coords[i + 1].Y * Math.PI / 180.0;
                double lon2 = coords[i + 1].X * Math.PI / 180.0;

                surface += (lon2 - lon1) * (2 + Math.Sin(lat1) + Math.Sin(lat2));
            }

            return Math.Abs(surface) * earthRadius * earthRadius / 2.0;
        }
    }
}

}
