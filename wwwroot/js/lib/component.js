var bsClassName = {};
bsClassName.pill = 'badge rounded-pill bg-danger ';
bsClassName.modal = {};

//create Element
function newElement(tabName, idName, className, innerText) {
    let element = `<${tabName} id="${idName}" class="${className}">${innerText}</${tabName}>`
    return element
}
function newPill(idName, className, innerText) {
    return newElement('span', idName, bsClassName.pill + className, innerText);
}
function addInput(idName, labelText) {
    let input = `<div class="mt-3">`;
    input += ` <label for="label-${idName}" class="form-label mb-3">${labelText}</label>`;
    input += `<input type="text" class="form-control" id="input-${idName}">`
    input += `</div>`
    return input;
}
function addValidationInput(idName, labelText) {
    let input = `<div class="mt-3">`;
    input += ` <label for="label-${idName}" class="form-label mb-3" style="display:block;">${labelText}</label>`;
    input += `<input type="text" class="form-control is-invalid" style="display:inline-block; width:66%;" id="input-${idName}" required>`;
    input += `<button id="acc-page-format-check" type="button" class="btn-base ml-5p" style="display:inline-block;">Check</button>`
    input += `<div id="label-feedback" class="invalid-feedback">`;    
    input += `</div>`;    
    input += `</div>`;
    return input;
}
function addTextArea(idName, labelText, titleDisplay) {
    let input = `<div class="mt-3">`;
    input += ` <div style="width:0!important; height:0!important; visibility:hidden!important;">${labelText}</div>`;
    input += ` <label for="label-${idName}" class="form-label mb-3" style="display:block;">${titleDisplay}</label>`;
    input += `<textarea type="text" class="form-control" style="display:inline-block; width:100%;" id="input-${idName}">`;
    input += `<button id="acc-page-format-check" type="button" class="btn-base ml-5p" row="5" style="display:inline-block;">Check</button>`
    input += `</div>`;
    return input;
}
function addTextAreaWithShowTitle(idName, labelText, titleDisplay) {
    let input = `<div class="mt-3">`;
    input += ` <div style="width:0!important; height:0!important; visibility:hidden!important;">${labelText}</div>`;
    input += ` <label for="label-${idName}" class="form-label mb-3" style="display:block;">${titleDisplay}</label>`;
    input += `<textarea type="text" class="form-control" style="display:inline-block; width:100%;" id="input-${idName}">`;
    return input;
}
function addTextAreaPlusTitle(idName, labelText, titleDisplay) {
    let input = `<div class="mt-3">`;
    input += ` <div style="width:0!important; height:0!important; visibility:hidden!important;">${labelText}</div>`;
    input += ` <label for="label-${idName}" class="form-label mb-3" style="display:block;">${titleDisplay}</label>`;
    input += `<input type="text" class="form-control mb-2" style="display:inline-block; width:100%;" id="input-title-${idName}">`;
    input += `<textarea type="text" class="form-control" style="display:inline-block; width:100%;" id="input-content-${idName}">`;
    return input;
}
function addBtn(idName, className, innerText) {
    let btn = `<button id="${idName}" type="button" class="${className}">${innerText}</button>`;
    return btn;
}
function addDisabledBtn(idName, className, innerText) {
    let btn = `<button id="${idName}" type="button" class="${className}" disabled>${innerText}</button>`;
    return btn;
}


//changeValue
function setInnerText(idName, value) {
    let target = document.getElementById(idName);
    if (target != undefined) {
        document.getElementById(idName).innerText = value;
    }
}

//Elememt Function
function getEvtTargetId(event) {
    let target = event.target;
    if (target == undefined) {
        return "";
    }
    let targetId = event.target.id;
    if (targetId == undefined) {
        return "";
    }
    return String(targetId);
}
//String Function
function getSubStringAfter(string, compared) {
    let stringType = String(string);
    let stringCompared = String(compared);
    let strLength = stringType.length;
    if (strLength < 1 || stringCompared.length < 1 || stringCompared.length > strLength) {
        return "";
    }
    let result = "";
    result = stringType.substring(stringType.indexOf(stringCompared) + stringCompared.length, stringType.length);
    if (result != undefined) {
        return result;
    }
    else {
        return "";
    }
}

//Json Function
function toJson(method, itemName, itemId) {
    let dataJson = {};
    let jsonString;
    dataJson["method"] = String(method);
    dataJson["item"] = String(itemName);
    dataJson["id"] = String(itemId);
    jsonString = JSON.stringify(dataJson);
    return jsonString;
}
function postRequest(url, jsonString) {
    $.ajax({
        url: url,
        data: jsonString,
        contentType: "application/json;charset=utf-8",
        type: 'POST',
        success:
            function (response) {
                return readRequest(response);
            },
    });
}
function readRequest(request) {
    var temp;
    temp = request;
    console.log(temp);
}

//Create navigator Tab
function navTab(GroupIdName, buttonGroupContent, targetGroupContent) {
    var nt = `<ul class="nav nav-tabs" id="${GroupIdName}" role="tablist">`;
    nt += buttonGroupContent;
    nt += "</ul>\n";
    var ntc = `<div class="tab-content" id="${GroupIdName}Content">`;
    ntc += targetGroupContent;
    ntc += "</div>\n";
    return nt + ntc;
}
//Create a <a> basic button
function smallButton(url, innerText, className) {
    var btn = `<a class="${className}" href="${url}">${innerText}</a>`;
    return btn;
}
//Create a button with setting additional functions
function addFunctionalButton(innerText, className, bsToggleType, bsTargetIdName) {
    var btn = `<button class="${className}" type="button" `;
    btn += `data-bs-toggle="${bsToggleType}" data-bs-target="${bsTargetIdName}">`;
    btn += "\n" + innerText + "\n" + `</button>`;
    return btn;
}
//Create AddAccordion Element
function addAccordion(idName, accordionContent) {
    var accor = `<div class="accordion accordion-flush" id="${idName}">` + "\n";
    accor += accordionContent + "\n" + `</div>` + "\n";
    return accor;
}
//Create a empty Dropdown list
function addDropdownList(idName, buttonText, buttonClassName, listIdName, loadButtonIdName) {
    let item = `<div id="${idName}" class="btn-group">` + '\n';
    item += `<button class="btn ${buttonClassName} dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">` + '\n';
    item += `</button>` + '\n';
    item += `<ul id="${listIdName}" class="dropdown-menu" aria-labelledby="dropdownMenuLink">` + '\n';
    item += `<li class=""><a id="${loadButtonIdName}">${buttonText}</a></li>` + '\n';
    item += `</ul>` + '\n';
    return item;
}
//Bootstrap extension function
function closeModal(idName) {
    let myModalEl = document.getElementById(idName);
    let modal = bootstrap.Modal.getInstance(myModalEl);
    modal.dispose();
}

//Send the DocMsg
function sendDocMsg(target, msgType, msg) {
    var docMsg = [target, msgType, msg];
    parent.postMessage({ docMsg: docMsg }, '*');
}
//Translate the DocMsg
function recieveDocMsg(event) {
    let documentMessage = event.data.docMsg;
    let TargetId = String(documentMessage[0]);
    let Type = String(documentMessage[1]);
    let Msg = documentMessage[2];
    //myDocMsg(TargetId, Type, Msg);
    if (Type == 'changeText') {
        docMsgChangeText(TargetId, Msg);
    }
    if (Type == 'message') {
        docMsgGetMsg(Msg);
    }
    if (Type == 'resizeh') {
        docMsgResizeHeight(TargetId, Msg);
    }
    if (Type == 'resizew') {
        docMsgResizeWidth(TargetId, Msg);
    }
}
//Get TargetId in DocMsg
function targetOfDocMsg(event) {
    let documentMessage = event.data.docMsg;
    return String(documentMessage[0]);
}

//Do Actions from DocMsg
function docMsgChangeText(targetId, text) {
    let target = targetById(targetId);
    target.innerText = text;
}
function docMsgGetMsg(message) {
    return message;
}
function docMsgResizeHeight(targetId, height) {
    var target = targetById(targetId);
    target.style.height = (parseInt(height) + 40) + 'px';
}
function docMsgResizeWidth(targetId, width) {
    var target = targetById(targetId);
    target.style.width = (parseInt(width) + 40) + 'px';
}

//Functions for operating LocalStorage
function getLocalStorageContent(storageName) {
    let storage = [];
    let item = localStorage.getItem(storageName);
    if (item) {
        storage = JSON.parse(item);
    }
    return storage;
}
function addToLocalStorage(storageName, item, override) {
    Override = Boolean(override);
    let storage = [];

    if (Override) {
        storage = [];
        storage.push(item);
    }
    else {
        storage = localStorage.getItem(storageName);
        if (storage.length > 0) {
            let i = storage.find(item);
            if (i != null) {
                storage.push(item);
            }
        }
        else {
            storage = [];
            storage.push(item);
        }
    }
    localStorage.setItem(storageName, JSON.stringify(storage));
}
function addToLocalStorage(storageName, item) {
    addToLocalStorage(storageName, item, false);
}


//UsedItemCreateFunctions
function targetById(idName) {
    return document.getElementById(idName);
}
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
//Parts of navigator Tab
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
//Parts of accordion
function accordionGroup(idName, accordionOptionTitle, accordionOption) {
    var accItemGroup = `<div id="${idName}" class="accordion-item">` + "\n";
    accItemGroup += accordionOptionTitle + "\n" + `</div>` + "\n";
    return accItemGroup;
}
function accordionOptionTitle(idName, innerText, optionGroupIdName) {
    var accOpTitle = `<h2 id="${idName}" class="accordion-header">`;
    accOpTitle += addFunctionalButton(innerText, "accordion-button collapsed", "collapse", optionGroupIdName);
    accOpTitle += "\n" + `</h2>` + "\n";
    return accOpTitle;
}
function accordionOptions(idName, parentIdName, optionContent) {
    var accItem = `<div id="${idName}" class="accordion-collapse collapse" data-bs-parent="${parentIdName}">` + "\n";
    accItem += `<div class="accordion-body">` + "\n";
    accItem += optionContent + "\n";
    accItem += "</div>\n</div>";
    return accItem;
}
function accordionOption(idName, innerText) {
    var accBtn = `<button class="myBtn btn" id="${idName}">${innerText}</button>` + "\n";
    return accBtn;
}


