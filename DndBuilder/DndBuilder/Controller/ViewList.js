var req;
var name = 1;
function getList()
{
    req = null;
    req = new XMLHttpRequest();
    var dest = "/DnbDB/get/all";
    req.open("GET",dest,true);
    req.onreadystatechange = getListComplete;
    req.send();
}

function getListComplete()
{
    if (req.readyState == 4)
    {
        if (req.status = 200)
        {
            var result = JSON.parse(req.responseText);
            for (i in result)
            {
                table_list;
                var row = table_list.insertRow(-1);
                var cell_name = row.insertCell(0);
                var cell_race = row.insertCell(1);
                var cell_class = row.insertCell(2);
                var cell_level = row.insertCell(3);
                var cell_button = row.insertCell(4);
                cell_name.innerHTML = result[i]["name"];
                cell_race.innerHTML = result[i]["race"];
                cell_class.innerHTML = result[i]["class"];
                cell_level.innerHTML = result[i]["level"];
                var btn = document.createElement("BUTTON");
                btn.innerHTML = "View";
                cell_button.appendChild(btn);
                var str = toCode(result[i]["name"]);
                btn.setAttribute("onclick","location.href='View.html?name=" +str+"'");
            }
        }
        else
        {
            alert("Async failed");
            req = null;
        }
     }
}

function toCode(txt)
{
    var text = txt.toString();
    var str = '';
    var i;
    for (i = 0;i<text.length;i++)
    {
        if (text[i] == " ")
        {
            str += "+";
        }
        else
        if(/[a-zA-Z0-9]/.test(text[i]))
        {
            str += text[i];
        }
        else
        {
            str += "%"+str_to_hex(text[i]);
        }
    }
    return str;
}


function str_to_hex(chr)
{
    var result = chr.charCodeAt(0).toString(16);
    return result;
}