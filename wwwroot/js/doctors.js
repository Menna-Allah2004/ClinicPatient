
// JavaScript مبسط لصفحة الأطباء - فقط للتفاعلات الأساسية

document.addEventListener('DOMContentLoaded', function () {
    // Toggle Advanced Filters
    const toggleAdvancedFilters = document.getElementById('toggleAdvancedFilters');
    const advancedFilters = document.getElementById('advancedFilters');

    if (toggleAdvancedFilters && advancedFilters) {
        toggleAdvancedFilters.addEventListener('click', function () {
            advancedFilters.classList.toggle('d-none');
        });
    }

    // Auto-submit search form with delay
    const searchInput = document.querySelector('input[name="searchQuery"]');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function () {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.form.submit();
            }, 500);
        });
    }
});