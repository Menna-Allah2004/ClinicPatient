document.addEventListener('DOMContentLoaded', function() {
    // تحديث التاريخ والوقت
    const currentDateTimeElement = document.getElementById('current-date-time');
    if (currentDateTimeElement) {
      currentDateTimeElement.textContent = formatDateTimeArabic();
      
      // تحديث كل دقيقة
      setInterval(() => {
        currentDateTimeElement.textContent = formatDateTimeArabic();
      }, 60000);
    }
  
    // تحديث التحية
    const doctorNameElement = document.getElementById('doctor-name');
    if (doctorNameElement) {
      const greeting = document.querySelector('.greeting');
      if (greeting) {
        greeting.textContent = `${getGreeting()}, ${doctorNameElement.textContent}!`;
      }
    }
    
    // التقويم والمواعيد
    const dayButtons = document.querySelectorAll('.day-button');
    if (dayButtons.length) {
      dayButtons.forEach(button => {
        button.addEventListener('click', function() {
          // إزالة الفئة النشطة من جميع الأزرار
          dayButtons.forEach(btn => btn.classList.remove('active'));
          
          // إضافة الفئة النشطة إلى الزر المضغوط
          this.classList.add('active');
          
          // هنا يمكن إضافة منطق لجلب المواعيد للتاريخ المحدد
          // في الإصدار الحالي سنتظاهر بأن اليوم 13 هو الوحيد الذي لديه مواعيد
          const appointmentsList = document.querySelector('.appointments-list');
          if (this.textContent.trim() === '13') {
            appointmentsList.innerHTML = `
              <div class="appointment-item d-flex p-2">
                <div class="appointment-time text-muted me-3">2:00 م</div>
                <div class="appointment-info">
                  <div class="appointment-title">اجتماع مع الطبيب د. وليامز</div>
                  <div class="appointment-person text-muted small">د. وليامز</div>
                </div>
              </div>
              <div class="appointment-item d-flex p-2">
                <div class="appointment-time text-muted me-3">3:30 م</div>
                <div class="appointment-info">
                  <div class="appointment-title">استشارة مع السيد وايت</div>
                  <div class="appointment-person text-muted small">السيد وايت</div>
                </div>
              </div>
              <div class="appointment-item d-flex p-2">
                <div class="appointment-time text-muted me-3">5:00 م</div>
                <div class="appointment-info">
                  <div class="appointment-title">استشارة مع السيدة بيسي</div>
                  <div class="appointment-person text-muted small">السيدة بيسي</div>
                </div>
              </div>
              <div class="appointment-item d-flex p-2">
                <div class="appointment-time text-muted me-3">5:30 م</div>
                <div class="appointment-info">
                  <div class="appointment-title">فحص للسيدة لورا تويكل</div>
                  <div class="appointment-person text-muted small">السيدة لورا</div>
                </div>
              </div>
            `;
          } else {
            appointmentsList.innerHTML = `
              <div class="text-center py-4 text-muted">
                لا توجد مواعيد لهذا اليوم
              </div>
            `;
          }
        });
      });
    }
    
    // تهيئة التقدم الدائري
    const progressCircle = document.querySelector('.progress-ring-circle');
    if (progressCircle) {
      // قيمة التقدم 95%
      const radius = progressCircle.r.baseVal.value;
      const circumference = radius * 2 * Math.PI;
      const offset = circumference - (95 / 100) * circumference;
      
      progressCircle.style.strokeDasharray = `${circumference} ${circumference}`;
      progressCircle.style.strokeDashoffset = offset;
      
      // إضافة تدرج لشريط التقدم
      const svg = document.querySelector('.progress-ring');
      if (svg) {
        const defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
        const linearGradient = document.createElementNS('http://www.w3.org/2000/svg', 'linearGradient');
        linearGradient.setAttribute('id', 'circleGradient');
        linearGradient.setAttribute('x1', '0%');
        linearGradient.setAttribute('y1', '0%');
        linearGradient.setAttribute('x2', '100%');
        linearGradient.setAttribute('y2', '100%');
        
        const stops = [
          { offset: '0%', color: '#9966FF' },
          { offset: '25%', color: '#FF6384' },
          { offset: '50%', color: '#FF9F40' },
          { offset: '75%', color: '#4BC0C0' },
          { offset: '100%', color: '#36A2EB' }
        ];
        
        stops.forEach(stopData => {
          const stop = document.createElementNS('http://www.w3.org/2000/svg', 'stop');
          stop.setAttribute('offset', stopData.offset);
          stop.setAttribute('stop-color', stopData.color);
          linearGradient.appendChild(stop);
        });
        
        defs.appendChild(linearGradient);
        svg.insertBefore(defs, svg.firstChild);
      }
    }
    
    // تهيئة القائمة المنسدلة لفترات المهام
    const periodSelect = document.querySelector('.period-select');
    if (periodSelect) {
      periodSelect.addEventListener('change', function() {
        // هنا يمكنك إضافة منطق لتحديث بيانات المهام بناءً على الفترة المحددة
        console.log('تم تغيير فترة المهام إلى:', this.value);
        // أظهر تنبيه لتوضيح التفاعل
        showToast(`تم تحديث بيانات المهام لفترة: ${this.value}`, 'info');
      });
    }
    
    // زر إضافة مهمة
    const addTaskButton = document.querySelector('button.btn-link');
    if (addTaskButton) {
      addTaskButton.addEventListener('click', function() {
        // هنا يمكنك إضافة منطق لفتح نموذج إضافة مهمة جديدة
        console.log('تم النقر على زر إضافة مهمة');
        // أظهر تنبيه لتوضيح التفاعل
        showToast('سيتم فتح نموذج إضافة مهمة جديدة', 'primary');
      });
    }
    
    // زر تعديل الملف الشخصي
    const editProfileButton = document.querySelector('.card-body .btn-primary');
    if (editProfileButton) {
      editProfileButton.addEventListener('click', function() {
        // هنا يمكنك إضافة منطق لفتح نموذج تعديل الملف الشخصي
        console.log('تم النقر على زر تعديل الملف الشخصي');
        // أظهر تنبيه لتوضيح التفاعل
        showToast('سيتم فتح نموذج تعديل الملف الشخصي', 'primary');
      });
    }
    
    // أزرار تنقل التقويم
    const calendarNavButtons = document.querySelectorAll('.card-title + div .btn-outline-secondary');
    if (calendarNavButtons.length) {
      calendarNavButtons.forEach(button => {
        button.addEventListener('click', function() {
          // هنا يمكنك إضافة منطق للتنقل بين أسابيع التقويم
          console.log('تم النقر على زر تنقل التقويم');
          // أظهر تنبيه لتوضيح التفاعل
          showToast('سيتم تحديث التقويم للأسبوع المحدد', 'info');
        });
      });
    }
    
    // تضمين الرسوم المتحركة للعناصر
    const statCards = document.querySelectorAll('.stat-card');
    if (statCards.length) {
      statCards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.1}s`;
      });
    }
  });
  