<template>
    <li class="dd-item" :class="{ expanded: isExpanded || !hasChildren }" :data-id="item.id" :data-item-type="item.$typeId" :data-level="level">
        <div class="nav-menu-item" :class="{ dimmed: item.isUnpublished || item.isScheduled }">
            <div v-if="piranha.navigation.permissions.menus.editItems" class="handle dd-handle"><i class="fas fa-ellipsis-v"></i></div>
            <div class="link">
                <span class="actions" v-if="hasChildren">
                    <a v-if="isExpanded" v-on:click.prevent="toggle()" class="expand" href="#"><i class="fas fa-minus"></i></a>
                    <a v-else v-on:click.prevent="toggle()" class="expand" href="#"><i class="fas fa-plus"></i></a>
                </span>
                <a v-if="piranha.navigation.permissions.menus.editItems" href="#" v-on:click.prevent="piranha.navigation.menuedit.editItem(item)">
                    <dynamic-item-label v-if="!!itemType && !!itemType.listTitle" v-bind:template="itemType.listTitle" v-bind:item="item"></dynamic-item-label>
                    <span v-else>{{ defaultLabel }}</span>
                </a>
                <span v-else class="title">
                    <dynamic-item-label v-if="!!itemType && !!itemType.listTitle" v-bind:template="itemType.listTitle" v-bind:item="item"></dynamic-item-label>
                    <span v-else>{{ defaultLabel }}</span>
                </span>
            </div>
            <div class="type d-none d-md-block">
                <dynamic-item-label v-if="!!itemType && !!itemType.listType" v-bind:template="itemType.listType" v-bind:item="item"></dynamic-item-label>
                <span v-else>{{ defaultTypeName }}</span>
            </div>
            <div class="item-actions">
                <template v-if="piranha.navigation.permissions.menus.editItems">
                    <add-item-link v-if="canAddChildren" link-class="action" :position="{ parentId: item.id }" header="Add Child..." :available-types="availableItemTypes">
                        <i class="fas fa-plus"></i>
                    </add-item-link>
                    <span v-else style="visibility:hidden;"><i class="fas fa-plus"></i></span>
                </template>
                <a v-if="piranha.navigation.permissions.menus.deleteItems" v-on:click.prevent="piranha.navigation.menuedit.removeItem(item.id)" class="action text-danger" href="#"><i class="fas fa-trash"></i></a>
            </div>
        </div>
        <ol v-if="hasChildren" class="dd-list">
            <navigation-menu-item v-for="child in item.children" v-bind:key="child.id" v-bind:item="child" v-bind:menu="menu" v-bind:level="(level || 0) + 1">
            </navigation-menu-item>
        </ol>
    </li>
</template>

<script>
export default {
    props: ["item", "menu", "level"],
    inject: ['root'],
    data: function () {
        return {
            isExpanded: true,
            isNode: true
        }
    },
    methods: {
        toggle: function() {
            this.isExpanded = !this.isExpanded;
        },
        getProperty: function (obj, path) {
            var arr = path.split(".").filter(x => !!x);
            while (arr.length > 0 && (obj = obj[this.toCamelCase(arr.shift())]));
            return obj;
        },
        toCamelCase: function (str) {
            return str.charAt(0).toLowerCase() + str.slice(1)
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
                return this.level < (ancestor.maxDepth + ancestor.level);
            }
            return this.level < this.menu.settings.maxDepth || !this.menu.settings.maxDepth;
        },
        hasChildren: function() {
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
    }
}
</script>