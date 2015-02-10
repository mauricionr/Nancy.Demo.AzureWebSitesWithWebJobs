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
    dropzoneLoaded: KnockoutObservable<boolean>;
    loading: KnockoutObservable<boolean>;

    constructor() {
        var that = this;
        that.router = router;
        that.pages = ko.observableArray([]);
        that.currentPage = ko.observable(1);
        that.images = ko.observableArray([]);
        that.imageCount = ko.observable(0);
        that.dropzoneLoaded = ko.observable(false);
        that.loading = ko.observable(true);
    }

    activate(): JQueryPromise<any> {
        var that = this;
        return http.get("/api/images/list/count").done(response => {
            if (response.imageCount < 1)
                response.imageCount = 1;
            var pages = Math.floor(response.imageCount / 12);
            var pageArray = [];
            for (var i = 1; i <= pages; i++) {
                pageArray.push(i);
            }
            that.pages(pageArray);
            that.imageCount(response.imageCount);
            return that.getImages(0);
        });
    }

    compositionComplete(): void {
        var that = this;
        $('#image-upload').on('shown.bs.modal',() => {
            $("#image-container").empty();
            if (that.dropzoneLoaded())
                return;
            that.dropzoneLoaded(true);
        });
    }

    fileLoaded(file: any, data: any): void {
        var that = this;

        // add preview
        var img = $(document.createElement("img"));
        img.attr("src", data);
        img.addClass("preview");
        $("#image-container").append(img);

        // Start upload
        var reader = new FileReader();
        reader.onloadend = that.imageLoaded(file);
        reader.readAsArrayBuffer(file);
    }

    private imageLoaded(file): any {
        var that = this;
        return (theFile => e => {
            $.get("/api/images/upload/url").done(url => {
                that.uploadImage(url, e.target.result);
            });
        })(file);
    }

    private uploadImage(url: string, data): void {
        var that = this;
        var ajaxRequest = new XMLHttpRequest();

        try {
            ajaxRequest.open('PUT', url, true);
            ajaxRequest.setRequestHeader('Content-Type', 'image/jpeg');
            ajaxRequest.setRequestHeader('x-ms-blob-type', 'BlockBlob');
            ajaxRequest.send(data);
            that.reportImageUploaded(url);
        }
        catch (e) {
            alert("can't upload the image to server.\n" + e.toString());
        }
    }

    private reportImageUploaded(url: string): void {
        var uri = "/api/images/upload/complete";
        var bag = {
            storageUrl: url
        };
        $.ajax(uri, {
            data: bag,
            dataType: "json",
            type: "post"
        }).done(() => {

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
        if (index === that.currentPage())
            return;
        if (index > that.pages().length)
            index = that.pages().length;
        that.currentPage(index);
        that.getImages(index * 12);
    }
}

export = HomeViewModel;