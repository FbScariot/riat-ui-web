"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/RIAT.UI.Web/chatHub", { useDefaultPath: false }).build();

// Calls when user successfully logged in
connection.on("Connected", function (id, userName, allUsers, messages) {

    $('#hdId').val(id);
    $('#hdUserName').val(userName);
    $('#spanUser').html(userName);

    // Add Existing Messages
    for (i = 0; i < messages.length; i++) {

        AddMessage(messages[i].UserName, messages[i].Message);
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

$('#btnSendMsg').click(function () {

    var msg = $("#txtMessage").val();
    if (msg.length > 0) {

        var userName = $('#hdUserName').val();
        connection.invoke("sendMessageToAll", userName, msg).catch(function (err) {
            return console.error(err.toString());
        });
        $("#txtMessage").val('');
    }
});

$('#btnSendMsgRoom').click(function () {

    var msg = $("#txtMessage").val();
    if (msg.length > 0) {

        var userName = $('#hdUserName').val();
        connection.invoke("sendMessageToGroup", userName, msg).catch(function (err) {
            return console.error(err.toString());
        });
        $("#txtMessage").val('');
    }
});

connection.on("messageReceived", function (userName, message) {
    AddMessage(userName, message);
});

function AddMessage(userName, message) {
    $('#divChatWindow').append('<div class="message"><span class="userName">' + userName + '</span>: ' + message + '</div>');

    var height = $('#divChatWindow')[0].scrollHeight;
    $('#divChatWindow').scrollTop(height);
}