// Write your JavaScript code.
$('.select-all').click(function () {
    $('input[type="checkbox"]', this.closest('table')).prop('checked', this.checked);
});
