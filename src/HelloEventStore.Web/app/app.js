/// <reference path="../bower_components/angular/angular.js" />

(function() {

    'use strict';
    var app = angular.module('app', ['ngRoute', 'ngSanitize']);
    app.filter('splitCamel', function() {
        return function(input) {
            var result = input.replace(/[a-z][A-Z]/g, function(str, offset) {
                return str[0] + ' ' + str[1].toLowerCase();
            });
            return result;
        };
    });

    app.directive("inputField", ["$compile", "$templateCache", function ($compile, $templateCache) {
        var templateNameTemplate = "directives/objectModelInput/{type}.html";
        var templateTemplateMapping = {
            "System.String": "string",
            "System.Int32": "integer",
            "System.DateTime": "datetime",
            "System.Guid": "string",
        };
        return {
            restrict: "E",
            scope: {
                value: "=",
                type: "="
            },
            replace: true,
            link: function(scope, element) {
                var templateName = templateNameTemplate.replace("{type}", templateTemplateMapping[scope.type]);
                var template = $templateCache.get(templateName);
                var el = $compile(template)(scope);
                element.replaceWith(el);
            }
        };
    }]);

    app.directive("objectModelInput", [function () {
        return {
            restrict: 'E',
            scope: {
                objectModel: '=',
                deletable: '@',
                onDelete: '&onDelete'
            },
            replace: true,
            templateUrl: "directives/objectModelInput.html",
            link: function (scope, element) {
                scope.deleteItem = function() {
                    scope.onDelete({ objectModel: scope.objectModel });
                };
            }
        };
    }]);
    
    app.controller("IndexCtrl", ["$scope", "rootService", function($scope, rootService) {
    }]);
    
    app.controller("SpecificationCtrl", ["$scope", "rootService", function ($scope, rootService) {
        var steps = ["precondition", "action", "postcondition", "result"];
        $scope.steps = steps;
        $scope.stepIndex = 0;
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
        $scope.deletePreCondition = function (preConItem) {
            deleteCondition(preConItem, $scope.preConditionList);
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
            rootService.executeSpecification($scope.specification).then(function() {
                $scope.nextStep();
            });
        };

        var baseTemplateUrl = "specification/{name}.html";
        $scope.stepTemplate = function() {
            var templateUrl = baseTemplateUrl.replace("{name}", steps[$scope.stepIndex]);
            return templateUrl;
        };

        $scope.previousDisabled = function () {
            return $scope.stepIndex == 0;
        };

        $scope.nextDisabled = function () {
            return $scope.stepIndex >= steps.length - 2;
        };

        $scope.executeDisabled = function() {
            return $scope.stepIndex != steps.length - 2;
        };

        $scope.nextStep = function () {
            $scope.stepIndex = $scope.stepIndex + 1;
        };

        $scope.previousStep = function () {
            $scope.stepIndex = $scope.stepIndex - 1;
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
            return $http({ method: "POST", url: "/api/specification", data: specification });
        };
        return {
            getIndex: getIndex,
            getCommands: function () { return getRoots("commands"); },
            getEvents: function () { return getRoots("events"); },
            executeSpecification: executeSpecification
        };
    }]);
})();