

function setChartData(labels, messages, pulls) {
    return {
        labels: labels,
        datasets: [
            {
                label: "messages",
                fill: false,
                lineTension: 0.1,
                backgroundColor: "rgba(75,192,192,0.4)",
                borderColor: "rgba(75,192,192,1)",
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                pointBorderColor: "rgba(75,192,192,1)",
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: "rgba(75,192,192,1)",
                pointHoverBorderColor: "rgba(220,220,220,1)",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                data: messages,
                spanGaps: false
            },
            {
                label: "pulls",
                fill: false,
                lineTension: 0.1,
                backgroundColor: "rgba(220,192,192,0.4)",
                borderColor: "rgba(200,192,192,1)",
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                pointBorderColor: "rgba(200,192,192,1)",
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: "rgba(75,192,192,1)",
                pointHoverBorderColor: "rgba(220,220,220,1)",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                data: pulls,
                spanGaps: false
            }
        ]
    };
}

angular.module("dotQueueApp").directive('chartData', function () {
    return {
        restrict: 'AE',
        replace: true,
        scope: {
            data: "="
        },
        link: function ($scope, $element, $attrs, $location) {
            console.log('creating chart');
            var ctx = document.getElementById($attrs.id);
            var myChart = new Chart(ctx, {
                type: 'line',
                data: $scope.data,
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                    }
                }
            });
            //$attrs.$observe('chartData',bindChart);
        }
    }
});

angular.module("dotQueueApp").controller("messagesController", function ($scope) {
    $scope.products = ["M1", "M2", "Cheese"];
});

angular.module("dotQueueApp")
    .factory('chartDataService',
        function ($http) {
            return {
                getByHours: function (callback) {
                    $http.jsonp('http://localhost:8083/api/Chart/GetByHours?callback=JSON_CALLBACK')
                        .then(function (data) {
                            console.log(data.data);
                            callback(data.data);
                        });
                }
            }
        });

angular.module("dotQueueApp").controller("dashboardController", function ($scope, chartDataService) {
    $scope.products = ["Milk", "Bread", "Cheese"];

    $scope.chartData = setChartData(['a','b','c'], [1,2,3], [2,3,4]);

    $scope.refresh = function () {
        chartDataService.getByHours(function (data) {
            $scope.chartData = setChartData(data.Labels, data.Messages, data.Pulls);
            console.log($scope.chartData);
        });
    };
});