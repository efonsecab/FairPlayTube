function downloadFromUrl(options) {
    var _a;
    var anchorElement = document.createElement('a');
    anchorElement.href = options.url;
    anchorElement.download = (_a = options.fileName) !== null && _a !== void 0 ? _a : '';
    anchorElement.click();
    anchorElement.remove();
}
function downloadFromByteArray(options) {
    // The byte array in .NET is encoded to base64 string when it passes to JavaScript.
    // So we can pass that base64 encoded string to the browser as a "data URL" directly.
    var url = "data:" + options.contentType + ";base64," + options.byteArray;
    downloadFromUrl({ url: url, fileName: options.fileName });
}