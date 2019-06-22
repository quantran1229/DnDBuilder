//script for adding new character, adding some checking as well
//global field
var req,req_1;
var hit_die,spellcasting;
var con_value = 0,dex_value = 0,str_value = 0,cha_value = 0,int_value = 0,wis_value = 0,level_value = 1;

//function loadInformation
//input: none
//output:none
//purpose:get list of all classes and races from server at the start and puting them as options for race and class.
function loadInformation()
{
    req = null;
    req = new XMLHttpRequest();
    var dest = "/Dnb/races";
    req.open("GET",dest,true);
    req.onreadystatechange = getListOfRaces;
    req.send();
    req_1 = null;
    req_1 = new XMLHttpRequest();
    dest = "/Dnb/classes";
    req_1.open("GET",dest,true);
    req_1.onreadystatechange = getListOfClasses;
    req_1.send();
}

function getListOfRaces()
{
    if (req.readyState == 4)
    {
        if (req.status = 200)
        {
            var result = JSON.parse(req.responseText);
            var i;
            for (i = 0;i<result["count"];i++)
            {
                char_race[i] = new Option(result["data"][i]);
            }
            char_race.selectedIndex = 0;
            onChangeRace();
        }
        else
        {
            alert("Async failed" + req.responseText);
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
            char_class.selectedIndex = 0;
            onChangeClass();
        }
        else
        {
            alert("Async failed" + req_1.responseText);
            req_1 = null;
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
            alert("Async failed" + req.responseText);
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
        var hp = (char_level.value*hit_die) + (parseInt(in_con.value)+parseInt(con.value))
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
            alert("Async failed" + req_1.responseText);
            req_1 = null;
        }
    }
}

function submit()
{
    var data = {"name":char_name.value,
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
     req.open("POST","/DnbDB/add",true);
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
                alert("Character is saved");
                window.location.href = "Main.html";
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

function checkage()
{
    
}

function checkname()
{
   
}