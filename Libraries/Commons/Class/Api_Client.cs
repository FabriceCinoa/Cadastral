using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

// Options de configuration du client
public class ApiClientOptions
{
    public bool EnableGzipCompression { get; set; } = false;
    public bool EnableGzipDecompression { get; set; } = false;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public string UserAgent { get; set; } = "ApiClient/1.0";
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;
}

// Réponse API avec métadonnées
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Error { get; set; }
    public int StatusCode { get; set; }
    public bool WasCompressed { get; set; }
    public double CompressionRatio { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public long OriginalSize { get; set; }
    public long CompressedSize { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}

// Client API flexible avec support Gzip optionnel
public class ApiClient
{
    private readonly HttpClient _httpClient;

    private readonly ApiClientOptions _options;



    public ApiClient(  ApiClientOptions options = null)
    {
        _httpClient =  new HttpClient();        
        _options = options ?? new ApiClientOptions();
        
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.Timeout = _options.Timeout;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", _options.UserAgent);
        
        // Configurer les en-têtes de compression selon les options
        if (_options.EnableGzipDecompression)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        }
    }

    // GET avec support Gzip optionnel
    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, bool? useGzip = null) where T : class
    {
        var useGzipCompression = useGzip ?? _options.EnableGzipCompression;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {

            
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            
            // Ajouter les en-têtes de compression si nécessaire
            if (useGzipCompression && _options.EnableGzipDecompression)
            {
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
            }
            
            var response = await _httpClient.SendAsync(request);
            stopwatch.Stop();
            
            return await ProcessResponse<T>(response, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
        
            stopwatch.Stop();
            
            return new ApiResponse<T>
            {
                Success = false,
                Error = ex.Message,
                ResponseTime = stopwatch.Elapsed
            };
        }
    }

    // POST avec support Gzip optionnel
    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, bool? useGzip = null) where T : class
    {
        var useGzipCompression = useGzip ?? _options.EnableGzipCompression;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            
            var jsonContent = JsonSerializer.Serialize(data);
            var originalSize = Encoding.UTF8.GetByteCount(jsonContent);
            
            HttpContent content;
            long compressedSize = originalSize;
            
            if (useGzipCompression)
            {
                var compressedData = await CompressStringAsync(jsonContent);
                compressedSize = compressedData.Length;
                
                content = new ByteArrayContent(compressedData);
                content.Headers.Add("Content-Type", "application/json");
                content.Headers.Add("Content-Encoding", "gzip");
                
             
            }
            else
            {
                content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }
            
            var response = await _httpClient.PostAsync(endpoint, content);
            stopwatch.Stop();
            
            var result = await ProcessResponse<T>(response, stopwatch.Elapsed);
            result.OriginalSize = originalSize;
            result.CompressedSize = compressedSize;
            
            return result;
        }
        catch (Exception ex)
        {
           
            stopwatch.Stop();
            
            return new ApiResponse<T>
            {
                Success = false,
                Error = ex.Message,
                ResponseTime = stopwatch.Elapsed
            };
        }
    }

    // PUT avec support Gzip optionnel
    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, bool? useGzip = null) where T : class
    {
        var useGzipCompression = useGzip ?? _options.EnableGzipCompression;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
          
            
            var jsonContent = JsonSerializer.Serialize(data);
            HttpContent content;
            
            if (useGzipCompression)
            {
                var compressedData = await CompressStringAsync(jsonContent);
                content = new ByteArrayContent(compressedData);
                content.Headers.Add("Content-Type", "application/json");
                content.Headers.Add("Content-Encoding", "gzip");
            }
            else
            {
                content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }
            
            var response = await _httpClient.PutAsync(endpoint, content);
            stopwatch.Stop();
            
            return await ProcessResponse<T>(response, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
         
            stopwatch.Stop();
            
            return new ApiResponse<T>
            {
                Success = false,
                Error = ex.Message,
                ResponseTime = stopwatch.Elapsed
            };
        }
    }

    // DELETE simple
    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
          
            
            var response = await _httpClient.DeleteAsync(endpoint);
            stopwatch.Stop();
            
            var result = await ProcessResponse<bool>(response, stopwatch.Elapsed);
            if (result.Success)
            {
                result.Data = true;
            }
            
            return result;
        }
        catch (Exception ex)
        {
     
            stopwatch.Stop();
            
            return new ApiResponse<bool>
            {
                Success = false,
                Error = ex.Message,
                ResponseTime = stopwatch.Elapsed
            };
        }
    }

    // Appel avec retry automatique
    public async Task<ApiResponse<T>> GetWithRetryAsync<T>(string endpoint, bool? useGzip = null) where T : class
    {
        var useGzipCompression = useGzip ?? _options.EnableGzipCompression;
        
        for (int attempt = 0; attempt <= _options.MaxRetries; attempt++)
        {
            try
            {
               
                
                var result = await GetAsync<T>(endpoint, useGzipCompression);
                
                if (result.Success || attempt >= _options.MaxRetries)
                {
                    return result;
                }
                
                if (IsRetryableStatus(result.StatusCode))
                {
               
                    await Task.Delay(_options.RetryDelay);
                    continue;
                }
                
                return result;
            }
            catch (Exception ex) when (attempt < _options.MaxRetries)
            {
            
                await Task.Delay(_options.RetryDelay);
            }
        }
        
        return new ApiResponse<T>
        {
            Success = false,
            Error = "Nombre maximum de tentatives atteint"
        };
    }

    // Traiter la réponse HTTP
    private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response, TimeSpan responseTime)
    {
        try
        {
            var headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value));
            var contentHeaders = response.Content.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value));
            
            // Fusionner les headers
            foreach (var header in contentHeaders)
            {
                headers[header.Key] = header.Value;
            }
            
            if (response.IsSuccessStatusCode)
            {
                var content = await DecompressResponseAsync(response);
                var wasCompressed = IsResponseCompressed(response);
                
                T data = default(T);
                if (typeof(T) == typeof(string))
                {
                    data = (T)(object)content;
                }
                else if (typeof(T) == typeof(bool))
                {
                    data = (T)(object)true;
                }
                else if (!string.IsNullOrEmpty(content))
                {
                    data = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                
                return new ApiResponse<T>
                {
                    Success = true,
                    Data = data,
                    StatusCode = (int)response.StatusCode,
                    WasCompressed = wasCompressed,
                    CompressionRatio = GetCompressionRatio(response),
                    ResponseTime = responseTime,
                    Headers = headers
                };
            }
            else
            {
                var errorContent = await DecompressResponseAsync(response);
                return new ApiResponse<T>
                {
                    Success = false,
                    Error = $"HTTP {response.StatusCode}: {response.ReasonPhrase}. {errorContent}",
                    StatusCode = (int)response.StatusCode,
                    ResponseTime = responseTime,
                    Headers = headers
                };
            }
        }
        catch (Exception ex)
        {
       
            return new ApiResponse<T>
            {
                Success = false,
                Error = ex.Message,
                ResponseTime = responseTime
            };
        }
    }

    // Compression d'une chaîne
    private async Task<byte[]> CompressStringAsync(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, _options.CompressionLevel))
        {
            await gzipStream.WriteAsync(bytes, 0, bytes.Length);
        }
        
        return memoryStream.ToArray();
    }

    // Décompression de la réponse
    private async Task<string> DecompressResponseAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        
        if (IsResponseCompressed(response))
        {
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
        else
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }

    // Vérifier si la réponse est compressée
    private bool IsResponseCompressed(HttpResponseMessage response)
    {
        return response.Content.Headers.ContentEncoding.Any(encoding => 
            encoding.Equals("gzip", StringComparison.OrdinalIgnoreCase) ||
            encoding.Equals("deflate", StringComparison.OrdinalIgnoreCase));
    }

    // Calculer le ratio de compression
    private double GetCompressionRatio(HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentLength.HasValue && 
            response.Headers.Contains("X-Original-Size"))
        {
            var originalSize = long.Parse(response.Headers.GetValues("X-Original-Size").First());
            var compressedSize = response.Content.Headers.ContentLength.Value;
            return (double)compressedSize / originalSize;
        }
        
        return 1.0; // Pas de compression détectée
    }

    // Vérifier si le statut HTTP est réessayable
    private bool IsRetryableStatus(int statusCode)
    {
        return statusCode >= 500 || statusCode == 408 || statusCode == 429;
    }
}

// Configuration dans Program.cs
/*
builder.Services.AddHttpClient<ApiClient>();
builder.Services.AddSingleton(new ApiClientOptions
{
    EnableGzipCompression = true,
    EnableGzipDecompression = true,
    MaxRetries = 3,
    RetryDelay = TimeSpan.FromSeconds(2),
    Timeout = TimeSpan.FromSeconds(30),
    CompressionLevel = CompressionLevel.Optimal
});
builder.Services.AddScoped<ApiClient>();
*/

