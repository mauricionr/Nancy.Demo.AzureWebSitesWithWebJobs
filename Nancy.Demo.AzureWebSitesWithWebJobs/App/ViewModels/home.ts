/// <reference path="../../scripts/typings/knockout.mapping/knockout.mapping.d.ts" />
/// <reference path="../models/image.ts" />

import router = require('plugins/router');
import http = require('plugins/http');

class HomeViewModel {
    router: DurandalRouter;
    images: KnockoutObservableArray<Models.Image>;
    imageIndex: KnockoutObservable<number>;
    loading: KnockoutObservable<boolean>;

    constructor() {
        var that = this;
        that.router = router;
        that.images = ko.observableArray([]);
        that.imageIndex = ko.observable(0);
        that.loading = ko.observable(true);
    }

    activate(): JQueryPromise<any> {
        var that = this;
        return that.getImages(0);
    }

    getImages(offset: number): JQueryPromise<any> {
        var that = this;
        that.loading(true);

        return http.get("/api/images/list/12/" + offset).done(response => {
            var images = ko.mapping.fromJS(response);
            that.images(images());
        });
    }

    compositionComplete(view: any): void {

    }
}

export = HomeViewModel;