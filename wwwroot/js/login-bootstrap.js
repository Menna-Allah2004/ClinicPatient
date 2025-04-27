// ======= Login Page JavaScript Logic (Cleaned Version - No Backend) =======

document.addEventListener('DOMContentLoaded', function () {
    const patientBtn = document.getElementById('patientBtn');
    const doctorBtn = document.getElementById('doctorBtn');
    const loginForm = document.getElementById('loginForm');
    const togglePasswordBtn = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('password');
    const passwordIcon = document.getElementById('passwordIcon');

    let userType = 'patient';

    patientBtn.addEventListener('click', () => setUserType('patient'));
    doctorBtn.addEventListener('click', () => setUserType('doctor'));
    togglePasswordBtn.addEventListener('click', togglePasswordVisibility);
    loginForm.addEventListener('submit', handleSubmit);

    function setUserType(type) {
        userType = type;
        patientBtn.classList.toggle('active-tab', type === 'patient');
        doctorBtn.classList.toggle('active-tab', type === 'doctor');
    }

    function togglePasswordVisibility() {
        const isHidden = passwordInput.type === 'password';
        passwordInput.type = isHidden ? 'text' : 'password';
        passwordIcon.classList.toggle('fa-eye', !isHidden);
        passwordIcon.classList.toggle('fa-eye-slash', isHidden);
    }

    function handleSubmit(event) {
        event.preventDefault();

        showToast(
            'تم تسجيل الدخول (تجريبي)',
            userType === 'patient'
                ? 'أهلاً بالمريض! (لن يتم توجيهك فعليًا)'
                : 'أهلاً بالطبيب! (لن يتم توجيهك فعليًا)'
        );
    }

    function showToast(title, message) {
        let toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(toastContainer);
        }

        const toastEl = document.createElement('div');
        toastEl.className = 'toast show fade-in';
        toastEl.setAttribute('role', 'alert');
        toastEl.setAttribute('aria-live', 'assertive');
        toastEl.setAttribute('aria-atomic', 'true');

        toastEl.innerHTML = `
      <div class="toast-header bg-primary text-white">
        <strong class="ms-auto">${title}</strong>
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="إغلاق"></button>
      </div>
      <div class="toast-body">
        ${message}
      </div>
    `;

        toastContainer.appendChild(toastEl);

        setTimeout(() => toastEl.remove(), 5000);
        toastEl.querySelector('.btn-close').addEventListener('click', () => toastEl.remove());
    }
});