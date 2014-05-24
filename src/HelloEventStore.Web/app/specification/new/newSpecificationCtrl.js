(function () {
    'use strict';
    var controllerId = 'newSpecificationCtrl';
    angular.module('app').controller(controllerId, [newSpecificationCtrl]);

    function newSpecificationCtrl() {
        var vm = this;
        vm.title = 'New specification';
    }
})();