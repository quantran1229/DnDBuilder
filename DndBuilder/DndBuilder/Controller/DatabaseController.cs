using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace DndBuilder.Controller
{
    public class DatabaseController
    {
        private string db_name;
        public DatabaseController(string name)
        {
            db_name = name;
        }
        //get an entry from database with a name of character. Throw a exception when found nothing
        public Dictionary<string, object> getEntry(string name)
        {
            Dictionary<string, object> entry = new Dictionary<string, object>();
            if (name == null || string.Equals(name,""))
            {
                throw new Exception("Entry is null");
            }
            try
            {
                name = name.ToLower();
                using (SqliteConnection db_con = new SqliteConnection("Data Source=" + db_name + ";Version=3;"))
                {
                    db_con.Open();
                    string str = "select Name from Characters WHERE lower(Name)=@name";
                    SqliteCommand command = new SqliteCommand(str, db_con);
                    command.Parameters.Add(new SqliteParameter("name", name));
                    object check = command.ExecuteScalar();
                    if (check == null)
                    {
                        throw new Exception("No entry is found in database");
                    }
                    str = "select * from Characters WHERE lower(Name)=@name";

                    command = new SqliteCommand(str, db_con);
                    command.Parameters.Add(new SqliteParameter("name", name));
                    SqliteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    entry["name"] = reader["Name"];
                    entry["age"] = reader["Age"];
                    entry["gender"] = reader["Gender"];
                    entry["bio"] = reader["Biography"];
                    entry["level"] = reader["Level"];
                    entry["race"] = reader["Race"];
                    entry["class"] = reader["Class"];
                    entry["CON"] = reader["Con"];
                    entry["DEX"] = reader["Dex"];
                    entry["STR"] = reader["Str"];
                    entry["CHA"] = reader["Cha"];
                    entry["INT"] = reader["Int"];
                    entry["WIS"] = reader["Wis"];
                    return entry;
                }
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Dictionary<string,object>> getAllEntries()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            try
            {
                using (SqliteConnection db_con = new SqliteConnection("Data Source=" + db_name + ";Version=3;"))
                {
                    db_con.Open();
                    SqliteCommand command = new SqliteCommand("SELECT Name,Race,Class,Level FROM Characters", db_con);
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Dictionary<string, object> entry = new Dictionary<string, object>();
                        entry["name"] = reader["Name"];
                        entry["race"] = reader["Race"];
                        entry["class"] = reader["Class"];
                        entry["level"] = reader["Level"];
                        list.Add(entry);
                    }
                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while getting list:" + e.Message);
            }
        }


        public void changeEntry(string ini_name, Dictionary<string, object> entry)
        {
            try
            {
                using (SqliteConnection db_con = new SqliteConnection("Data Source=" + db_name + ";Version=3;"))
                {
                    ini_name = ini_name.ToLower();
                    db_con.Open();
                    SqliteCommand checkname = new SqliteCommand("SELECT Name FROM Characters WHERE lower(Name)=@name", db_con);
                    checkname.Parameters.Add(new SqliteParameter("name", ini_name));
                    object check = checkname.ExecuteScalar();
                    if (check == null)
                    {
                        throw new Exception("Name no found in database");
                    }
                    //sanction
                    string name;
                    if (string.Equals(ini_name, entry["name"].ToString().ToLower()))
                    {
                        name = entry["name"].ToString();
                    }
                    else
                    {
                        name = checkName(entry["name"].ToString());
                    }

                    int age = checkAge(entry["age"].ToString());
                    string gender = checkGender(entry["gender"].ToString());
                    string bio = checkBio(entry["bio"].ToString());
                    int level = checkLevel(entry["level"].ToString());
                    string race = checkRace(entry["race"].ToString());
                    string class_name = checkClass(entry["class"].ToString());
                    int con = checkCon(entry["CON"].ToString());
                    int dex = checkCon(entry["DEX"].ToString());
                    int str = checkCon(entry["STR"].ToString());
                    int cha = checkCon(entry["CHA"].ToString());
                    int itl = checkCon(entry["INT"].ToString());
                    int wis = checkCon(entry["WIS"].ToString());
                    if (con + dex + str + cha + itl + wis > 20)
                    {
                        throw new Exception("Total add up points is over 20 points");
                    }

                    SqliteCommand insert = new SqliteCommand("UPDATE Characters SET Name = @Name,Age = @Age,Gender = @Gender,Biography = @Biography,Level = @Level,Race = @Race,Class = @Class,Con = @Con,Dex = @Dex,Str = @Str,Cha = @Cha,Int = @Int,Wis = @Wis WHERE lower(Name) = @ini_name", db_con);
                    insert.Parameters.Add(new SqliteParameter("ini_name", ini_name));
                    insert.Parameters.Add(new SqliteParameter("Name", name));
                    insert.Parameters.Add(new SqliteParameter("Age", age));
                    insert.Parameters.Add(new SqliteParameter("Level", level));
                    insert.Parameters.Add(new SqliteParameter("Gender", gender));
                    insert.Parameters.Add(new SqliteParameter("Biography", bio));
                    insert.Parameters.Add(new SqliteParameter("Race", race));
                    insert.Parameters.Add(new SqliteParameter("Class", class_name));
                    insert.Parameters.Add(new SqliteParameter("Con", con));
                    insert.Parameters.Add(new SqliteParameter("Dex", dex));
                    insert.Parameters.Add(new SqliteParameter("Str", str));
                    insert.Parameters.Add(new SqliteParameter("Cha", cha));
                    insert.Parameters.Add(new SqliteParameter("Int", itl));
                    insert.Parameters.Add(new SqliteParameter("Wis", wis));
                    insert.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while changing enrty:" + e.Message);
            }
        }

        //delete an entry
        public void deleteEntry(string name)
        {
            try
            {
                using (SqliteConnection db_con = new SqliteConnection("Data Source=" + db_name + ";Version=3;"))
                {
                    db_con.Open();
                    name = name.ToLower();
                    SqliteCommand checkname = new SqliteCommand("SELECT Name FROM Characters WHERE lower(Name)=@name", db_con);
                    checkname.Parameters.Add(new SqliteParameter("name", name));
                    object check = checkname.ExecuteScalar();
                    if (check == null)
                    {
                        throw new Exception("Name no found in database");
                    }
                    SqliteCommand deletea = new SqliteCommand("DELETE FROM Characters WHERE lower(Name)=@name", db_con);
                    deletea.Parameters.Add(new SqliteParameter("name", name));
                    deletea.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while delete enrty:" + e.Message);
            }
        }

        //modify a new entry to database and checking data before adding to database
        public void addEntry(Dictionary<string,object> entry)
        {
            try
            {
                using (SqliteConnection db_con = new SqliteConnection("Data Source=" + db_name + ";Version=3;"))
                {
                    db_con.Open();
                    //sanitisation
                    string name = checkName(entry["name"].ToString());
                    int age = checkAge(entry["age"].ToString());
                    string gender = checkGender(entry["gender"].ToString());
                    string bio = checkBio(entry["bio"].ToString());
                    int level = checkLevel(entry["level"].ToString());
                    string race = checkRace(entry["race"].ToString());
                    string class_name = checkClass(entry["class"].ToString());
                    int con = checkCon(entry["CON"].ToString());
                    int dex = checkCon(entry["DEX"].ToString());
                    int str = checkCon(entry["STR"].ToString());
                    int cha = checkCon(entry["CHA"].ToString());
                    int itl = checkCon(entry["INT"].ToString());
                    int wis = checkCon(entry["WIS"].ToString());
                    if (con + dex + str + cha + itl + wis > 20)
                    {
                        throw new Exception("Total add up points is over 20 points");
                    }

                    //SQl injection
                    SqliteCommand insert = new SqliteCommand("INSERT INTO Characters(Name,Age,Gender,Biography,Level,Race,Class,Con,Dex,Str,Cha,Int,Wis) VALUES (@Name,@Age,@Gender,@Biography,@Level,@Race,@Class,@Con,@Dex,@Str,@Cha,@Int,@Wis)",db_con);
                    insert.Parameters.Add(new SqliteParameter("Name", name));
                    insert.Parameters.Add(new SqliteParameter("Age", age));
                    insert.Parameters.Add(new SqliteParameter("Level", level));
                    insert.Parameters.Add(new SqliteParameter("Gender", gender));
                    insert.Parameters.Add(new SqliteParameter("Biography", bio));
                    insert.Parameters.Add(new SqliteParameter("Race", race));
                    insert.Parameters.Add(new SqliteParameter("Class", class_name));
                    insert.Parameters.Add(new SqliteParameter("Con", con));
                    insert.Parameters.Add(new SqliteParameter("Dex", dex));
                    insert.Parameters.Add(new SqliteParameter("Str", str));
                    insert.Parameters.Add(new SqliteParameter("Cha", cha));
                    insert.Parameters.Add(new SqliteParameter("Int", itl));
                    insert.Parameters.Add(new SqliteParameter("Wis", wis));
                    insert.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while saving enrty:" + e.Message);
            }
        }

        //check if name already exist in table
        private string checkName(string name)
        {
            if (name == null || string.Equals(name, ""))
            {
                throw new Exception("Invalid name:null");
            }
            try
            {
                string name_lower = name.ToLower();
                using (SqliteConnection con = new SqliteConnection("Data Source=" + db_name + ";Version=3;"))
                {
                    con.Open();
                    SqliteCommand checkname = new SqliteCommand("SELECT Name FROM Characters WHERE lower(Name)=@name", con);
                    checkname.Parameters.Add(new SqliteParameter("name",name_lower));
                    object check = checkname.ExecuteScalar();
                    if (check != null)
                    {
                        throw new Exception("Name already existed in database");
                    }
                    return name;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error with character's name:" + e.Message);
            }
        }

        //check if age >= 0 or age <= 500 and age is a number
        private int checkAge(string age)
        {
            if (age == null || string.Equals(age, ""))
            {
                throw new Exception("Invalid age:null");
            }
            int n;
            bool isNumber = int.TryParse(age, out n);
            if (isNumber)
            {
                if ((n < 0) || (n > 500))
                {
                    throw new Exception("Age is over limit");
                }
                Console.WriteLine(n);
                return n;
            }
            else
            {
                throw new Exception("Value age is not a number");
            }
        }

        //check if bio has less than 500 character
        private string checkBio(string bio)
        {
            if (bio == null)
            {
                throw new Exception("Invalid bio:null");
            }
            if (bio.Length > 500)
            {
                throw new Exception("Bio is over 500 characters limit");
            }
            return bio;
        }

        //check if level isbetween 1-20 and a number
        private int checkLevel(string level)
        {
            if (level == null || string.Equals(level, ""))
            {
                throw new Exception("Invalid level:null");
            }
            int n;
            bool isNumber = int.TryParse(level, out n);
            if (isNumber)
            {
                if ((n < 1) || (n > 20))
                {
                    throw new Exception("level is over limit");
                }
                Console.WriteLine(n);
                return n;
            }
            else
            {
                throw new Exception("Value level is not a number");
            }
        }

        private string checkGender(string gender)
        {
            if (gender == null || string.Equals(gender, ""))
            {
                throw new Exception("Invalid gender");
            }
            if (string.Equals(gender.ToLower(),"male") || string.Equals(gender.ToLower(), "m"))
            {
                Console.WriteLine("male");
                return "m";
            }
            if (string.Equals(gender.ToLower(), "female") || string.Equals(gender.ToLower(), "f"))
            {
                Console.WriteLine("female");
                return "f";
            }
            return "o";
        }

        //nothing to check
        private string checkRace(string race_name)
        {
            if (race_name == null || string.Equals(race_name,""))
            {
                throw new Exception("Invalid race");
            }
            Console.WriteLine(race_name);
            return race_name;
        }

        private string checkClass(string class_name)
        {
            if (class_name == null || string.Equals(class_name, ""))
            {
                throw new Exception("Invalid class");
            }
            Console.WriteLine(class_name);
            return class_name;
        }

        private int checkCon(string con)
        {
            if (con == null || string.Equals(con, ""))
            {
                throw new Exception("Invalid constitution");
            }

            int n;
            bool isNumber = int.TryParse(con, out n);
            if (isNumber)
            {
                if ((n < 0) || (n > 20))
                {
                    throw new Exception("constitution is over limit");
                }
                Console.WriteLine(con);
                return n;
            }
            else
            {
                throw new Exception("Value constitution is not a number");
            }
        }

        private int checkDex(string dex)
        {
            if (dex == null || string.Equals(dex, ""))
            {
                throw new Exception("Invalid Dexterity");
            }

            int n;
            bool isNumber = int.TryParse(dex, out n);
            if (isNumber)
            {
                if ((n < 0) || (n > 20))
                {
                    throw new Exception("Dexterity is over limit");
                }
                Console.WriteLine(dex);
                return n;
            }
            else
            {
                throw new Exception("Value dexterity is not a number");
            }
        }

        private int checkStr(string str)
        {
            if (str == null || string.Equals(str, ""))
            {
                throw new Exception("Invalid strength");
            }

            int n;
            bool isNumber = int.TryParse(str, out n);
            if (isNumber)
            {
                if ((n < 0) || (n > 20))
                {
                    throw new Exception("Strength is over limit");
                }
                Console.WriteLine(str);
                return n;
            }
            else
            {
                throw new Exception("Value Strength is not a number");
            }
        }

        private int checkCha(string cha)
        {
            if (cha == null || string.Equals(cha, ""))
            {
                throw new Exception("Invalid charisma");
            }

            int n;
            bool isNumber = int.TryParse(cha, out n);
            if (isNumber)
            {
                if ((n < 0) || (n > 20))
                {
                    throw new Exception("Charisma is over limit");
                }
                Console.WriteLine(cha);
                return n;
            }
            else
            {
                throw new Exception("Value charisma is not a number");
            }
        }

        private int checkInt(string itl)
        {
            if (itl == null || string.Equals(itl, ""))
            {
                throw new Exception("Invalid Inteligence");
            }

            int n;
            bool isNumber = int.TryParse(itl, out n);
            if (isNumber)
            {
                if ((n < 0) || (n > 20))
                {
                    throw new Exception("Inteligence is over limit");
                }
                Console.WriteLine(itl);
                return n;
            }
            else
            {
                throw new Exception("Value inteligence is not a number");
            }
        }

        private int checkWis(string wis)
        {
            if (wis == null || string.Equals(wis, ""))
            {
                throw new Exception("Invalid wisdom");
            }

            int n;
            bool isNumber = int.TryParse(wis, out n);
            if (isNumber)
            {
                if ((n < 0) || (n > 20))
                {
                    throw new Exception("Wisdom is over limit");
                }
                Console.WriteLine(wis);
                return n;
            }
            else
            {
                throw new Exception("Value wisdom is not a number");
            }
        }
    }
}
