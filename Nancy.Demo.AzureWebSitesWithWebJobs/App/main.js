requirejs.config({
    paths: {
        'text': '/Scripts/text',
        'durandal': '/Scripts/durandal',
        'plugins': '/Scripts/durandal/plugins',
        'transitions': '/Scripts/durandal/transitions'
    }
});
define('jquery', function () { return jQuery; });
define('knockout', function () { return ko; });
define(['durandal/system', 'durandal/app', 'durandal/viewLocator'], function (system, app, viewLocator) {
    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");
    app.title = 'Nancy Demo running on Azure websites with WebJobs';
    app.configurePlugins({
        router: true
    });
    app.start().then(function () {
        viewLocator.useConvention();
        app.setRoot('viewmodels/shell', 'entrance');
    });
});
//# sourceMappingURL=main.js.map