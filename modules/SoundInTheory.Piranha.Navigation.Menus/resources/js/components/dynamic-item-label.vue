<template></template>
<script>
    export default {
        data: function() {
            return {}
        },
        props: ['template', 'item', 'itemIndex', 'parent', 'level'],
        created () {
            this.$options.template = `<span>${this.template || ''}</span>`;
        }
    }
</script>