// القائمة الجانبية - التوسيع والتقليص
document.addEventListener('DOMContentLoaded', function() {
    const sidebar = document.getElementById('sidebar');
    const toggleSidebar = document.getElementById('toggle-sidebar');
    const mobileSidebarOverlay = document.getElementById('mobile-sidebar-overlay');
    const mobileMenuToggle = document.getElementById('mobile-menu-toggle');
    const mainContent = document.querySelector('.main-content');
    
    // زر تبديل القائمة الجانبية
    if (toggleSidebar) {
      toggleSidebar.addEventListener('click', function() {
        sidebar.classList.toggle('collapsed');
        
        // تخزين الحالة في localStorage
        localStorage.setItem('sidebarCollapsed', sidebar.classList.contains('collapsed'));
      });
    }
    
    // استعادة حالة القائمة الجانبية
    const sidebarCollapsed = localStorage.getItem('sidebarCollapsed');
    if (sidebarCollapsed === 'true') {
      sidebar.classList.add('collapsed');
    }
    
    // قائمة الجوال
    if (mobileMenuToggle) {
      mobileMenuToggle.addEventListener('click', function() {
        sidebar.classList.add('mobile-visible');
        mobileSidebarOverlay.classList.remove('d-none');
      });
    }
    
    if (mobileSidebarOverlay) {
      mobileSidebarOverlay.addEventListener('click', function() {
        sidebar.classList.remove('mobile-visible');
        mobileSidebarOverlay.classList.add('d-none');
      });
    }
    
    // تحديث القائمة النشطة حسب الصفحة الحالية
    const currentPath = window.location.pathname;
    const fileName = currentPath.substring(currentPath.lastIndexOf('/') + 1) || 'index.html';
    
    const menuItems = document.querySelectorAll('.sidebar-menu-item');
    menuItems.forEach(item => {
      const link = item.querySelector('a');
      if (link && link.getAttribute('href') === fileName) {
        item.classList.add('active');
      } else {
        item.classList.remove('active');
      }
    });
    
    // إعادة ضبط القائمة الجانبية عند تغيير حجم النافذة
    window.addEventListener('resize', function() {
      if (window.innerWidth >= 992) {
        mobileSidebarOverlay.classList.add('d-none');
        sidebar.classList.remove('mobile-visible');
      }
    });
  });
  