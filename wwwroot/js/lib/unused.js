//Create link to trigger Chat Modal
function addChatList(groupId, groupTitle) {
    let chatList = document.getElementById('chat-list');
    let chLItem = document.getElementsByClassName('chatListItem navbar-dropdown-list-option');
    let repeated = false;
    for (let i = 0; i < chLItem.length; i++) {
        if (chLItem[i].id == `group-${groupId}`) {
            repeated = true;
            break;
        }
    }
    if (!repeated || chLItem.length == 1) {
        let chatItem = createDropdownChatModalLink(String(groupId), String(groupTitle));
        chatList.firstElementChild.after(chatItem);
        document.getElementById(`chat_${groupId}_modalLink_pill`).innerText = "";
    }
}
function createDropdownChatModalLink(chatId, message) {
    let wrap = document.createElement('li');
    let msgtxt = newNotifyPill(`chat_${chatId}_modalLink_pill`, 'dropdown-chat-notify-spot', "");
    msgtxt += `<a id="chat_trigger_${chatId}" class="inDropdownTriggerLink">${message}</a>`;
    wrap.id = `group-${chatId}`;
    wrap.innerHTML = msgtxt;
    wrap.className = 'chatListItem navbar-dropdown-list-option';
    return wrap;
}
//Add Tab and ContentPage To Chat Modal
function addChatModalContent(chatId, chatTitle) {
    let chModal = document.getElementById(`chatModalTab_${chatId}`);
    if (chModal == undefined) {
        let mbtn = `<button class="nav-link chatModalNavBtn" id="chatModalTab_${chatId}" data-bs-toggle="pill" data-bs-target="#chatModalContent_${chatId}" type="button">`;
        mbtn += newNotifyPill(`chat_${chatId}_unread_pill`, 'modal-chat-notify-spot', '');
        mbtn += `<span class="in-modal-chat-title">${chatTitle}</span></button >`
        let mCnt = `<div class="tab-pane fade" id="chatModalContent_${chatId}" role="tabpanel">`
        mCnt += `</div>`;
        let cMTab = document.getElementById('chat-modal-tab');
        let cMContent = document.getElementById('chat-modal-tabContent');
        cMTab.innerHTML += mbtn;
        cMContent.innerHTML += mCnt;
        ChatStart(chatId);
    }
    let myModalBtn = document.getElementById(`chatModalBtn`);
    myModalBtn.click();
    let myModalEl = document.getElementById(`chatModalTab_${chatId}`);
    myModalEl.click();
    CheckNotifyCount();
}

//Create a complete modal
function addModal(buttonText, idName, headerContent, bodyContent, footerContent) {
    var mbutton = modalButton(idName, buttonText);
    var mbody = modalHeader(headerContent) + modalBody(bodyContent) + modalFooter(footerContent);
    return mbutton + modalMain(idName, mbody);
}
//Create button part of moadl
function addModalButton(buttonText, idName) {
    var mbutton = modalButton(idName, buttonText);
    return mbutton;
}
//Create main body part of moadl
function addModalBody(idName, headerContent, bodyContent, footerContent) {
    var mbody = modalHeader(headerContent) + modalBody(bodyContent) + modalFooter(footerContent);
    return modalMain(idName, mbody);
}

//Temp
function myDocMsg(targetId, type, msg) {
    if (type == 'notify') {
        docMsgNotify(targetId, msg);
    }
    if (type == 'loginSuccess') {
        userOption('login', msg);
    }
}
function docMsgNotify(targetId, message) {
    var allNotify = document.getElementById('all-notify');
    if (allNotify.innerText == 'unread notify') {
        allNotify.innerText = "";
    }
    if (message[0] == 'NewMsg') {
        var notify_spot = targetById(targetId);
        if (notify_spot.innerText == 'NaN') {
            notify_spot.innerText = 1;
        }
        else {
            notify_spot.innerText++;
        }
    }
    if (message[0] == 'ClearChatGroupNotify') {
        var notify_spot = targetById(targetId);
        notify_spot.innerText = "";
    }
    if (message[0] == 'Clear') {
        var notify_spot = targetById(targetId);
        notify_spot.innerText = "";
    }
}


//Wnd Functions
function selectWnd(obj) {
    if (selectedWnd != undefined) {//表示已經選擇wnd
    }
    else {//表示尚未選擇wnd
        e = obj || window.event;
        if (e.target.className == 'wnd') {
            var wndId = e.target.id;
            selectedWnd = document.getElementById(wndId);
            selectedWnd.addEventListener('mousedown', (event) => {
                mouseX = event.clientX - parseInt(getComputedStyle(selectedWnd).left);
                mouseY = event.clientY - parseInt(getComputedStyle(selectedWnd).top);
                sitX = (event.clientX - mouseX);
                sitY = (event.clientY - mouseY);
            });
            wnd_id.addEventListener('mouseup', (event) => {
                if (sitX < 0) {
                }
                if (sitY < 0) {
                }
                setTimeout(100);
                selectedWnd = undefined;
            });
        }
        else {
            selectedWnd = undefined;
        }
    }
}
addEventListener('mousemove', (event) => {
    if (selectedWnd != undefined) {
        sitX = (event.clientX - mouseX); //物件(0,0)之座標
        sitY = (event.clientY - mouseY);
        selectedWnd.style.left = event.clientX - mouseX + 'px';
        selectedWnd.style.top = event.clientY - mouseY + 'px';
    }
});
