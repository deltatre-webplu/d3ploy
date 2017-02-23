app.service("pathHelper", [PathHelper]);

app.controller("TreeController", [

	"$scope", "tree", "$location", "accountApi",
	function ($scope, tree, $location, accountApi) {

		$scope.tree = tree;
		$scope.user = null;

		$scope.isNotALibrary = function (item) {
			return item.type === "script";
		}
		$scope.isALibrary = function (item) {
			return item.type === "libraryScript";
		}

		accountApi.getUser().then(function (data) {
			$scope.user = data;
		});

		$scope.selectItem = function (item) {
			$location.path(item.path);
			//$state.go("tree.detail", { path: item.path});
		};
	}

]);

app.controller("DetailController", [

	"$scope", "currentItem", "assetsApi", "$location",
	function ($scope, currentItem, assetsApi, $location) {

		$scope.tree.currentItem = currentItem;
		$scope.currentItem = currentItem;

		$scope.deleteItem = function () {
			var path = currentItem.path;
			var result = confirm("Are you sure to delete '" + path + "'?");
			if (result) {
				assetsApi.deleteItem(path)
					.then(function () {
						currentItem.parent.removeItem(currentItem);
						$location.path(currentItem.parent.path);
					})
					.catch(function (err) {
						alert(err);
					});
			}
		};

	}

]);

app.controller("DetailDirectoryController", [

	"$scope", "assetsApi", "pathHelper",
	function ($scope, assetsApi, pathHelper) {

		$scope.newScript = function () {
			var result = prompt("File name (start with '_' to create a library)");
			if (result !== null) {
				var dirPath = $scope.currentItem.path;
				var newPath = pathHelper.combine(dirPath, result);
				assetsApi.newItem(newPath, "script")
					.then(function (newScript) {
						$scope.currentItem.addFile(newScript.name, newScript.type);
					})
					.catch(function (err) {
						alert(err);
					});
			}
		};

		$scope.newDirectory = function () {
			var result = prompt("Directory name");
			if (result !== null) {
				var dirPath = $scope.currentItem.path;
				var newPath = pathHelper.combine(dirPath, result);
				assetsApi.newItem(newPath, "directory")
					.then(function (newDir) {
						$scope.currentItem.addDirectory(newDir.name);
					})
					.catch(function (err) {
						alert(err);
					});
			}
		};

	}

]);

app.controller("DetailScriptController", [

	"$scope", "assetsApi", "signalR",
	function ($scope, assetsApi, signalR) {

		var originalContent;
		$scope.path = $scope.currentItem.path;
		$scope.content = null;
		$scope.saveStatus = "";

		function openItem() {
			assetsApi.getContent($scope.path)
				.then(function (result) {
					originalContent = result;
					$scope.content = result;
				})
				.catch(function (err) {
					alert(err);
				});
		}

		openItem();

		function saveFile() {
			$scope.saveStatus = "saving";
			return assetsApi.save($scope.path, $scope.content)
				.then(function () {
					originalContent = $scope.content;
					$scope.saveStatus = "saved";
					console.info("saved");
				})
				.catch(function (err) {
					$scope.saveStatus = "error";
					alert(err);
				});
		}

		function isContentChanged() {
			return $scope.content !== originalContent;
		}
		$scope.isContentChanged = isContentChanged;

		$scope.executeFile = function () {
			var path = $scope.path;

			//var result = confirm("Are you sure to execute '" + path + "'?");
			//if (!result) {
			//	return;
			//}

			if (isContentChanged()) {
				saveFile().then(function () {
					signalR.server.executeAsset(path);
				});
			}
			else {
				signalR.server.executeAsset(path);
			}
		};

		$scope.aceBlur = function () {
			if (isContentChanged()) {
				saveFile();
			}
		};

		$scope.aceLoaded = function (_editor) {
			_editor.commands.addCommand({
				name: 'Save',
				bindKey: { win: 'Ctrl-S', mac: 'Command-S' },
				exec: function (editor) {
					saveFile();
				},
				readOnly: true // false if this command should not apply in readOnly mode
			});
		};
	}
]);


app.controller("TerminalController", [

	"$scope", "signalR", "$timeout", "focus", "webNotification",
	function ($scope, signalR, $timeout, focus, webNotification) {

		$scope.messages = [];
		$scope.executing = false;
		$scope.readlineMode = "text";
		$scope.readlineWaiting = false;
		$scope.readlineInput = "";

		$scope.readlineFocus = function() {
			focus("readline");
		};

		function showNotification(msg, autoClose) {
			autoClose = autoClose || 5000;
			webNotification.showNotification('d3ploy', {
				body: msg,
				icon: '/client/images/notification.png',
				onClick: function onNotificationClicked() {
					console.log('Notification clicked.');
				},
				autoClose: autoClose
			}, function onShow(error, hide) {
				if (error) {
					console.error('Unable to show notification: ' + error.message);
				} else {
					console.log('Notification Shown.');
				}
			});
		}

		function execLine(value) {
			if ($scope.readlineMode === "password") {
				writeLine("*********");
			} else {
				writeLine(value);
			}

			if ($scope.readlineWaiting) {
				signalR.server.onReadLine(value);
			}
			else {
				if (BUILT_IN_COMMANDS[value]) {
					BUILT_IN_COMMANDS[value]();
				}
				else {
					signalR.server.execute(value);
				}
			}
		}
		function beep() {
			var snd = new Audio("data:audio/wav;base64,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qJrx4KOQjahZxWbcZgztj2c49nKmkId44S71j0c8eV9yDK6uPRzx5X18eDvjvQ6yKo9ZSS6l//8elePK/Lf//IInrOF/FvDoADYAGBMGb7FtErm5MXMlmPAJQVgWta7Zx2go+8xJ0UiCb8LHHdftWyLJE0QIAIsI+UbXu67dZMjmgDGCGl1H+vpF4NSDckSIkk7Vd+sxEhBQMRU8j/12UIRhzSaUdQ+rQU5kGeFxm+hb1oh6pWWmv3uvmReDl0UnvtapVaIzo1jZbf/pD6ElLqSX+rUmOQNpJFa/r+sa4e/pBlAABoAAAAA3CUgShLdGIxsY7AUABPRrgCABdDuQ5GC7DqPQCgbbJUAoRSUj+NIEig0YfyWUho1VBBBA//uQZB4ABZx5zfMakeAAAAmwAAAAF5F3P0w9GtAAACfAAAAAwLhMDmAYWMgVEG1U0FIGCBgXBXAtfMH10000EEEEEECUBYln03TTTdNBDZopopYvrTTdNa325mImNg3TTPV9q3pmY0xoO6bv3r00y+IDGid/9aaaZTGMuj9mpu9Mpio1dXrr5HERTZSmqU36A3CumzN/9Robv/Xx4v9ijkSRSNLQhAWumap82WRSBUqXStV/YcS+XVLnSS+WLDroqArFkMEsAS+eWmrUzrO0oEmE40RlMZ5+ODIkAyKAGUwZ3mVKmcamcJnMW26MRPgUw6j+LkhyHGVGYjSUUKNpuJUQoOIAyDvEyG8S5yfK6dhZc0Tx1KI/gviKL6qvvFs1+bWtaz58uUNnryq6kt5RzOCkPWlVqVX2a/EEBUdU1KrXLf40GoiiFXK///qpoiDXrOgqDR38JB0bw7SoL+ZB9o1RCkQjQ2CBYZKd/+VJxZRRZlqSkKiws0WFxUyCwsKiMy7hUVFhIaCrNQsKkTIsLivwKKigsj8XYlwt/WKi2N4d//uQRCSAAjURNIHpMZBGYiaQPSYyAAABLAAAAAAAACWAAAAApUF/Mg+0aohSIRobBAsMlO//Kk4soosy1JSFRYWaLC4qZBYWFRGZdwqKiwkNBVmoWFSJkWFxX4FFRQWR+LsS4W/rFRb/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////VEFHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAU291bmRib3kuZGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAwNGh0dHA6Ly93d3cuc291bmRib3kuZGUAAAAAAAAAACU=");
			snd.play();
		}
		$scope.keydown = function ($event) {
			if ($scope.executing && !$scope.readlineWaiting) {
				beep();
				$event.preventDefault();
			}

			if ($event.which === 13) {
				execLine($scope.readlineInput);

				$scope.readlineInput = "";
				$event.preventDefault();
			}
		}

		signalR.bootstrap()
			.done(function () {
				signalR.server.getSessionStatus()
					.done(function (status) {
						if (status === "executing") {
							$scope.executing = true;
							$scope.$digest();
						}
					})
					.fail(function (err) {
						alert(err);
					});
			});

		var BUILT_IN_COMMANDS = {
			"clear": clear,
			"help": help,
			"reset": signalR.server.resetSession,
			"sessions": signalR.server.printSessions
		};

		// Terminal methods
		function writeMsg(msg) {
			msg.severity = msg.severity || "info";
			$scope.messages.push(msg);

			$scope.readlineFocus();
		}
		function clear() {
			$scope.messages = [];
		}
		function help() {
			writeLine("d3ploy commands:");
			writeLine(" You can type any PowerShell commands that will be executed on the server");
			writeLine(" or one of these special terminal commands:");
			writeLine("  - help : show this help");
			writeLine("  - clear : clear the terminal output");
			writeLine("  - sessions : get the list of active sessions");
			writeLine("  - reset : recrete the powershell runspace");
		}

		function writeLine(value) {
			writeMsg({ value: value, mode: "writeLine" });
		}

		function getAttention() {
			beep();
			$scope.readlineFocus();
		}

		function executionFinish(msg) {
			$scope.executing = false;
			$scope.readlineMode = "text";
			$scope.readlineWaiting = false;

			showNotification(msg);
		}

		$scope.$on("signalR.writeMessage", function (event, msg) {
			writeMsg(msg);
		});

		$scope.$on("signalR.readLine", function (event, mode) {
			$scope.readlineMode = mode || "text";
			$scope.readlineWaiting = true;

			showNotification("Waiting for input ...");
			getAttention();
		});

		$scope.$on("signalR.endReadLine", function (event) {
			$scope.readlineMode = "text";
			$scope.readlineWaiting = false;
		});

		$scope.$on("signalR.executionStarted", function () {
			$scope.executing = true;
		});

		$scope.$on("signalR.executionCompleted", function () {
			executionFinish("Execution completed!");
		});

		$scope.$on("signalR.executionFailed", function (event, error) {
			writeLine(error);

			executionFinish("Execution FAILED!!!");
		});

		$timeout(function () {
			writeLine("Welcome to d3ploy");
			writeLine("Please type 'help' to open a list of commands");
		}, 200);

	}
]);