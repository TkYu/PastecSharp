using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PastecSharp
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Pastec
    {
        /// <summary>
        /// 
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// http request timeout
        /// </summary>
        public int Timeout { get; set; } = 5000;

        /// <summary>
        /// retry when http request error
        /// </summary>
        public int RetryTimes { get; set; } = 3;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="host">the Pastec api url</param>
        public Pastec(string host)
        {
            Host = host.TrimEnd('/');
        }

        #region Method

        /// <summary>
        /// Ping Pastec
        /// This call sends a simple PING command to pastec that answers with a PONG.
        /// </summary>
        /// <returns>ElapsedMilliseconds</returns>
        /// <exception cref="Exception"></exception>
        public long Ping()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var ping = Post(Host, "{\"type\":\"PING\"}");
            if(!ping.Contains("PONG"))
                throw new Exception("invalid host");
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Adding an image to the index
        /// </summary>
        /// <param name="data">the binary data of the image to add compressed in JPEG</param>
        /// <param name="image_id">image id</param>
        /// <returns></returns>
        public bool Add(byte[] data,int image_id)
        {
            var url = $"{Host}/index/images/{image_id}";
            var ret = Put(url, data);
            if (ret == null) return false;
            var dec = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
            if (dec.ContainsKey("type"))
            {
                if (dec["type"] == "IMAGE_ADDED")
                    return true;
                else
                    throw new Exception(dec["type"]);
            }
            return false;
        }

        /// <summary>
        /// Adding an image to the index
        /// </summary>
        /// <param name="path">the image path</param>
        /// <param name="image_id">image id</param>
        /// <returns></returns>
        public bool Add(string path, int image_id)
        {
            var data = File.ReadAllBytes(path);
            return Add(data, image_id);
        }

        /// <summary>
        /// Removing an image from the index
        /// This call removes the signature of an image in the index thanks to its id. Be careful to not call often this method if your index is big because it is currently very slow.
        /// </summary>
        /// <param name="image_id">image id</param>
        /// <returns></returns>
        public bool Remove(int image_id)
        {
            var url = $"{Host}/index/images/{image_id}";
            var ret = Delete(url);
            if (ret == null) return false;
            var dec = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
            if (dec.ContainsKey("type"))
            {
                if (dec["type"] == "IMAGE_REMOVED")
                    return true;
                else
                    throw new Exception(dec["type"]);
            }
            return false;
        }

        /// <summary>
        /// Search request
        /// This call performs a search in the index thanks to a request image. It returns the id of the matched images from the most to the least relevant ones.
        /// </summary>
        /// <param name="data">the binary data of the request image compressed in JPEG</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int[] Search(byte[] data)
        {
            var url = $"{Host}/index/searcher";
            var ret = Post(url,data);
            if (ret == null) return null;
            var dec = JsonConvert.DeserializeObject<JObject>(ret);
            if (dec.HasValues)
            {
                if (dec["type"].ToString() == "SEARCH_RESULTS")
                    return dec["image_ids"].ToObject<int[]>();
                else
                    throw new Exception(dec["type"].ToString());
            }
            return null;
        }

        /// <summary>
        /// Search request
        /// This call performs a search in the index thanks to a request image. It returns the id of the matched images from the most to the least relevant ones.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int[] Search(string path)
        {
            var data = File.ReadAllBytes(path);
            return Search(data);
        }

        /// <summary>
        /// Clear an index
        /// This call erases all the data currently contained in the index.
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            var url = $"{Host}/index/io";
            var ret = Post(url, "{\"type\":\"CLEAR\"}");
            if (ret == null) return false;
            var dec = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
            if (dec.ContainsKey("type"))
            {
                if (dec["type"] == "INDEX_CLEARED")
                    return true;
                else
                    throw new Exception(dec["type"]);
            }
            return false;
        }
        /// <summary>
        /// Load an index
        /// This call loads the index data in a provided path.
        /// </summary>
        /// <param name="index_path">the path where to read the index</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Load(string index_path)
        {
            var url = $"{Host}/index/io";
            var ret = Post(url, $"{{\"type\":\"LOAD\", \"index_path\":\"{index_path}\"}}");
            if (ret == null) return false;
            var dec = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
            if (dec.ContainsKey("type"))
            {
                if (dec["type"] == "INDEX_LOADED")
                    return true;
                else
                    throw new Exception(dec["type"]);
            }
            return false;
        }
        /// <summary>
        /// Save an index
        /// This call saves the index data in a specified path.
        /// </summary>
        /// <param name="index_path">the path where to read the index</param>
        public bool Save(string index_path)
        {
            var url = $"{Host}/index/io";
            var ret = Post(url, $"{{\"type\":\"WRITE\", \"index_path\":\"{index_path}\"}}");
            if (ret == null) return false;
            var dec = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
            if (dec.ContainsKey("type"))
            {
                if (dec["type"] == "INDEX_WRITTEN")
                    return true;
                else
                    throw new Exception(dec["type"]);
            }
            return false;
        }
#if !NET20 && !NET40
        /// <summary>
        /// Adding an image to the index
        /// </summary>
        /// <param name="data">the binary data of the image to add compressed in JPEG</param>
        /// <param name="image_id">image id</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> AddAsync(byte[] data, int image_id)
        {
            var url = $"{Host}/index/images/{image_id}";
            var ret = await PutAsync(url, data);
            if (ret == null) return false;
            var dec = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
            if (dec.ContainsKey("type"))
            {
                if (dec["type"] == "IMAGE_ADDED")
                    return true;
                else
                    throw new Exception(dec["type"]);
            }
            return false;
        }

        /// <summary>
        /// Adding an image to the index
        /// </summary>
        /// <param name="path">the image path</param>
        /// <param name="image_id">image id</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<bool> AddAsync(string path, int image_id)
        {
            var data = File.ReadAllBytes(path);
            return AddAsync(data, image_id);
        }

        /// <summary>
        /// Search request
        /// This call performs a search in the index thanks to a request image. It returns the id of the matched images from the most to the least relevant ones.
        /// </summary>
        /// <param name="data">the binary data of the request image compressed in JPEG</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async System.Threading.Tasks.Task<int[]> SearchAsync(byte[] data)
        {
            var url = $"{Host}/index/searcher";
            var ret = await PostAsync(url, data);
            if (ret == null) return null;
            var dec = JsonConvert.DeserializeObject<JObject>(ret);
            if (dec.HasValues)
            {
                if (dec["type"].ToString() == "SEARCH_RESULTS")
                    return dec["image_ids"].ToObject<int[]>();
                else
                    throw new Exception(dec["type"].ToString());
            }
            return null;
        }

        /// <summary>
        /// Search request
        /// This call performs a search in the index thanks to a request image. It returns the id of the matched images from the most to the least relevant ones.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<int[]> SearchAsync(string path)
        {
            var data = File.ReadAllBytes(path);
            return SearchAsync(data);
        }
#endif
        #endregion
    }
}
