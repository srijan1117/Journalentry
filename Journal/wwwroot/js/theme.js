window.applyTheme = function (theme) {
    document.documentElement.setAttribute('data-theme', theme);
};

window.initTheme = function () {
    var saved = localStorage.getItem('theme') || 'light';
    window.applyTheme(saved);
    return saved;
};

window.toggleTheme = function () {
    var current = document.documentElement.getAttribute('data-theme') || 'light';
    var next = current === 'dark' ? 'light' : 'dark';
    localStorage.setItem('theme', next);
    window.applyTheme(next);
    return next;
};

document.addEventListener('DOMContentLoaded', function () {
    window.initTheme();
});
