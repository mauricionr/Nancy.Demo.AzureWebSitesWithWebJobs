/// <reference path="../../scripts/typings/knockout.mapping/knockout.mapping.d.ts" />
/// <reference path="../models/image.ts" />

import router = require('plugins/router');
import http = require('plugins/http');

class HomeViewModel {
    router: DurandalRouter;
    pages: KnockoutObservableArray<number>;
    currentPage: KnockoutObservable<number>;
    images: KnockoutObservableArray<Models.Image>;
    imageCount: KnockoutObservable<number>;
    loading: KnockoutObservable<boolean>;

    constructor() {
        var that = this;
        that.router = router;
        that.pages = ko.observableArray([]);
        that.currentPage = ko.observable(1);
        that.images = ko.observableArray([]);
        that.imageCount = ko.observable(0);
        that.loading = ko.observable(true);
    }

    activate(): JQueryPromise<any> {
        var that = this;
        return http.get("/api/images/list/count").done(response => {
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
    }

    getImages(offset: number): JQueryPromise<any> {
        var that = this;
        that.loading(true);

        return http.get("/api/images/list/12/" + offset).done(response => {
            var images = ko.mapping.fromJS(response);
            that.images(images());
        });
    }

    updateImage(img: Models.Image): void {
        $("#image-gallery-title").text(img.title());
        $("#image-gallery-image").attr("src", img.source());
    }

    gotoPrevious(): void {
        var that = this;
        that.gotoPage(that.currentPage() - 1);
    }

    gotoNext(): void {
        var that = this;
        that.gotoPage(that.currentPage() + 1);
    }

    gotoPage(index: number): void {
        var that = this;
        if (index > that.pages().length)
            index = that.pages().length;
        that.currentPage(index);
        that.getImages(index * 12);
    }
}

export = HomeViewModel;