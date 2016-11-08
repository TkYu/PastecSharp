using System;
using System.Collections.Generic;
#if NET20 || NET40
using System.Net;
#else
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
#endif
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PastecSharp
{
    public partial class Pastec
    {
        internal string Get(string url)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    string value;
#if NET20 || NET40
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = Timeout;
                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    var rs = httpWebResponse.GetResponseStream();
                    if (rs == null) throw new Exception("return null");
                    using (var streamReader = new StreamReader(rs))
                    {
                        value = streamReader.ReadToEnd();
                    }
                }
#else
                    using (var hc = new HttpClient())
                    {
                        hc.Timeout = TimeSpan.FromMilliseconds(Timeout);
                        var t = hc.GetByteArrayAsync(url);
                        t.Wait(Timeout);
                        if (!t.IsCompleted)
                            throw new TimeoutException("request timeout");
                        var data = t.Result;
                        value = Encoding.UTF8.GetString(data);
                    }
#endif
                    return value;
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }
        internal string Post(string url, params KeyValuePair<string, string>[] kv)
        {
#if NET20 || NET40
            var lst = new Dictionary<string, string>();
            if (kv != null)
            {
                foreach (var keyValuePair in kv)
                    lst.Add(keyValuePair.Key, keyValuePair.Value);
            }
            var requestBody = JsonConvert.SerializeObject(lst);
#else
            var requestBody = JsonConvert.SerializeObject(kv.ToDictionary(k => k.Key, v => v.Value));
#endif
            return Post(url, requestBody);
        }
        internal string Post(string url, string requestBody)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    string value;
#if NET20 || NET40
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = Timeout;
                var btBodys = Encoding.UTF8.GetBytes(requestBody);
                httpWebRequest.ContentLength = btBodys.Length;
                httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);
                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    var rs = httpWebResponse.GetResponseStream();
                    if (rs == null) throw new Exception("return null");
                    using (var streamReader = new StreamReader(rs))
                    {
                        value = streamReader.ReadToEnd();
                    }
                }
#else
                    using (var hc = new HttpClient())
                    {
                        var t1 = hc.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));
                        t1.Wait(Timeout);
                        if (!t1.IsCompleted)
                            throw new TimeoutException("request timeout");
                        var data = t1.Result;
                        var t2 = data.Content.ReadAsByteArrayAsync();
                        t2.Wait(Timeout);
                        if (!t2.IsCompleted)
                            throw new TimeoutException("request timeout");
                        value = Encoding.UTF8.GetString(t2.Result);
                    }
#endif
                    return value;
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

        internal string Post(string url, byte[] requestBody)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    string value;
#if NET20 || NET40
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = Timeout;
                httpWebRequest.ContentLength = requestBody.Length;
                httpWebRequest.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    var rs = httpWebResponse.GetResponseStream();
                    if (rs == null) throw new Exception("return null");
                    using (var streamReader = new StreamReader(rs))
                    {
                        value = streamReader.ReadToEnd();
                    }
                }
#else
                    using (var hc = new HttpClient())
                    {
                        var t1 = hc.PostAsync(url, new ByteArrayContent(requestBody));
                        t1.Wait(Timeout);
                        if (!t1.IsCompleted)
                            throw new TimeoutException("request timeout");
                        var data = t1.Result;
                        var t2 = data.Content.ReadAsByteArrayAsync();
                        t2.Wait(Timeout);
                        if (!t2.IsCompleted)
                            throw new TimeoutException("request timeout");
                        value = Encoding.UTF8.GetString(t2.Result);
                    }
#endif
                    return value;
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

        internal string Put(string url, byte[] btBodys)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    string value;
#if NET20 || NET40
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "PUT";
                    httpWebRequest.ContentLength = btBodys.Length;
                    httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);
                    using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        var rs = httpWebResponse.GetResponseStream();
                        if (rs == null) throw new Exception("return null");
                        using (var streamReader = new StreamReader(rs))
                        {
                            value = streamReader.ReadToEnd();
                        }
                    }
#else
                    using (var hc = new HttpClient())
                    {
                        var t1 = hc.PutAsync(url, new ByteArrayContent(btBodys));
                        t1.Wait(Timeout);
                        if (!t1.IsCompleted)
                            throw new TimeoutException("request timeout");
                        var data = t1.Result;
                        var t2 = data.Content.ReadAsByteArrayAsync();
                        t2.Wait(Timeout);
                        if (!t2.IsCompleted)
                            throw new TimeoutException("request timeout");
                        value = Encoding.UTF8.GetString(t2.Result);
                    }
#endif
                    return value;
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

        internal string Delete(string url)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    string value;
#if NET20 || NET40
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "DELETE";
                    using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        var rs = httpWebResponse.GetResponseStream();
                        if (rs == null) throw new Exception("return null");
                        using (var streamReader = new StreamReader(rs))
                        {
                            value = streamReader.ReadToEnd();
                        }
                    }
#else
                    using (var hc = new HttpClient())
                    {
                        var t1 = hc.DeleteAsync(url);
                        t1.Wait(Timeout);
                        if (!t1.IsCompleted)
                            throw new TimeoutException("request timeout");
                        var data = t1.Result;
                        var t2 = data.Content.ReadAsByteArrayAsync();
                        t2.Wait(Timeout);
                        if (!t2.IsCompleted)
                            throw new TimeoutException("request timeout");
                        value = Encoding.UTF8.GetString(t2.Result);
                    }
#endif
                    return value;
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

#if !NET20 && !NET40
        internal async Task<string> GetAsync(string url)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    using (var hc = new HttpClient())
                    {
                        hc.Timeout = TimeSpan.FromMilliseconds(Timeout);
                        var t = await hc.GetByteArrayAsync(url);
                        return Encoding.UTF8.GetString(t);
                    }
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

        internal Task<string> PostAsync(string url, params KeyValuePair<string, string>[] kv)
        {
            var requestBody = JsonConvert.SerializeObject(kv.ToDictionary(k => k.Key, v => v.Value));
            return PostAsync(url, requestBody);
        }

        internal async Task<string> PostAsync(string url, string requestBody)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    using (var hc = new HttpClient())
                    {
                        var t = await hc.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));
                        var data = await t.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(data);
                    }
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

        internal async Task<string> PostAsync(string url, byte[] requestBody)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    using (var hc = new HttpClient())
                    {
                        var t = await hc.PostAsync(url, new ByteArrayContent(requestBody));
                        var data = await t.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(data);
                    }
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

        internal async Task<string> PutAsync(string url, byte[] requestBody)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    using (var hc = new HttpClient())
                    {
                        var t = await hc.PutAsync(url, new ByteArrayContent(requestBody));
                        var data = await t.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(data);
                    }
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }

        internal async Task<string> DeleteAsync(string url)
        {
            for (int i = 1; i <= RetryTimes; i++)
            {
                try
                {
                    using (var hc = new HttpClient())
                    {
                        var t = await hc.DeleteAsync(url);
                        var data = await t.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(data);
                    }
                }
                catch (Exception)
                {
                    if (i == RetryTimes)
                        throw;
                }
            }
            return null;
        }
#endif
    }
}
