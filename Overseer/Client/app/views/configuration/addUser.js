﻿angular.module("overseer").controller("addUserController", [
    "$scope",
    "$location",
    "configuration",
    function ($scope, $location, configuration) {
        "use strict";

        var self = this;
        self.user = {};

        self.addUser = function () {
            self.loading = true;
            configuration.addUser(self.user).then(function () {
                $location.path("/configuration");
            }, function (e) {
                self.error = e;
            }).finally(function () {
                self.loading = false;
            });
        };
    }
]);