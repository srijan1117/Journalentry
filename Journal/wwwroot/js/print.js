window.printHtml = function (title, html) {
    const iframe = document.createElement("iframe");
    iframe.style.display = "none";
    document.body.appendChild(iframe);

    const doc = iframe.contentWindow.document;
    doc.open();
    doc.write(html ?? "<html><body></body></html>");
    doc.close();

    iframe.contentWindow.focus();
    iframe.contentWindow.print();

    setTimeout(() => iframe.remove(), 1000);
};
