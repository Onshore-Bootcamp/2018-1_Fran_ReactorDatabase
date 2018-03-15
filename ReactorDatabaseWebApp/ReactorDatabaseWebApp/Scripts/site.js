function Confirmation(text) {
    return confirm(text);
}

function toggleBookmark(id) {
    var addButton = document.getElementById("addBookmark" + id);
    if (addButton.style.display === "none") {
        addButton.style.display = "inline";
    }
    else {
        addButton.style.display = "none";
    }
    var removeButton = document.getElementById("removeBookmark" + id);
    if (removeButton.style.display === "none") {
        removeButton.style.display = "inline";
    }
    else {
        removeButton.style.display = "none";
    }
}

function bodyTransparency() {
    var bodyWrap = document.getElementById("bodyWrap");
    bodyWrap.style.backgroundColor = "rgba(255, 255, 255, 0.75)";
}