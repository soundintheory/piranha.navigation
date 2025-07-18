<template>
    <div class="add-item dropdown" :class="wrapClass || ''" v-if="types.length > 0">
        <a role="button" :class="linkClass || ''" href="#" data-toggle="dropdown">
            <slot></slot>
        </a>
        <div class="dropdown-menu dropdown-menu-right">
            <h6 class="dropdown-header" v-if="header">{{ header }}</h6>
            <a class="dropdown-item" href="#" v-for="type in types" v-on:click.prevent="piranha.navigation.menuedit.addItem(type.id, position)">{{ type.title }}</a>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['availableTypes', 'position', 'wrapClass', 'linkClass', 'header'],
        inject: ['root'],
        data: function() {
            return {}
        },
        computed: {
            types: function () {
                if (this.availableTypes && this.availableTypes.length > 0) {
                    return this.root.availableItemTypes.filter(x => this.availableTypes.includes(x.id));
                }
                return this.root.availableItemTypes;
            }
        }
    }
</script>