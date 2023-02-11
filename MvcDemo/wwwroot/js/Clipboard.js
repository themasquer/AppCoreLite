function copyToClipboard(elementIdWithoutSharp, buttonIdWihtoutSharp, copiedMessage) {
    var element = $("#" + elementIdWithoutSharp);
    if (element.length) {
        var tempElement = $("<textarea>");
        $("body").append(tempElement);
        tempElement.val(element.text()).select();
        document.execCommand("copy");
        tempElement.remove();
        var button = $("#" + buttonIdWihtoutSharp);
        button.text(copiedMessage);
    }
}