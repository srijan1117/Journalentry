window._quill = null;

window.initQuill = function (editorId) {
    const editorElement = document.getElementById(editorId);
    if (!editorElement) return;

    // Check if we have an existing valid instance on this element
    if (window._quill) {
        // If the editor is already attached to this element and connected to DOM, skip
        if (window._quill.root && window._quill.root.isConnected && editorElement.contains(window._quill.root)) {
            return;
        }
        // Otherwise, the old instance is stale (detached), so we clean up
        window._quill = null;
    }

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

window.enableQuill = function (enabled) {
    if (!window._quill) return;
    window._quill.enable(enabled);
};

window.destroyQuill = function (editorId) {
    try {
        window._quill = null;
        const el = document.getElementById(editorId);
        if (el) el.innerHTML = "";
    } catch { }
};
