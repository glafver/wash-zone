// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Password show/hide toggle
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.password-toggle').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var wrap = btn.closest('.form-floating');
            var input = wrap.querySelector('input');
            var eye = btn.querySelector('.icon-eye');
            var eyeOff = btn.querySelector('.icon-eye-off');

            if (!input) return;

            var show = input.type === 'password';
            input.type = show ? 'text' : 'password';

            if (eye) eye.style.display = show ? 'none' : 'block';
            if (eyeOff) eyeOff.style.display = show ? 'block' : 'none';

            btn.setAttribute('aria-label', show ? 'Hide password' : 'Show password');
        });
    });
});

// Filter dropdowns — Bootstrap dropdowns that act as form selects.
// Clicking an item stores its value in a hidden input and submits the form.
document.addEventListener('click', function (e) {
    var item = e.target.closest('.filter-dropdown .dropdown-item');
    if (!item) return;
    e.preventDefault();

    var dd = item.closest('.filter-dropdown');
    var hidden = dd.querySelector('input[type="hidden"]');
    var button = dd.querySelector('button.dropdown-toggle');

    if (hidden) hidden.value = item.getAttribute('data-value');
    if (button) button.textContent = item.textContent.trim();

    dd.querySelectorAll('.dropdown-item').forEach(function (i) {
        i.classList.remove('is-selected');
    });
    item.classList.add('is-selected');

    var form = dd.closest('form');
    if (form) form.submit();
});

// Select-like dropdowns (no auto-submit) — just update a hidden input + button text.
document.addEventListener('click', function (e) {
    var item = e.target.closest('.select-dropdown .dropdown-item');
    if (!item) return;
    e.preventDefault();

    var dd = item.closest('.select-dropdown');
    var hidden = dd.querySelector('input[type="hidden"]');
    var button = dd.querySelector('button.dropdown-toggle');

    if (hidden) hidden.value = item.getAttribute('data-value');
    if (button) button.textContent = item.textContent.trim();

    dd.querySelectorAll('.dropdown-item').forEach(function (i) {
        i.classList.remove('is-selected');
    });
    item.classList.add('is-selected');
});

// Custom select (native-style dropdown). Clicking an option stores its value in a
// hidden input, updates the trigger label and either submits the form (`.custom-select--submit`)
// or dispatches a 'change' event so pages can react (e.g. load packages / render the calendar).
document.addEventListener('click', function (e) {
    var option = e.target.closest('.custom-select .custom-select__option');
    if (!option) return;
    e.preventDefault();

    var select = option.closest('.custom-select');
    var hidden = select.querySelector('input[type="hidden"]');
    var valueEl = select.querySelector('.custom-select__value');

    if (hidden) hidden.value = option.getAttribute('data-value');
    if (valueEl) valueEl.textContent = option.textContent.trim();

    select.querySelectorAll('.custom-select__option').forEach(function (o) {
        o.classList.remove('is-selected');
    });
    option.classList.add('is-selected');

    if (select.classList.contains('custom-select--submit')) {
        var form = select.closest('form');
        if (form) form.submit();
    } else if (hidden) {
        hidden.dispatchEvent(new Event('change', { bubbles: true }));
    }
});
