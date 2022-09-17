/// <reference path="lib/component.js" />
/// <reference path="build-in.bundle.js" />
//TEMP
var ChatRoomList = [];
//Hub global variable
"use strict";
var ClientId;
var connState;
var Connection = new signalR.HubConnectionBuilder().withUrl("/functionhub").build();

function editNickName(nickName) {
    Connection.invoke("ChangeNickName", nickName).catch(function (err) {
        return console.error(err.toString());
    });
}
function editPassword(password) {
    Connection.invoke("ChangePassword", password).catch(function (err) {
        return console.error(err.toString());
    });
}
function editEmail(email) {
    Connection.invoke("ChangeEmail", email).catch(function (err) {
        return console.error(err.toString());
    });
}
function editIntroduction(intro) {
    Connection.invoke("ChangeIntroduction", intro).catch(function (err) {
        return console.error(err.toString());
    });
}
function editPostContent(postId, text) {
    Connection.invoke("EditPost", postId, text).catch(function (err) {
        return console.error(err.toString());
    });
}
function replyPost(postId, text) {
    Connection.invoke("ReplyPost", postId, text).catch(function (err) {
        return console.error(err.toString());
    });
}
function createPost(forumId, postTitle, text) {
    Connection.invoke("NewPost", forumId, postTitle, text).catch(function (err) {
        return console.error(err.toString());
    });
}
function LoadFriendList() {
    document.getElementById('friend-verified').innerHTML = '';
    document.getElementById('friend-waiting').innerHTML = '';
    document.getElementById('friend-unconfirm').innerHTML = '';
    Connection.invoke("GetFriendList").catch(function (err) {
        return console.error(err.toString());
    });
}
function LoadFollowerList() {
    document.getElementById('follower').innerHTML = '';
    Connection.invoke("GetFollowerList").catch(function (err) {
        return console.error(err.toString());
    });
}
function LoadFollowingList() {
    document.getElementById('following').innerHTML = '';
    Connection.invoke("GetFollowingList").catch(function (err) {
        return console.error(err.toString());
    });
}
function createTag(tagName) {
    Connection.invoke("CreateNewTag", tagName).catch(function (err) {
        return console.error(err.toString());
    });
}


//Create Comment
function createComment(userId,context) {
    let comment = `<div data-user-id="${userId}">${context}</div>`;
}

Connection.on("AddElement", function (targetId, element) {
    document.getElementById(String(targetId)).innerHTML += element;
});
Connection.on("AddFriendToList", function (nickName, userPage, photoUrl) {
    document.getElementById('friend-verified').innerHTML += createNewLinkedImageViewBox('', nickName, userPage, photoUrl,'75','75');
});
Connection.on("AddWFriendToList", function (nickName, userPage, photoUrl) {
    document.getElementById('friend-waiting').innerHTML += createNewLinkedImageViewBox('', nickName, userPage, photoUrl, '75', '75');
});
Connection.on("AddUFriendToList", function (nickName, userPage, photoUrl) {
    document.getElementById('friend-unconfirm').innerHTML += createNewLinkedImageViewBox('', nickName, userPage, photoUrl, '75', '75');
});
Connection.on("AddFollowerToList", function (nickName, userPage, photoUrl) {
    document.getElementById('follower').innerHTML += createNewLinkedImageViewBox('', nickName, userPage, photoUrl, '75', '75');
});
Connection.on("AddFollowingToList", function (nickName, userPage, photoUrl) {
    document.getElementById('following').innerHTML += createNewLinkedImageViewBox('', nickName, userPage, photoUrl, '75', '75');
});
//----------------AutoLoad------------------
function getConnectionState() {
    connState = document.getElementById('connection-state');
    connState.addEventListener('change', function () {
        let content = connState.innerText;
        if (content == 'isConnected') {
            GetChatList();
        }
    });
}
function addChatListAndModalContent(groupId, groupTitle) {
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
        let chatItem = createDropdownChatModalLinkWithoutClick(String(groupId), String(groupTitle));
        chatList.firstElementChild.after(chatItem);
        document.getElementById(`chat_${groupId}_modalLink_pill`).innerText = "";
        addChatModalContentWithoutOpenModal(String(groupId), String(groupTitle));
    }
}
function createDropdownChatModalLinkWithoutClick(chatId, message) {
    let wrap = document.createElement('li');
    let msgtxt = newNotifyPill(`chat_${chatId}_modalLink_pill`, 'dropdown-chat-notify-spot', "");
    msgtxt += `<a id="chat_trigger_${chatId}" class="inDropdownTriggerLink">${message}</a>`;
    wrap.id = `group-${chatId}`;
    wrap.innerHTML = msgtxt;
    wrap.className = 'chatListItem navbar-dropdown-list-option';
    return wrap;
}
function addChatModalContentWithoutOpenModal(chatId, chatTitle) {
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
    CheckNotifyCount();
}
function createDropdownNotifyLink(message, url, time) {
    let wrap = document.createElement('li');
    let msgtxt = `<a href="${url}" class="inDropdownTriggerLink" title="${time}">${message}</a>`;
    wrap.innerHTML = msgtxt;
    wrap.className = 'chatListItem navbar-dropdown-list-option';
    return wrap;
}



//CreateMessage(inMsgList)
function createChatMessage(className, message, time) {
    let wrap = document.createElement('div');
    let msg = document.createElement('div');
    msg.className = className;
    msg.innerText = message;
    wrap.appendChild(msg);
    wrap.className = 'wrap-msg';
    wrap.title = time;
    return wrap;
}
function createNotifyMessage(message, url, time) {
    let nwrap = document.createElement('li');
    let msg = document.createElement('a');
    msg.href = String(url);
    msg.innerText = String(message);
    nwrap.className = 'wrap-msg';
    nwrap.appendChild(msg);
    nwrap.title = time;
    return nwrap;
}
function createChatMessage(className, message, time, userId) {
    let wrap = document.createElement('div');
    let imga = document.createElement('div');
    let img = document.createElement('img');
    let msg = document.createElement('div');
    msg.className = className;
    msg.innerText = message;
    img.src = `/_GodchData/UserData/${userId}/photo.jpg`;
    img.className = 'img-fluid';
    imga.className = 'img-view-box-base h-32px w-32px';    
    imga.appendChild(img);

    if (String(className).includes('self')) {
        wrap.appendChild(msg);
    }
    else {
        wrap.appendChild(imga);
        wrap.appendChild(msg);
    }
    wrap.className = 'wrap-msg';
    wrap.title = time;
    return wrap;
}
function newNotifyPill(idName, className, innerText) {
    return newPill(idName, 'notify-spot ' + className, innerText);
}

//add Notify
function newMsgToNotify(chatId) {
    let target = targetById(`chat_${chatId}_unread_pill`);
    if (target.innerText == "") {
        document.getElementById(`chat_${chatId}_unread_pill`).innerText = 0;
    }
    let number = parseInt(target.innerText);
    number += 1;
    document.getElementById(`chat_${chatId}_unread_pill`).innerText = number; 

    let chat_unread_pill = document.getElementById(`chat_${chatId}_unread_pill`);
    let chat_unread_title = chat_unread_pill.nextElementSibling;
    let chat_title = chat_unread_title.innerText;

    let li_groupModalLink = document.getElementById(`group-${chatId}`);
    li_groupModalLink.remove();

    let chatList = document.getElementById('chat-list');

    let chatItem = createDropdownChatModalLinkWithoutClick(String(chatId), String(chat_title));
    chatList.firstElementChild.after(chatItem);

    let new_unread_pill = document.getElementById(`chat_${chatId}_unread_pill`);
    chat_unread_pill.innerText = number;
    document.getElementById(`chat_${chatId}_modalLink_pill`).innerText = number;

    CheckNotifyCount();
}
//Update Notify in chat Modal
function CheckNotifyCount() {
    let btnsInModal = document.getElementsByClassName('badge rounded-pill bg-danger notify-spot dropdown-chat-notify-spot');
    let notifyOfChatConut = 0;
    for (let i = 0; i < btnsInModal.length; i++) {
        let num = btnsInModal[i].innerText;
        if (num != "") {
            notifyOfChatConut++;
        }
    }
    if (notifyOfChatConut == 0) {
        document.getElementById('pill-nav-msg-unread').innerText = "";
    }
    else {
        document.getElementById('pill-nav-msg-unread').innerText = notifyOfChatConut;
    }
}

//Create ChatBox in Modal
function ChatStart(targetId) {
    if (ChatRoomList.indexOf(targetId) < 0) {
        let newMsgBox = addMessageBox(targetId);
        document.getElementById(`chatModalContent_${targetId}`).innerHTML = newMsgBox;
        Connection.invoke("ConnectChatRoom", targetId).catch(function (err) {
            return console.error(err.toString());
        });
    }
}
function addMessageBox(chatRoomId) {
    let TmsgBox = `<div class="container mt-4 mb-4 msg-box" id="msgBox_${chatRoomId}">`;
    TmsgBox += `<div class="form-control container history-msg" onclick="MessageRead(${chatRoomId})" id="hMsg_${chatRoomId}">`;
    TmsgBox += `<button class="GetOldMsg" id="oldMsg_${chatRoomId}" onclick="GetOldMessage(${chatRoomId})" title="Show old message"><div>^</div></button>` + "\n" + "</div>";
    TmsgBox += `<textarea id="textInput_${chatRoomId}" class="form-control mt-2 text-input" name="Data" rows="1"></textarea>`;
    TmsgBox += `<button id="sendBtn_${chatRoomId}" onclick="SendMessage(${chatRoomId})" class="btn btn-success mt-2 SendMsg">Send</button>` + "\n" + "</div>";
    return TmsgBox;
}
//Create View Box
function createNewLinkedImageViewBox(idName, title, url, imgUrl, w, h) {
    let box = `<a id="${idName}" class="img-view-box-base" href="${url}" title="${title}" style="width:${w}px; height:${h}px;">`;
    box += `<img class="img-fluid" src='${imgUrl}'></a>`;
    return box;
}

//-------Show---------
function ShowMessage(chatRoomId, uid, userName, message, time) {
    let msglist = document.getElementById(`hMsg_${chatRoomId}`);
    if (uid == ClientId) {
        let submsg = createChatMessage("msg self", message, time, uid);
        msglist.appendChild(submsg);
    }
    else {
        let submsg = createChatMessage("msg else", message, time, uid);
        msglist.appendChild(submsg);
    }
    toLatestMsg(chatRoomId);
    newMsgToNotify(chatRoomId);
}
function ShowReadMessage(chatRoomId, uid, userName, message, time) {
    let msglist = document.getElementById(`hMsg_${chatRoomId}`);
    if (uid == ClientId) {
        let submsg = createChatMessage("msg self", message, time,uid);
        msglist.appendChild(submsg);
    }
    else {
        let submsg = createChatMessage("msg else", message, time, uid);
        msglist.appendChild(submsg);
    }
    toLatestMsg(chatRoomId);
}
function ShowOldMessage(chatRoomId, uid, userName, message, time) {
    let msglist = document.getElementById(`hMsg_${chatRoomId}`);
    if (uid == ClientId) {
        let msg = createChatMessage("msg self", message, time, uid);
        msglist.firstElementChild.after(msg);
    }
    else {
        let msg = createChatMessage("msg else", message, time, uid);
        msglist.firstElementChild.after(msg);
    }
}
function addNotify(message, url, time) {
    let notifyList = document.getElementById('notify-list');
    //let notifyItem = createDropdownNotifyLink(message, url, time);
    let notifyItem = createNotifyMessage(message, url, time);
    let nUnread = document.getElementById('pill-nav-notify-unread');
    let nCount = String(nUnread.innerText);
    if (nCount == "") {
        nCount = 1;
    } else {
        nCount = parseInt(nCount) + 1;
    }
    nUnread.innerText = nCount;
    notifyList.appendChild(notifyItem);
}
function addOldNotify(message, url, time) {
    let notifyList = document.getElementById('notify-list');
    let notifyItem = createNotifyMessage(message, url, time);
    notifyList.firstElementChild.after(notifyItem);    
}
function toLatestMsg(chatRoomId) {
    let msglist = document.getElementById(`hMsg_${chatRoomId}`);
    var PosY = msglist.lastChild.offsetTop;
    msglist.scrollTo({
        top: PosY,
        behavior: "smooth"
    });
}
function serverResponse(msg) {
    alert(msg);
}

//-----------DisplayAdjust------------
function ChatSurfaceAdjust() {
    let chContainer = document.getElementById('chatContainer');
    let bdy = document.getElementsByTagName('body')[0];
    let h = parseInt(chContainer.clientHeight);
    bdy.style.height = (h + 10) + 'px'
    sendDocMsg(`ch${GroupID}`, 'resizeh', (h + 30));
    sendDocMsg('container', 'resizeh', (h + 50));
}

//-------------Calling Function of server--------------
function GetChatList() {
    Connection.invoke("GetGroupList").catch(function (err) {
        return console.error(err.toString());
    });
}
function SendMessage(chatId) {
    let msgInput = document.getElementById(`textInput_${chatId}`);
    let message = String(msgInput.value);
    let roomId = String(chatId);
    Connection.invoke("SendChatMessage", roomId, message).catch(function (err) {
        return console.error(err.toString());
    });
    msgInput.value = null;
    event.preventDefault();
}
function GetOldMessage(chatRoomId) {
    Connection.invoke("OldMessage", chatRoomId).catch(function (err) {
        return console.error(err.toString());
    });
}
function MessageRead(chatRoomId) {
    Connection.invoke("ReadMessage", chatRoomId).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById(`hMsg_${chatRoomId}`).addEventListener('scroll', function () {
        document.getElementById(`chat_${chatRoomId}_unread_pill`).innerText = "";  
        document.getElementById(`chat_${chatRoomId}_modalLink_pill`).innerText = "";
    });
    document.getElementById(`hMsg_${chatRoomId}`).addEventListener('click', function () {
        document.getElementById(`chat_${chatRoomId}_unread_pill`).innerText = "";
        document.getElementById(`chat_${chatRoomId}_modalLink_pill`).innerText = "";
    });
    CheckNotifyCount();
}
function NotifyRead() {
    Connection.invoke("ReadNotify").catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById('pill-nav-notify-unread').innerText = '';    
}
function GetOldNotify() {
    Connection.invoke("OldNotify").catch(function (err) {
        return console.error(err.toString());
    });
}

function NewPrivateGroup(userId) {
    let UserId = String(userId);
    if (UserId == undefined || UserId.length < 1) {
        return;
    }
    Connection.invoke("NewPrivateGroup", UserId).then(function () {
        GetChatList();

    }).catch(function (err) {
        return console.error(err.toString());
    });
}

//----------Call by server-------
Connection.on("ReceiveMessage", function (chatId, uid, userName, message, time) {
    ShowMessage(chatId, uid, userName, message, time);
});
Connection.on("ReceiveReadMessage", function (chatId, uid, userName, message, time) {
    ShowReadMessage(chatId, uid, userName, message, time);
});
Connection.on("ReceiveOldMessage", function (chatId, uid, userName, message, time) {
    ShowOldMessage(chatId, uid, userName, message, time);
});
Connection.on("ReceiveNotify", function (message, url, time) {
    addNotify(message, url, time);
});
Connection.on("ReceiveOldNotify", function (message, url, time) {
    addOldNotify(message, url, time);
});
Connection.on("ReceiveUnreadNotify", function (message, url, time) {
    addOldNotify(message, url, time);
});

Connection.on("ReceiveChatList", function (groupId, groupTitle) {
    //addChatList(groupId, groupTitle);
    addChatListAndModalContent(groupId, groupTitle);
});
Connection.on("CheckChatList", function () {
    GetChatList();
});
Connection.on("TriggerCalledGroup", function (groupId) {
    let myModalBtn = document.getElementById(`chatModalBtn`);
    myModalBtn.click();
    let chatTab = document.getElementById(`chatModalTab_${groupId}`);
    chatTab.click();
});
Connection.on("GetResponse", function (serverMsg) {
    serverResponse(serverMsg);
    if (String(serverMsg) == 'Success!') {
        if ($('#modifyModal') != undefined) {
            $('#modify-window').modal('toggle');
        }
    }
});
Connection.on("ChangeText", function (idName,text) {
    document.getElementById(idName).innerText = String(text);
});
Connection.on("ReceiveCommand", function (targetIdName, type, message) {
    LaunchCommand(targetIdName, type, message);
});

//command (target,action,arg)
function LaunchCommand(targetIdName, type, message) {
    let cmd;
    let arg1 = String(targetIdName);
    let arg2 = String(type);
    let docMsg = [arg1, arg2, message];
    cmd.data.docMsg = docMsg;
    return cmd;
}

//Connection Initiate
Connection.start().then(function () {
    Connection.invoke('ConnectHub').then(function () {
        let currentUser = document.getElementById('user-now').innerHTML;
        ClientId = String(currentUser);
        GetChatList();
        document.getElementById('connection-state').innerText = 'isConnected';
    }).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});