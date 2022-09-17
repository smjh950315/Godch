/// <reference path="lib/component.js" />
/// <reference path="build-in.bundle.js" />
//TEMP
function Launch() {
    let url = `get`;
    let data = {};

    console.log(data);
    GetTest(url);
    PostTest(url);
}
function ShowSent(data) {
    apiUrl = `http://localhost:11920/api/show`;
    $.ajax({
        url: apiUrl,
        data: JSON.stringify(data),
        contentType: "application/json;charset=utf-8",
        type: 'POST',
        dataType: 'JSON',
        success:
            function (data) {
                console.log(data);
            },
    });
}
function GetTest(url) {
    apiUrl = `http://localhost:11920/api/${url}`;
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success:
            function (data) {
                console.log(data);
            },
    });
}
function PostTest(url,data) {
    apiUrl = `http://localhost:11920/api/${url}`;
    $.ajax({
        url: apiUrl,
        data: JSON.stringify(data),
        contentType: "application/json;charset=utf-8",
        type: 'POST',
        dataType: 'JSON',
        success:
            function (request) {
                console.log(request);
                document.getElementById('result').innerHTML = request.data;
            },
    });
}
