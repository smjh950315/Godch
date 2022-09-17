
function targetById(idName) {
    return document.getElementById(idName);
}

//Create Item
function addModal(buttonText, idName, headerContent, bodyContent, footerContent) {
    var mbutton = modalButton(idName, buttonText);
    var mbody = modalHeader(headerContent) + modalBody(bodyContent) + modalFooter(footerContent);
    return mbutton + modalMain(idName, mbody);
}
function addModalButton(buttonText, idName) {
    var mbutton = modalButton(idName, buttonText);
    return mbutton;
}
function addModalBody(idName, headerContent, bodyContent, footerContent) {
    var mbody = modalHeader(headerContent) + modalBody(bodyContent) + modalFooter(footerContent);
    return modalMain(idName, mbody);
}
function navTab(GroupIdName, buttonGroupContent, targetGroupContent) {
    var nt = `<ul class="nav nav-tabs" id="${GroupIdName}" role="tablist">`;
    nt += buttonGroupContent;
    nt += "</ul>\n";
    var ntc = `<div class="tab-content" id="${GroupIdName}Content">`;
    ntc += targetGroupContent;
    ntc += "</div>\n";
    return nt + ntc;
}

//Bootstrap extension function
function closeModal(idName) {
    var myModalEl = document.getElementById('Modal');
    var modal = bootstrap.Modal.getInstance(myModalEl);
    modal.dispose();
}

//UsedFunctions
function createItem(tabName, className, attribute, idName, content) {
    var item = "";
    item += `<${tabName} class="${className}" id="${idName}" ${attribute}>` + "\n";
    item += content + "\n";
    item += `</${tabName}>` + "\n";
    return item;
}
function modalButton(idName, text) {
    var btn = "\n";
    btn += `<a class="btn" id="${idName}" data-bs-toggle="modal" data-bs-target="#modal_${idName}">${text}</a>` + "\n";
    return btn;
}
function modalMain(idName, bodyContent) {
    var mbody = "";
    mbody += `<div class="modal fade" id="modal_${idName}" tabindex="-1" `;
    mbody += `aria-labelledby="${idName}" aria-hidden="true">` + "\n";
    mbody += `<div class="modal-dialog">` + "\n" + `<div class="modal-content">` + "\n";
    mbody += bodyContent;
    mbody += "\n" + `</div></div></div>`;
    return mbody;
}
function modalHeader(content) {
    var header = "";
    header += `<div class="modal-header">` + "\n";
    header += content + "\n";
    header += `</div>` + "\n";
    return header;
}
function modalBody(content) {
    var body = "";
    body += `<div class="modal-body">` + "\n";
    body += content + "\n";
    body += `</div>` + "\n";
    return body;
}
function modalFooter(content) {
    var footer = "";
    footer += `<div class="modal-footer">` + "\n";
    footer += content + "\n";
    footer += `</div>` + "\n";
    return footer;
}
function cardMain(idName, bodyContent) {
    var cbody = "";
    cbody += `<div class="card" id="${idName}">` + "\n";
    cbody += bodyContent + "\n";
    cbody += `</div>` + "\n";
    return cbody;
}
function cardHeader(content) {
    var cbody = "";
    cbody += `<div class="card-header">` + "\n";
    cbody += content + "\n";
    cbody += `</div>` + "\n";
    return cbody;
}
function cardBody(content) {
    var cbody = "";
    cbody += `<div class="cardBody">` + "\n";
    cbody += content + "\n";
    cbody += `</div>` + "\n";
    return cbody;
}
function cardFooter(content) {
    var cbody = "";
    cbody += `<div class="cardFooter">` + "\n";
    cbody += content + "\n";
    cbody += `</div>` + "\n";
    return cbody;
}
function navTabFirstButton(idName, targetId, text) {
    var ntfb = `<li class="nav-item">`;
    ntfb += `<button class="nav-link active" id="${idName}" data-bs-toggle="tab" data-bs-target="#${targetId}" type="button">`;
    ntfb += text + "</div>\n</li>";
    return ntfb;
}
function navTabFirstContent(idName, content) {
    var ntfc = `<div class="tab-pane fade show active" id="${idName}">` + "\n";
    ntfc += content + "\n</div>\n";
    return ntfc;
}
function navTabButton(idName, targetId, text) {
    var ntb = `<li class="nav-item">`;
    ntb += `<button class="nav-link" id="${idName}" data-bs-toggle="tab" data-bs-target="#${targetId}" type="button" >`;
    ntb += text + "</div>\n</li>";
    return ntb;
}
function navTabContent(idName, content) {
    var ntc = `<div class="tab-pane fade" id="${idName}">` + "\n";
    ntc += content + "\n</div>\n";
    return ntc;
}
function notifyComponent(idName) {
    var ntf = `<span id="${idName}" class="badge rounded-pill bg-danger notify-spot"> </span>`;
    return ntf;
}

//Document Message
function sendDocMsg(target, msgType, msg) {
    var docMsg = [target, msgType, msg];
    parent.postMessage({ docMsg: docMsg }, '*');
}
function recieveDocMsg(event) {
    var documentMessage = event.data.docMsg;
    var TargetId = String(documentMessage[0]);
    var Type = String(documentMessage[1]);
    var Msg = documentMessage[2];
    myDocMsg(TargetId, Type, Msg);
    if (Type == 'changeText') {
        docMsgChangeText(TargetId, Msg);
    }
    if (Type == 'message') {
        docMsgGetMsg(Msg);
    }
    if (Type == 'resizeh') {
        docMsgResizeHeight(TargetId, Msg);
    }
}
function targetOfDocMsg(event) {
    var documentMessage = event.data.docMsg;
    return String(documentMessage[0]);
}

//Document Message Type
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
function docMsgChangeText(targetId, text) {
    var target = targetById(targetId);
    target.innerText = text;
}
function docMsgGetMsg(message) {
    return message;
}
function docMsgResizeHeight(targetId, height) {
    var target = targetById(targetId);
    target.style.height = (parseInt(height) + 40) + 'px';
}
