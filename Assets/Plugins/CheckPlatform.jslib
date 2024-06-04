mergeInto(LibraryManager.library, {

    IsAndroid: function () {
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        console.log("Is Android: " + /android/i.test(userAgent));
        return /android/i.test(userAgent);
    },

    IsIOS: function () {
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        var x = /iPad|iPhone|iPod/.test(userAgent) && !window.MSStream; 
        window.alert("Is iOS: " + x);
        return x;
    },
    
});