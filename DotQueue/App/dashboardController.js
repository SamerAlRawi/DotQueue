angular.module("dotQueueApp").controller("dashboardController", function ($scope) {
    $scope.products = ["Milk", "Bread", "Cheese"];
});

angular.module("dotQueueApp").controller("messagesController", function ($scope) {
    $scope.products = ["M1", "M2", "Cheese"];
});