/* global app */

app.config([
	"$stateProvider", "$urlRouterProvider",
	function ($stateProvider, $urlRouterProvider) {

		$urlRouterProvider.otherwise("/");

		$stateProvider.state("tree", {
			abstract: true,
			templateUrl: "/client/html/tree.html",
			controller: "TreeController",
			resolve: {
				tree: function (assetsApi) {
					return assetsApi.getTree();
				}
			}
		});

		$stateProvider.state("tree.detail", {
			url: "/{path:.*}",
			templateUrl: "/client/html/detail.html",
			controller: "DetailController",
			resolve: {
				currentItem: function (tree, pathHelper, $stateParams) {
					return tree.findNode($stateParams.path, true);
				}
			}
		}

	);

	}
]);