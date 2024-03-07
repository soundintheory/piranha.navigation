/*global
    piranha
*/

piranha.navigation = piranha.navigation || {};
piranha.navigation.menuList = new Vue({
    el: "#menulist",
    data: {
        loading: true,
        items: []
    },
    methods: {
        bind: function (result) {
            this.items = result.menus;
        },
        load: function () {
            return fetch(piranha.baseUrl + "manager/api/navigation/menus/list")
                .then((response) => response.json())
                .then((result) => {
                    this.bind(result);
                    this.loading = false;
                })
                .catch((error) => { console.log("error:", error); });
        },
        remove: function (id) {
            /*
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deletePageConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/content/delete", {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders(),
                        body: JSON.stringify(id)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        self.load(self.group.id);
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
            */
        },
    },
    created: function () {
    }
});
