/*global
    piranha
*/

piranha.navigation = piranha.navigation || {};

piranha.navigation.menuedit = new Vue({
    el: "#menuedit",
    data: {
        loading: true,
        menu: null,
        availableItemTypes: [],
        addSiteTitle: null,
        addToSiteId: null,
        addPageId: null,
        currentState: null
    },
    provide: function() {
        return {
            root: this
        };
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
                    if (result.availableMenuItemTypes) {
                        this.availableItemTypes = result.availableMenuItemTypes;
                        piranha.navigation.linkmodal.setAvailableItemTypes(result.availableMenuItemTypes);
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

            $(".menu-container").each((i, e) => {
                $(e).nestable({
                    //maxDepth: this.menu.settings.maxDepth ? this.menu.settings.maxDepth : 100,
                    group: i,
                    onDragStart: (l, el) => {
                        document.documentElement.classList.add('dd-dragging');
                        this.currentState = JSON.stringify($(l).nestable("serialize"));
                        this.draggingItemType = this.availableItemTypes.find(x => x.id === $(el).data('item-type'));
                    },
                    beforeDragStop: (l, el, p) => {
                        const $parent = this.getParentNode(p);
                        const parentLevel = parseInt($parent.data('level'));

                        if (this.draggingItemType && $parent && $parent.length > 0) {

                            const parentItemType = this.availableItemTypes.find(x => x.id === $parent.data('item-type'));

                            if (this.draggingItemType.maxLevel <= parentLevel) {
                                return false;
                            }

                            if (this.draggingItemType.allowedParents && this.draggingItemType.allowedParents.length > 0 && !this.draggingItemType.allowedParents.includes(parentItemType.id)) {
                                return false;
                            }

                            if (parentItemType.allowedChildren && parentItemType.allowedChildren.length > 0 && !parentItemType.allowedChildren.includes(this.draggingItemType.id)) {
                                return false;
                            }
                        } else if (this.draggingItemType && (!$parent || $parent.length === 0)) {
                            if (this.draggingItemType.allowedParents && this.draggingItemType.allowedParents.length > 0 && !this.draggingItemType.allowedParents.includes("root")) {
                                return false;
                            }
                        }
                        
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
        addItem: function (typeId, position) {

            piranha.navigation.linkmodal.open((model) => {

                fetch(this.baseApiUrl + "/items", {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify({
                        position: position,
                        item: model
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

            }, { ['$typeId']: typeId });
        },
        editItem: function(item) {

            piranha.navigation.linkmodal.open((model) => {

                Object.assign(item, model);

                fetch(this.baseApiUrl + "/items", {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify({ item })
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

            }, item);

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
        },
        getParentNode: function (el) {
            return $(el).parents('[data-item-type].dd-item').first();
        }
    },
    computed: {
        availableTypes: function () {
            let typeIds = this.availableItemTypes.map(x => x.id);

            return typeIds.filter(typeId => {
                const allowedType = this.availableItemTypes.find(x => x.id === typeId);
                if (!allowedType) {
                    return false;
                }
                if (allowedType.allowedParents && allowedType.allowedParents.length > 0 && !allowedType.allowedParents.includes("root")) {
                    return false;
                }
                return true;
            });
        }
    },
    created: function () {
    }
});
