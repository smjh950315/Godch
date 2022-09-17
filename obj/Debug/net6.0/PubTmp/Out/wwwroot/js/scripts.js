/// <reference path="lib/component.js" />

function myDocMsg(targetId, type, msg) {
    if (type == 'notify') {
        docMsgNotify(targetId, msg);
    }
    if (type == 'loginSuccess') {
        userOption('login', msg);
    }
}

class Effect {
    constructor() { }
    static selectedElement = undefined;
    static HiLight(obj, myClassName) {
        var styleSaved;
        if (obj.target.className == myClassName) {
            this.selectedElement = document.getElementById(obj.target.id);
            styleSaved = this.selectedElement.style;
            this.selectedElement.style.border = "1px solid #ffffff";
            this.selectedElement.style.boxShadow = "0px 0px 10px #ffffff,inset 0px 0px 10px #ffffff";
            this.selectedElement.style.textShadow = "0px 0px 5px #ffffff";
            this.selectedElement.addEventListener('mouseleave', (e) => {
                this.selectedElement.style = styleSaved;
            });
        }
    }
}

function loadChatGroups(rawdata) {
    var navTFB = navTabFirstButton("main-list", "main-page", "Main");
    var navTFC = navTabFirstContent("main-page", "Main page");
    for (var i = 0; i < rawdata.length; i++) {
        var data = chatGroupDataArray(rawdata[i]);
        var tabBtn = data[1] + notifyComponent(`chat_ntf_${data[0]}`);
        navTFB += navTabButton(`chatbtn_${data[0]}`, `chat_${data[0]}`, tabBtn);
        navTFC += navTabContent(`chat_${data[0]}`, chatIframe(data[0]));
    }
    return navTab("chatTab", navTFB, navTFC);
}
function chatGroupDataArray(data) {
    return [data.groupId, data.groupTitle];
}
function chatIframe(groupId) {
    var frame = `<iframe class="chatFrame" id="ch${groupId}" style="height:100px;" src="/Chat/Group?gid=${groupId}"></iframe>`;
    return frame;
}

function userOption(method, msg) {
    var user_opt = document.getElementById('user-option');
    if (method == 'login') {
        closeModal('Modal');
        user_opt.innerHTML = navUserTabLogin(msg);
    }
}
function navUserTabLogin(user) {
    var userId = user[0];
    var userName = user[1];
    var photo = user[2];
    var tab = `<div class="btn-group">` + "\n" + `<button class="btn btn-primary">Notify` + "\n";
    tab += notifyComponent('all-notify') + "</button>\n";
    tab += `<a class="btn btn-primary" href="/Chat/Index">Msgs</a>` + "\n";
    tab += `<a class="user-nav-head" data-bs-toggle="tooltip" data-bs-placement="right" title="${userName}" href="/User/Index?uid=${userId}">`;
    tab += `<img class="img-fluid" src='/_GodchData/Img/${photo}' /></a>` + `<a class="btn" href="/Account/Logout">Logout</a>`;
    return tab;
}
function navUserTabLogout() {
    var tab = `<div class="nav-bar-text">Guest</div>` + "\n" + `<a class="btn" id="myframe" data-bs-toggle="modal" data-bs-target="#Modal">Login</a>`;
    tab += "\n" + `<div class="modal fade" id="Modal" tabindex="-1" aria-labelledby="ModalLabel" aria-hidden="true">` + "\n" + `<div class="modal-dialog">`;
    tab += `<div class="modal-content" style="padding: 0.5rem; max-width:350px; margin:auto"><div class="modal-header">`;
    tab += `<div class="modal-title">Login</div><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="close"></button></div>`;
    tab += `<div class="modal-body" style="height:200px"><iframe class="container position-absolute top-50 start-50 translate-middle" id="login-box" src="/Account/Login">`;
    tab += `</iframe></div><div class="modal-footer"><button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button></div>`;
    tab += `</div></div></div>`;
    return tab;
}