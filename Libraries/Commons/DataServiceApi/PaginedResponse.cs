
using System.Collections;
using System.Linq;
using System.Net;

namespace Common.Library.DataServiceApi;


public static class PaginedResponseFactory
{
    /// <summary>
    /// Génere un objet retour pour une liste
    /// </summary>
    /// <param name="list">Contenu de la liste à retourner</param>
    /// <param name="totalPage">Nombre total de page</param>
    /// <param name="totalItems">Nombre total d'item</param>
    /// <param name="baseUri">Url de base permettant la génération de liens suivant et précedents</param>
    /// <param name="skiped">Nombre d'élément déja sautés</param>
    /// <param name="taked">Nombre d'élément prs</param>
    /// <returns></returns>
    public static Response<IEnumerable<T>> MakeResult<T>(IList<T> list, int totalPage, int? totalItems = null,
        string baseUri = null, int? skiped = null, int? taked = null,
        Func<Dictionary<string, object>> metas = null, bool listIsFull = false, Dictionary<string, string> urlRequestParameters = null) where T : new()
    {
        var ret = new Response<IEnumerable<T>>()
        {
            Data = listIsFull ? list.Skip(skiped ?? 0).Take(taked ?? 50) : list
        };
        var count  = list?.Count ?? 0;
        ret.Meta.Add("count", count);
        ret.Meta.Add("page", ( skiped / taked) +1);
        ret.Meta.Add("totalPages", totalPage);
        if (totalItems != null)
            ret.Meta.Add("totalCount", totalItems);
        if (metas != null)
        {
            var metasDic = metas();
            if (metasDic?.Any() ?? false)
            {
                foreach (var kvp in metasDic)
                {
                    if (ret.Meta.ContainsKey(kvp.Key))
                        continue;
                    ret.Meta.Add(kvp.Key, kvp.Value);
                }
            }
        }

        if (skiped != null && taked != null && list != null && baseUri != null)
        {
            if (taked != int.MaxValue)
            {
                int pageindex = (skiped ?? 0) / (taked ?? 1);
                pageindex += 1;
                int skip = pageindex * taked ?? 0;

                int previousSkip = (skiped ?? 0) - (taked ?? 0);
                var uri = baseUri.TrimEnd('/');
                var startScheme = baseUri.Contains("?") ? '&' : '?';

                if ((skip / taked ?? 1) < totalPage)
                {
                    var request = $"skip={skip}&take={taked}";
                    request = addRequestParameters(urlRequestParameters, request);
                    ret.Meta.Add("next", $"{baseUri}{startScheme}{request}");
                }
                if (previousSkip >= 0)
                {
                    var request = $"skip={previousSkip}&take={taked}";
                    request = addRequestParameters(urlRequestParameters, request);
                    ret.Meta.Add("previous", $"{baseUri}{startScheme}{request}");
                }

            }
        }

        return ret;

        static string addRequestParameters(Dictionary<string, string> urlRequestParameters, string request)
        {
            if (urlRequestParameters != null)
            {
                foreach (var kvp in urlRequestParameters
                    .Where(c=>
                        c.Key.ToLower() != "skip" &&
                         c.Key.ToLower() != "take"))
                {
                    if (kvp.Key != string.Empty && kvp.Value != string.Empty)
                        request += $"&{kvp.Key}={kvp.Value}";
                }

            }

            return request;
        }
    }


}


