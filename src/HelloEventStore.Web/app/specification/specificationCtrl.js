(function () {
    'use strict';
    var controllerId = 'specificationCtrl';
    angular.module('app').controller(controllerId, [specificationCtrl]);

    function specificationCtrl() {
        var vm = this;
        vm.title = 'Specifications';
    }
})();