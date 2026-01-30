<template>
    <div class="modal modal-panel fade" id="linkmodal" ref="modal">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <!-- Modal Header -->
                <div class="modal-header border-bottom-0">
                    <h5 class="modal-title"><i class="fas fa-link mr-1"></i> {{ actionName }} {{ typeName }}</h5>
                    <button type="button" class="close" aria-label="Close" data-dismiss="modal">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <!-- Modal Body -->
                <div class="modal-body bg-light" v-if="currentModel" ref="modalBody">

                    <!-- Dynamic Field Rendering using Vue Dynamic Components -->
                    <div v-for="field in fields" :key="field.id" class="form-group row">
                        <label class="col-sm-2 col-form-label" :for="field.meta.uid">{{ field.meta.name }}</label>
                        <div class="col-sm-10">
                            <div class="field-body">
                                <div :id="'tb-' + field.meta.uid" class="component-toolbar"></div>
                                <component v-if="getFieldModel(field) != null"
                                           :is="field.meta.component"
                                           :uid="field.meta.uid"
                                           :model="getFieldModel(field)"
                                           :meta="field.meta"
                                           :toolbar="'tb-' + field.meta.uid">
                                </component>
                            </div>
                            <div v-if="field.meta.description != null" v-html="field.meta.description" class="field-description small text-muted"></div>
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
                currentModel: {},
                availableMenuItemTypes: [],
                defaultTypeId: "Link"
            };
        },
        computed: {
            actionName: function () {
                return !!this.currentModel && !!this.currentModel.id ? "Edit" : "Add";
            },
            typeName: function () {
                return !!this.itemType ? this.itemType.title : 'Menu Item';
            },
            typeId: function () {
                return this.currentModel["$typeId"] || this.defaultTypeId;
            },
            itemType: function () {
                if (this.typeId) {
                    return this.availableMenuItemTypes.find(x => x.id === this.typeId);
                }
                return null;
            },
            fields: function () {
                if (this.itemType) {
                    return this.itemType.fields || [];
                }
                return [];
            },
            defaultItemType: function () {
                return this.availableMenuItemTypes.find(x => x.id === this.defaultTypeId);
            }
        },
        methods: {
            setAvailableItemTypes: function (types) {
                if (types) {
                    this.availableMenuItemTypes = types;
                }
            },
            save: function () {
                if (this.callback) {
                    // Add the $type discriminator based on selected type
                    var modelToSave = JSON.parse(JSON.stringify(this.currentModel));
                    console.log('SAVE: ', modelToSave);
                    this.callback(modelToSave);
                    this.callback = null;
                }

                $(this.$refs.modal).modal("hide");
            },
            open: function (model, callback) {
                this.callback = callback;
                this.setCurrentModel(model);
                $(this.$refs.modal).modal("show");
            },
            setCurrentModel: function(model) {
                const typeId = !!model && !!model.$typeId ? model.$typeId : this.defaultType;
                const itemType = this.availableMenuItemTypes.find(x => x.id === typeId) || this.defaultItemType;
                const fromModel = model || {};
                const currentModel = {
                    ["$typeId"]: !!itemType ? itemType.id : typeId
                };

                if (fromModel.id) {
                    currentModel.id = fromModel.id;
                }

                if (itemType) {
                    itemType.fields.forEach((field) => {
                        const propName = this.toCamelCase(field.meta.id);
                        currentModel[propName] = JSON.parse(JSON.stringify(fromModel[propName] || field.model));
                        delete currentModel[propName]["$type"];
                    });
                }
                
                // Update the state
                this.currentModel = currentModel;

                console.log('set current model: ', itemType.id, model, currentModel);

            },
            getFieldModel: function (field) {
                // Return the field model for the component
                const propName = this.toCamelCase(field.meta.id);
                if (!this.currentModel[propName]) {
                    return { };
                }
                return this.currentModel[propName];
            },
            updateFieldValue: function(fieldId, value) {
                // Update the field value when the component emits an update
                this.currentModel[fieldId] = value;
            },
            toCamelCase: function (str) {
                return str.charAt(0).toLowerCase() + str.slice(1)
            }
        }
    }
</script>
