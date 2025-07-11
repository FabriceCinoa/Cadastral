using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Common.Library.DataServiceApi;

public class Payload<T>
{
    public T Data { get; set; }
}


public class Response<T>
{
    public T Data { get; set; }
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public Dictionary<string, object> Meta { get; } = new Dictionary<string, object>() { };

    public void AddData<T>(string key, T value){

        if (this.Meta.ContainsKey(key))
            return;
        this.Meta.Add(key, value);

    }

    public void SetError(object error, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        this.StatusCode = statusCode;
        this.Meta.Clear();
        this.Meta.Add("error", JsonSerializer.Serialize(error));
    }

}


