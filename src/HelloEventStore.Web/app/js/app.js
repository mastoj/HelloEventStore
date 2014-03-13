/// <reference path="../bower_components/angular/angular.js" />

(function () {

    'use strict';
    var app = angular.module('app', ['ngRoute', 'ngSanitize']);

    app.config(['$routeProvider', function($routeProvider) {
        $routeProvider.
            when("/", { controller: 'SearchCtrl', templateUrl: "search.html" }).
            //when("/second", { controller: "DetailsCtrl", templateUrl: "details.html" }).
            otherwise({ redirectTo: "/" });
    }]);
})();