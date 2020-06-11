document.getElementById("login").addEventListener("click", login);
document.getElementById("callApi").addEventListener("click", callApi);
document.getElementById("refresh").addEventListener("click", refresh);
document.getElementById("logout").addEventListener("click", logout);

var settings = {
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }),
    authority: "https://localhost:10001",
    client_id: "client_id_js",
    response_type: "code",
    scope: "openid profile OrdersAPI",

    redirect_uri: "https://localhost:9001/callback.html",
    silent_redirect_uri : "https://localhost:9001/refresh.html",
    post_logout_redirect_uri : "https://localhost:9001/index.html"
}

var manager = new Oidc.UserManager(settings);

manager.getUser().then(function (user) {
    if (user) {
        print("Log in success", user);
    } else {
        print("User not logged in");
    }
});

manager.events.addUserSignedOut(function() {
    print("User sing out. Good bye.");
});

function logout() {
    manager.signoutRedirect();
}

function refresh() {
    manager.signinSilent()
        .then(function (user) {
            print("Token refreshed", user);
        }).catch(function (error) {
            print("Something went wrong");
        });
}

function callApi() {
    manager.getUser().then(function (user) {
        if (user === null) {
            print("Unauthorized");
        }

        const xhr = new XMLHttpRequest();
        xhr.open("GET", "https://localhost:5001/site/secret");
        xhr.onload = function () {
            if (xhr.status === 200) {
                print(xhr.responseText, xhr.response);
            } else {
                print("Something went wrong", xhr);
            }
        }

        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();

    }).catch(function (error) {
        print(error);
    });
}


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