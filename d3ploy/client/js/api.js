/* globals app angular _ */

app.factory("assetsApi",

["$http", "$q", "pathHelper",
	function ($http, $q, pathHelper) {

		function createDirectoryModel(obj, parent) {
			var dir;
			if (parent) {
				dir = parent.addDirectory(obj.name);
			}
			else {
				dir = new Tree();
			}
			angular.forEach(obj.directories, function (d) {
				createDirectoryModel(d, dir);
			});
			angular.forEach(obj.files, function (f) {
				dir.addFile(f.name, f.type);
			});
			return dir;
		}

		function getTree() {

			var deferred = $q.defer();

			$http.get("api/assets")
			.success(function (data) {
				var tree = createDirectoryModel(data);
				deferred.resolve(tree);
			})
			.error(deferred.reject);

			return deferred.promise;

		}

		function getContent(path) {
			var deferred = $q.defer();

			var req = {
				method: "GET",
				url: "api/asset",
				headers: {
					"Accept": "text/plain"
				},
				params: { path: path }
			};

			$http(req).success(deferred.resolve).error(deferred.reject);

			return deferred.promise;
		}

		function save(path, content) {

			var deferred = $q.defer();

			var req = {
				method: "PUT",
				url: "api/asset",
				params: { path: path },
				data: content
			};

			$http(req).success(deferred.resolve).error(deferred.reject);

			return deferred.promise;

		}

		function newItem(path, type) {
			var deferred = $q.defer();

			var req = {
				method: "POST",
				url: "api/asset",
				params: { path: path, type: type }
			};

			$http(req).success(deferred.resolve).error(deferred.reject);

			return deferred.promise;
		}

		function deleteItem(path) {
			var deferred = $q.defer();

			var req = {
				method: "DELETE",
				url: "api/asset",
				params: { path: path }
			};

			$http(req).success(deferred.resolve).error(deferred.reject);

			return deferred.promise;
		}

		return {
			getTree: getTree,
			getContent: getContent,
			save: save,
			newItem: newItem,
			deleteItem: deleteItem
		};

	}

]);

app.factory("accountApi",

["$http", "$q", 
	function ($http, $q) {

		function getUser() {
			var deferred = $q.defer();

			$http.get("api/account")
			.success(function (data) {
				deferred.resolve(data);
			})
			.error(deferred.reject);

			return deferred.promise;
		}

		return {
			getUser: getUser
		};

	}

]);