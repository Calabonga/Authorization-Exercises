document.getElementById("login").addEventListener("click", login);

var settings = {
    authority: "https://localhost:10001",
    client_id: "client_id_js",
    response_type: "code",
    scope: "openid profile OrdersAPI",

    redirect_uri: "https://localhost:9001/callback.html"
}

var manager = new Oidc.UserManager(settings);

manager.getUser().then(function (user) {
    if (user) {
        print("Log in success", user);
    } else {
        print("User not logged in");
    }
});


function login() {
    manager.signinRedirect();
}


function print(message, data) {
    if (message) {
        document.getElementById("message").innerText = message;
    } else {
        document.getElementById("message").innerText = "";
    }
    if (data && typeof data === "object") {
        document.getElementById("data").innerText = JSON.stringify(data, null, 2);
    } else {
        document.getElementById("data").innerText = "";
    }
}