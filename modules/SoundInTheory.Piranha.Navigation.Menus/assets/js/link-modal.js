Vue.component("link-modal", {
  data: function () {
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
    setCurrentModel: function (model) {
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
        itemType.fields.forEach(field => {
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
        return {};
      }
      return this.currentModel[propName];
    },
    updateFieldValue: function (fieldId, value) {
      // Update the field value when the component emits an update
      this.currentModel[fieldId] = value;
    },
    toCamelCase: function (str) {
      return str.charAt(0).toLowerCase() + str.slice(1);
    }
  },
  template: "\n<div class=\"modal modal-panel fade\" id=\"linkmodal\" ref=\"modal\">\n    <div class=\"modal-dialog modal-lg\">\n        <div class=\"modal-content\">\n            <!-- Modal Header -->\n            <div class=\"modal-header border-bottom-0\">\n                <h5 class=\"modal-title\"><i class=\"fas fa-link mr-1\"></i> {{ actionName }} {{ typeName }}</h5>\n                <button type=\"button\" class=\"close\" aria-label=\"Close\" data-dismiss=\"modal\">\n                    <span aria-hidden=\"true\">&times;</span>\n                </button>\n            </div>\n\n            <!-- Modal Body -->\n            <div class=\"modal-body bg-light\" v-if=\"currentModel\" ref=\"modalBody\">\n\n                <!-- Dynamic Field Rendering using Vue Dynamic Components -->\n                <div v-for=\"field in fields\" :key=\"field.id\" class=\"form-group row\">\n                    <label class=\"col-sm-2 col-form-label\" :for=\"field.meta.uid\">{{ field.meta.name }}</label>\n                    <div class=\"col-sm-10\">\n                        <div class=\"field-body\">\n                            <div :id=\"'tb-' + field.meta.uid\" class=\"component-toolbar\"></div>\n                            <component v-if=\"getFieldModel(field) != null\"\n                                       :is=\"field.meta.component\"\n                                       :uid=\"field.meta.uid\"\n                                       :model=\"getFieldModel(field)\"\n                                       :meta=\"field.meta\"\n                                       :toolbar=\"'tb-' + field.meta.uid\">\n                            </component>\n                        </div>\n                        <div v-if=\"field.meta.description != null\" v-html=\"field.meta.description\" class=\"field-description small text-muted\"></div>\n                    </div>\n                </div>\n\n            </div>\n            <div class=\"modal-footer\">\n                <button class=\"btn btn-primary\" v-on:click=\"save()\">Save</button>\n                <button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">Close</button>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("menu-link-field", {
  props: {
    uid: String,
    model: Object
  },
  data: function () {
    return {
      currentModel: this.model || this.getLinkDefaults()
    };
  },
  mounted: function () {
    this.initSelect2(10);
  },
  computed: {
    linkTargetBlank: {
      get: function () {
        if (!this.currentModel || !this.currentModel.attributes) return false;
        return this.currentModel.attributes.target === '_blank';
      },
      set: function (value) {
        if (!this.currentModel.attributes) this.currentModel.attributes = {};
        this.currentModel.attributes.target = value ? '_blank' : null;
        this.emitUpdate();
      }
    },
    linkNofollow: {
      get: function () {
        if (!this.currentModel || !this.currentModel.attributes) return false;
        return this.currentModel.attributes.rel === 'nofollow';
      },
      set: function (value) {
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
      return this.currentModel && this.currentModel.contentLink && (this.currentModel.type === 'Page' || this.currentModel.type === 'Post');
    }
  },
  watch: {
    'currentModel.text': function () {
      this.emitUpdate();
    },
    'currentModel.url': function () {
      this.emitUpdate();
    },
    'currentModel.useContentTitle': function (value) {
      if (value && this.currentModel.useContentTitle && this.canUseContentTitle) {
        this.currentModel.text = this.currentModel.contentLink.text;
      }
      this.emitUpdate();
    },
    model: function (newModel) {
      this.currentModel = newModel || this.getLinkDefaults();
      this.initSelect2(10);
    }
  },
  methods: {
    emitUpdate: function () {
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
            data: params => {
              return {
                search: params.term
              };
            },
            processResults: data => {
              return {
                results: data || []
              };
            }
          },
          templateResult: link => {
            if (!link.id) {
              return link.text;
            }
            return $('<span><span class="badge badge-light">' + link.type + '</span> ' + link.text + '</span>');
          },
          templateSelection: link => {
            if (!link.id) {
              return link.text;
            }
            return $('<span><span class="badge badge-info">' + link.type + '</span> ' + link.text + '</span>');
          }
        }).on('select2:open', e => {
          var searchInput = $(e.currentTarget).data('select2').$dropdown.find('.select2-search__field').eq(0);
          if (searchInput.length > 0) {
            searchInput.attr('placeholder', 'Start typing to search...');
            setTimeout(() => {
              searchInput[0].focus();
            }, 5);
          }
        }).on('select2:select', e => {
          var link = e.params ? e.params.data : null;
          this.selectLink(link);
        }).on('select2:clear', e => {
          this.selectLink(null);
          setTimeout(() => {
            $(e.currentTarget).select2('close');
          });
        });
      }, !delay && delay !== 0 ? 10 : delay);
    },
    getLinkDefaults: function () {
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
      };
    }
  },
  template: "\n<div class=\"link-field-container\">\n    <div class=\"form-group\">\n        <label>Link Text</label>\n        <template v-if=\"canUseContentTitle\">\n            <div class=\"input-group\">\n                <input class=\"form-control\" type=\"text\" v-model=\"currentModel.text\" :disabled=\"currentModel.useContentTitle\">\n                <div class=\"input-group-append\">\n                    <label class=\"input-group-text form-check\">\n                        <input type=\"checkbox\" class=\"form-check-input\" v-model=\"currentModel.useContentTitle\">\n                        Use {{currentModel.type.toLowerCase()}} title\n                    </label>\n                </div>\n            </div>\n        </template>\n        <template v-else>\n            <input class=\"form-control\" type=\"text\" v-model=\"currentModel.text\">\n        </template>\n    </div>\n    <div class=\"form-group\">\n        <label>Link URL</label>\n        <input class=\"form-control\" type=\"text\" v-model=\"currentModel.url\" :disabled=\"currentModel.type != 'Custom'\">\n    </div>\n    <div class=\"form-group\">\n        <select class=\"form-control\" ref=\"existingContentSelect\"></select>\n        <div ref=\"dropdownContainer\" class=\"select2-dropdown-inline\"></div>\n    </div>\n    <div class=\"row justify-content-end\">\n        <div class=\"col-sm-12\">\n            <div class=\"row\">\n                <div class=\"col-auto\">\n                    <div class=\"custom-control custom-checkbox\">\n                        <input type=\"checkbox\" class=\"custom-control-input\" :id=\"'target-blank-' + uid\" v-model=\"linkTargetBlank\">\n                        <label class=\"custom-control-label\" :for=\"'target-blank-' + uid\">Open in a new tab</label>\n                    </div>\n                </div>\n                <div class=\"col-auto\">\n                    <div class=\"custom-control custom-checkbox\">\n                        <input type=\"checkbox\" class=\"custom-control-input\" :id=\"'nofollow-' + uid\" v-model=\"linkNofollow\">\n                        <label class=\"custom-control-label\" :for=\"'nofollow-' + uid\">No follow</label>\n                    </div>\n                </div>\n                <div class=\"col-auto\">\n                    <div class=\"custom-control custom-checkbox\">\n                        <input type=\"checkbox\" class=\"custom-control-input\" :id=\"'noopener-' + uid\" v-model=\"linkNoopener\">\n                        <label class=\"custom-control-label\" :for=\"'noopener-' + uid\">No opener</label>\n                    </div>\n                </div>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
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
