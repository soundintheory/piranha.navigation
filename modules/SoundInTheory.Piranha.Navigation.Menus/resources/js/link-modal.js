/*global
    piranha
*/

piranha.navigation = piranha.navigation || {};

piranha.navigation.linkmodal = new Vue({
    name: 'Link Modal',
    data: function () {
        return {
        }
    },
    methods: {
        open: function (callback, model) {
            this.init();
            this.$refs.modal.open(
                JSON.parse(JSON.stringify(model || this.$refs.modal.getDefaults())),
                callback
            );
        },
        init: function () {
            if (!this._isMounted) {
                $('#linkmodalwrap').remove();
                $(document.body).append('<div id="linkmodalwrap"><link-modal ref="modal"></link-modal></div>');
                this.$mount('#linkmodalwrap');
            }
        }
    }
});