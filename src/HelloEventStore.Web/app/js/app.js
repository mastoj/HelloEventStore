/// <reference path="../bower_components/angular/angular.js" />

(function() {

    'use strict';
    var app = angular.module('app', ['ngRoute', 'ngSanitize']);
    app.config(['$routeProvider', function($routeProvider) {
        $routeProvider.
            when("/", { controller: 'IndexCtrl', templateUrl: "index.html" }).
            //when("/second", { controller: "DetailsCtrl", templateUrl: "details.html" }).
            otherwise({ redirectTo: "/" });
    }]);

    app.filter('splitCamel', function() {
        return function(input) {
            var result = input.replace(/[a-z][A-Z]/g, function(str, offset) {
                return str[0] + ' ' + str[1].toLowerCase();
            });
            return result;
        };
    });

    app.directive("objectModelInput", [function () {
        return {
            restrict: 'E',
            scope: {
                objectModel: '=',
                deletable: '@',
                onDelete: '&onDelete'
            },
            templateUrl: "directives/objectModelInput.html",
            link: function (scope, element) {
                scope.deleteItem = function() {
                    scope.onDelete({ objectModel: scope.objectModel });
                };
            }
        };
    }]);

    app.controller("IndexCtrl", ["$scope", "rootService", function ($scope, rootService) {
        $scope.specification = {};
        $scope.specification.preCondition = {};
        $scope.preConditionList = [];
        $scope.specification.postCondition = [];
        $scope.specification.action = undefined;
        var addCondition = function(objectModel, list) {
            var item = angular.copy(objectModel);
            delete item["$$hashKey"]; 
            list.push(item);
            return item;
        };
        var deleteCondition = function(objectModel, list) {
            var index = list.indexOf(objectModel);
            if (index >= 0) {
                list.splice(index, 1);
            }
        };
        $scope.addPreCondition = function (objectModel) {
            var item = angular.copy(objectModel);
            delete item["$$hashKey"];
            var preConditionItem = {
                id: null,
                objectModel: item
            };
            $scope.preConditionList.push(preConditionItem);
        };
        $scope.addPostCondition = function (objectModel) {
            addCondition(objectModel, $scope.specification.postCondition);
        };
        $scope.deletePreCondition = function (objectModel) {
            deleteCondition(objectModel, $scope.preConditionList);
        };
        $scope.deletePostCondition = function (objectModel) {
            deleteCondition(objectModel, $scope.specification.postCondition);
        };
        $scope.setAction = function (action) {
            var item = angular.copy(action);
            $scope.specification.action = item;
        };
        $scope.hasAction = function() {
            return $scope.specification.action != undefined;
        };
        var convertPreConditionList = function(list) {
            var preConditions = {};
            _.forEach(list, function(item) {
                if (!preConditions[item.id]) {
                    preConditions[item.id] = [];
                }
                preConditions[item.id].push(item.objectModel);
            });
            return preConditions;
        };
        $scope.runSpecification = function () {
            $scope.specification.preCondition = convertPreConditionList($scope.preConditionList);
            rootService.executeSpecification($scope.specification);
        };
        var init = function () {
            rootService.getCommands().then(function (data) {
                $scope.commands = data;
            });
            rootService.getEvents().then(function (data) {
                $scope.events = data;
            });
        };
        init();
    }]);

    app.factory('rootService', ["$http", "$q", function($http, $q) {
        var index;
        var urlRoot = "/api";
        var getIndex = function() {
            if (!index) {
                index = $http.get(urlRoot);
            }
            return index;
        };
        var getDeferreds = {};
        var getRoots = function (rel) {
            if (!getDeferreds[rel]) {
                getDeferreds[rel] = $q.defer();
                getIndex().success(function (links) {
                    var link = _.filter(links, function (l) { return l.rel == rel; })[0];
                    $http.get(link.href).success(function (items) {
                        getDeferreds[rel].resolve(items);
                    });
                });
            }
            return getDeferreds[rel].promise;
        };
        var executeSpecification = function(specification) {
            $http({ method: "POST", url: "/api/specification", data: specification });
        };
        return {
            getIndex: getIndex,
            getCommands: function () { return getRoots("commands"); },
            getEvents: function () { return getRoots("events"); },
            executeSpecification: executeSpecification
        };
    }]);
})();