import router = require('plugins/router');

class ShellViewModel {
    router: DurandalRouter;

    constructor() {
        this.router = router;
    }

    activate(): JQueryPromise<any> {
        router.map([
            { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
            { route: 'about', title: 'About', moduleId: 'viewmodels/about', nav: true }
        ]).buildNavigationModel();

        return router.activate();
    }
}

export = ShellViewModel;