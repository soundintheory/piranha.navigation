<template>
    <div class="link-selector">
        <input id="link-url-input" class="form-control" type="text" ref="input" v-model="currentValue.url" :disabled="linkIsSelected">
        <select class="form-control" ref="select"></select>
        <div ref="dropdown" class="dropdown"></div>
    </div>
</template>

<script>
    export default {
        props: ['value'],
        data: function() {
            return {
                currentValue: null
            };
        },
        mounted: function () {
            this.initSelect2();
        },
        computed: {
            linkIsSelected: function () {
                return this.currentValue && this.currentValue.id;
            }
        },
        methods: {
            setValue: function (link) {
                if (link) {
                    this.currentValue = {
                        id: link.id,
                        type: link.type,
                        url: link.url,
                        text: link.text
                    }
                } else {
                    this.currentValue = this.getDefaults();
                }
            },
            initSelect2: function () {

                var input = $(this.$refs.select);
                var selectedValue = this.currentModel && this.currentModel.contentLink ? this.currentModel.contentLink : null;

                // destroy if already initialised
                if (input.hasClass("select2-hidden-accessible")) {
                    input.select2('destroy');
                }

                // clear it out
                input.empty();

                // Initialise select2
                input.select2({
                    theme: 'bootstrap4',
                    allowClear: true,
                    dropdownParent: this.$refs.dropdownContainer,
                    minimumInputLength: 2,
                    placeholder: 'Select existing content to link to',
                    data: selectedValue ? [selectedValue] : null,
                    ajax: {
                        url: '/manager/api/navigation/links/all',
                        dataType: 'json',
                        delay: 250,
                        data: (params) => {
                            return {
                                search: params.term
                            }
                        },
                        processResults: (data) => {
                            return {
                                results: data || []
                            };
                        }
                    },
                    templateResult: (link) => {
                        if (!link.id) {
                            return link.text;
                        }
                        return $('<span><span class="badge badge-light">' + link.type + '</span> ' + link.text + '</span>');
                    },
                    templateSelection: (link) => {
                        if (!link.id) {
                            return link.text;
                        }
                        return $('<span><span class="badge badge-info">' + link.type + '</span> ' + link.text + '</span>');
                    }
                });
            },
            bindEvents: function () {

                var select = $(this.$refs.select);

                // set search input placeholder and focus the input when the select opens
                select.on('select2:open', (e) => {
                    var searchInput = $(e.currentTarget).data('select2').$dropdown.find('.select2-search__field').eq(0);
                    if (searchInput.length > 0) {
                        searchInput.attr('placeholder', 'Start typing to search...');
                        setTimeout(() => {
                            searchInput[0].focus();
                        }, 5);
                    }
                });

                // when a selection is made
                select.on('select2:select', (e) => {
                    this.setValue(e.params ? e.params.data : null);
                });

                // when the selection is cleared
                select.on('select2:clear', (e) => {
                    this.setValue(null);
                    setTimeout(() => {
                        $(e.currentTarget).select2('close');
                    });
                });
            },
            getDefaults: function() {
                return {
                    id: null,
                    type: 'Custom',
                    url: '',
                    text: null
                }
            }
        }
    }
</script>