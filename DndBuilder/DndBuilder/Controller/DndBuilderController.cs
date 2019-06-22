using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace DndBuilder.Controller
{
    public class DndBuilderController : ApiController
    {
        //private field
        private const string DB_NAME = "DnbBuilderDbs.sqlite";
        private const string API_URL = "http://www.dnd5eapi.co/api/";
        private DnbController dnbControl;
        private DatabaseController bdControl;

        public DndBuilderController()
        {
            dnbControl = new DnbController(API_URL);
            bdControl = new DatabaseController(DB_NAME);
        }

        [HttpGet]
        [Route("Dnb/races")]
        public Dictionary<string, object> GetRaces()
        {
            Dictionary<string, object> races = new Dictionary<string, object>();
            try
            {
                races["count"] = dnbControl.getIndex("races");
                races["data"] = dnbControl.getListOf("races");
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
            return races;
        }

        [HttpGet]
        [Route("Dnb/classes")]
        public Dictionary<string, object> GetClasses()
        {
            Dictionary<string, object> races = new Dictionary<string, object>();
            try
            {
                races["count"] = dnbControl.getIndex("classes");
                races["data"] = dnbControl.getListOf("classes");
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
            return races;
        }

        [HttpGet]
        [Route("Dnb/races/{name}")]
        public Dictionary<string, int> GetRacialAbility(string name)
        {
            Dictionary<string, int> ability = new Dictionary<string, int>();
            try
            {
                ability = dnbControl.getRacialAbility(name);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
            return ability;
        }

        [HttpGet]
        [Route("Dnb/classes/{name}")]
        public Dictionary<string, object> GetClassAbility(string name)
        {
            Dictionary<string, object> ability = new Dictionary<string, object>();
            try
            {
                ability = dnbControl.getClassAbility(name);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
            return ability;
        }

        [HttpPost]
        [Route("DnbDB/add")]
        public int Post([FromBody] Dictionary<string, object> req)
        {
            try
            {
                if (!dnbControl.isClass(req["class"].ToString())) throw new Exception(req["class"].ToString() + " is not a real class");
                if (!dnbControl.isRace(req["race"].ToString())) throw new Exception(req["race"].ToString() + " is not a real race");
                bdControl.addEntry(req);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
            return 1;
        }


        [HttpPost]
        [Route("DnbDB/get")]
        public Dictionary<string, object> PostEntry([FromBody] Dictionary<string, string> req)
        {
            try
            {
                Dictionary<string, object> entry = bdControl.getEntry(req["name"]);
                return entry;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
        }

        [HttpGet]
        [Route("DnbDB/get/all")]
        public List<Dictionary<string, object>> Get()
        {
            try
            {
                return bdControl.getAllEntries();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
        }

        [HttpDelete]
        [Route("DnbDB/delete")]
        public int Delete([FromBody] Dictionary<string, string> req)
        {
            try
            {
                Console.WriteLine("Delete entry:");
                if (req["name"] == null) throw new Exception("Invalid name: null");
                bdControl.deleteEntry(req["name"]);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
        }

        [HttpPut]
        [Route("DnbDB/modify")]
        public int Modify([FromBody] Dictionary<string, object> req)
        {
            try
            {
                Console.WriteLine("Modify entry:");
                bdControl.changeEntry(req["ini_name"].ToString(), req);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
        }

        [HttpPost]
        [Route("DnbDB/download")]
        public string Download([FromBody] Dictionary<string, string> req)
        {
            try
            {
                Dictionary<string, object> entry = bdControl.getEntry(req["name"]);
                Dictionary<string, int> racialAbility = dnbControl.getRacialAbility(entry["race"].ToString());
                Dictionary<string, object> classAbility = dnbControl.getClassAbility(entry["class"].ToString());
                int con_value = racialAbility["CON"] + Convert.ToInt32(entry["CON"]);
                int dex_value = racialAbility["DEX"] + Convert.ToInt32(entry["DEX"]);
                int int_value = racialAbility["INT"] + Convert.ToInt32(entry["INT"]);
                int wis_value = racialAbility["WIS"] + Convert.ToInt32(entry["WIS"]);
                int cha_value = racialAbility["CHA"] + Convert.ToInt32(entry["CHA"]);
                int str_value = racialAbility["STR"] + Convert.ToInt32(entry["STR"]);
                int hit_point = Convert.ToInt32(classAbility["hit_die"])*(Convert.ToInt32(entry["level"]))+con_value;
                //decode
                entry["race"] = HttpUtility.HtmlDecode(entry["race"].ToString());
                entry["class"] = HttpUtility.HtmlDecode(entry["class"].ToString());
                if (String.Equals(entry["gender"].ToString(),"m"))
                {
                    entry["gender"] = "Male";
                }
                else
                if (String.Equals(entry["gender"].ToString(), "f"))
                {
                    entry["gender"] = "Female";
                }
                else entry["gender"] = "Other";


                string url = "../Resources/DownloadXML/";
                string filename = DateTime.Now.Ticks.ToString()+".xml";

                using (XmlWriter writer = XmlWriter.Create("Resources/DownloadXML/" + filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Character");
                    writer.WriteElementString("Name", entry["name"].ToString());
                    writer.WriteElementString("Age", entry["age"].ToString());
                    writer.WriteElementString("Biography", entry["bio"].ToString());
                    writer.WriteElementString("Gender", entry["gender"].ToString());
                    writer.WriteElementString("Level", entry["level"].ToString());
                    writer.WriteElementString("Race", entry["race"].ToString());
                    writer.WriteElementString("Class", entry["class"].ToString());
                    writer.WriteElementString("Spellcaster", classAbility["spellcasting"].ToString());
                    writer.WriteElementString("HP", hit_point.ToString());
                    writer.WriteStartElement("Ability_Scores");
                    writer.WriteElementString("Constitution", con_value.ToString());
                    writer.WriteElementString("Dexterity", dex_value.ToString());
                    writer.WriteElementString("Strength", str_value.ToString());
                    writer.WriteElementString("Charisma", cha_value.ToString());
                    writer.WriteElementString("Inteligence", int_value.ToString());
                    writer.WriteElementString("Wisdom", wis_value.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                return url+filename;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred:" + e.Message));
            }
        }
    }
}
