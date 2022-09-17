/// <reference path="../build-in.bundle.js" />
/// <reference path="signalr.min.js" />

"use strict";
var msglist = document.getElementById("history-msg");
var sendbtn = document.getElementById("SendMsg");
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var SelfID, GroupID;

msglist.addEventListener('scroll', function () {
    connection.invoke("MsgRead").catch(function (err) {
        return console.error(err.toString());
    });
    chatNotify(GroupID, 'ClearChatGroupNotify');    
});
sendbtn.disabled = true;

connection.on("ReceiveMessage", function (uid, user, message, time) {
    ShowMsg(uid, user, message, time);
});
connection.on("ReceiveOldMessage", function (uid, user, message, time) {
    GetOldMessage(uid, user, message, time);
});
connection.start().then(function () {
    SelfID = SID;
    GroupID = GID;
    document.getElementById('conn_data').remove();
    connection.invoke("ConnectGroup", GroupID).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("SendMsg").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("SendMsg").addEventListener("click", function (event) {
    var message = document.getElementById("text_input").value;
    connection.invoke("Send", message).catch(function (err) {
        return console.error(err.toString());
    });
    var msgbox = document.getElementById("text_input");
    msgbox.value = null;
    event.preventDefault();
});
document.getElementById("GetOldMsg").addEventListener("click", function () {
    connection.invoke("OldMsg").catch(function (err) {
        return console.error(err.toString());
    });
});

function GetOldMessage(uid, name, message, time) {
    if (uid == SelfID) {
        var msg = createMsg("msg self", message, time);
        msglist.firstElementChild.after(msg);
    }
    else {
        var msg = createMsg("msg else", message,time);
        msglist.firstElementChild.after(msg);
    }
}
function ShowMsg(uid, name, message, time) {
    if (uid == SelfID) {
        var msg = createMsg("msg self", message, time);
        msglist.appendChild(msg);
    }
    else {
        var msg = createMsg("msg else", message, time);
        msglist.appendChild(msg);
    }
    chatNotify(GroupID, 'NewMsg');
    toLatestMsg();
}
function toLatestMsg() {
    var PosY = msglist.lastChild.offsetTop;
    msglist.scrollTo({
        top: PosY,
        behavior: "smooth"
    });
}
function createMsg(className, message, time) {
    var wrap = document.createElement('div');
    var msg = document.createElement('div');
    msg.className = className;
    msg.innerText = message;
    wrap.appendChild(msg);
    wrap.className = 'wrap-msg';
    wrap.title = time;
    return wrap;
}
function chatNotify(chatId, event) {
    var targetId = `chat_ntf_${chatId}`;
    sendDocMsg(targetId, 'notify', [event, chatId]);
}

function chatSurfaceAdjust() {
    var chContainer = document.getElementById('chatContainer');
    var bdy = document.getElementsByTagName('body')[0];
    var h = parseInt(chContainer.clientHeight);
    bdy.style.height = (h + 10) + 'px'
    sendDocMsg(`ch${GroupID}`, 'resizeh', (h + 30));
    sendDocMsg('container', 'resizeh', (h + 50));
}