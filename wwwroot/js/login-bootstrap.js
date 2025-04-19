// ======= Login Page JavaScript Logic for Bootstrap Version =======

// DOM Elements
document.addEventListener('DOMContentLoaded', function() {
  const patientBtn = document.getElementById('patientBtn');
  const doctorBtn = document.getElementById('doctorBtn');
  const loginForm = document.getElementById('loginForm');
  const togglePasswordBtn = document.getElementById('togglePassword');
  const passwordInput = document.getElementById('password');
  const passwordIcon = document.getElementById('passwordIcon');

  // User type state (default: patient)
  let userType = 'patient';

  // User type tabs functionality
  patientBtn.addEventListener('click', () => setUserType('patient'));
  doctorBtn.addEventListener('click', () => setUserType('doctor'));
  
  // Toggle password visibility
  togglePasswordBtn.addEventListener('click', togglePasswordVisibility);
  
  // Form submission
  loginForm.addEventListener('submit', handleSubmit);

  // Set user type function
  function setUserType(type) {
    userType = type;
    
    if (type === 'patient') {
      patientBtn.classList.add('active-tab');
      doctorBtn.classList.remove('active-tab');
    } else {
      doctorBtn.classList.add('active-tab');
      patientBtn.classList.remove('active-tab');
    }
  }

  // Toggle password visibility
  function togglePasswordVisibility() {
    if (passwordInput.type === 'password') {
      passwordInput.type = 'text';
      passwordIcon.classList.remove('fa-eye');
      passwordIcon.classList.add('fa-eye-slash');
    } else {
      passwordInput.type = 'password';
      passwordIcon.classList.remove('fa-eye-slash');
      passwordIcon.classList.add('fa-eye');
    }
  }

  // Handle form submission
  function handleSubmit(event) {
    event.preventDefault();
    
    // Form validation
    if (!loginForm.checkValidity()) {
      event.stopPropagation();
      loginForm.classList.add('was-validated');
      return;
    }
    
    // Get form data
    const formData = {
      email: document.getElementById('email').value,
      password: document.getElementById('password').value,
      rememberMe: document.getElementById('rememberMe').checked,
      userType: userType
    };
    
    console.log('Login submitted', formData);
    
    // Show toast notification
    showToast('تم تسجيل الدخول بنجاح', 'جاري توجيهك إلى لوحة التحكم الخاصة بك');
    
    // Redirect based on user type
    setTimeout(() => {
      if (userType === 'patient') {
        window.location.href = 'patient-profile.html';
      } else {
        window.location.href = 'doctor-dashboard.html';
      }
    }, 1500);
  }

  // Simple toast notification
  function showToast(title, message) {
    // Create toast container if it doesn't exist
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
      toastContainer = document.createElement('div');
      toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
      document.body.appendChild(toastContainer);
    }
    
    // Create toast element
    const toastEl = document.createElement('div');
    toastEl.className = 'toast show fade-in';
    toastEl.setAttribute('role', 'alert');
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');
    
    // Toast content
    toastEl.innerHTML = `
      <div class="toast-header bg-primary text-white">
        <strong class="ms-auto">${title}</strong>
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="إغلاق"></button>
      </div>
      <div class="toast-body">
        ${message}
      </div>
    `;
    
    // Add toast to container
    toastContainer.appendChild(toastEl);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
      toastEl.remove();
    }, 5000);
    
    // Close button functionality
    const closeBtn = toastEl.querySelector('.btn-close');
    closeBtn.addEventListener('click', () => {
      toastEl.remove();
    });
  }
});