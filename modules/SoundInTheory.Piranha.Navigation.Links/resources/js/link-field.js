/*global
    piranha
*/

piranha.linkfieldmodal = new Vue({
    name: 'Link Field Modal',
    data: function () {
        return {
        }
    },
    methods: {
        open: function (callback, model, settings) {
            this.init();
            Vue.nextTick(() => {
                this.$refs.modal.open(
                    JSON.parse(JSON.stringify(model || this.$refs.modal.getDefaults())),
                    callback,
                    settings
                );
            });
        },
        init: function () {
            if (!this._isMounted) {
                $('#linkfieldmodalwrap').remove();
                $(document.body).append('<div id="linkfieldmodalwrap"><link-field-modal ref="modal"></link-field-modal></div>');
                this.$mount('#linkfieldmodalwrap');
            }
        }
    }
});