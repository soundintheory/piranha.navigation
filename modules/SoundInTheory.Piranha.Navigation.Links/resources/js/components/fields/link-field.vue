
<template>
    <div class="link-field">
        <div v-if="!isEmpty" class="btn-toolbar">
            <div class="input mr-3">
                <div class="input-group">
                    <div class="form-control" @click.prevent="showModal()" title="Edit Link">
                        <span class="badge badge-info" v-if="hasContent">{{model.type}}</span>
                        <small v-else><i class="fas fa-link text-muted"></i></small>
                        <span class="value pl-1"<span class="display">{{displayValue}}</span> <small class="url pl-1 text-muted">{{ url }}</small> <small class="hover pl-1"><i class="fas fa-pen"></i></small></span>
                    </div>
                </div>
            </div>
            <div class="btn-group" role="group">
                <a class="btn btn-info" title="Open Link" :href="url" target="_blank"><i class="fas fa-external-link-alt"></i></a>
                <button class="btn btn-primary" type="button" @click="showModal()" title="Edit Link"><i class="fas fa-pen"></i></button>
                <button class="btn btn-danger" type="button" @click="clear()" title="Clear Link"><i class="fas fa-times"></i></button>
            </div>
        </div>
        <div v-else>
            <button @click="showModal()" class="btn btn-outline-primary">Add Link...</button>
        </div>
    </div>
</template>

<script>
    export default{
        props: ["uid", "model", "meta"],
        methods: {
            clear(){
                this.update(this.getDefaults());
            },
            update(newModel) {
                if (!newModel) {
                    return;
                }
                Object.assign(this.model, this.getDefaults(), newModel);

                if (this.useContentTitle || !this.useText) {
                    this.model.text = null;
                }

                if (this.hasContent) {
                    this.model.url = null;
                }
            },
            showModal(){
                piranha.linkfieldmodal.open(this.update, this.model, (this.meta || {}).settings);
            },
            getDefaults() {
                return {
                    id: null,
                    type: 'Custom',
                    url: null,
                    path: null,
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
        },
        computed: {
            isEmpty() {
                return !this.model || (!this.url && !this.text);
            },
            url() {
                if (this.hasContent) {
                    var url = this.model.contentLink.url;
                    if (this.model.path) {
                        url += '/' + this.model.path.trim('/');
                    }
                    return url;
                }
                return this.model.url;
            },
            text() {
                return this.useContentTitle ? this.model.contentLink.text : this.model.text;
            },
            useContentTitle() {
                return this.hasContent && this.model.useContentTitle;
            },
            hasContent() {
                return this.model && this.model.contentLink && (this.model.type === 'Page' || this.model.type === 'Post');
            },
            useText() {
                return (!this.meta || !this.meta.settings || !this.meta.settings.HideLinkText) && !!this.text;
            },
            displayValue() {
                return this.useText ? this.text : this.url;
            }
        }
    }
</script>