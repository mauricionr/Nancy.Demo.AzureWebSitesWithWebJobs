/// <reference path="../../scripts/typings/knockout.mapping/knockout.mapping.d.ts" />
/// <reference path="../models/image.ts" />
define(["require", "exports", 'plugins/router', 'plugins/http'], function (require, exports, router, http) {
    var HomeViewModel = (function () {
        function HomeViewModel() {
            var that = this;
            that.router = router;
            that.pages = ko.observableArray([]);
            that.currentPage = ko.observable(1);
            that.images = ko.observableArray([]);
            that.imageCount = ko.observable(0);
            that.loading = ko.observable(true);
        }
        HomeViewModel.prototype.activate = function () {
            var that = this;
            return http.get("/api/images/list/count").done(function (response) {
                if (response.imageCount < 1)
                    response.imageCount = 1;
                var pages = Math.floor(response.imageCount / 12);
                //if (response.imageCount % 12 > 0)
                //    pages++;
                var pageArray = [];
                for (var i = 1; i <= pages; i++) {
                    pageArray.push(i);
                }
                that.pages(pageArray);
                that.imageCount(response.imageCount);
                return that.getImages(0);
            });
        };
        HomeViewModel.prototype.getImages = function (offset) {
            var that = this;
            that.loading(true);
            return http.get("/api/images/list/12/" + offset).done(function (response) {
                var images = ko.mapping.fromJS(response);
                that.images(images());
            });
        };
        HomeViewModel.prototype.gotoPrevious = function () {
            var that = this;
            that.gotoPage(that.currentPage() - 1);
        };
        HomeViewModel.prototype.gotoNext = function () {
            var that = this;
            that.gotoPage(that.currentPage() + 1);
        };
        HomeViewModel.prototype.gotoPage = function (index) {
            var that = this;
            if (index > that.pages().length)
                index = that.pages().length;
            that.currentPage(index);
            that.getImages(index * 12);
        };
        return HomeViewModel;
    })();
    return HomeViewModel;
});
//# sourceMappingURL=home.js.map