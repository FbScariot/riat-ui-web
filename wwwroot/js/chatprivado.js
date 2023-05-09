"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/RIAT.UI.Web/chatHub", { useDefaultPath: false }).build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

// Calls when user successfully logged in
connection.on("Connected", function (id, userName, allUsers, messages) {

    setScreen(true);

    $('#hdId').val(id);
    $('#hdUserName').val(userName);
    $('#spanUser').html(userName);

    // Add All Users
    for (i = 0; i < allUsers.length; i++) {

        AddUser(chatHub, allUsers[i].ConnectionId, allUsers[i].UserName);
    }

    // Add Existing Messages
    for (i = 0; i < messages.length; i++) {

        AddMessage(messages[i].UserName, messages[i].Message);
    }


});

$('#btnSendMsg').click(function () {

    var msg = $("#txtMessage").val();
    if (msg.length > 0) {

        var userName = $('#hdUserName').val();
        //chatHub.server.sendMessageToAll(userName, msg);
        connection.invoke("sendMessageToAll", userName, msg).catch(function (err) {
            return console.error(err.toString());
        });
        $("#txtMessage").val('');
    }
});


connection.on("ReceiveMessage", function (userName, message) {

    AddMessage(userName, message);
});

connection.on("messageReceived", function (userName, message) {

    AddMessage(userName, message);
});

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " disse " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("btnStartChat").addEventListener("click", function (event) {
    var name = document.getElementById("txtNickName").value;

    if (name.length > 0) {
        //connection.server.connect(name);
        connection.invoke("Connect", name).catch(function (err) {
            return console.error(err.toString());
        });

    }
    else {
        alert("Please enter name");
    }

});


function AddMessage(userName, message) {
    $('#divChatWindow').append('<div class="message"><span class="userName">' + userName + '</span>: ' + message + '</div>');

    var height = $('#divChatWindow')[0].scrollHeight;
    $('#divChatWindow').scrollTop(height);
}

function setScreen(isLogin) {

    if (!isLogin) {

        $("#divChat").hide();
        $("#divLogin").show();
    }
    else {

        $("#divChat").show();
        $("#divLogin").hide();
    }

}

function AddUser(chatHub, id, name) {

    var userId = $('#hdId').val();

    var code = "";

    if (userId == id) {

        code = $('<div class="loginUser">' + name + "</div>");

    }
    else {

        code = $('<a id="' + id + '" class="user" >' + name + '<a>');

        $(code).dblclick(function () {

            var id = $(this).attr('id');

            if (userId != id)
                OpenPrivateChatWindow(chatHub, id, name);

        });
    }

    $("#divusers").append(code);

}

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessage", message, false).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("sendPrivateButton").addEventListener("click", function (event) {
    var to = document.getElementById("toUserInput").value;
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendToUser", to, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});