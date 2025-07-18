<template>
    <div class="link-field-container">
        <div class="form-group">
            <label>Link Text</label>
            <template v-if="canUseContentTitle">
                <div class="input-group">
                    <input class="form-control" type="text" v-model="currentModel.text" :disabled="currentModel.useContentTitle">
                    <div class="input-group-append">
                        <label class="input-group-text form-check">
                            <input type="checkbox" class="form-check-input" v-model="currentModel.useContentTitle">
                            Use {{currentModel.type.toLowerCase()}} title
                        </label>
                    </div>
                </div>
            </template>
            <template v-else>
                <input class="form-control" type="text" v-model="currentModel.text">
            </template>
        </div>
        <div class="form-group">
            <label>Link URL</label>
            <input class="form-control" type="text" v-model="currentModel.url" :disabled="currentModel.type != 'Custom'">
        </div>
        <div class="form-group">
            <select class="form-control" ref="existingContentSelect"></select>
            <div ref="dropdownContainer" class="select2-dropdown-inline"></div>
        </div>
        <div class="row justify-content-end">
            <div class="col-sm-12">
                <div class="row">
                    <div class="col-auto">
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" :id="'target-blank-' + uid" v-model="linkTargetBlank">
                            <label class="custom-control-label" :for="'target-blank-' + uid">Open in a new tab</label>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" :id="'nofollow-' + uid" v-model="linkNofollow">
                            <label class="custom-control-label" :for="'nofollow-' + uid">No follow</label>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" :id="'noopener-' + uid" v-model="linkNoopener">
                            <label class="custom-control-label" :for="'noopener-' + uid">No opener</label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        props: {
            uid: String,
            model: Object
        },
        data: function() {
            return {
                currentModel: this.model || this.getLinkDefaults()
            };
        },
        mounted: function() {
            this.initSelect2(10);
        },
        computed: {
            linkTargetBlank: {
                get: function() {
                    if (!this.currentModel || !this.currentModel.attributes) return false;
                    return this.currentModel.attributes.target === '_blank';
                },
                set: function(value) {
                    if (!this.currentModel.attributes) this.currentModel.attributes = {};
                    this.currentModel.attributes.target = value ? '_blank' : null;
                    this.emitUpdate();
                }
            },
            linkNofollow: {
                get: function() {
                    if (!this.currentModel || !this.currentModel.attributes) return false;
                    return this.currentModel.attributes.rel === 'nofollow';
                },
                set: function(value) {
                    if (!this.currentModel.attributes) this.currentModel.attributes = {};
                    this.currentModel.attributes.rel = value ? 'nofollow' : null;
                    this.emitUpdate();
                }
            },
            linkNoopener: {
                get: function () {
                    if (!this.currentModel || !this.currentModel.attributes) return false;
                    return this.currentModel.attributes.rel === 'noopener';
                },
                set: function (value) {
                    if (!this.currentModel.attributes) this.currentModel.attributes = {};
                    this.currentModel.attributes.rel = value ? 'noopener' : null;
                    this.emitUpdate();
                }
            },
            canUseContentTitle: function () {
                return this.currentModel && this.currentModel.contentLink && 
                       (this.currentModel.type === 'Page' || this.currentModel.type === 'Post');
            }
        },
        watch: {
            'currentModel.text': function() {
                this.emitUpdate();
            },
            'currentModel.url': function() {
                this.emitUpdate();
            },
            'currentModel.useContentTitle': function (value) {
                if (value && this.currentModel.useContentTitle && this.canUseContentTitle) {
                    this.currentModel.text = this.currentModel.contentLink.text;
                }
                this.emitUpdate();
            },
            model: function(newModel) {
                this.currentModel = newModel || this.getLinkDefaults();
                this.initSelect2(10);
            }
        },
        methods: {
            emitUpdate: function() {
                this.$emit('update', this.currentModel);
            },
            selectLink: function (link) {
                this.currentModel.contentLink = link;

                if (link) {
                    this.currentModel.url = link.url;
                    this.currentModel.type = link.type;
                    this.currentModel.id = link.id;
                    if (this.currentModel.useContentTitle) {
                        this.currentModel.text = link.text;
                    } else if (!this.currentModel.text) {
                        this.currentModel.useContentTitle = true;
                    }
                } else {
                    this.currentModel.type = 'Custom';
                    this.currentModel.id = null;
                    this.currentModel.useContentTitle = false;
                }
                this.emitUpdate();
            },
            initSelect2: function (delay) {
                var input = $(this.$refs.existingContentSelect);
                var selectedValue = this.currentModel && this.currentModel.contentLink ? this.currentModel.contentLink : null;

                // destroy if already initialised
                if (input.hasClass("select2-hidden-accessible")) {
                    input.select2('destroy');
                }

                // clear it out
                input.empty();

                // Initialise select2
                setTimeout(() => {
                    input.select2({
                        theme: 'bootstrap4',
                        allowClear: true,
                        dropdownParent: this.$refs.dropdownContainer,
                        minimumInputLength: 2,
                        placeholder: 'Select existing content to link to',
                        data: selectedValue ? [selectedValue] : null,
                        ajax: {
                            url: '/manager/api/links/all',
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
                    })
                    .on('select2:open', (e) => {
                        var searchInput = $(e.currentTarget).data('select2').$dropdown.find('.select2-search__field').eq(0);
                        if (searchInput.length > 0) {
                            searchInput.attr('placeholder', 'Start typing to search...');
                            setTimeout(() => {
                                searchInput[0].focus();
                            }, 5);
                        }
                    })
                    .on('select2:select', (e) => {
                        var link = e.params ? e.params.data : null;
                        this.selectLink(link);
                    })
                    .on('select2:clear', (e) => {
                        this.selectLink(null);
                        setTimeout(() => {
                            $(e.currentTarget).select2('close');
                        });
                    });

                }, !delay && delay !== 0 ? 10 : delay);
            },
            getLinkDefaults: function() {
                return {
                    id: null,
                    type: 'Custom',
                    url: null,
                    text: null,
                    contentLink: null,
                    useContentTitle: false,
                    attributes: {
                        class: null,
                        target: null,
                        rel: null
                    }
                }
            }
        }
    }
</script>
