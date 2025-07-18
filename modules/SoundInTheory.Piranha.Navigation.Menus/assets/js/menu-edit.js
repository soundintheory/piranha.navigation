Vue.component("navigation-menu-item", {
  props: ["item", "menu", "level"],
  inject: ['root'],
  data: function () {
    return {
      isExpanded: true,
      isNode: true
    };
  },
  methods: {
    toggle: function () {
      this.isExpanded = !this.isExpanded;
    },
    getProperty: function (obj, path) {
      var arr = path.split(".").filter(x => !!x);
      while (arr.length > 0 && (obj = obj[this.toCamelCase(arr.shift())]));
      return obj;
    },
    toCamelCase: function (str) {
      return str.charAt(0).toLowerCase() + str.slice(1);
    },
    isObject: function (value) {
      return typeof value === 'object' && !Array.isArray(value) && value !== null;
    },
    closest: function (selector) {
      let node = this;
      while (!!node && node.isNode) {
        if (selector(node)) {
          return node;
        }
        node = node.$parent;
      }
      return null;
    }
  },
  computed: {
    maxDepth: function () {
      return !!this.itemType ? this.itemType.maxDepth : null;
    },
    hasMaxDepth: function () {
      return !!this.itemType && typeof this.itemType.maxDepth === "number";
    },
    canAddChildren: function () {
      let ancestor = this.closest(x => x.hasMaxDepth);
      if (ancestor) {
        return this.level < ancestor.maxDepth + ancestor.level;
      }
      return this.level < this.menu.settings.maxDepth || !this.menu.settings.maxDepth;
    },
    hasChildren: function () {
      return this.item.children && this.item.children.length > 0;
    },
    defaultTypeName: function () {
      return !!this.itemType ? this.itemType.title : this.item["$typeId"];
    },
    defaultLabel: function () {
      if (this.item.link && this.item.link.text) {
        return this.item.link.text;
      }
      if (this.item.text && this.item.text.value) {
        return this.item.text.value;
      }
      if (this.item.title && this.item.title.value) {
        return this.item.title.value;
      }
      return '';
    },
    itemType: function () {
      return this.root.availableItemTypes.find(x => x.id === this.item.$typeId);
    },
    availableItemTypes: function () {
      let typeIds = this.root.availableItemTypes.map(x => x.id);
      if (this.itemType && this.itemType.allowedChildren && this.itemType.allowedChildren.length > 0) {
        typeIds = this.itemType.allowedChildren;
      }
      return typeIds.filter(typeId => {
        const allowedType = this.root.availableItemTypes.find(x => x.id === typeId);
        if (!allowedType) {
          return false;
        }
        if (allowedType.maxLevel > 0 && allowedType.maxLevel <= this.level) {
          return false;
        }
        if (allowedType.allowedParents && allowedType.allowedParents.length > 0 && !allowedType.allowedParents.includes(this.item.$typeId)) {
          return false;
        }
        return true;
      });
    }
  },
  template: "\n<li class=\"dd-item\" :class=\"{ expanded: isExpanded || !hasChildren }\" :data-id=\"item.id\" :data-item-type=\"item.$typeId\" :data-level=\"level\">\n    <div class=\"nav-menu-item\" :class=\"{ dimmed: item.isUnpublished || item.isScheduled }\">\n        <div v-if=\"piranha.navigation.permissions.menus.editItems\" class=\"handle dd-handle\"><i class=\"fas fa-ellipsis-v\"></i></div>\n        <div class=\"link\">\n            <span class=\"actions\" v-if=\"hasChildren\">\n                <a v-if=\"isExpanded\" v-on:click.prevent=\"toggle()\" class=\"expand\" href=\"#\"><i class=\"fas fa-minus\"></i></a>\n                <a v-else v-on:click.prevent=\"toggle()\" class=\"expand\" href=\"#\"><i class=\"fas fa-plus\"></i></a>\n            </span>\n            <a v-if=\"piranha.navigation.permissions.menus.editItems\" href=\"#\" v-on:click.prevent=\"piranha.navigation.menuedit.editItem(item)\">\n                <dynamic-item-label v-if=\"!!itemType && !!itemType.listTitle\" v-bind:template=\"itemType.listTitle\" v-bind:item=\"item\"></dynamic-item-label>\n                <span v-else>{{ defaultLabel }}</span>\n            </a>\n            <span v-else class=\"title\">\n                <dynamic-item-label v-if=\"!!itemType && !!itemType.listTitle\" v-bind:template=\"itemType.listTitle\" v-bind:item=\"item\"></dynamic-item-label>\n                <span v-else>{{ defaultLabel }}</span>\n            </span>\n        </div>\n        <div class=\"type d-none d-md-block\">\n            <dynamic-item-label v-if=\"!!itemType && !!itemType.listType\" v-bind:template=\"itemType.listType\" v-bind:item=\"item\"></dynamic-item-label>\n            <span v-else>{{ defaultTypeName }}</span>\n        </div>\n        <div class=\"item-actions\">\n            <template v-if=\"piranha.navigation.permissions.menus.editItems\">\n                <add-item-link v-if=\"canAddChildren\" link-class=\"action\" :position=\"{ parentId: item.id }\" header=\"Add Child...\" :available-types=\"availableItemTypes\">\n                    <i class=\"fas fa-plus\"></i>\n                </add-item-link>\n                <span v-else style=\"visibility:hidden;\"><i class=\"fas fa-plus\"></i></span>\n            </template>\n            <a v-if=\"piranha.navigation.permissions.menus.deleteItems\" v-on:click.prevent=\"piranha.navigation.menuedit.removeItem(item.id)\" class=\"action text-danger\" href=\"#\"><i class=\"fas fa-trash\"></i></a>\n        </div>\n    </div>\n    <ol v-if=\"hasChildren\" class=\"dd-list\">\n        <navigation-menu-item v-for=\"child in item.children\" v-bind:key=\"child.id\" v-bind:item=\"child\" v-bind:menu=\"menu\" v-bind:level=\"(level || 0) + 1\">\n        </navigation-menu-item>\n    </ol>\n</li>\n"
});
Vue.component("dynamic-item-label", {
  data: function () {
    return {};
  },
  props: ['template', 'item'],
  created() {
    this.$options.template = `<span>${this.template || ''}</span>`;
  },
  template: ""
});
Vue.component("add-item-link", {
  props: ['availableTypes', 'position', 'wrapClass', 'linkClass', 'header'],
  inject: ['root'],
  data: function () {
    return {};
  },
  computed: {
    types: function () {
      if (this.availableTypes && this.availableTypes.length > 0) {
        return this.root.availableItemTypes.filter(x => this.availableTypes.includes(x.id));
      }
      return this.root.availableItemTypes;
    }
  },
  template: "\n<div class=\"add-item dropdown\" :class=\"wrapClass || ''\" v-if=\"types.length > 0\">\n    <a role=\"button\" :class=\"linkClass || ''\" href=\"#\" data-toggle=\"dropdown\">\n        <slot></slot>\n    </a>\n    <div class=\"dropdown-menu dropdown-menu-right\">\n        <h6 class=\"dropdown-header\" v-if=\"header\">{{ header }}</h6>\n        <a class=\"dropdown-item\" href=\"#\" v-for=\"type in types\" v-on:click.prevent=\"piranha.navigation.menuedit.addItem(type.id, position)\">{{ type.title }}</a>\n    </div>\n</div>\n"
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
