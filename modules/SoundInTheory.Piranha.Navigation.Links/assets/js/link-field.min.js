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
Vue.component("link-field", {
  props: ["uid", "model", "meta"],
  methods: {
    clear() {
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
    showModal() {
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
      };
    }
  },
  computed: {
    isEmpty() {
      return !this.model || !this.url && !this.text;
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
  },
  template: "\n<div class=\"link-field\">\n    <div v-if=\"!isEmpty\" class=\"btn-toolbar\">\n        <div class=\"input mr-3\">\n            <div class=\"input-group\">\n                <div class=\"form-control\" @click.prevent=\"showModal()\" title=\"Edit Link\">\n                    <span class=\"badge badge-info\" v-if=\"hasContent\">{{model.type}}</span>\n                    <small v-else><i class=\"fas fa-link text-muted\"></i></small>\n                    <span class=\"value pl-1\"<span class=\"display\">{{displayValue}}</span> <small class=\"url pl-1 text-muted\">{{ url }}</small> <small class=\"hover pl-1\"><i class=\"fas fa-pen\"></i></small></span>\n                </div>\n            </div>\n        </div>\n        <div class=\"btn-group\" role=\"group\">\n            <a class=\"btn btn-info\" title=\"Open Link\" :href=\"url\" target=\"_blank\"><i class=\"fas fa-external-link-alt\"></i></a>\n            <button class=\"btn btn-primary\" type=\"button\" @click=\"showModal()\" title=\"Edit Link\"><i class=\"fas fa-pen\"></i></button>\n            <button class=\"btn btn-danger\" type=\"button\" @click=\"clear()\" title=\"Clear Link\"><i class=\"fas fa-times\"></i></button>\n        </div>\n    </div>\n    <div v-else>\n        <button @click=\"showModal()\" class=\"btn btn-outline-primary\">Add Link...</button>\n    </div>\n</div>\n"
});
Vue.component("link-field-modal", {
  data: function () {
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
  mounted: function () {},
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
      get: function () {
        return this.currentModel.attributes.target === '_blank';
      },
      set: function (value) {
        this.currentModel.attributes.target = value ? '_blank' : null;
      }
    },
    nofollow: {
      get: function () {
        return this.currentModel.attributes.rel === 'nofollow';
      },
      set: function (value) {
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
    toggleAdvancedSettings: function () {
      this.showAdvancedSettings = !this.showAdvancedSettings;
    },
    selectLink: function (link) {
      var oldUrl = this.url;
      if (link) {
        this.currentModel.contentLink = Object.assign({}, link, {
          url: this.removeUrlPath(link.url, link.path)
        });
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
      while (start < end && str[start] === ch) ++start;
      while (end > start && str[end - 1] === ch) --end;
      return start > 0 || end < str.length ? str.substring(start, end) : str;
    },
    trimEnd: function (str, ch) {
      var end = str.length;
      while (end > 0 && str[end - 1] === ch) --end;
      return end < str.length ? str.substring(0, end) : str;
    },
    escapeRegex: function (str) {
      return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    },
    setCurrentModel: function (model) {
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
            data: params => {
              return {
                search: params.term
              };
            },
            processResults: data => {
              return {
                results: (data || []).map(link => {
                  if (link.path) {
                    link.id = link.id + '__' + link.path;
                  }
                  return link;
                })
              };
            }
          },
          templateResult: link => {
            if (!link.id && !link.parentId) {
              return link.text;
            }
            return $('<span><span class="badge badge-light">' + link.type + '</span> ' + link.text + '</span>');
          },
          templateSelection: link => {
            if (!link.id && !link.parentId) {
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
          if (link && link.id) {
            link = Object.assign({}, link);
            link.id = link.id.split('__')[0];
          }
          this.selectLink(link);
        }).on('select2:clear', e => {
          this.selectLink(null);
          setTimeout(() => {
            $(e.currentTarget).select2('close');
          });
        });
      }, !delay && delay !== 0 ? 10 : delay);
    },
    getDefaults: function () {
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
      };
    },
    getDefaultSettings: function () {
      return {
        HideLinkText: false
      };
    }
  },
  template: "\n<div class=\"modal modal-panel fade\" id=\"linkfieldmodal\" ref=\"modal\">\n    <div class=\"modal-dialog modal-lg\">\n        <div class=\"modal-content\">\n            <!-- Modal Header -->\n            <div class=\"modal-header border-bottom-0\">\n                <h5 class=\"modal-title\"><i class=\"fas fa-link mr-1\"></i> Edit Link</h5>\n                <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\">\n                    <span aria-hidden=\"true\">&times;</span>\n                </button>\n            </div>\n\n            <!-- Modal Body -->\n            <div class=\"modal-body bg-light\" v-if=\"currentModel\" ref=\"modalBody\">\n                <div class=\"form-group row\" v-if=\"!settings.HideLinkText\">\n                    <label class=\"col-sm-2 col-form-label\" for=\"link-text-input\">Link Text</label>\n                    <div class=\"col-sm-10\">\n                        <template v-if=\"canUseContentTitle\">\n                            <div class=\"input-group\">\n                                <input id=\"link-text-input\" class=\"form-control\" type=\"text\" v-model=\"currentModel.text\" :disabled=\"currentModel.useContentTitle\">\n                                <div class=\"input-group-append\">\n                                    <label class=\"input-group-text form-check\">\n                                        <input type=\"checkbox\" class=\"form-check-input\" v-model=\"currentModel.useContentTitle\">\n                                        Use {{currentModel.type.toLowerCase()}} title\n                                    </label>\n                                </div>\n                            </div>\n                        </template>\n                        <template v-else>\n                            <input id=\"link-text-input\" class=\"form-control\" type=\"text\" v-model=\"currentModel.text\">\n                        </template>\n                    </div>\n                </div>\n                <div class=\"form-group row\">\n                    <label class=\"col-sm-2 col-form-label\" for=\"link-url-input\">Link</label>\n                    <div class=\"col-sm-10\">\n                        <input id=\"link-url-input\" class=\"form-control\" type=\"text\" ref=\"urlInput\" v-model=\"url\" :disabled=\"hasContentLink\">\n                    </div>\n                </div>\n                <div class=\"row mb-4\">\n                    <div class=\"col-sm-2\"></div>\n                    <div class=\"col-sm-10\">\n                        <select class=\"form-control\" ref=\"existingContentSelect\"></select>\n                        <div ref=\"dropdownContainer\" class=\"select2-dropdown-inline\"></div>\n                    </div>\n                </div>\n\n                <!-- div class=\"form-group row\">\n                    <label class=\"col-sm-2 col-form-label\" for=\"link-html-class\">HTML Class</label>\n                    <div class=\"col-sm-10\">\n                        <input id=\"link-html-class\" class=\"form-control\" type=\"text\" v-model=\"currentModel.attributes.class\">\n                    </div>\n                </div -->\n                <div class=\"row justify-content-end mb-4\">\n                    <div class=\"col-sm-10\">\n                        <div class=\"row\">\n                            <div class=\"col-auto\">\n                                <div class=\"custom-control custom-checkbox\">\n                                    <input type=\"checkbox\" class=\"custom-control-input\" id=\"link-target-blank-input\" v-model=\"targetBlank\">\n                                    <label class=\"custom-control-label\" for=\"link-target-blank-input\">Open in a new tab</label>\n                                </div>\n                            </div>\n                            <div class=\"col-auto\">\n                                <div class=\"custom-control custom-checkbox\">\n                                    <input type=\"checkbox\" class=\"custom-control-input\" id=\"link-nofollow-input\" v-model=\"nofollow\">\n                                    <label class=\"custom-control-label\" for=\"link-nofollow-input\">No follow</label>\n                                </div>\n                            </div>\n                            <div class=\"col-auto\">\n                                <div class=\"custom-control custom-checkbox\">\n                                    <input type=\"checkbox\" class=\"custom-control-input\" id=\"link-noopener-input\" v-model=\"nofollow\">\n                                    <label class=\"custom-control-label\" for=\"link-noopener-input\">No opener</label>\n                                </div>\n                            </div>\n                        </div>\n\n                    </div>\n                </div>\n            </div>\n            <div class=\"modal-footer\">\n                <button class=\"btn btn-primary\" v-on:click=\"save()\">Save</button>\n                <button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">Close</button>\n            </div>\n        </div>\n    </div>\n</div>\n"
});