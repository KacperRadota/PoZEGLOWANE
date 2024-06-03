mergeInto(LibraryManager.library, {

    IsAndroid: function () {
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        console.log("Is Android: " + /android/i.test(userAgent));
        return /android/i.test(userAgent);
    },

    IsIOS: function () {
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        console.log("Is iOS: " + /iPad|iPhone|iPod/.test(userAgent) && !window.MSStream);
        return /iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
    },
    
});