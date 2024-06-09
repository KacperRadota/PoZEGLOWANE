function openDatabase() {
    let db;
    const indexedDB =
        window.indexedDB ||
        window.mozIndexedDB ||
        window.webkitIndexedDB ||
        window.msIndexedDB ||
        window.shimIndexedDB;
    const request = indexedDB.open("BoatsDatabase");
    request.onerror = (event) => {
        console.error("Database error");
        db = -1;
    };
    request.onsuccess = (event) => {
        db = event.target.result;
    };
    return db;
}

mergeInto(LibraryManager.library, {

    
    
});