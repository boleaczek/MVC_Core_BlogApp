function navigate(appId) {
    return new Vue({
        el: appId,
        methods: {
            scrollTo: function (destination) {
                let container = document.querySelector(destination);
                console.log("inn");
                container.scrollIntoView();
            }
        }
    });
}