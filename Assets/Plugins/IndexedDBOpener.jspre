Module.OpenDatabase = function (callback) {
    const indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB || window.shimIndexedDB;
    const request = indexedDB.open("BoatsDatabase", 1);
    request.onerror = (event) => {
        console.error("Error with IndexedDB occurred");
        console.error(event);
        callback(null);
    };
    request.onupgradeneeded = function (event) {
        const db = event.target.result;
        db.createObjectStore("boats", {keyPath: "id"});
    };
    request.onsuccess = (event) => {
        console.log("Database opened");
        const db = event.target.result;
        callback(db);
    };
}