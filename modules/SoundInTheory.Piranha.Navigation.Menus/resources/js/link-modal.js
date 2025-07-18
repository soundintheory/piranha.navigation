/*global
    piranha
*/

piranha.navigation = piranha.navigation || {};

piranha.navigation.linkmodal = new Vue({
    name: 'Link Modal',
    data: function () {
        return {
            availableItemTypes: []
        }
    },
    methods: {
        open: function (callback, model) {
            this.init();
            this.$refs.modal.setAvailableItemTypes(this.availableItemTypes);
            this.$refs.modal.open(
                JSON.parse(JSON.stringify(model || {})),
                callback
            );
        },
        init: function () {
            if (!this._isMounted) {
                $('#linkmodalwrap').remove();
                $(document.body).append('<div id="linkmodalwrap"><link-modal ref="modal"></link-modal></div>');
                this.$mount('#linkmodalwrap');
            }
        },
        setAvailableItemTypes: function (types) {
            if (types) {
                this.availableItemTypes = types;
            }
        }
    }
});
