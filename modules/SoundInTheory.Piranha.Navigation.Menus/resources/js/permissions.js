/*global
    piranha
*/

piranha.navigation = piranha.navigation || {};
piranha.navigation.permissions = {
    loaded: false,
    menus: {
        editItems: false,
        deleteItems: false
    },

    load: function () {
        if (!this.loaded) {
            return fetch(piranha.baseUrl + "manager/api/navigation/permissions")
                .then((response) => response.json())
                .then((result) => {
                    this.menus.editItems = result.editItems;
                    this.menus.deleteItems = result.deleteItems;
                    this.loaded = true;
                    return result;
                })
                .catch(function (error) {
                    console.log("error:", error);
                });
        } else {
            return Promise.resolve();
        }
    }
};