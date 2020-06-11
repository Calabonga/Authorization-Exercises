const manager = new Oidc.UserManager({
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }),
    loadUserInfo: true,
    response_mode: "query"
});

manager.signinRedirectCallback()
    .then(function (user) {
        console.log(user);
        window.location.href = "https://localhost:9001";
    })
    .catch(function (error) {
        console.log(error);
    });
