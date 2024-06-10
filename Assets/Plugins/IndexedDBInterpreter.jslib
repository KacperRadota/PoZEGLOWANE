mergeInto(LibraryManager.library, {

    SaveToIndexedDB: function (jsonString) {
        Module.OpenDatabase(function (db) {
            if (!db) {
                return;
            }
            const transaction = db.transaction("boats", "readwrite");
            const boats = transaction.objectStore("boats");
            const convertedJsonString = UTF8ToString(jsonString);
            const jsonRecord = {
                id: 1, json: convertedJsonString
            };
            boats.put(jsonRecord);
            console.log("JSON record inputted");
            transaction.oncomplete = function () {
                console.log("Transaction completed - closing database");
                db.close();
            };
        });
    },

    LoadFromIndexedDB: function (callback) {
        Module.OpenDatabase(function (db) {
            if (!db) {
                let buffer = stringToNewUTF8("");
                {{{ makeDynCall('vi', 'callback') }}} (buffer);
                _free(buffer);
                return;
            }

            const transaction = db.transaction("boats", "readonly");
            const boats = transaction.objectStore("boats");
            var request = boats.get(1);
            transaction.oncomplete = function () {
                console.log("Transaction completed - closing database");
                db.close();
            };

            request.onsuccess = function () {
                var value = "";
                if (request.result !== undefined) {
                    value = request.result;
                    console.log("Retrieved string: " + value);
                } else {
                    value = "";
                }
                let buffer = stringToNewUTF8(value);
                {{{ makeDynCall('vi', 'callback') }}} (buffer);
                _free(buffer);
                return;
            };

            request.onerror = function () {
                console.error("Error occurred when retrieving the value");
                let buffer = stringToNewUTF8("");
                {{{ makeDynCall('vi', 'callback') }}} (buffer);
                _free(buffer);
                return;
            };
        });
    },

    ExistsInIndexedDB: function (callback) {
        Module.OpenDatabase(function (db) {
            if (!db) {
                {{{ makeDynCall('vi', 'callback') }}} (0);
                return;
            }

            const transaction = db.transaction("boats", "readonly");
            const boats = transaction.objectStore("boats");
            var request = boats.get(1);
            transaction.oncomplete = function () {
                console.log("Transaction completed - closing database");
                db.close();
            };

            request.onsuccess = function () {
                var value = 0;
                if (request.result !== undefined) {
                    console.log("Value found");
                    value = 1;
                } else {
                    value = 0;
                }
                {{{ makeDynCall('vi', 'callback') }}} (value);
                return;
            };

            request.onerror = function () {
                console.error("Error occurred when retrieving the value");
                {{{ makeDynCall('vi', 'callback') }}} (0);
                return;
            };
        });
    }

});