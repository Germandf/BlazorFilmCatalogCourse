using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GdFilms.Shared.Models
{
    public class Person
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Photo { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        public List<FilmActor> FilmActors { get; set; }
        [NotMapped]
        public string Character { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Person p2)
            {
                return Id == p2.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
