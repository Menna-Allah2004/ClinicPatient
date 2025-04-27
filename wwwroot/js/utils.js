// دوال مساعدة مستخدمة في جميع أنحاء التطبيق

// تنسيق التاريخ والوقت بالعربية
function formatDateTimeArabic() {
    const now = new Date();
    const locale = 'ar-SA';
    
    const options = {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    };
    
    return now.toLocaleDateString(locale, options);
  }
  
  // الحصول على التحية المناسبة حسب وقت اليوم
  function getGreeting() {
    const hour = new Date().getHours();
    
    if (hour < 12) {
      return 'صباح الخير';
    }
    else {
      return 'مساء الخير';
    }
  }
  
  // التقديم المختصر للأرقام الكبيرة
  function formatNumber(num) {
    if (num >= 1000000) {
      return (num / 1000000).toFixed(1) + ' مليون';
    } else if (num >= 1000) {
      return (num / 1000).toFixed(1) + ' ألف';
    }
    return num;
  }
  
  // إنشاء رسم بياني نسبة مئوية دائري
  function createCircleProgress(elementId, percent) {
    const circle = document.getElementById(elementId);
    if (!circle) return;
    
    const radius = circle.r.baseVal.value;
    const circumference = radius * 2 * Math.PI;
    
    circle.style.strokeDasharray = `${circumference} ${circumference}`;
    circle.style.strokeDashoffset = `${circumference}`;
    
    const offset = circumference - (percent / 100) * circumference;
    circle.style.strokeDashoffset = offset;
  }
  
  // للتأخير المؤقت (استخدام مع async/await)
  function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
  
  // التحقق من الجوال
  function isMobile() {
    return window.innerWidth < 992;
  }
  
  // تهيئة عناصر الإدخال في النماذج
  function initFormInputs() {
    // لتفعيل التلميحات التوضيحية الخاصة بـ Bootstrap
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    if (tooltipTriggerList.length) {
      tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
      });
    }
    
    // لمعالجة أحداث النماذج هنا
    const forms = document.querySelectorAll('.needs-validation');
    if (forms.length) {
      Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
          if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
          }
          
          form.classList.add('was-validated');
        }, false);
      });
    }
  }
  
  // بناء تذكير بسيط
  function showToast(message, type = 'success') {
    // تحقق من وجود مكون Bootstrap toast
    if (typeof bootstrap !== 'undefined') {
      // إنشاء عنصر toast
      const toastEl = document.createElement('div');
      toastEl.className = `toast align-items-center text-white bg-${type} border-0`;
      toastEl.setAttribute('role', 'alert');
      toastEl.setAttribute('aria-live', 'assertive');
      toastEl.setAttribute('aria-atomic', 'true');
      
      // محتوى التنبيه
      toastEl.innerHTML = `
        <div class="d-flex">
          <div class="toast-body">
            ${message}
          </div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="إغلاق"></button>
        </div>
      `;
      
      // إضافة العنصر إلى المستند
      document.body.appendChild(toastEl);
      
      // تهيئة وإظهار التنبيه
      const toast = new bootstrap.Toast(toastEl, { autohide: true, delay: 3000 });
      toast.show();
      
      // إزالة العنصر بعد الإخفاء
      toastEl.addEventListener('hidden.bs.toast', function () {
        document.body.removeChild(toastEl);
      });
    } else {
      // إذا لم يتم تحميل Bootstrap، استخدم تنبيه عادي
      alert(message);
    }
  }
  