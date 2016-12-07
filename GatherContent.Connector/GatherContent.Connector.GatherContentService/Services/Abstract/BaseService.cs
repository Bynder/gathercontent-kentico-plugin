namespace GatherContent.Connector.GatherContentService.Services.Abstract
{
  using System;
  using System.IO;
  using System.Net;
  using System.Text;

  using GatherContent.Connector.Entities;

  using Newtonsoft.Json;

  public abstract class BaseService
  {
    private static string _apiKey;

    private static string _apiUrl;

    private static string _userName;

    protected BaseService(GCAccountSettings accountSettings)
    {
      _apiUrl = accountSettings.ApiUrl;
      _apiKey = accountSettings.ApiKey;
      _userName = accountSettings.Username;
    }

    protected virtual string ServiceUrl
    {
      get
      {
        return string.Empty;
      }
    }

    protected static void AddPostData(string data, WebRequest webrequest)
    {
      var byteArray = Encoding.UTF8.GetBytes(data);
      webrequest.ContentType = "application/x-www-form-urlencoded";
      webrequest.ContentLength = byteArray.Length;

      var dataStream = webrequest.GetRequestStream();
      dataStream.Write(byteArray, 0, byteArray.Length);
      dataStream.Close();
    }

    protected static WebRequest CreateRequest(string url)
    {
      if (!_apiUrl.EndsWith("/"))
      {
        _apiUrl = _apiUrl + "/";
      }

      HttpWebRequest webrequest = WebRequest.Create(_apiUrl + url) as HttpWebRequest;

      if (webrequest != null)
      {
        string token = GetBasicAuthToken(_userName, _apiKey);
        webrequest.Accept = "application/vnd.gathercontent.v0.5+json";
        webrequest.Headers.Add("Authorization", "Basic " + token);

        return webrequest;
      }

      return null;
    }

    protected static string ReadResponse(WebRequest webrequest)
    {
      using (var responseStream = webrequest.GetResponse().GetResponseStream())
      {
        if (responseStream == null)
        {
          return null;
        }

        using (var responseReader = new StreamReader(responseStream))
        {
          return responseReader.ReadToEnd();
        }
      }
    }

    protected static T ReadResponse<T>(WebRequest webrequest) where T : class
    {
      T result = null;
      using (var responseStream = webrequest.GetResponse().GetResponseStream())
      {
        if (responseStream == null)
        {
          return result;
        }

        using (var responseReader = new StreamReader(responseStream))
        {
          var json = responseReader.ReadToEnd();
          result = JsonConvert.DeserializeObject<T>(json);
        }
      }

      return result;
    }

    private static string Base64Encode(string s)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(s);
      return Convert.ToBase64String(bytes);
    }

    private static string GetBasicAuthToken(string userName, string apiKey)
    {
      string tokenStr = string.Format("{0}:{1}", userName, apiKey);
      return Base64Encode(tokenStr);
    }
  }
}