var app = angular.module("dotQueueApp", ['ngRoute']);

app.config(function($routeProvider, $locationProvider) {
    $routeProvider
        .when('/',
        {
            templateUrl: 'Dashboard.html',
            controller: 'dashboardController',
            resolve: {
                delay: function($q, $timeout) {
                    var delay = $q.defer();
                    $timeout(delay.resolve, 500);
                    return delay.promise;
                }
            }
        }).when('/Dashboard',
        {
            templateUrl: 'Dashboard.html',
            controller: 'dashboardController',
            resolve: {
                delay: function($q, $timeout) {
                    var delay = $q.defer();
                    $timeout(delay.resolve, 500);
                    return delay.promise;
                }
            }
        }).when('/Messages',
        {
            templateUrl: 'Messages.html',
            controller: 'messagesController',
            resolve: {
                delay: function($q, $timeout) {
                    var delay = $q.defer();
                    $timeout(delay.resolve, 500);
                    return delay.promise;
                }
            }
        });
    //.when('/Book/:bookId/ch/:chapterId', {
    //    templateUrl: 'chapter.html',
    //    controller: 'ChapterController'
    //});

});