/// <reference path="build-in.bundle.js" />
/// <reference path="lib/component.js" />
var wnd_id;
var selectedWnd;
var selectedElement = undefined;
//Wnd
function getId(obj) {
    if (selectedWnd != undefined) {//表示已經選擇wnd
    }
    else {//表示尚未選擇wnd
        e = obj || window.event;
        if (e.target.className == 'wnd') {//若碰到的物件為wnd
            tmp = "c";
            tmp = e.target.id;
            wnd_id = document.getElementById(tmp);
            wnd_id.addEventListener('mousedown', function (event) {
                selectedElement = wnd_id;
                mouseX = event.clientX - parseInt(getComputedStyle(selectedElement).left); //滑鼠在物件的座標
                mouseY = event.clientY - parseInt(getComputedStyle(selectedElement).top);
            });
            wnd_id.addEventListener('mouseup', function (event) {
                if (sitX < 0) {
                }
                if (sitY < 0) {
                }
                selectedElement = undefined;
            });
        }
        else {
        }
    }
}
addEventListener('mousemove', function (event) {
    if (selectedElement == undefined) {
        return;
    }
    if (selectedElement.className == 'wnd') {
        sitX = (event.clientX - mouseX); //物件(0,0)之座標
        sitY = (event.clientY - mouseY);
        selectedElement.style.left = event.clientX - mouseX + 'px';
        selectedElement.style.top = event.clientY - mouseY + 'px';
    }
});
function createWnd(WndName, WndX, WndY, WndSizeX, WndSizeY, content) {
    let dspArea = document.getElementById('area-wnd-display');
    if (dspArea == undefined) {
        let displayArea = `<div id="area-wnd-display" class="vh-100 vw-100" style="padding:-1920px;"></div>`;
        document.body.innerHTML += displayArea;
    }
    let newWnd = `<div id="wnd-${WndName}" class="wnd" onmousemove="getId(event)" style="width:${WndSizeX}px; height:${WndSizeY}px">`;
    newWnd += `<div class="wnd-content" style="padding-left:0.5em;">`;
    newWnd += content;
    newWnd += `</div></div>`;
    dspArea.innerHTML = newWnd;
}

//Custom Ui
function createUiWnd() {
    let uiAdjSerface = "";
    uiAdjSerface += `<div><input class="color-adj mt-1 mb-1" id="color-adj-bg" type="color" /><span>Background Color</span></div>`;
    uiAdjSerface += `<div><input class="color-adj mt-1 mb-1" id="color-adj-text" type="color" /><span>Text Color</span></div>`;
    uiAdjSerface += `<div><input class="color-adj mt-1 mb-1" id="font-adj" type="color" /><span>?? Color</span></div>`;
    uiAdjSerface += `<div id="btn-ui-default" class="btn-base ml-1 mr-1" onclick="uiDefault()">Default</div>`;
    uiAdjSerface += `<div id="btn-ui-save" class="btn-base ml-1 mr-1" onclick="uiSave()">Save</div>`;
    uiAdjSerface += `<div id="btn-ui-cancel" class="btn-base ml-1 mr-1" onclick="uiClose()">Cancel</div>`;
    createWnd('UiSettings', 0, 0, 250, 150, uiAdjSerface);
}
function uiDefault() {
    document.getElementById('color-adj-bg').value='#FFFFFF';
    document.getElementById('color-adj-text').value='000000';
}
function uiSave() {
    let colorBg = document.getElementById('color-adj-bg').value;
    let colorText = document.getElementById('color-adj-text').value;
    let customUi = document.getElementById('custom-ui');
    if (customUi != undefined) {
        let uiContext = `:root{`;
        uiContext += `--theme-bg-color:${colorBg};`
        uiContext += `--theme-text-color:${colorText};`;
        uiContext += `}`;
        customUi.innerHTML = uiContext;
        let dataJson = {};
        dataJson["bgColor"] = colorBg;
        dataJson["textColor"] = colorText;
        $.ajax({
            url: '/api/save/ui',
            data: JSON.stringify(dataJson),
            contentType: "application/json;charset=utf-8",
            dataType: 'JSON',
            type: 'POST',
            success:
                function (result) {
                    console.log(result);
                }
        });
    }
}
function uiClose() {
    let wnd = document.getElementById('wnd-UiSettings');
    wnd.remove();
}

//Tag save
function loadUserTag() {
    $.ajax({
        url: '/api/get/userTag',
        type: 'GET',
        success:
            function (data) {
                document.getElementById('all-tag-list').innerHTML = '';
                for (let t of data) {
                    document.getElementById('all-tag-list').innerHTML
                        += addTagOption(t.tagId, t.tagName);
                }
            },
    })
}

//-------------NavItem-------------------
function userOption(method, msg) {
    let user_opt = document.getElementById('user-option');
    if (method == 'login') {
        closeModal('LoginModal');
        user_opt.innerHTML = navUserTabLogin(msg);
    }
}
function navUserTabLogin(user) {
    let userId = user[0];
    let userName = user[1];
    let photo = user[2];
    let tab = `<div id="user-now" style="height:0; visibility:hidden;">${userId}</div>`;

    tab += `<div id="all-notify" class="btn-group">`;
    tab += `<button class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">Notify`;
    tab += `<span id="notify-unread" class="badge rounded-pill bg-danger notify-spot"></span></button>`;
    tab += `<ul id="notify-list" class="dropdown-menu" aria-labelledby="dropdownMenuLink"><li><a id="">Load</a></li></ul></div>`;

    tab += `<div id="show-chat-list" class="btn-group">`;
    tab += `<button id="nav-chat-dropdown-btn" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false" data-bs-auto-close="false">`;
    tab += `Msgs<span id="pill-nav-msg-unread" class="badge rounded-pill bg-danger notify-spot"></span></button>`;
    tab += `<ul id="chat-list" class="dropdown-menu" aria-labelledby="dropdownMenuLink">`;
    tab += `<li class="chatListItem"><a id="LoadGroupBtn" onclick="GetChatList()">Load</a></li></ul></div>`;

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

//nav events
function navbarClickEvent(event) {
    let evtTargetId = event.target.id;
    let strEvtTargetId = "";
    let itemTargetId = "";
    if (evtTargetId != undefined) {
        strEvtTargetId = String(evtTargetId);
        if (strEvtTargetId.startsWith('chat_trigger')) {
            itemTargetId = getSubStringAfter(strEvtTargetId, 'chat_trigger_');
            if (itemTargetId.length > 0) {
                let myModalBtn = document.getElementById(`chatModalBtn`);
                myModalBtn.click();
                let myModalEl = document.getElementById(`chatModalTab_${itemTargetId}`);
                myModalEl.click();
            }
        }
        if (strEvtTargetId.startsWith('chat_invite')) {
            itemTargetId = getSubStringAfter(strEvtTargetId, 'chat_invite_');
            if (itemTargetId.length > 0) {
                let invitedUserId = itemTargetId;
                NewPrivateGroup(invitedUserId);
            }
        }
        if (strEvtTargetId == 'btn-load-old-notify') {
            GetOldNotify();
        }
        if (strEvtTargetId == 'nav-user-tag-modify') {
            navLoadTagList();
            let myModalEl = document.getElementById('btnNavTagModifyModal');
            myModalEl.click();
        }
        if (strEvtTargetId == 'nav-btn-create-tag') {
            let tagList = document.getElementsByClassName('nav-tag-options');
            let idOfSelectedTags = [];
            for (let tags of tagList) {
                if (tags != undefined && tags.checked) {
                    idOfSelectedTags.push(String(tags.id));
                }
            }
            let tagName = document.getElementById('nav-tag-create-input').value;
            document.getElementById('nav-tag-create-input').value = "";
            createTag(tagName);
            navLoadTagList();
            let tagList2 = document.getElementsByClassName('nav-tag-options');
            for (let tId of idOfSelectedTags) {
                for (let t2 of tagList2) {
                    if (tId == String(t2.id)) {
                        t2.checked;
                    }
                }
            }
            setTimeout(500);
            saveUserTagOption();
        }
        if (strEvtTargetId == 'nav-tag-save-btn') {
            let uT = getUserTagSelected();
            saveUserTagOption(uT);
        }
        if (strEvtTargetId == 'btn-nav-search') {
            let searchKeyword = document.getElementById('input-nav-search').value;
            let searchOption = document.getElementsByClassName('nav-search-option');
            let selectedOption;
            for (let opt of searchOption) {
                if (opt.selected) {
                    selectedOption = opt;
                    break;
                }
            }
            document.getElementById('btnNavSearchModal').click();
            if (selectedOption == undefined) { return; }
            if (searchKeyword.length > -1 && selectedOption.id != 'nav-not-selected') {
                let searchOpts = {};
                searchOpts["option"] = selectedOption.value;
                searchOpts["keyword"] = searchKeyword;
                console.log(searchOpts);
                $.ajax({
                    url: '/api/search',
                    data: JSON.stringify(searchOpts),
                    contentType: "application/json;charset=utf-8",
                    dataType: 'JSON',
                    type: 'POST',
                    success:
                        function (result) {
                            console.log(result);
                            if (result == undefined) { return; }
                            let resultDisplay = document.getElementById('nav-search-result');
                            resultDisplay.innerHTML = '';              
                            for (let r of result) {
                                if (selectedOption.value == 'Post') {
                                    let auId = r.authorId;
                                    let mName = r.title;
                                    let url = '/Post/Index?pid=' + r.postId;
                                    resultDisplay.innerHTML += navSearchResultPost(mName, url, auId);
                                }
                                if (selectedOption.value == 'Forum') {
                                    let mName = r.forumName;
                                    let url = '/Forum/Index?fid=' + r.forumId;
                                    resultDisplay.innerHTML += navSearchResultForum(mName, url);
                                }
                                if (selectedOption.value == 'Work') {
                                    let imgUrl = r.fileUrl;
                                    let mName = r.workName;
                                    let url = '/Work/Index?wid=' + r.workId;
                                    resultDisplay.innerHTML += navSearchResultWork(mName, url, imgUrl);
                                }                               
                            }
                        }
                });
            }
        }
    }
}
function navLoadTagList() {
    $.ajax({
        url: '/api/get/tagList',
        type: 'GET',
        success:
            function (data) {
                document.getElementById('nav-all-tag-list').innerHTML = '';
                for (let t of data) {
                    document.getElementById('nav-all-tag-list').innerHTML
                        += addNavTagOption(t.tagId, t.tagName);
                }
            },
    })
    navLoadUserTagSelection();
}
function navTagSearchFilter() {
    let inputText = document.getElementById('nav-tag-create-input').value;
    let tagList = document.getElementsByClassName('nav-tag-options');
    for (let t of tagList) {
        let tagValue = String(t.value).toLowerCase();
        let inputValue = String(inputText).toLowerCase();
        if (!tagValue.includes(inputValue)) {
            t.parentElement.style.visibility = 'hidden';
            t.parentElement.style.height = '0';
        }
        else {
            t.parentElement.style.visibility = '';
            t.parentElement.style.height = '';
        }
    }
}
function navLoadUserTagSelection() {
    let userId = document.getElementById('user-now').innerHTML;
    let dataJson = {};
    dataJson["uid"] = String(userId);
    $.ajax({
        url: '/api/get/userTag',
        data: JSON.stringify(dataJson),
        contentType: "application/json;charset=utf-8",
        dataType: 'JSON',
        type: 'POST',
        success:
            function (selectedTag) {
                console.log(selectedTag);
                let alltags = document.getElementsByClassName('nav-tag-options');
                for (let t of alltags) {
                    for (let st of selectedTag) {
                        if (String(t.id) == String(st.tagId)) {
                            t.checked = true;
                        }
                    }
                }
            },
    })
}
function getUserTagSelected() {
    let workId = document.getElementById('user-now').innerHTML;
    let selectedTag = [];
    let checkedSelections = document.getElementsByClassName('nav-tag-options');
    for (let s of checkedSelections) {
        if (s.checked) {
            let tagData = String(s.id);
            selectedTag.push(tagData);
        }
    }
    let dataJson = {};
    dataJson["uid"] = String(workId);
    dataJson["tags"] = selectedTag;
    return JSON.stringify(dataJson);
}
function saveUserTagOption(tagData) {
    console.log(tagData);
    $.ajax({
        url: '/api/set/userTag',
        data: tagData,
        contentType: "application/json;charset=utf-8",
        dataType: 'JSON',
        type: 'POST',
        success:
            function (data) {
                if (String(data) == 'ErrorNotLogin') {
                    alert('You not login!');
                }
                else {
                    console.log(data);
                    navLoadUserTagSelection();
                }
            },
    });
}
function addNavTagOption(tagId, tagName) {
    return `<div><input id="${tagId}" class="nav-tag-options" value="${tagName}" type="checkbox"/> ${tagName}</div>`
}
function navSearchResultForum(modelName,url) {
    let r = `<a href='${url}' class="forum-list-content">`;
    r += `<div style="width:100px; display:inline-block;">`;
    r += `</div>`;
    r += `<div style="display:inline-block;  height:100%; line-height:50px;">${modelName}</div>`;
    r += `</a>`;
    return r;
}
function navSearchResultWork(modelName, url, furl) {
    let r = `<a href='${url}' class="forum-list-content">`;
    r += `<div style="width:100px; display:inline-block; text-align:center;">`;
    r += `<img class="img-fluid" style="height:50px;" src='/_GodchData/Works/${furl}' />`;
    r += `</div>`;
    r += `<div style="display:inline-block; verticla-align:sub;">${modelName}</div>`;
    r += `</a>`;
    return r;
}
function navSearchResultPost(modelName, url, auId) {
    let r = `<a href='${url}' class="forum-list-content">`;
    r += `<div style="width:100px; display:inline-block; text-align:center;">`;
    r += `<img class="img-fluid" style="height:50px;" src='/_GodchData/UserData/${auId}/photo.jpg' />`;
    r += `</div>`;
    r += `<div style="display:inline-block; verticla-align:sub;">${modelName}</div>`;
    r += `</a>`;
    return r;
}

//profile events
function profileClickEvent(event) {
    let tid = event.target.id;
    let tId = String(tid);
    let modalBdy = document.getElementById('modify-content');
    if (tId.startsWith('change')) {
        let targetItem = getSubStringAfter(tId, 'change-');
        document.getElementById('modify-content').innerHTML = addValidationInput(`${targetItem}_modify`, `${targetItem}`);
        $('#modify-window').modal('toggle');
        let originalVal = document.getElementById(`show-${targetItem}`).innerText;
        document.getElementById(`input-${targetItem}_modify`).value = originalVal;
    }
    if (tId == 'acc-page-format-check') {
        let inModalDiv = modalBdy.firstElementChild;
        let modify_name = inModalDiv.firstElementChild.innerText;
        let inputBox = inModalDiv.getElementsByTagName('input')[0];
        let inputText = inputBox.value;
        let validationItem = document.getElementById('label-feedback');
        if (inputText.length < 1) {
            document.getElementById('save-btn').remove();
            document.getElementById('acc-modal-footer').innerHTML += addDisabledBtn('save-btn', 'btn-base-disable', 'Save changes');
            inputBox.className ='form-control is-invalid'
            validationItem.className = 'invalid-feedback';
            validationItem.innerText = 'invalid ' + modify_name;
        }
        else {
            document.getElementById('save-btn').remove();
            document.getElementById('acc-modal-footer').innerHTML += addBtn('save-btn', 'btn-base', 'Save changes');
            inputBox.className = 'form-control is-valid'
            validationItem.className = 'valid-feedback';
            validationItem.innerText = 'valid ' + modify_name;
        }
    }
    if (tId == 'show-photo') {
        document.getElementById('upload-modal-content').innerHTML = '<img width="200" class="img-preview" id="upload-preview" />';
        document.getElementById('upload-modal-content').innerHTML += '<input id="img-upload" type="file" />';
        document.getElementById('upload-save-btn').innerText = 'Upload';
        $('#upload-modal-window').modal('toggle');
        $('#img-upload').change(() => {
            let oFReader = new FileReader();            
            let file = document.getElementById('img-upload').files[0];            
            oFReader.readAsDataURL(file);
            oFReader.onload = function (event) {
                let src = event.target.result;
                $('#upload-preview').attr('src', src);
            }
        });
    }
    if (tId=='save-btn') {
        let inModalDiv = modalBdy.firstElementChild;
        let modify_name = inModalDiv.firstElementChild.innerText;
        let inputBox = inModalDiv.getElementsByTagName('input')[0];
        let inputText = inputBox.value;
        if (inputText.length > 0) {
            ModifyProfile(modify_name, inputText);
        } 
    }
    if (tId == 'friend-tab') {
        LoadFriendList();
    }
    if (tId == 'follower-tab') {
        LoadFollowerList();
    }
    if (tId == 'following-tab') {
        LoadFollowingList();
    }
    if (tId == 'upload-save-btn') {
        var formData = new FormData();
        formData.append('filename', $('#img-upload').val());
        formData.append('file', $('#img-upload')[0].files[0]);
        $.ajax({
            url: 'http://localhost:11920/Account/PhotoUpload',
            data: formData,
            processData: false,
            contentType: false,
            type: 'POST',
            success:
            function(response) {
                alert(String(response));
            },
        })
    }
}
function ModifyProfile(modifiedName, content) {
    if (modifiedName == 'nick-name') {
        editNickName(content);
    }
    if (modifiedName == 'password') {
        editPassword(content);
    }
    if (modifiedName == 'email') {
        editEmail(content);
    }
    if (modifiedName == 'introduction') {
        editIntroduction(content);
    }
}

//forum events
function forumClickEvent(event) {
    let tid = event.target.id;
    let tId = String(tid);
    let modalBdy = document.getElementById('modify-content');
    if (tId.startsWith('psot_Edit_')) {
        let targetItem = getSubStringAfter(tId, 'psot_Edit_');
        let title = document.getElementById('post-family-title').innerText;
        document.getElementById('modify-content').innerHTML = addTextArea(`${targetItem}_modify`, `${targetItem}`, title);
        $('#modify-window').modal('toggle');
        let originalVal = document.getElementById(`post_text_content_${targetItem}`).innerText;
        document.getElementById(`input-${targetItem}_modify`).value = originalVal;
        document.getElementById('save-btn').innerText = 'Save change';
    }
    if (tId.startsWith('psot_Reply_')) {
        let targetItem = getSubStringAfter(tId, 'psot_Reply_');
        let title = document.getElementById('post-family-title').innerText;
        document.getElementById('modify-content').innerHTML = addTextAreaWithShowTitle(`${targetItem}_reply`, `${targetItem}`, title);
        document.getElementById('save-btn').innerText = 'Reply';
        $('#modify-window').modal('toggle');
    }
    if (tId == 'btn-new-post') {
        let targetForumId = document.getElementById('forum-id').innerHTML;
        let targetForumName = document.getElementById('forum-name').innerHTML;
        document.getElementById('modify-content').innerHTML = addTextAreaPlusTitle(`new_post_${targetForumId}`, targetForumId, `New Post In ${targetForumName}`);
        document.getElementById('save-btn').innerText = 'Create post';
        $('#modify-window').modal('toggle');
    }
    if (tId == 'save-btn') {
        let inModalDiv = modalBdy.firstElementChild;
        let modify_name = inModalDiv.firstElementChild.innerHTML;
        let inputBox = inModalDiv.getElementsByTagName('textarea')[0];
        let inputText = inputBox.value;
        let btnTetx = document.getElementById('save-btn').innerText;
        if (inputText.length > 0) {
            if (btnTetx == 'Save change') {
                editPostContent(modify_name, inputText);
                document.createElement('input').checked
            }
            else if (btnTetx == 'Reply') {
                replyPost(modify_name, inputText);
            }
            else if (btnTetx == 'Create post') {
                let inputTitle = inModalDiv.getElementsByTagName('input')[0].value;
                createPost(modify_name, inputTitle, inputText);
            }
            else {               
            }
        }
    }
}

//work list event
function getSearchOptions() {
    document.getElementById('search-text-input').addEventListener('change', function () {
        let keyWords = document.getElementById('search-text-input').value;
        let options = document.getElementsByClassName('search-option');
        let selectedOption;
        for (let opt of options) {
            if (opt.selected) {
                selectedOption = opt;
                break;
            }
        }
        if (keyWords.length > -1 && selectedOption.id != 'not-selected') {
            let opts = document.getElementsByClassName('search-option');
            let searchOpt = {};
            for (let opt of opts) {
                if (opt.selected) {
                    searchOpt["option"] = opt.id;
                    searchOpt["keyword"] = keyWords;
                    break;
                }
            }
            getSearchResult(searchOpt);
            
        }
    });
}
function getSearchResult(searchOption) {
    let option = searchOption.option;
    let keyWord = searchOption.keyword;
    let postData = {};
    postData["target"] = 'work';
    postData["method"]='search';
    postData["parameter"] = String(option);
    postData["id"] = String(keyWord);
    console.log(postData);
    $.ajax({
        url: '/api/search/works',
        data: JSON.stringify(postData),
        contentType: "application/json;charset=utf-8",
        dataType: 'JSON',
        type: 'POST',
        success:
            function (result) {
                console.log(postData);
                let wList = document.getElementById('work-list');
                wList.innerHTML = '';
                if (result != undefined) {
                    for (let r of result) {
                        wList.innerHTML += addWorkView(r.id, r.workName, r.authorName, r.description, r.fileUrl);
                        console.log(r);
                    }
                }
            },
    });
} 
function addWorkView(workId,workName,authorName,workDescription,workFileUrl) {
    let wView = `<a class="view-box" id="w_${workId}" href="/Work/Index?wid=${workId}">`;
    wView += `<div class="img-box"><img class="main-img" src='/_GodchData/Works/${workFileUrl}' /></div>`;
    wView += `<div class="about-work"><div class="workName row"><div class="col">${workName}</div>`;
    wView += `<div class="col"></div><div class="col-auto">by ${authorName}</div></div>`;
    wView += `<div class="work-details row"><div class="col"></div><div class="col">${workDescription}</div>`
    wView += `<div class="col"></div></div></div></a>`;
    return wView;
}

//work view events
function workClickEvent(event) {
    let tid = event.target.id;
    let tId = String(tid);
    if (tId == 'send-comment') {
    }
    if (tId == 'btn-add-tag') {
        LoadTagList();
        $('#tag-modal-window').modal('toggle');
    }
    if (tId == 'btn-create-tag') {
        let tagList = document.getElementsByClassName('tag-options');
        let idOfSelectedTags = [];
        for (let tags of tagList) {
            if (tags != undefined && tags.checked) {
                idOfSelectedTags.push(String(tags.id));
            }            
        }        
        let tagName = document.getElementById('tag-create-input').value;
        document.getElementById('tag-create-input').value = "";
        createTag(tagName);
        LoadTagList();
        let tagList2 = document.getElementsByClassName('tag-options');
        for (let tId of idOfSelectedTags) {
            for (let t2 of tagList2) {
                if (tId == String(t2.id)) {
                    t2.checked;
                }
            }
        }
        setTimeout(500);
        LoadWorkTagSelection();
    }
    if (tId == 'tag-save-btn') {
        let opts = getTagSelected();
        saveTagOption(opts);
    }
}
function searchFilter() {
    let inputText = document.getElementById('tag-create-input').value;
    let tagList = document.getElementsByClassName('tag-options');
    for (let t of tagList) {
        let tagValue = String(t.value).toLowerCase();
        let inputValue = String(inputText).toLowerCase();
        if (!tagValue.includes(inputValue)) {
            t.parentElement.style.visibility = 'hidden';
            t.parentElement.style.height = '0';
        }
        else {
            t.parentElement.style.visibility = '';
            t.parentElement.style.height = '';
        }
    }
}
function LoadTagList() {    
    $.ajax({
        url: '/api/get/tagList',
        type: 'GET',
        success:
            function (data) {
                document.getElementById('all-tag-list').innerHTML = '';
                for (let t of data) {
                    document.getElementById('all-tag-list').innerHTML
                        += addTagOption(t.tagId, t.tagName);
                }
            },
    })
    LoadWorkTagSelection();
}
function LoadWorkTagSelection() {
    let workId = document.getElementById('workId').innerHTML;
    let dataJson = {};
    dataJson["wid"] = String(workId);
    $.ajax({
        url: '/api/get/workTags',
        data: JSON.stringify(dataJson),
        contentType: "application/json;charset=utf-8",
        dataType: 'JSON',
        type: 'POST',
        success:
            function (selectedTag) {                
                let alltags = document.getElementsByClassName('tag-options');
                for (let t of alltags) {
                    for (let st of selectedTag) {
                        if (String(t.id) == String(st.id)) {
                            t.checked = true;                            
                        }
                    }
                }
                setTagDisply(selectedTag);
            },
    })
}
function getTagSelected() {
    let workId = document.getElementById('workId').innerHTML;
    let selectedTag = [];
    let checkedSelections = document.getElementsByClassName('tag-options');
    for (let s of checkedSelections) {
        if (s.checked) {
            let tagData = String(s.id);
            selectedTag.push(tagData);
        } 
    }
    let dataJson = {};
    dataJson["wid"] =String(workId);
    dataJson["tags"] = selectedTag;
    return JSON.stringify(dataJson);
}
function saveTagOption(tagData) {
    $.ajax({
        url: '/api/set/tag',
        data: tagData,
        contentType: "application/json;charset=utf-8",
        dataType: 'JSON',
        type: 'POST',
        success:
            function (data) {
                if (String(data) == 'ErrorNotLogin') {
                    alert('You not login!');
                }
                else {
                    console.log(data);
                    setTagDisply(data.selectedTags);
                }
            },
    });
}
function addTagOption(tagId, tagName) {
    return `<div><input id="${tagId}" class="tag-options" value="${tagName}" type="checkbox"/> ${tagName}</div>`
}
function setTagDisply(selectedTagList) {
    let taglist = document.getElementById('related-tags');
    taglist.innerHTML = '';
    for (let st of selectedTagList) {
        taglist.innerHTML += `<span>[${st.tagName}]</span>`;
    }   
}

