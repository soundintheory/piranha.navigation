Vue.component("navigation-menu-item", {
  props: ["item", "menu", "level"],
  data: function () {
    return {
      isExpanded: true
    };
  },
  methods: {
    toggle: function () {
      this.isExpanded = !this.isExpanded;
    }
  },
  computed: {
    hasChildren: function () {
      return this.item.children && this.item.children.length > 0;
    },
    typeName: function () {
      switch (this.item.link.type) {
        case 'Custom':
          return 'Custom Link';
        case 'None':
          return 'Static Text';
      }
      return this.item.link.type;
    }
  },
  template: "\n<li class=\"dd-item\" :class=\"{ expanded: isExpanded || !hasChildren }\" :data-id=\"item.id\">\n    <div class=\"nav-menu-item\" :class=\"{ dimmed: item.isUnpublished || item.isScheduled }\">\n        <div v-if=\"piranha.navigation.permissions.menus.editItems\" class=\"handle dd-handle\"><i class=\"fas fa-ellipsis-v\"></i></div>\n        <div class=\"link\">\n            <span class=\"actions\" v-if=\"hasChildren\">\n                <a v-if=\"isExpanded\" v-on:click.prevent=\"toggle()\" class=\"expand\" href=\"#\"><i class=\"fas fa-minus\"></i></a>\n                <a v-else v-on:click.prevent=\"toggle()\" class=\"expand\" href=\"#\"><i class=\"fas fa-plus\"></i></a>\n            </span>\n            <a v-if=\"piranha.navigation.permissions.menus.editItems\" href=\"#\" v-on:click.prevent=\"piranha.navigation.menuedit.editItem(item)\">\n                <span>{{ item.link.text }}</span>\n            </a>\n            <span v-else class=\"title\">\n                <span>{{ item.link.text }}</span>\n            </span>\n        </div>\n        <div class=\"type d-none d-md-block\">{{ typeName }}</div>\n        <div class=\"actions\">\n            <template v-if=\"piranha.navigation.permissions.menus.editItems\">\n                <a v-if=\"!(menu.settings.enabledOptions && menu.settings.enabledOptions.includes('HideAddChildItem')) && (level < menu.settings.maxDepth || !menu.settings.maxDepth)\" href=\"#\" v-on:click.prevent=\"piranha.navigation.menuedit.addItem({ parentId: item.id })\" title=\"Add Child Item...\"><i class=\"fas fa-angle-down\"></i></a>\n                <a v-else class=\"disabled\" href=\"#\" style=\"visibility:hidden;\"><i class=\"fas fa-angle-down\"></i></a>\n                <a href=\"#\" v-if=\"!(menu.settings.enabledOptions && menu.settings.enabledOptions.includes('HideAppendItem'))\" v-on:click.prevent=\"piranha.navigation.menuedit.addItem({ afterId: item.id })\" title=\"Add Item After...\"><i class=\"fas fa-angle-right\"></i></a>\n                <a v-else class=\"disabled\" href=\"#\" style=\"visibility:hidden;\"><i class=\"fas fa-angle-down\"></i></a>\n            </template>\n            <a v-if=\"piranha.navigation.permissions.menus.deleteItems\" v-on:click.prevent=\"piranha.navigation.menuedit.removeItem(item.id)\" class=\"danger\" href=\"#\"><i class=\"fas fa-trash\"></i></a>\n        </div>\n    </div>\n    <ol v-if=\"hasChildren\" class=\"dd-list\">\n        <navigation-menu-item v-for=\"child in item.children\" v-bind:key=\"child.id\" v-bind:item=\"child\" v-bind:menu=\"menu\" v-bind:level=\"(level || 0) + 1\">\n        </navigation-menu-item>\n    </ol>\n</li>\n"
});
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
