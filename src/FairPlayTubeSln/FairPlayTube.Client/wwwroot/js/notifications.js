function DisplaySuccessNotification(text, duration, newWindow, close, gravity, position, backgroundColor, stopOnFocus) {
    Toastify({
        text: text,
        duration: duration,
        //        destination: "https://github.com/apvarun/toastify-js",
        newWindow: newWindow,
        close: close,
        gravity: gravity, // `top` or `bottom`
        position: position, // `left`, `center` or `right`
        backgroundColor: backgroundColor,
        stopOnFocus: stopOnFocus, // Prevents dismissing of toast on hover
        //onClick: function () { } // Callback after click
    }).showToast();
}

function DisplayErrorNotification(text, duration, newWindow, close, gravity, position, backgroundColor, stopOnFocus) {
    Toastify({
        text: text,
        duration: duration,
        //        destination: "https://github.com/apvarun/toastify-js",
        newWindow: newWindow,
        close: close,
        gravity: gravity, // `top` or `bottom`
        position: position, // `left`, `center` or `right`
        //backgroundColor: "#ff5959",
        backgroundColor: backgroundColor,
        stopOnFocus: stopOnFocus, // Prevents dismissing of toast on hover
        //onClick: function () { } // Callback after click
    }).showToast();
}

