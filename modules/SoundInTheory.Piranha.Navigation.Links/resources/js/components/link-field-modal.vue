<template>
    <div class="modal modal-panel fade" id="linkfieldmodal" ref="modal">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <!-- Modal Header -->
                <div class="modal-header border-bottom-0">
                    <h5 class="modal-title"><i class="fas fa-link mr-1"></i> Edit Link</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <!-- Modal Body -->
                <div class="modal-body bg-light" v-if="currentModel" ref="modalBody">
                    <div class="form-group row" v-if="!settings.HideLinkText">
                        <label class="col-sm-2 col-form-label" for="link-text-input">Link Text</label>
                        <div class="col-sm-10">
                            <template v-if="canUseContentTitle">
                                <div class="input-group">
                                    <input id="link-text-input" class="form-control" type="text" v-model="currentModel.text" :disabled="currentModel.useContentTitle">
                                    <div class="input-group-append">
                                        <label class="input-group-text form-check">
                                            <input type="checkbox" class="form-check-input" v-model="currentModel.useContentTitle">
                                            Use {{currentModel.type.toLowerCase()}} title
                                        </label>
                                    </div>
                                </div>
                            </template>
                            <template v-else>
                                <input id="link-text-input" class="form-control" type="text" v-model="currentModel.text">
                            </template>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-sm-2 col-form-label" for="link-url-input">Link</label>
                        <div class="col-sm-10">
                            <input id="link-url-input" class="form-control" type="text" ref="urlInput" v-model="url" :disabled="hasContentLink">
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-10">
                            <select class="form-control" ref="existingContentSelect"></select>
                            <div ref="dropdownContainer" class="select2-dropdown-inline"></div>
                        </div>
                    </div>

                    <!-- div class="form-group row">
                        <label class="col-sm-2 col-form-label" for="link-html-class">HTML Class</label>
                        <div class="col-sm-10">
                            <input id="link-html-class" class="form-control" type="text" v-model="currentModel.attributes.class">
                        </div>
                    </div -->
                    <div class="row justify-content-end mb-4">
                        <div class="col-sm-10">
                            <div class="row">
                                <div class="col-auto">
                                    <div class="custom-control custom-checkbox">
                                        <input type="checkbox" class="custom-control-input" id="link-target-blank-input" v-model="targetBlank">
                                        <label class="custom-control-label" for="link-target-blank-input">Open in a new tab</label>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="custom-control custom-checkbox">
                                        <input type="checkbox" class="custom-control-input" id="link-nofollow-input" v-model="nofollow">
                                        <label class="custom-control-label" for="link-nofollow-input">No follow</label>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" v-on:click="save()">Save</button>
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default{
        data: function() {
            return {
                callback: null,
                currentModel: this.getDefaults(),
                settings: this.getDefaultSettings(),
                currentLink: null,
                linkModelBackup: null,
                search: '',
                showAdvancedSettings: false,
                links: []
            };
        },
        mounted: function () {

        },
        computed: {
            url: {
                get: function () {
                    if (this.hasContentLink) {
                        var url = this.currentModel.contentLink.url;
                        if (this.currentModel.path) {
                            url += '/' + this.trim(this.currentModel.path, '/');
                        }
                        return url;
                    }
                    return this.currentModel.url;
                },
                set: function (value) {
                    if (!this.hasContentLink) {
                        this.currentModel.url = value;
                    }
                }
            },
            targetBlank: {
                get: function() {
                    return this.currentModel.attributes.target === '_blank';
                },
                set: function(value) {
                    this.currentModel.attributes.target = value ? '_blank' : null;
                }
            },
            nofollow: {
                get: function() {
                    return this.currentModel.attributes.rel === 'nofollow';
                },
                set: function(value) {
                    this.currentModel.attributes.rel = value ? 'nofollow' : null;
                }
            },
            canUseContentTitle: function () {
                return !this.currentModel.path && this.hasContentLink && this.currentModel.contentLink.text;
            },
            hasContentLink: function () {
                return this.currentModel && this.currentModel.contentLink && (this.currentModel.type === 'Page' || this.currentModel.type === 'Post');
            }
        },
        watch: {
            ['currentModel.useContentTitle']: function (value) {
                if (value && this.currentModel.useContentTitle && this.canUseContentTitle) {
                    this.currentModel.text = this.currentModel.contentLink.text;
                }
            }
        },
        methods: {
            save: function () {
                if (this.callback) {
                    this.callback(JSON.parse(JSON.stringify(this.currentModel)));
                    this.callback = null;
                }
                $(this.$refs.modal).modal("hide");
            },
            open: function (model, callback, settings) {
                this.callback = callback;
                this.applySettings(settings);
                this.setCurrentModel(model);
                $(this.$refs.modal).modal("show");
            },
            toggleAdvancedSettings: function() {
                this.showAdvancedSettings = !this.showAdvancedSettings
            },
            selectLink: function (link) {
                var oldUrl = this.url;

                if (link) {
                    this.currentModel.contentLink = Object.assign({}, link, { url: this.removeUrlPath(link.url, link.path) });
                    this.currentModel.url = link.url;
                    this.currentModel.path = link.path;
                    this.currentModel.type = link.type;
                    this.currentModel.id = link.id;
                    if (this.currentModel.useContentTitle) {
                        this.currentModel.text = link.text;
                    } else if (!this.currentModel.text) {
                        this.currentModel.useContentTitle = true;
                    }
                } else {
                    this.currentModel.contentLink = null;
                    this.currentModel.url = oldUrl;
                    this.currentModel.path = null;
                    this.currentModel.type = 'Custom';
                    this.currentModel.id = null;
                    this.currentModel.useContentTitle = false;
                }
            },
            removeUrlPath: function (url, path) {
                if (!url || !path || !path.trim()) {
                    return url;
                }
                let regex = new RegExp(this.escapeRegex(this.trimChar(path, '/')) + '$', 'g');
                return this.trimEnd(this.trimEnd(url, '/').replace(regex, ''), '/');
            },
            trimChar: function (str, ch) {
                var start = 0,
                    end = str.length;

                while (start < end && str[start] === ch)
                    ++start;

                while (end > start && str[end - 1] === ch)
                    --end;

                return (start > 0 || end < str.length) ? str.substring(start, end) : str;
            },
            trimEnd: function (str, ch) {
                var end = str.length;

                while (end > 0 && str[end - 1] === ch)
                    --end;

                return end < str.length ? str.substring(0, end) : str;
            },
            escapeRegex: function(str) {
                return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
            },
            setCurrentModel: function(model) {
                this.currentModel = !model ? this.getDefaults() : Object.assign(this.getDefaults(), model);
                if (!this.currentModel.attributes) {
                    this.currentModel.attributes = this.getDefaults().attributes;
                }
                this.initSelect2(10);
            },
            applySettings: function (settings) {
                if (settings) {
                    Object.assign(this.settings, this.getDefaultSettings(), settings);
                }
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
                                    results: (data || []).map((link) => {
                                        if (link.path) {
                                            link.id = link.id + '__' + link.path;
                                        }
                                        return link;
                                    })
                                };
                            }
                        },
                        templateResult: (link) => {
                            if (!link.id && !link.parentId) {
                                return link.text;
                            }
                            return $('<span><span class="badge badge-light">' + link.type + '</span> ' + link.text + '</span>');
                        },
                        templateSelection: (link) => {
                            if (!link.id && !link.parentId) {
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
                        if (link && link.id) {
                            link = Object.assign({}, link);
                            link.id = link.id.split('__')[0];
                        }
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
            getDefaults: function() {
                return {
                    id: null,
                    type: 'Custom',
                    url: null,
                    path: null,
                    text: null,
                    content: null,
                    useContentTitle: false,
                    attributes: {
                        class: null,
                        target: null,
                        rel: null
                    }
                }
            },
            getDefaultSettings: function() {
                return {
                    HideLinkText: false
                }
            }
        }
    }
</script>