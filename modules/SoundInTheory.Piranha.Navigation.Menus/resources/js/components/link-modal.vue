<template>
    <div class="modal modal-panel fade" id="linkmodal" ref="modal">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <!-- Modal Header -->
                <div class="modal-header border-bottom-0">
                    <h5 class="modal-title"><i class="fas fa-link mr-1"></i> Edit Menu Item</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <!-- Modal Body -->
                <div class="modal-body bg-light" v-if="currentModel" ref="modalBody">
                    <div class="form-group row">
                        <label class="col-sm-2 col-form-label" for="link-text-input">Type</label>
                        <div class="col-sm-10">
                            <select id="link-type-input" class="form-control" v-model="itemType">
                                <option disabled value="">Please select a type</option>
                                <option value="Link">Link</option>
                                <option value="None">Static Text</option>
                            </select>
                        </div>
                    </div>
                    <template v-if="itemType === 'None'">
                        <div class="form-group row">
                            <label class="col-sm-2 col-form-label" for="link-text-input">Text</label>
                            <div class="col-sm-10">
                                <input id="link-text-input" class="form-control" type="text" v-model="currentModel.text">
                            </div>
                        </div>
                    </template>
                    <template v-else>
                        <div class="form-group row">
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
                                <input id="link-url-input" class="form-control" type="text" ref="urlInput" v-model="currentModel.url" :disabled="currentModel.type != 'Custom'">
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
                                            <input type="checkbox" class="custom-control-input" id="link-nofollow-input" v-model="nofollow" v-on:click="ensureOnlyOneSelected">
                                            <label class="custom-control-label" for="link-nofollow-input">No follow</label>
                                        </div>
                                    </div>
                                    <div class="col-auto">
                                        <div class="custom-control custom-checkbox">
                                            <input type="checkbox" class="custom-control-input" id="link-noopener-input" v-model="noopener" v-on:click="ensureOnlyOneSelected">
                                            <label class="custom-control-label" for="link-noopener-input">No opener</label>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </template>
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
                currentLink: null,
                linkModelBackup: null,
                itemTypeInternal: "Link",
                search: '',
                showAdvancedSettings: false,
                links: []
            };
        },
        mounted: function () {

        },
        computed: {
            showAdvancedSettingsText: function () {
                return this.showAdvancedSettings ? "Hide advanced settings..." : "Show advanced settings...";
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
            noopener: {
                get: function () {
                    return this.currentModel.attributes.rel === 'noopener';
                },
                set: function (value) {
                    this.currentModel.attributes.rel = value ? 'noopener' : null;
                }
            },
            itemType: {
                get: function () {
                    return this.itemTypeInternal;
                },
                set: function (value) {
                    if (value === "None") {
                        if (this.itemTypeInternal !== "None") {
                            this.linkModelBackup = JSON.parse(JSON.stringify(this.currentModel));
                        }
                        this.currentModel = Object.assign(this.getDefaults(), { type: 'None', text: this.currentModel.text });
                    } else {
                        if (this.itemTypeInternal === "None" && !!this.linkModelBackup) {
                            this.currentModel = Object.assign(this.linkModelBackup, { text: this.currentModel.text });
                            this.linkModelBackup = null;
                            this.initSelect2();
                        }
                    }
                    this.itemTypeInternal = value;
                }
            },
            canUseContentTitle: function () {
                return this.currentModel && this.currentModel.contentLink && this.currentModel.type === 'Page' || this.currentModel.type === 'Post';
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
            open: function (model, callback) {
                this.callback = callback;
                this.setCurrentModel(model);
                $(this.$refs.modal).modal("show");
            },
            toggleAdvancedSettings: function() {
                this.showAdvancedSettings = !this.showAdvancedSettings
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
            },
            setCurrentModel: function(model) {
                console.log('setting current model: ', model);
                this.currentModel = !model ? this.getDefaults() : Object.assign(this.getDefaults(), model);
                this.itemType = this.currentModel.type === "None" ? "None" : "Link";
                if (!this.currentModel.attributes) {
                    this.currentModel.attributes = this.getDefaults().attributes;
                }
                this.initSelect2(10);
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
            getDefaults: function() {
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
            },
            ensureOnlyOneSelected: function(clickedCheckboxEvent) {
                var checkbox = clickedCheckboxEvent.target;
                if (checkbox.attr('checked'))
                {
                    if (checkbox.attr('id') === 'link-nofollow-input')
                    {
                        document.getElementById('link-noopener-input').attr('checked', false);
                    }
                    else if (checkbox.attr('id') === 'link-noopener-input')
                    {
                        document.getElementById('link-nofollow-input').attr('checked', false);
                    }
                }
            }
        }
    }
</script>