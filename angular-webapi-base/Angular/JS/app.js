///////////////////////////////////////////////////////////////////////////////////////////////////
// our main entry point

//define our app
var app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'ngAnimate']);



// configure our routes
app.config(function ($routeProvider) {
    $routeProvider
        .when('/main', {
            templateUrl: 'Angular/Views/main.html',
            controller: 'maincontroller'
        })
        .otherwise({
            redirectTo: '/main'
        });
});


// setup globals
app.run(function ($rootScope) {

    $rootScope.Config = {
        Debug: false,
        BaseURL: "",
    }
});