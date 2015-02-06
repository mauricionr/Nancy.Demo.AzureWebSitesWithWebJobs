module Models {
    export class Image {
        id: KnockoutObservable<string>;
        title: KnockoutObservable<string>;
        thumbnail: KnockoutObservable<string>;
        source: KnockoutObservable<string>;

        constructor(id: string, title: string, thumbnail: string, source: string) {
            this.id = ko.observable(id);
            this.title = ko.observable(title);
            this.thumbnail = ko.observable(thumbnail);
            this.source = ko.observable(source);
        }
    }
}
//export = Image;