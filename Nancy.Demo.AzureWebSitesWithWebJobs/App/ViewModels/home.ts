import router = require('plugins/router');

class HomeViewModel {
    router: DurandalRouter;

    constructor() {
        this.router = router;
    }
}

export = HomeViewModel;