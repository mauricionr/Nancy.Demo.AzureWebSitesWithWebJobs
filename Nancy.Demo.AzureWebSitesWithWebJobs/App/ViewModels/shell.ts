import router = require('plugins/router');

class ShellViewModel {
    router: DurandalRouter;

    constructor() {
        this.router = router;
    }

    activate(): JQueryPromise<any> {
        router.map([
            { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
            { route: 'about', title: 'About', moduleId: 'viewmodels/about', nav: true },
            { route: 'contact', title: 'Contact', moduleId: 'viewmodels/contact', nav: true }
        ]).buildNavigationModel();

        return router.activate();
    }
}

export = ShellViewModel;