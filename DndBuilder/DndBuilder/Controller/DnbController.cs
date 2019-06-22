using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DndBuilder.Controller
{
    public class DnbController
    {
        private string API_URL;
        public DnbController(string url)
        {
            API_URL = url;
        }

        public int getIndex(string category)
        {
            string res = null;
            string url = API_URL + category;
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            response.Close();
            JObject jsonRes = JObject.Parse(res);
            int count = Convert.ToInt32(jsonRes["count"]);
            return count;
        }

        public List<string> getListOf(string category)
        {
            List<string> list = new List<string>();
            string res = null;
            string url = API_URL + category;
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            response.Close();
            JObject jsonRes = JObject.Parse(res);
            int count = Convert.ToInt32(jsonRes["count"]);
            List<Dictionary<string, string>> results = jsonRes["results"].ToObject<List<Dictionary<string, string>>>();
            for (int i = 0; i < count; i++)
            {
                //sanitise
                list.Add(HttpUtility.HtmlEncode(results[i]["name"]));
            }
            return list;
        }

        public Dictionary<string, int> getRacialAbility(string name)
        {
            //decode the name
            name = HttpUtility.HtmlDecode(name);
            Dictionary<string, int> info = new Dictionary<string, int>();
            string res = null;
            string url = API_URL + "races/";
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            response.Close();
            JObject jsonRes = JObject.Parse(res);
            int count = Convert.ToInt32(jsonRes["count"]);
            List<Dictionary<string, string>> results = jsonRes["results"].ToObject<List<Dictionary<string, string>>>();
            for (int i = 0; i < count; i++)
            {
                if (string.Compare(results[i]["name"], name) == 0)
                {
                    int index = i + 1;
                    url = API_URL + "races/" + index;
                    webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    webRequest.ContentType = "application/json";
                    webRequest.Method = "GET";

                    response = (HttpWebResponse)webRequest.GetResponse();
                    data = response.GetResponseStream();
                    reader = new StreamReader(data);
                    res = reader.ReadToEnd();
                    response.Close();
                    JObject raceJson = JObject.Parse(res);
                    List<string> ability = raceJson["ability_bonuses"].ToObject<List<string>>();
                    info["CON"] = Convert.ToInt32(ability[0]);
                    info["DEX"] = Convert.ToInt32(ability[1]);
                    info["STR"] = Convert.ToInt32(ability[2]);
                    info["CHA"] = Convert.ToInt32(ability[3]);
                    info["INT"] = Convert.ToInt32(ability[4]);
                    info["WIS"] = Convert.ToInt32(ability[5]);
                }
            }
            return info;
        }

        public Dictionary<string, object> getClassAbility(string name)
        {
            //decode
            name = HttpUtility.HtmlDecode(name);
            Dictionary<string, object> info = new Dictionary<string, object>();
            string res = null;
            string url = API_URL + "classes/";
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            response.Close();
            JObject jsonRes = JObject.Parse(res);
            int count = Convert.ToInt32(jsonRes["count"]);
            List<Dictionary<string, string>> results = jsonRes["results"].ToObject<List<Dictionary<string, string>>>();
            for (int i = 0; i < count; i++)
            {
                if (string.Compare(results[i]["name"], name) == 0)
                {
                    int index = i + 1;
                    url = API_URL + "classes/" + index;
                    webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    webRequest.ContentType = "application/json";
                    webRequest.Method = "GET";

                    response = (HttpWebResponse)webRequest.GetResponse();
                    data = response.GetResponseStream();
                    reader = new StreamReader(data);
                    res = reader.ReadToEnd();
                    response.Close();
                    JObject classJson = JObject.Parse(res);
                    info["hit_die"] = Convert.ToInt32(classJson["hit_die"]);
                    if (classJson["spellcasting"] != null)
                    {
                        info["spellcasting"] = true;
                    }
                    else
                        info["spellcasting"] = false;
                }
            }
            return info;
        }

        public bool isClass(string name)
        {
            //decode
            name = HttpUtility.HtmlDecode(name);
            string res = null;
            string url = API_URL + "classes/";
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            response.Close();
            JObject jsonRes = JObject.Parse(res);
            int count = Convert.ToInt32(jsonRes["count"]);
            List<Dictionary<string, string>> results = jsonRes["results"].ToObject<List<Dictionary<string, string>>>();
            for (int i = 0; i < count; i++)
            {
                if (string.Compare(results[i]["name"], name) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isRace(string name)
        {
            //decode
            name = HttpUtility.HtmlDecode(name);
            string res = null;
            string url = API_URL + "races/";
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            response.Close();
            JObject jsonRes = JObject.Parse(res);
            int count = Convert.ToInt32(jsonRes["count"]);
            List<Dictionary<string, string>> results = jsonRes["results"].ToObject<List<Dictionary<string, string>>>();
            for (int i = 0; i < count; i++)
            {
                if (string.Compare(results[i]["name"], name) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
