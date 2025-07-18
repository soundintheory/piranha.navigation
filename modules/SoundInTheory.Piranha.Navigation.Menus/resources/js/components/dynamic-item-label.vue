<template></template>
<script>
    export default {
        data: function() {
            return {}
        },
        props: ['template', 'item'],
        created () {
            this.$options.template = `<span>${this.template || ''}</span>`;
        }
    }
</script>