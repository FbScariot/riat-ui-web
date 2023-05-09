$(document).ready(function () {
    window.chat = createChatController();
    window.chat.loadUser();
});

function createChatController() {
    var user = {
        signalRConnectionId: null,
        //name: null,
        userId: null,
        oneSignalId: null,
        dtConnection: null
    }

    return {
        state: user,
        connection: null,
        loadUser: function () {
            this.state.signalRConnectionId = new Date().valueOf();
            this.state.userId = $('#hdUserId').val();
            //this.state.name = $('#hdName').val(); //prompt('Digite seu nome para entrar no chat', 'Usuário');            
            this.state.oneSignalId = $('#hdOneSignalId').val();
            this.state.dtConnection = new Date();
            this.connectUserToChat();
        },
        connectUserToChat: function () {
            //Aqui iniciamos a conexão e deixamos ela aberta
            startConnection(this);
        },
        sendMessage: function (to) {
            var chatMessage = {
                sender: this.state,
                message: to.message,
                receiverSignalRConnectionId: to.receiverSignalRConnectionId
            };

            //Esse trecho é responsável por encaminhar a mensagem para o usuário selecionado
            this.connection.invoke("SendMessage", (chatMessage))
                .catch(err => console.log(x = err));

            //Método responsável por inserir a mensagem no chat
            insertMessage(chatMessage.receiverSignalRConnectionId, 'me', chatMessage.message);
            to.field.val('').focus();
        },
        //Método responsável por receber as mensagens
        onReceiveMessage: function () {
            this.connection.on("Receive", (sender, message) => {
                openChat(null, sender, message);
            });
        }
    };
}

//Método responsável por realizar a conexão do usuário no nosso Hub
async function startConnection(chat) {
    try {

        //var OneSignal = window.OneSignal || [];
        //OneSignal.push(function () {
        //    OneSignal.init({
        //        appId: "03644759-a342-4958-9f26-2a032da0485e",
        //        notifyButton: {
        //            enable: true,
        //        },
        //        subdomainName: "rism-com",
        //        allowLocalhostAsSecureOrigin: true,
        //    });
        //    OneSignal.showNativePrompt();
        //});

        //chat.connection = new signalR.HubConnectionBuilder().withUrl("/RIAT.UI.Web/chat?user=" + JSON.stringify(window.chat.state)).build();

        var addressSite = document.location.origin;
        var urlSite = "";

        if (addressSite.indexOf("localhost") != -1) {
            urlSite = addressSite + "/RIAT.UI.Web";
            //urlSite = addressSite;
        }

        //alert(addressSite);

        //var connection = new signalR.HubConnectionBuilder('https://test2-vncloud.service.signalr.net/client/?hub=chathub')
        //    .withUrl('/chathub', { accessTokenFactory: () => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ1c2VyMSIsIm5iZiI6MTU2MzQ0MDI3MiwiZXhwIjoxNTYzNDQzODcyLCJpYXQiOjE1NjM0NDAyNzIsImF1ZCI6Imh0dHBzOi8vdGVzdDItdm5jbG91ZC5zZXJ2aWNlLnNpZ25hbHIubmV0L2NsaWVudC8_aHViPWNoYXRodWIifQ.7MrRtfdtKjSh7S87xgC_EoR_EldQpfg69LtsG24dqek" })
        //    .build();

        chat.connection = new signalR.HubConnectionBuilder(urlSite + '?hub=chat')
            .withUrl(urlSite + "/chat?user=" + JSON.stringify(window.chat.state), { accessTokenFactory: () => "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJqdGkiOiI1ZDI0YTQ4MzA2NjY0ZTBjYTI2OGRlMGMxYmVlMjk5YiIsInVuaXF1ZV9uYW1lIjoibWFyY2VsQHJpc20uY29tLmJyIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6Im1hcmNlbEByaXNtLmNvbS5iciIsInN1YiI6Im1hcmNlbEByaXNtLmNvbS5iciIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMzE2ZDFmNzQtNjNkMC00NGJjLWI4ZTAtZmUxYzBjM2Q5YmFhIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUGFjaWVudGVzIiwibmJmIjoxNjI1NzY0MTk5LCJleHAiOjE2NTczMDAxOTksImlzcyI6IkV4ZW1wbG9Jc3N1ZXIiLCJhdWQiOiJFeGVtcGxvQXVkaWVuY2UifQ.yMgSAXOgWyV92lCpXK9226e0WsnQKiV0sk2t4Z1bM7Xp_M2RQLnjsGlPC6hKQ5JUTc-k8SyAp8Sl6gNjNj-vPJD4iIEoSOX1gRheMzO4CFq8VdqOQHx5qL9BPQqddvAX8MUtGCeg5A6T0g0GntApoYVMXv_OnMCve-AWLciUOZOe6gwMQSs8mfDFzlTWUN2YhifzL86oUQ2x9o7wcV19_8EWBOdskFCXbqKUs7T_5Ma5IMh2jDNnqWsbqj_tsR7mFyADyJllf98G-om4Jeb2DpV6SKE-5kuIjKAB0cHQJpN6gPcEwkw1dOE3dOqD1NM-eVP4GdUZnvNEek9N_er8Ow" })
            .build();

        //chat.connection = new signalR.HubConnectionBuilder()
        //                             .withUrl(urlSite + "/chat?user=" + JSON.stringify(window.chat.state))
        //                             //.withUrl("/chat?user=" + JSON.stringify(window.chat.state) + "&access_token=124123")
        //                             .build();
        //$('.main').empty();

        await chat.connection.start();

        //Carrega usuários no chat
        loadChat(chat.connection);

        //Caso a conexão caia por algum motivo, esse trecho fará o trabalho para reconectar o cliente
        chat.connection.onclose(async () => {
            await startConnection(chat);
        });

        //Realiza o bind da nossa função para receber mensagem
        chat.onReceiveMessage();

    } catch (err) {
        setTimeout(() => startConnection(chat.connection), 5000);
    }
};

//Função para carregar usuários no chat
async function loadChat(connection) {
    connection.on('chat', (users, user) => {
        //if (window.chat.state.name === "") {
        //    window.chat.state = user;
        //}

        const listUsers = (data) => {
            return users.map((u) => {
                if (!checkIfElementExist(u.signalRConnectionId, 'id') && u.signalRConnectionId != window.chat.state.signalRConnectionId)
                    return (
                        `
                      <section class="user box_shadow_0" onclick="openChat(this)" data-id="${u.signalRConnectionId}" data-name="${u.name}">
                      <span class="user_icon">${u.name.charAt(0)}</span>
                      <p class="user_name"> ${u.name} </p>
                      <span class="user_date"> ${new Date(u.dtConnection).toLocaleDateString()}</span>
                      </section>
                      `
                    )
            }).join('')
        }

        //this.state.name = user.name;
        //this.state.signalRConnectionId = user.signalRConnectionId;
        //this.state.dtConnection = user.dtConnection;

        $('.main').append(listUsers);
    });
}

//Método responsável por iniciar um novo chat
function openChat(e, sender, message) {

    var user = {
        id: e ? $(e).data('id') : sender.signalRConnectionId,
        name: e ? $(e).data('name') : sender.name
    }

    if (!checkIfElementExist(user.id, 'chat')) {
        const chat =
            `
        <section class="chat" data-chat="${user.id}">
        <header>
            ${user.name}
        </header>
        <main>
        </main>
        <footer>
            <input type="text" placeholder="Digite aqui sua mensagem" data-chat="${user.id}">
            <a onclick="sendMessage(this)" data-chat="${user.id}">Enviar</a>
        </footer>
        </section>
        `

        $('.chats_wrapper').append(chat);
    }
    if (sender && message)
        insertMessage(sender.signalRConnectionId, 'their', message);
}

//Método responsável por inserir a mensagem no chat
function insertMessage(target, who, message) {
    const chatMessage = `
    <div class="message ${who}">${message} <span>${new Date().toLocaleTimeString()}</span></div>
    `;
    $(`section[data-chat="${target}"]`).find('main').append(chatMessage);
}

//Método responsável por capturar a mensagem e enviar
function sendMessage(e) {

    var input = {
        receiverSignalRConnectionId: $(e).data('chat'),
        field: $(`input[data-chat="${$(e).data('chat')}"]`),
        message: $(`input[data-chat="${$(e).data('chat')}"]`).val()
    }

    window.chat.sendMessage(input);
}

//Função genérica para verificar se o elemento já existe na DOM
function checkIfElementExist(id, data) {
    return $('section[data-' + data + '="' + id + '"]') && $('section[data-' + data + '="' + id + '"]').length > 0;
}