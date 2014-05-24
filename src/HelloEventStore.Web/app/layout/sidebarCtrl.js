(function () {
    'use strict';
    var controllerId = 'sidebarCtrl';
    angular.module('app').controller(controllerId, ['$route', 'routes', sidebarCtrl]);

    function sidebarCtrl($route, routes) {
        var vm = this;
        vm.title = 'New specification';
        vm.routes = routes.sort(function(r1, r2) {
            return r1.config.nav - r2.config.nav;
        });
        vm.isActive = function(route) {
            if (!route.config.title || !$route.current || !$route.current.title) {
                return '';
            }
            var menuName = route.config.title;
            return $route.current.title.substr(0, menuName.length) === menuName;
        };
    }
})();