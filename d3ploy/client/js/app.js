var app = angular.module("d3ployApp", ["ui.router", "ngSanitize", "ui.ace", "angular-web-notification"]);

app.filter("trusted", ["$sce", function ($sce) {
	return function (url) {
		return $sce.trustAsResourceUrl(url);
	};
}]);

app.directive("contenteditable", function () {
	return {
		restrict: "A",
		require: "ngModel",
		link: function (scope, element, attrs, ngModel) {

			function read() {
				ngModel.$setViewValue(element.html());
			}

			ngModel.$render = function () {
				element.html(ngModel.$viewValue || "");
			};

			element.bind("blur keyup change", function () {
				scope.$apply(read);
			});
		}
	};
});

app.directive('focusOn', function ($timeout) {
	return function (scope, elem, attr) {

		scope.$on('focusOn', function (e, name) {
			if (name === attr.focusOn) {
				$timeout(function () {
					elem[0].focus();
					elem[0].scrollIntoView();
				});
			}
		});
	};
});

app.factory('focus', function ($rootScope, $timeout) {
	return function (name) {
		$timeout(function () {
			$rootScope.$broadcast('focusOn', name);
		});
	}
});

app.service("signalR", [

		"$rootScope", "$timeout", "$log",
		function ($rootScope, $timeout, $log) {

			var hubConnection = $.connection.hub;
			var hub = $.connection.d3PloyHub;
			var bootstrapPromise = null;

			hubConnection.stateChanged(function (change) {
				$log.log("signalR.stateChanged", change);
				$rootScope.$broadcast("signalR.stateChanged", change.newState);
			});

			//var tryingToReconnect;
			//hubConnection.error(function () {
			//	$log.log("connection.error");
			//});

			//hubConnection.reconnecting(function () {
			//	$log.log("connection.reconnecting");
			//	tryingToReconnect = true;
			//	$rootScope.$emit("signalR.reconnecting");
			//});

			//hubConnection.reconnected(function () {
			//	$log.log("connection.reconnected");
			//	tryingToReconnect = false;
			//	$rootScope.$emit("signalR.reconnected");
			//});

			//hubConnection.disconnected(function () {
			//	$log.log("connection.disconnected");
			//	if (tryingToReconnect) {
			//		$rootScope.$emit("signalR.disconnected");
			//	}
			//});

			//hubConnection.connectionSlow(function () {
			//	$log.log("connection.connectionSlow");
			//	$rootScope.$emit("signalR.connectionSlow");
			//});

			var bootstrap = function () {
				if (bootstrapPromise) {
					return bootstrapPromise;
				}

				bootstrapPromise = hubConnection.start({
					// Chrome doesn't support win authentication over websocket
					// http://stackoverflow.com/questions/26863242/signalr-mvc-5-websocket-no-valid-credentials
					transport: "longPolling" 
				});

				bootstrapPromise.done(function () {
					$log.log("signalRService - bootstrapped");
				});

				return bootstrapPromise;
			};

			hub.client.writeMessage = function (msg) {
				$timeout(function () {
					$log.log("signalR.writeMessage", msg);
					$rootScope.$broadcast("signalR.writeMessage", msg);
				});
			};
			hub.client.readLine = function (mode) {
				$timeout(function () {
					$log.log("signalR.readLine", mode);
					$rootScope.$broadcast("signalR.readLine", mode);
				});
			};
			hub.client.endReadLine = function () {
				$timeout(function () {
					$log.log("signalR.endReadLine");
					$rootScope.$broadcast("signalR.endReadLine");
				});
			};
			hub.client.executionStarted = function () {
				$timeout(function () {
					$log.log("signalR.executionStarted");
					$rootScope.$broadcast("signalR.executionStarted");
				});
			};
			hub.client.executionCompleted = function () {
				$timeout(function () {
					$log.log("signalR.executionCompleted");
					$rootScope.$broadcast("signalR.executionCompleted");
				});
			};
			hub.client.executionFailed = function (error) {
				$timeout(function () {
					$log.log("signalR.executionFailed");
					$rootScope.$broadcast("signalR.executionFailed", error);
				});
			};

			return {
				bootstrap: bootstrap,
				server: hub.server
			};

		}]);

app.run([
	'$rootScope', '$state', '$stateParams', 'signalR',
	function ($rootScope, $state, $stateParams, signalR) {

		$rootScope.$state = $state;
		$rootScope.$stateParams = $stateParams;

		signalR.bootstrap();
	}
]);