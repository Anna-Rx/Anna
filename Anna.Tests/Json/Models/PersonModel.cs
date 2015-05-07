using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anna.Tests.Json.Models
{
    public class PersonModel
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public override bool Equals(object obj)
        {
            PersonModel person = obj as PersonModel;

            if (person != null  &&
                person.DateOfBirth == DateOfBirth &&
                person.FirstName == FirstName &&
                person.LastName == LastName &&
                person.PersonId == PersonId)
            {
                return true;
            }

            return false;
        }
    }
}
