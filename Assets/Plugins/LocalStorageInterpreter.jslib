mergeInto(LibraryManager.library, {

    SaveToLocalStorage: function (key, value) {
        localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
    },

    LoadFromLocalStorage: function (key) {
        var value = localStorage.getItem(UTF8ToString(key));
        if (value !== null) {
            var lengthBytes = lengthBytesUTF8(value) + 1;
            var stringOnHeap = _malloc(lengthBytes);
            stringToUTF8(value, stringOnHeap, lengthBytes);
            return stringOnHeap;
        }
        return 0;
    },

    ExistsInLocalStorage: function (key) {
        return localStorage.getItem(UTF8ToString(key)) !== null
    },
    
});