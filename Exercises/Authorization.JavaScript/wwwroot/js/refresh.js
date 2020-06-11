new Oidc.UserManager({
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage })
}).signinSilentCallback();