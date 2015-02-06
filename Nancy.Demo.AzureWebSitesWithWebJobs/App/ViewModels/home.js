/// <reference path="../../scripts/typings/knockout.mapping/knockout.mapping.d.ts" />
/// <reference path="../models/image.ts" />
define(["require", "exports", 'plugins/router', 'plugins/http'], function (require, exports, router, http) {
    var HomeViewModel = (function () {
        function HomeViewModel() {
            var that = this;
            that.router = router;
            that.images = ko.observableArray([]);
            that.imageIndex = ko.observable(0);
            that.loading = ko.observable(true);
        }
        HomeViewModel.prototype.activate = function () {
            var that = this;
            return that.getImages(0);
        };
        HomeViewModel.prototype.getImages = function (offset) {
            var that = this;
            that.loading(true);
            return http.get("/api/images/list/12/" + offset).done(function (response) {
                var images = ko.mapping.fromJS(response);
                that.images(images());
            });
        };
        HomeViewModel.prototype.compositionComplete = function (view) {
        };
        return HomeViewModel;
    })();
    return HomeViewModel;
});
//# sourceMappingURL=home.js.map