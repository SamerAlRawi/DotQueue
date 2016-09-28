

function setChartData() {
    return {
        labels: ["Jan", "February", "March", "April", "May", "June", "July"],
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
                data: [65, 59, 80, 81, 56, 55, 40],
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
                data: [62, 60, 77, 79, 55, 55, 38],
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
            data: "&"
        },
        controller: function ($scope, $element, $attrs, $location) {
            var ctx = document.getElementById($attrs.id);
            var myChart = new Chart(ctx, {
                type: 'line',
                data: $scope.data(),
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

        }
    }
});

angular.module("dotQueueApp").controller("messagesController", function ($scope) {
    $scope.products = ["M1", "M2", "Cheese"];
});

angular.module("dotQueueApp")
    .factory('chartDataService',
        function($http) {
            return {
                getByHours:function() {
                    $http.get('http://localhost:8083/api/Chart/GetByHours')
                        .then(function(data) {
                            console.log(data);
                        });
                }
            }
        });

angular.module("dotQueueApp").controller("dashboardController", function ($scope, chartDataService) {
    $scope.products = ["Milk", "Bread", "Cheese"];

    $scope.chartData = setChartData();

    $scope.refresh = function() {
        chartDataService.getByHours();
    };
});