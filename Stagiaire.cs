using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetExamen
{
    public class Stagiaire
    {
        public int Num { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        //public DateTime DateNaissance { get; set; }
        public int Age { get; set; }
        public string Sexe { get; set; }
        public string Programme { get; set; }

        public Stagiaire(int num, string nom, string prenom, DateTime dateNaissance, string sexe, string programme)
        {
            this.Num = num;
            this.Nom = nom;
            this.Prenom = prenom;
            this.Age = getAge(dateNaissance);
            this.Sexe = sexe;
            this.Programme = programme;
        }

        public int getAge(DateTime dateNaissance)
        {
            int age = 0;
            age = DateTime.Now.Subtract(dateNaissance).Days;
            age = age / 365;
            return age;
        }
    }
}
