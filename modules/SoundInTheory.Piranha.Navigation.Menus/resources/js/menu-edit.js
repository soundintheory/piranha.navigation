/*global
    piranha
*/

piranha.navigation = piranha.navigation || {};

piranha.navigation.menuedit = new Vue({
    el: "#menuedit",
    data: {
        loading: true,
        menu: null,
        addSiteTitle: null,
        addToSiteId: null,
        addPageId: null,
        currentState: null
    },
    methods: {
        load: function (menuId) {
            this.baseApiUrl = piranha.baseUrl + "manager/api/navigation/menus/" + menuId;

            return fetch(this.baseApiUrl)
                .then((response) => response.json())
                .then((result) => {
                    if (result.menu) {
                        this.setMenu(result.menu);
                    }
                })
                .catch((error) => { console.log("error:", error); });
        },
        reload: function () {
            if (this.menu && this.menu.id) {
                this.load(this.menu.id);
            }
        },
        removeItem: function (id) {

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: "Are you sure you want to delete this item?",
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: () => {
                    fetch(this.baseApiUrl + "/items/" + id, {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders()
                    })
                    .then((response) => response.json())
                    .then((result) => {
                        piranha.notifications.push(result);
                        this.reload();
                    })
                    .catch((error) => { console.log("error:", error ); });
                }
            });
        },
        bind: function () {

            console.log('binding!');

            $(".menu-container").each((i, e) => {
                $(e).nestable({
                    maxDepth: this.menu.settings.maxDepth ? this.menu.settings.maxDepth : 100,
                    group: i,
                    onDragStart: (l, e) => {
                        document.documentElement.classList.add('dd-dragging');
                        this.currentState = JSON.stringify($(l).nestable("serialize"));
                    },
                    beforeDragStop: (l, e) => {
                        document.documentElement.classList.remove('dd-dragging');
                    },
                    callback: (l, e) => {

                        document.documentElement.classList.remove('dd-dragging');
                        var newState = $(l).nestable("serialize");
                        if (JSON.stringify(newState) === this.currentState) return;
                        
                        fetch(this.baseApiUrl + "/structure", {
                            method: "post",
                            headers: piranha.utils.antiForgeryHeaders(),
                            body: JSON.stringify({
                                menu: {
                                    id: this.menu.id,
                                    items: newState
                                }
                            })
                        })
                        .then((response) => response.json())
                        .then((result) => {
                            if (result.status.type === "success") {
                                this.setMenu(result.menu);
                            } else {
                                piranha.notifications.push(result.status);
                            }
                        })
                        .catch((error) => {
                            console.log("error:", error);
                        });
                    }
                });
            });
        },
        addItem: function (position) {

            piranha.navigation.linkmodal.open((model) => {

                fetch(this.baseApiUrl + "/items", {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify({
                        position: position,
                        item: {
                            link: model
                        }
                    })
                })
                .then((response) => { return response.json(); })
                .then((result) => {
                    if (result.status) {
                        piranha.notifications.push(result.status);

                        if (result.status.type === "success") {
                            this.setMenu(result.menu);
                        }
                    }
                })
                .catch((error) => {
                    console.log("error:", error);
                });

            }, null);
        },
        editItem: function(item) {

            piranha.navigation.linkmodal.open((model) => {

                item.link = model;

                fetch(this.baseApiUrl + "/items", {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify({
                        item: {
                            id: item.id,
                            link: item.link,
                            sortOrder: item.sortOrder,
                            parentId: item.parentId,
                            menuId: item.menuId
                        }
                    })
                })
                .then((response) => response.json())
                .then((result) => {
                    if (result.status) {
                        piranha.notifications.push(result.status);
                        if (result.status.type === "success") {
                            this.setMenu(result.menu);
                        }
                    }
                })
                .catch((error) => {
                    console.log("error:", error);
                });

            }, item.link);

        },
        setMenu: function (menu) {

            console.log('setMenu: ', menu);
            if (this.menu) {
                $('.menu-container').nestable('destroy');
                this.menu = null;
            }
            Vue.nextTick(() => {
                this.menu = menu;
                Vue.nextTick(() => {
                    this.bind();
                    this.loading = false;
                });
            });
        }
    },
    created: function () {
    }
});
