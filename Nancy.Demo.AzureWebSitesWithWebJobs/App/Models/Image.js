var Models;
(function (Models) {
    var Image = (function () {
        function Image(id, title, thumbnail, source) {
            this.id = ko.observable(id);
            this.title = ko.observable(title);
            this.thumbnail = ko.observable(thumbnail);
            this.source = ko.observable(source);
        }
        return Image;
    })();
    Models.Image = Image;
})(Models || (Models = {}));
//export = Image; 
//# sourceMappingURL=Image.js.map