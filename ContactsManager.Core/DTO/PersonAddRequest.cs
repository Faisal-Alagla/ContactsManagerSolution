﻿using System;
using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// DTO class for adding a new Person
	/// </summary>
	public class PersonAddRequest
	{
		[Required(ErrorMessage = "Person Name can't be blank")]
		public string? PersonName { get; set; }

		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage = "Invalid Email")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }

		[DataType(DataType.Date)]
		public DateTime? DateOfBirth { get; set; }

		[Required(ErrorMessage = "A gender must be selected")]
		public GenderOptions? Gender { get; set; }

		[Required(ErrorMessage = "A country must be selected")]
		public Guid? CountryID { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }

		/// <summary>
		/// Converts the current object of PersonAddRequest into a new Person object
		/// </summary>
		/// <returns>A Person object that has the same data as the current PersonAddRequest object</returns>
		public Person ToPerson()
		{
			return new Person()
			{
				PersonName = PersonName,
				Email = Email,
				DateOfBirth = DateOfBirth,
				Gender = Gender.ToString(),
				CountryID = CountryID,
				Address = Address,
				ReceiveNewsLetters = ReceiveNewsLetters
			};
		}
	}
}
