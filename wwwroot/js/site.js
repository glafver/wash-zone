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

// Searchable select: type in the input to filter the dropdown options.
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.search-select').forEach(function (ss) {
        var input = ss.querySelector('input[type="text"]');
        var menu = ss.querySelector('.search-select__menu');
        var hidden = ss.querySelector('input[type="hidden"]');
        if (!input || !menu) return;

        var options = Array.prototype.slice.call(ss.querySelectorAll('.search-select__option'));

        function filter() {
            var q = input.value.toLowerCase().trim();
            var any = false;
            options.forEach(function (opt) {
                var match = q === '' || opt.textContent.toLowerCase().indexOf(q) !== -1;
                opt.style.display = match ? '' : 'none';
                if (match) any = true;
            });
            menu.classList.toggle('is-open', any);
        }

        input.addEventListener('input', filter);
        input.addEventListener('focus', function () {
            filter();
            menu.classList.add('is-open');
        });

        menu.addEventListener('mousedown', function (e) {
            var opt = e.target.closest('.search-select__option');
            if (!opt) return;
            e.preventDefault();
            if (hidden) hidden.value = opt.getAttribute('data-value');
            input.value = opt.textContent.trim();
            menu.classList.remove('is-open');
        });

        document.addEventListener('click', function (e) {
            if (!ss.contains(e.target)) menu.classList.remove('is-open');
        });
    });
});

// Auto-show toast notifications on page load.
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.toast').forEach(function (toast) {
        new bootstrap.Toast(toast).show();
    });
});
