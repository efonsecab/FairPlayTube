function appendMetaTag(tagName, tagValue) {
    var element = document.createElement("meta");
    element.setAttribute(tagName, tagValue);
    document.head.appendChild(element);

}