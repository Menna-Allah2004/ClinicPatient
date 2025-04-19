using ClinicPatient.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ClinicPatient.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Specialization is required")]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }

        [Display(Name = "Bio")]
        public string Bio { get; set; }

        [Display(Name = "Education")]
        public string Education { get; set; }

        [Display(Name = "Experience (Years)")]
        public int? ExperienceYears { get; set; }

        [Display(Name = "Rating")]
        public decimal Rating { get; set; }

        [Display(Name = "Rating Count")]
        public int RatingCount { get; set; }

        [Display(Name = "Consultation Fee")]
        [DataType(DataType.Currency)]
        public decimal? ConsultationFee { get; set; }

        [Display(Name = "Profile Image")]
        public string ImageUrl { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Profile Image Upload")]
        public IFormFile ProfileImage { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        public List<DoctorAvailability> Availabilities { get; set; }

        public List<AppointmentViewModel> Appointments { get; set; }
    }

    public class DoctorsListViewModel
    {
        public List<DoctorViewModel> Doctors { get; set; }
        public string SearchTerm { get; set; }
        public string Specialization { get; set; }
        public List<string> Specializations { get; set; }
    }
}