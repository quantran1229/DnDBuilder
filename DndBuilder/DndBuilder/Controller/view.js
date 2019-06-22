var req,req_1,req_2;
var result;
var hit_die,spellcasting;
var con_value = 0,dex_value = 0,str_value = 0,cha_value = 0,int_value = 0,wis_value = 0,level_value = 1;
var ini_name;

function hex_to_char(str)
{
    var hex = str.toString();
    var result = '';
    var i;
    for (i = 0;i < hex.length;i += 2)
    {
        result += String.fromCharCode(parseInt(hex.substr(i,2),16));
    }
    return result;
} 

function read_name()
{
    var nm = location.search.substring(6);
    var str = "";
    var i = 0;
    while (i<nm.length)
    {
        if (nm[i] == "+")
        {
            str += " ";
        }
        else if (nm[i] == "%")
        {
            i++;
            var number = nm.substring(i,i+2);
            str += hex_to_char(number);  
            i++;
        }
        else
        {
            str += nm[i];
        }
        i++;
    }
    return str;
}
   
function loadInformation()
{
    download_link.style.display = "none";
    submit_btn.style.display = "none";
    edit_btn.style.display = "none";
    delete_btn.style.display = "none";
    download_btn.style.display = "none";
    req_2 = null;
    req_2 = new XMLHttpRequest();
    var dest = "/Dnb/races";
    req_2.open("GET",dest,true);
    req_2.onreadystatechange = getListOfRaces;
    req_2.send();
    req_1 = null;
    req_1 = new XMLHttpRequest();
    dest = "/Dnb/classes";
    req_1.open("GET",dest,false);
    req_1.onreadystatechange = getListOfClasses;
    req_1.send();
    req = null;
    var name = read_name();
    ini_name = name;
    var data = {"name":name};
    req = new XMLHttpRequest();
    req.open("POST","/DnbDB/get",true);
    req.onreadystatechange = onComplete;
    req.setRequestHeader("Content-type","application/json");
    req.setRequestHeader("Response-type","application/json");
    req.send(JSON.stringify(data));
}

function getListOfRaces()
{
    if (req_2.readyState == 4)
    {
        if (req_2.status = 200)
        {
            var result = JSON.parse(req_2.responseText);
            var i;
            for (i = 0;i<result["count"];i++)
            {
                char_race[i] = new Option(result["data"][i]);
            }
            updateHP();
        }
        else
        {
            alert("Async failed");
            req = null;
        }
    }
}

function getListOfClasses()
{
    if (req_1.readyState == 4)
    {
        if (req_1.status = 200)
        {
            var result = JSON.parse(req_1.responseText);
            var i;
            for (i = 0;i<result["count"];i++)
            {
                char_class[i] = new Option(result["data"][i]);
            }
            updateHP();
        }
        else
        {
            alert("Async failed");
            req_1 = null;
        }
    }
}

function onComplete()
{
    if (req.readyState == 4)
    {
        if (req.status = 200)
        {
            var result = JSON.parse(req.responseText);
            char_name.value = result["name"];
            if (result["name"].length>0) 
            {
                edit_btn.style.display = "inline";
                delete_btn.style.display = "inline";
                download_btn.style.display = "inline";
            }
            char_age.value = result["age"];
            char_level.value = result["level"];
            char_bio.value = result["bio"];
            if (result["gender"] == "m")
            {
                char_gender.selectedIndex = 0;
            }
            else if (result["gender"] == "f")
            {
                char_gender.selectedIndex = 1;
            }
            else
            {
                char_gender.selectedIndex = 2;
            }
            var i;
            for (i = 0;i < char_race.length;i++)
            {
                if (char_race.options[i].value == result["race"])
                {
                    char_race.selectedIndex = i;
                    break;
                }
            }
            
            for (i = 0;i < char_class.length;i++)
            {
                if (char_class.options[i].value == result["class"])
                {
                    char_class.selectedIndex = i;
                    break;
                }
            }
            con.value = parseInt(result["CON"]);
            dex.value = parseInt(result["DEX"]);
            str.value = parseInt(result["STR"]);
            cha.value = parseInt(result["CHA"]);
            int.value = parseInt(result["INT"]);
            wis.value = parseInt(result["WIS"]);
            setUpAbility();
            onChangeRace();
            onChangeClass();
            updateHP();
        }
        else
        {
            alert("Async failed:"+req.responseText);
            req = null;
        }
    }
}

function onChangeRace()
{
    req = null;
    req = new XMLHttpRequest();
    var dest = "/Dnb/races/"+char_race.value;
    req.open("GET",dest,true);
    req.onreadystatechange = setRacialAbility;
    req.send();
}

function setRacialAbility()
{
    if (req.readyState == 4)
    {
        if (req.status = 200)
        {
            var result = JSON.parse(req.responseText);
            in_con.value = result.CON;
            in_cha.value = result.CHA;
            in_dex.value = result.DEX;
            in_int.value = result.INT;
            in_str.value = result.STR;
            in_wis.value = result.WIS;
            updateHP();
        }
        else
        {
            alert("Async failed");
            req = null;
        }
    }
}

function updateHP()
{
    if (isNaN(parseInt(char_level.value)))
    {
        char_level.value = level_value;
    }
    if ((parseInt(char_level.value) <= 0) || (parseInt(char_level.value)>20))
    {
        char_level.value = level_value;
    }
    else
    {
        level_value = char_level.value;
        var hp = (char_level.value*hit_die) + (parseInt(in_con.value)+parseInt(con.value));
        char_hp.value = hp;
        char_hp.setAttribute('style','width:'+hp+'px;');
    }
}

function onChangeClass()
{
    req_1 = null;
    req_1 = new XMLHttpRequest();
    var dest = "/Dnb/classes/"+char_class.value;
    req_1.open("GET",dest,true);
    req_1.onreadystatechange = setHitPoint;
    req_1.send();
}

function updateAbility(ability)
{
    var total_points = parseInt(con.value) + parseInt(dex.value) + parseInt(str.value)+parseInt(cha.value)+parseInt(int.value)+parseInt(wis.value);
    if ((isNaN(parseInt(ability.value)))||((total_points > 20) || (parseInt(ability.value)<0)))
    {
        con.value = con_value;
        dex.value = dex_value;
        str.value = str_value;
        int.value = int_value;
        cha.value = cha_value;
        wis.value = wis_value;
    }
    else
    {
        updateHP();
        setUpAbility();
    }
}

function setUpAbility()
{
    var total_points = parseInt(con.value) + parseInt(dex.value) + parseInt(str.value)+parseInt(cha.value)+parseInt(int.value)+parseInt(wis.value);
    var remain_points = 20 - total_points;
    total_point.value = remain_points;
    con.setAttribute("max",parseInt(con.value)+remain_points);
    dex.setAttribute("max",parseInt(dex.value)+remain_points);
    str.setAttribute("max",parseInt(str.value)+remain_points);
    cha.setAttribute("max",parseInt(cha.value)+remain_points);
    int.setAttribute("max",parseInt(int.value)+remain_points);
    wis.setAttribute("max",parseInt(wis.value)+remain_points);
    con_value = parseInt(con.value);
    dex_value = parseInt(dex.value);
    str_value = parseInt(str.value);
    int_value = parseInt(int.value);
    wis_value = parseInt(wis.value);
    cha_value = parseInt(cha.value);
    updateHP();
}

function setHitPoint()
{
    if (req_1.readyState == 4)
    {
        if (req_1.status == 200)
        {
            var result = JSON.parse(req_1.responseText);
            hit_die = parseInt(result["hit_die"]);
            spellcasting = result["spellcasting"];
            if (spellcasting == true) char_spellcaser.checked = true;
                else char_spellcaser.checked = false;
            updateHP();
        }
        else
        {
            alert("Async failed"+ req.responseText);
            req_1 = null;
        }
    }
}

function edit()
{
    submit_btn.style.display = "inline";
    edit_btn.style.display = "none";
    delete_btn.style.display = "none";
    download_btn.style.display = "none";
    char_name.disabled = false;
    char_age.disabled = false;
    char_level.disabled = false;
    char_class.disabled = false;
    char_race.disabled = false;
    char_bio.disabled = false;
    char_gender.disabled = false;
    con.disabled = false;
    dex.disabled = false;
    str.disabled = false;
    int.disabled = false;
    cha.disabled = false;
    wis.disabled = false;
    
}

function submit()
{
    var data = {"ini_name":ini_name,
                "name":char_name.value,
                "age":char_age.value,
                "gender":char_gender.value,
                "bio":char_bio.value,
                "level":char_level.value,
                "race":char_race.value,
                "class":char_class.value,
                "CON":con.value,
                "DEX":dex.value,
                "STR":str.value,
                "CHA":cha.value,
                "INT":int.value,
                "WIS":wis.value};
     req = null;
     req = new XMLHttpRequest();
     req.open("PUT","/DnbDB/modify",true);
     req.onreadystatechange = onSubmitComplete;
     req.setRequestHeader("Content-type","application/json");
     req.send(JSON.stringify(data));
}

function onSubmitComplete()
{
    if (req.readyState == 4)
    {
        if (req.status == 200)
        {
            if (req.responseText == 1)
            {
                alert("Character is modified");
                ini_name = char_name.value;
                char_gender.disabled = true;
                char_name.disabled = true;
                char_age.disabled = true;
                char_level.disabled = true;
                char_class.disabled = true;
                char_race.disabled = true;
                char_bio.disabled = true;
                con.disabled = true;
                dex.disabled = true;
                str.disabled = true;
                int.disabled = true;
                cha.disabled = true;
                wis.disabled = true;
                edit_btn.style.display = "inline";
                delete_btn.style.display = "inline";
                download_btn.style.display = "inline";
                submit_btn.style.display = "none";
            }
            else
            {
                alert(req.responseText);
                delete_btn.style.display = "none";
                download_btn.style.display = "none";
            }
        }
        else
        {
            alert("Async failed"+ req.responseText);
            req = null;
        }
    }
}

function onDelete()
{
    var data = {"name":char_name.value};
    req = null;
    req = new XMLHttpRequest();
    req.open("DELETE","/DnbDB/delete",true);
    req.onreadystatechange = onDeleteComplete;
    req.setRequestHeader("Content-type","application/json");
    req.send(JSON.stringify(data));
}

function onDeleteComplete()
{
    if (req.readyState == 4)
    {
        if (req.status = 200)
        {
            if (req.responseText == 1)
            {
                alert("Character is deleted");
                char_gender.disabled = true;
                char_name.disabled = true;
                char_age.disabled = true;
                char_level.disabled = true;
                char_class.disabled = true;
                char_race.disabled = true;
                char_bio.disabled = true;
                con.disabled = true;
                dex.disabled = true;
                str.disabled = true;
                int.disabled = true;
                cha.disabled = true;
                wis.disabled = true;
                submit_btn.style.display = "none";
                edit_btn.style.display = "none";
                download_btn.style.display = "none";
                delete_btn.style.display = "none";
                download_btn.style.display = "none";
            }
            else
            {
                alert(req.responseText);
            }
        }
        else
        {
            alert("Async failed" + req.responseText);
            req = null;
        }
    }
}

function onDownload()
{
    var data = {"name":ini_name};
    req = null;
    req = new XMLHttpRequest();
    req.open("POST","/DnbDB/download",true);
    req.onreadystatechange = onDownloadComplete;
    req.setRequestHeader("Content-type","application/json");
    req.send(JSON.stringify(data));
}

function onDownloadComplete()
{
    if (req.readyState == 4)
    {
        if (req.status == 200)
        {
            download_link.href = JSON.parse(req.responseText);
            download_link.style.display = "inline";
        }
        else
        {
            alert("Async failed"+ req.responseText);
            req = null;
        }
    }
}