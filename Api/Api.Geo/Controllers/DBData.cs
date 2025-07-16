using Api.Geo.Models;
using Common.Library;
using Common.Repository.Interfaces;
using GeoRepository.Entities;
using GeoRepository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Geo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatasController : ControllerBase
    {
        private readonly GeometryFactory _geometryFactory;

        public  IConfiguration _configuration { get; init ; }
        public IGeoRepository _repository { get; init; }
        public ILogger _logger { get; }

        public DatasController(IConfiguration configuration, IGeoRepository repository, GeometryFactory geometryFactory, ILogger logger)
        {
            _configuration = configuration;
            _repository = repository;
            _geometryFactory = geometryFactory;
            _logger = logger;
        }

        [HttpPost("zones/test")]
        public async Task<IActionResult> test()
        {
            this._logger.Log(LogLevel.Debug, "Test Called");
            return Ok("test");
        }

        [HttpPost("zones/add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadGeoJson(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Aucun fichier fourni");
            }

            if (!file.FileName.EndsWith(".geojson", StringComparison.OrdinalIgnoreCase) &&
                !file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Le fichier doit être au format GeoJSON (.geojson ou .json)");
            }

            try
            {
                string geoJsonContent;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    geoJsonContent = await reader.ReadToEndAsync();
                }

                var geoJsonObject = JObject.Parse(geoJsonContent);

                if (geoJsonObject["type"]?.ToString() != "FeatureCollection")
                {
                    return BadRequest("Le fichier doit contenir une FeatureCollection");
                }

                var features = geoJsonObject["features"] as JArray;
                if (features == null)
                {
                    return BadRequest("Aucune feature trouvée dans le fichier");
                }

                var savedFeatures = new FeatureCollection();
                var geoJsonReader = new GeoJsonReader(_geometryFactory,new JsonSerializerSettings { });

                foreach (var feature in features)
                {
                    try
                    {
                       var geoFeature = await ProcessFeature(feature, geoJsonReader);
                       if (geoFeature != null)
                        {
                            var zone = JsonConvert.DeserializeObject<Zone>(geoFeature.Properties);
                            zone.CodeInsee = zone.ZoneCode.Split("_").Last();
                     //      JsonConvert.DeserializeObject<Zone>(geoFeature.Properties,) 

                            zone.Geometry = geoFeature.Geometry;
                            var ret = this._repository.CreateZone(zone);
                            if( ret == null)
                            {
                                throw new Exception("Error on inserting Zone");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Erreur lors du traitement d'une feature: {ex.Message}");
                        continue; // Continue avec la feature suivante
                    }
                }

                //await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Features importées avec succès",
                    TotalFeatures = features.Count,
                  
                });
            }
            catch (JsonException)
            {
                return BadRequest("Format JSON invalide");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement du fichier GeoJSON");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<GeoFeature?> ProcessFeature(JToken feature, GeoJsonReader geoJsonReader)
        {
            var geometry = feature["geometry"];
            var properties = feature["properties"] as JObject;

            if (geometry == null)
            {
                return null;
            }

            // Conversion de la géométrie
            var geometryObj = geoJsonReader.Read<NetTopologySuite.Geometries.Geometry>(geometry.ToString());

            if (geometryObj == null)
            {
                return null;
            }

            // Extraction des propriétés
            var name = properties?["name"]?.ToString() ??
                      properties?["Name"]?.ToString() ??
                      $"Feature_{Guid.NewGuid().ToString()[..8]}";

            var description = properties?["description"]?.ToString() ??
                             properties?["Description"]?.ToString() ??
                             string.Empty;

            var geoFeature = new GeoFeature
            {

                Geometry = geometryObj,
                Properties = properties.ToString()
            }; 

            return geoFeature;
        }
    }
}
