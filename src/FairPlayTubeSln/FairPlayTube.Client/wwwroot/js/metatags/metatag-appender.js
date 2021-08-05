function appendMetaTag(tagName, tagValue) {
    var element = document.createElement("meta");
    element.setAttribute("property", tagName);
    element.setAttribute("content", tagValue);
    document.head.appendChild(element);

}