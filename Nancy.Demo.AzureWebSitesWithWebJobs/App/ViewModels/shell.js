define(["require", "exports", 'plugins/router'], function (require, exports, router) {
    var ShellViewModel = (function () {
        function ShellViewModel() {
            this.router = router;
        }
        ShellViewModel.prototype.activate = function () {
            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
                { route: 'about', title: 'About', moduleId: 'viewmodels/about', nav: true },
                { route: 'contact', title: 'Contact', moduleId: 'viewmodels/contact', nav: true }
            ]).buildNavigationModel();
            return router.activate();
        };
        return ShellViewModel;
    })();
    return ShellViewModel;
});
//# sourceMappingURL=shell.js.map