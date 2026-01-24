window._quill = null;

window._quill = null;

window.initQuill = function (editorId) {
    const editorElement = document.getElementById(editorId);
    if (!editorElement) return;

    // Prevent double initialization
    if (window._quill) return;

    // Ensure container is empty before initializing
    editorElement.innerHTML = "";

    window._quill = new Quill(editorElement, {
        theme: "snow",
        modules: {
            toolbar: [
                [{ header: [1, 2, 3, false] }],
                ["bold", "italic", "underline", "strike"],
                [{ list: "ordered" }, { list: "bullet" }],
                ["blockquote", "code-block"],
                ["link"],
                ["clean"]
            ]
        }
    });
};


window.getQuillHtml = function () {
    if (!window._quill) return "";
    return window._quill.root.innerHTML;
};

window.setQuillHtml = function (html) {
    if (!window._quill) return;
    window._quill.clipboard.dangerouslyPasteHTML(html || "");
};

window.destroyQuill = function (editorId) {
    try {
        window._quill = null;
        const el = document.getElementById(editorId);
        if (el) el.innerHTML = "";
    } catch { }
};


