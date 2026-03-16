// Simple lamp toggle: toggles a data attribute on <body> so styles can react
document.addEventListener('DOMContentLoaded', function () {
    var btn = document.getElementById('lampToggle');
    if (!btn) return;

    btn.addEventListener('click', function () {
        var isOn = document.body.getAttribute('data-on') === 'true';
        document.body.setAttribute('data-on', (!isOn).toString());
        // optional: save preference
        try { localStorage.setItem('lampOn', (!isOn).toString()); } catch (e) { }
    });

    // restore saved state
    try {
        var saved = localStorage.getItem('lampOn');
        if (saved === 'true') document.body.setAttribute('data-on', 'true');
    } catch (e) { }
});
