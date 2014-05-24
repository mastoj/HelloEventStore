(function () {
    'use strict';
    var app = angular.module("app");
    app.constant("routes", getRoutes());

    app.config(['$routeProvider', 'routes', function ($routeProvider, routes) {
        routes.forEach(function(r) {
            $routeProvider.when(r.url, r.config);
        });

        $routeProvider.
            //when("/", { controller: 'IndexCtrl', templateUrl: "index.html" }).
            //when("/specification", { controller: 'SpecificationCtrl', templateUrl: "specification.html" }).
            //when("/second", { controller: "DetailsCtrl", templateUrl: "details.html" }).
            otherwise({ redirectTo: routes[0].url });
    }]);



    function getRoutes() {
        return [
            {
                url: "/specification",
                config: {
                    templateUrl: "/app/specification/specification.html",
                    title: "specification",
                    order: 1,
                    content: "<i class='fa fa-file-text'></i> Specification"
                }
            },
            {
                url: "/specification/new",
                config: {
                    templateUrl: "/app/specification/new/newspecification.html",
                    title: "new-specification",
                    order: 1,
                    content: "<i class='fa fa-plus'></i> New specification"
                }
            }
        ];
    }
})();