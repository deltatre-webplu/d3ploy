function PathHelper() {
	this.addHeadingSlash = function (path) {
		if (path.substr(0, 1) !== "/") {
			path = "/" + path;
		}

		return path;
	};
	this.addTrailingSlash = function (path) {
		if (path.substr(path.length - 1, 1) !== "/") {
			path = path + "/";
		}

		return path;
	};
	this.removeTrailingSlash = function (path) {
		if (path.substr(path.length - 1, 1) === "/") {
			path = path.substr(0, path.length - 1);
		}

		return path;
	};
	this.combine = function (path1, path2) {
		return this.removeTrailingSlash(path1) + this.addHeadingSlash(path2);
	};
	this.equal = function (path1, path2) {
		return path1.toLowerCase() === path2.toLowerCase();
	};
}
var _pathHelper = new PathHelper();



function Directory(name, parent) {
	this.type = "directory";
	this.path = _pathHelper.combine(parent.path, _pathHelper.addTrailingSlash(name));
	this.name = name;
	this.isOpen = false;
	this.files = [];
	this.directories = [];
	this.parent = parent;
}

Directory.prototype.addFile = function (name, type) {
	var item = new File(name, type, this);
	this.files.push(item);
	return item;
};
Directory.prototype.addDirectory = function (name) {
	var item = new Directory(name, this);
	this.directories.push(item);
	return item;
};
Directory.prototype.removeFile = function (file) {
	var index = this.files.indexOf(file);
	if (index > -1) {
		this.files.splice(index, 1);
	}
};
Directory.prototype.removeDirectory = function (dir) {
	var index = this.directories.indexOf(dir);
	if (index > -1) {
		this.directories.splice(index, 1);
	}
};
Directory.prototype.removeItem = function (item) {
	this.removeFile(item);
	this.removeDirectory(item);
}
Directory.prototype.findNode = function (path, ensureOpen) {
	path = _pathHelper.addHeadingSlash(path);

	if (_pathHelper.equal(this.path, path)) {
		return this;
	}

	var i;
	for (i = 0; i < this.files.length; i++) {
		if (_pathHelper.equal(this.files[i].path, path)) {
			if (ensureOpen) {
				this.isOpen = true;
			}
			return this.files[i];
		}
	}

	for (i = 0; i < this.directories.length; i++) {
		if (_pathHelper.equal(this.directories[i].path, path)) {
			if (ensureOpen) {
				this.isOpen = true;
			}
			return this.directories[i];
		}
		var found = this.directories[i].findNode(path, ensureOpen);
		if (found) {
			if (ensureOpen) {
				this.isOpen = true;
			}
			return found;
		}
	}

	return null;
};


function File(name, type, parent) {
	this.type = type;
	this.path = _pathHelper.combine(parent.path, name);
	this.name = name;
	this.parent = parent;
}

function Tree() {
	this.path = "/";
	this.name = "root";
	this.isOpen = true;
	this.files = [];
	this.directories = [];
	this.type = "directory";
	this.parent = null;
}

Tree.prototype = Object.create(Directory.prototype);
Tree.prototype.constructor = Tree;
