<template>
    <li class="dd-item" :class="{ expanded: isExpanded || !hasChildren }" :data-id="item.id">
        <div class="nav-menu-item" :class="{ dimmed: item.isUnpublished || item.isScheduled }">
            <div v-if="piranha.navigation.permissions.menus.editItems" class="handle dd-handle"><i class="fas fa-ellipsis-v"></i></div>
            <div class="link">
                <span class="actions" v-if="hasChildren">
                    <a v-if="isExpanded" v-on:click.prevent="toggle()" class="expand" href="#"><i class="fas fa-minus"></i></a>
                    <a v-else v-on:click.prevent="toggle()" class="expand" href="#"><i class="fas fa-plus"></i></a>
                </span>
                <a v-if="piranha.navigation.permissions.menus.editItems" href="#" v-on:click.prevent="piranha.navigation.menuedit.editItem(item)">
                    <span>{{ item.link.text }}</span>
                </a>
                <span v-else class="title">
                    <span>{{ item.link.text }}</span>
                </span>
            </div>
            <div class="type d-none d-md-block">{{ typeName }}</div>
            <div class="actions">
                <template v-if="piranha.navigation.permissions.menus.editItems">
                    <a v-if="!(menu.settings.enabledOptions && menu.settings.enabledOptions.includes('HideAddChildItem')) && (level < menu.settings.maxDepth || !menu.settings.maxDepth)" href="#" v-on:click.prevent="piranha.navigation.menuedit.addItem({ parentId: item.id })" title="Add Child Item..."><i class="fas fa-angle-down"></i></a>
                    <a v-else class="disabled" href="#" style="visibility:hidden;"><i class="fas fa-angle-down"></i></a>
                    <a href="#" v-if="!(menu.settings.enabledOptions && menu.settings.enabledOptions.includes('HideAppendItem'))" v-on:click.prevent="piranha.navigation.menuedit.addItem({ afterId: item.id })" title="Add Item After..."><i class="fas fa-angle-right"></i></a>
                    <a v-else class="disabled" href="#" style="visibility:hidden;"><i class="fas fa-angle-down"></i></a>
                </template>
                <a v-if="piranha.navigation.permissions.menus.deleteItems" v-on:click.prevent="piranha.navigation.menuedit.removeItem(item.id)" class="danger" href="#"><i class="fas fa-trash"></i></a>
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
    data: function () {
        return {
            isExpanded: true
        }
    },
    methods: {
        toggle: function() {
            this.isExpanded = !this.isExpanded;
        }
    },
    computed: {
        hasChildren: function() {
            return this.item.children && this.item.children.length > 0;
        },
        typeName: function() {
            switch (this.item.link.type) {
                case 'Custom':
                    return 'Custom Link';
                case 'None':
                    return 'Static Text';
            }
            return this.item.link.type;
        }
    }
}
</script>