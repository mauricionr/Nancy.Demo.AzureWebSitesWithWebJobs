/// <reference path="../../scripts/typings/knockout.mapping/knockout.mapping.d.ts" />
/// <reference path="../models/image.ts" />
/// <reference path="../../scripts/typings/dropzone/dropzone.d.ts" />

import router = require('plugins/router');
import http = require('plugins/http');
import Dropzone = require('dropzone');

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
        $('#image-upload').on('shown.bs.modal', function () {
            if (that.dropzoneLoaded())
                return;
            that.dropzoneLoaded(true);
            var myDropzone = new Dropzone("#uploadForm");
        });

        
        //, { 
        //    thumbnailWidth: 80,
        //    thumbnailHeight: 80,
        //    parallelUploads: 20,
        //    previewTemplate: previewTemplate,
        //    autoQueue: false, // Make sure the files aren't queued until manually added
        //    previewsContainer: "#previews", // Define the container to display the previews
        //    clickable: ".fileinput-button" // Define the element that should be used as click trigger to select files.
        //});
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